using ChronoFall.Network.Transport;
using ChronoFall.Network.Transport.LiteNetLib;

NetworkEndpoint endpoint = new("127.0.0.1", 7777);
using LiteNetLibNetworkTransport transport = new();
transport.Start(0);
if (transport.TryGetPeerStatistics(new NetworkPeerId(0), out _))
{
    throw new InvalidOperationException("An unknown peer unexpectedly returned statistics.");
}

Console.WriteLine($"ChronoFall network transport source consumer OK ({endpoint}).");
