using ChronoFall.Network.Transport;

namespace ChronoFall.Network.Transport.Tests;

public sealed class NetworkContractTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EndpointRejectsMissingHost(string? host)
    {
        Assert.Throws<ArgumentException>(() => new NetworkEndpoint(host!, 7777));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void EndpointRejectsInvalidRemotePort(int port)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NetworkEndpoint("127.0.0.1", port));
    }

    [Fact]
    public void EndpointPreservesHostPortAndDisplayForm()
    {
        NetworkEndpoint endpoint = new("localhost", 7777);

        Assert.Equal("localhost", endpoint.Host);
        Assert.Equal(7777, endpoint.Port);
        Assert.Equal("localhost:7777", endpoint.ToString());
    }

    [Fact]
    public void PeerIdRejectsNegativeAndPreservesNonNegativeValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NetworkPeerId(-1));

        NetworkPeerId peer = new(0);
        Assert.Equal(0, peer.Value);
        Assert.Equal("0", peer.ToString());
    }
}
