using System.Net;
using global::LiteNetLib;

namespace ChronoFall.Network.Transport.LiteNetLib;

public sealed class LiteNetLibNetworkTransport : INetworkTransport, INetworkTransportDiagnostics
{
    public const byte MaximumChannel = 63;

    private readonly EventBasedNetListener listener;
    private readonly NetManager manager;
    private readonly Dictionary<NetworkPeerId, NetPeer> peers = [];
    private INetworkEventHandler? pollHandler;
    private bool started;
    private bool disposed;

    public LiteNetLibNetworkTransport()
    {
        listener = new EventBasedNetListener();
        listener.ConnectionRequestEvent += request => request.Accept();
        listener.PeerConnectedEvent += OnPeerConnected;
        listener.PeerDisconnectedEvent += OnPeerDisconnected;
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.NetworkErrorEvent += OnNetworkError;
        listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;

        manager = new NetManager(listener)
        {
            AutoRecycle = true,
            ChannelsCount = MaximumChannel + 1,
            EnableStatistics = true,
        };
    }

    internal bool StatisticsEnabled => manager.EnableStatistics;

    public void Start(int port)
    {
        ThrowIfDisposed();

        if (started)
        {
            throw new InvalidOperationException("Network transport has already been started.");
        }

        if (port is < 0 or > 65535)
        {
            throw new ArgumentOutOfRangeException(
                nameof(port),
                port,
                "Listen port must be between 0 and 65535.");
        }

        if (!manager.Start(port))
        {
            throw new InvalidOperationException($"Network transport failed to start on UDP port {port}.");
        }

        started = true;
    }

    public NetworkPeerId Connect(NetworkEndpoint endpoint)
    {
        ThrowIfDisposed();
        EnsureStarted();

        NetPeer peer = manager.Connect(endpoint.Host, endpoint.Port, string.Empty)
            ?? throw new InvalidOperationException(
                $"Network transport could not start a connection to {endpoint}.");

        NetworkPeerId peerId = ToPeerId(peer);
        peers[peerId] = peer;
        return peerId;
    }

    public void Send(
        NetworkPeerId peerId,
        ReadOnlySpan<byte> packet,
        NetworkDelivery delivery,
        byte channel = 0)
    {
        ThrowIfDisposed();
        EnsureStarted();

        if (channel > MaximumChannel)
        {
            throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                $"Network delivery channel must be between 0 and {MaximumChannel}.");
        }

        if (!peers.TryGetValue(peerId, out NetPeer? peer) ||
            peer.ConnectionState != ConnectionState.Connected)
        {
            throw new InvalidOperationException($"Network peer {peerId} is not connected.");
        }

        peer.Send(packet, channel, delivery.ToLiteNetLib());
    }

    public void Disconnect(NetworkPeerId peerId)
    {
        ThrowIfDisposed();
        EnsureStarted();

        if (!peers.TryGetValue(peerId, out NetPeer? peer))
        {
            throw new InvalidOperationException($"Network peer {peerId} is unknown.");
        }

        manager.DisconnectPeer(peer);
    }

    public void Poll(INetworkEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ThrowIfDisposed();
        EnsureStarted();

        if (pollHandler is not null)
        {
            throw new InvalidOperationException("Network transport is already polling events.");
        }

        pollHandler = handler;
        try
        {
            manager.PollEvents();
        }
        finally
        {
            pollHandler = null;
        }
    }

    public bool TryGetPeerStatistics(
        NetworkPeerId peerId,
        out NetworkPeerStatistics statistics)
    {
        ThrowIfDisposed();
        EnsureStarted();

        if (!peers.TryGetValue(peerId, out NetPeer? peer) ||
            peer.ConnectionState != ConnectionState.Connected)
        {
            statistics = default;
            return false;
        }

        NetStatistics source = peer.Statistics;
        statistics = new NetworkPeerStatistics(
            peer.Ping,
            peer.RoundTripTime,
            peer.Mtu,
            peer.TimeSinceLastPacket,
            source.PacketsSent,
            source.PacketsReceived,
            source.BytesSent,
            source.BytesReceived,
            source.PacketLoss,
            source.PacketLossPercent);
        return true;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        manager.Stop();
        peers.Clear();
        pollHandler = null;
        disposed = true;
    }

    private void OnPeerConnected(NetPeer peer)
    {
        NetworkPeerId peerId = ToPeerId(peer);
        peers[peerId] = peer;
        pollHandler?.Connected(peerId, ToEndpoint(peer));
    }

    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        NetworkPeerId peerId = ToPeerId(peer);
        peers.Remove(peerId);
        pollHandler?.Disconnected(peerId, disconnectInfo.Reason.ToNetworkDisconnectReason());
    }

    private void OnNetworkReceive(
        NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod delivery)
    {
        byte[] packet = new byte[reader.UserDataSize];
        Buffer.BlockCopy(reader.RawData, reader.UserDataOffset, packet, 0, packet.Length);
        pollHandler?.PacketReceived(ToPeerId(peer), packet, delivery.ToNetworkDelivery(), channel);
    }

    private void OnNetworkError(IPEndPoint endpoint, System.Net.Sockets.SocketError socketError)
    {
        NetworkEndpoint? networkEndpoint = endpoint is null
            ? null
            : new NetworkEndpoint(endpoint.Address.ToString(), endpoint.Port);

        pollHandler?.NetworkError(networkEndpoint, socketError);
    }

    private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        pollHandler?.LatencyUpdated(ToPeerId(peer), latency);
    }

    private static NetworkPeerId ToPeerId(NetPeer peer) => new(peer.Id);

    private static NetworkEndpoint ToEndpoint(NetPeer peer) =>
        new(peer.Address.ToString(), peer.Port);

    private void EnsureStarted()
    {
        if (!started)
        {
            throw new InvalidOperationException("Network transport has not been started.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(LiteNetLibNetworkTransport));
        }
    }
}
