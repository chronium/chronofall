namespace ChronoFall.Network.Transport;

public interface INetworkTransportDiagnostics
{
    bool TryGetPeerStatistics(NetworkPeerId peerId, out NetworkPeerStatistics statistics);
}
