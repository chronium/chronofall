using System.Net;
using System.Net.Sockets;
using ChronoFall.Network.Transport.LiteNetLib;

namespace ChronoFall.Network.Transport.Tests;

public sealed class LiteNetLibNetworkTransportLoopbackTests
{
    [Fact]
    public async Task ExchangesCopiedPacketsDiagnosticsAndDisconnectOverUdpLoopback()
    {
        int serverPort = ReserveUdpPort();
        using LiteNetLibNetworkTransport server = new();
        using LiteNetLibNetworkTransport client = new();
        RecordingNetworkEventHandler serverEvents = new();
        RecordingNetworkEventHandler clientEvents = new();

        server.Start(serverPort);
        client.Start(0);
        NetworkPeerId clientPeer = client.Connect(new NetworkEndpoint("127.0.0.1", serverPort));

        await PumpUntilAsync(
            server,
            serverEvents,
            client,
            clientEvents,
            () => serverEvents.Connections.Count == 1 && clientEvents.Connections.Count == 1);

        NetworkPeerId serverPeer = Assert.Single(serverEvents.Connections).PeerId;
        Assert.Equal(clientPeer, Assert.Single(clientEvents.Connections).PeerId);

        byte[] firstClientPacket = [1, 2, 3, 4];
        client.Send(clientPeer, firstClientPacket, NetworkDelivery.Sequenced, channel: 2);
        await PumpUntilAsync(
            server,
            serverEvents,
            client,
            clientEvents,
            () => serverEvents.Packets.Count == 1);

        ReceivedPacket retainedFirstPacket = Assert.Single(serverEvents.Packets);
        Assert.Equal(serverPeer, retainedFirstPacket.PeerId);
        Assert.Equal(NetworkDelivery.Sequenced, retainedFirstPacket.Delivery);
        Assert.Equal(2, retainedFirstPacket.Channel);
        Assert.Equal(firstClientPacket, retainedFirstPacket.Payload.ToArray());

        byte[] secondClientPacket = [9, 8, 7];
        client.Send(clientPeer, secondClientPacket, NetworkDelivery.ReliableUnordered, channel: 3);
        await PumpUntilAsync(
            server,
            serverEvents,
            client,
            clientEvents,
            () => serverEvents.Packets.Count == 2);

        Assert.Equal(
            firstClientPacket,
            retainedFirstPacket.Payload.ToArray());
        Assert.Equal(secondClientPacket, serverEvents.Packets[1].Payload.ToArray());

        byte[] serverPacket = [5, 6, 7];
        server.Send(serverPeer, serverPacket, NetworkDelivery.ReliableOrdered, channel: 1);
        await PumpUntilAsync(
            server,
            serverEvents,
            client,
            clientEvents,
            () => clientEvents.Packets.Count == 1);

        ReceivedPacket receivedFromServer = Assert.Single(clientEvents.Packets);
        Assert.Equal(clientPeer, receivedFromServer.PeerId);
        Assert.Equal(NetworkDelivery.ReliableOrdered, receivedFromServer.Delivery);
        Assert.Equal(1, receivedFromServer.Channel);
        Assert.Equal(serverPacket, receivedFromServer.Payload.ToArray());

        Assert.True(client.TryGetPeerStatistics(clientPeer, out NetworkPeerStatistics clientStatistics));
        Assert.True(server.TryGetPeerStatistics(serverPeer, out NetworkPeerStatistics serverStatistics));
        Assert.True(clientStatistics.MaximumTransmissionUnitBytes > 0);
        Assert.True(clientStatistics.PacketsSent >= 2);
        Assert.True(clientStatistics.BytesSent >= firstClientPacket.Length + secondClientPacket.Length);
        Assert.True(clientStatistics.PacketsReceived >= 1);
        Assert.True(serverStatistics.PacketsSent >= 1);
        Assert.True(serverStatistics.PacketsReceived >= 2);

        Assert.Empty(serverEvents.Errors);
        Assert.Empty(clientEvents.Errors);

        client.Disconnect(clientPeer);
        await PumpUntilAsync(
            server,
            serverEvents,
            client,
            clientEvents,
            () => serverEvents.Disconnections.Count == 1 || clientEvents.Disconnections.Count == 1);

        Assert.True(
            serverEvents.Disconnections.Count == 1 || clientEvents.Disconnections.Count == 1);
    }

    private static int ReserveUdpPort()
    {
        using UdpClient socket = new(new IPEndPoint(IPAddress.Loopback, 0));
        return ((IPEndPoint)socket.Client.LocalEndPoint!).Port;
    }

    private static async Task PumpUntilAsync(
        LiteNetLibNetworkTransport server,
        RecordingNetworkEventHandler serverEvents,
        LiteNetLibNetworkTransport client,
        RecordingNetworkEventHandler clientEvents,
        Func<bool> condition)
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));

        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            server.Poll(serverEvents);
            client.Poll(clientEvents);
            await Task.Delay(10, timeout.Token);
        }
    }
}
