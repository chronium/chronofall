using System.Net.Sockets;

namespace ChronoFall.Network.Transport.Tests;

internal sealed class RecordingNetworkEventHandler : INetworkEventHandler
{
    public List<(NetworkPeerId PeerId, NetworkEndpoint Endpoint)> Connections { get; } = [];

    public List<(NetworkPeerId PeerId, NetworkDisconnectReason Reason)> Disconnections { get; } = [];

    public List<ReceivedPacket> Packets { get; } = [];

    public List<(NetworkEndpoint? Endpoint, SocketError Error)> Errors { get; } = [];

    public List<(NetworkPeerId PeerId, int Milliseconds)> Latencies { get; } = [];

    public void Connected(NetworkPeerId peerId, NetworkEndpoint endpoint) =>
        Connections.Add((peerId, endpoint));

    public void Disconnected(NetworkPeerId peerId, NetworkDisconnectReason reason) =>
        Disconnections.Add((peerId, reason));

    public void PacketReceived(
        NetworkPeerId peerId,
        ReadOnlyMemory<byte> packet,
        NetworkDelivery delivery,
        byte channel) =>
        Packets.Add(new ReceivedPacket(peerId, packet, delivery, channel));

    public void NetworkError(NetworkEndpoint? endpoint, SocketError socketError) =>
        Errors.Add((endpoint, socketError));

    public void LatencyUpdated(NetworkPeerId peerId, int latencyMilliseconds) =>
        Latencies.Add((peerId, latencyMilliseconds));
}

internal readonly record struct ReceivedPacket(
    NetworkPeerId PeerId,
    ReadOnlyMemory<byte> Payload,
    NetworkDelivery Delivery,
    byte Channel);
