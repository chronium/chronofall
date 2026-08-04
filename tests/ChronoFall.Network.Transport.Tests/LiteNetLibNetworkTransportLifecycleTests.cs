using ChronoFall.Network.Transport.LiteNetLib;

namespace ChronoFall.Network.Transport.Tests;

public sealed class LiteNetLibNetworkTransportLifecycleTests
{
    [Fact]
    public void ConstructorEnablesPeerStatistics()
    {
        using LiteNetLibNetworkTransport transport = new();

        Assert.True(transport.StatisticsEnabled);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(65536)]
    public void StartRejectsInvalidPort(int port)
    {
        using LiteNetLibNetworkTransport transport = new();

        Assert.Throws<ArgumentOutOfRangeException>(() => transport.Start(port));
    }

    [Fact]
    public void StartAcceptsEphemeralPortAndRejectsSecondStart()
    {
        using LiteNetLibNetworkTransport transport = new();
        transport.Start(0);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(() => transport.Start(0));
        Assert.Contains("already been started", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void OperationsBeforeStartFailExplicitly()
    {
        using LiteNetLibNetworkTransport transport = new();
        RecordingNetworkEventHandler handler = new();

        Assert.Throws<InvalidOperationException>(
            () => transport.Connect(new NetworkEndpoint("127.0.0.1", 7777)));
        Assert.Throws<InvalidOperationException>(
            () => transport.Send(new NetworkPeerId(0), [1], NetworkDelivery.Unreliable));
        Assert.Throws<InvalidOperationException>(() => transport.Disconnect(new NetworkPeerId(0)));
        Assert.Throws<InvalidOperationException>(() => transport.Poll(handler));
        Assert.Throws<InvalidOperationException>(
            () => transport.TryGetPeerStatistics(new NetworkPeerId(0), out _));
    }

    [Fact]
    public void OperationsAfterDisposeFailAndRepeatedDisposeIsSafe()
    {
        LiteNetLibNetworkTransport transport = new();
        transport.Start(0);
        transport.Dispose();
        transport.Dispose();

        Assert.Throws<ObjectDisposedException>(() => transport.Start(0));
        Assert.Throws<ObjectDisposedException>(
            () => transport.Connect(new NetworkEndpoint("127.0.0.1", 7777)));
        Assert.Throws<ObjectDisposedException>(
            () => transport.Send(new NetworkPeerId(0), [1], NetworkDelivery.Unreliable));
        Assert.Throws<ObjectDisposedException>(() => transport.Disconnect(new NetworkPeerId(0)));
        Assert.Throws<ObjectDisposedException>(() => transport.Poll(new RecordingNetworkEventHandler()));
        Assert.Throws<ObjectDisposedException>(
            () => transport.TryGetPeerStatistics(new NetworkPeerId(0), out _));
    }

    [Fact]
    public void SendRejectsUnsupportedChannelBeforePeerLookup()
    {
        using LiteNetLibNetworkTransport transport = new();
        transport.Start(0);

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => transport.Send(
                new NetworkPeerId(0),
                [1],
                NetworkDelivery.Unreliable,
                channel: LiteNetLibNetworkTransport.MaximumChannel + 1));

        Assert.Equal("channel", exception.ParamName);
    }

    [Fact]
    public void UnknownPeerOperationsAreExplicit()
    {
        using LiteNetLibNetworkTransport transport = new();
        transport.Start(0);
        NetworkPeerId unknown = new(999);

        Assert.Throws<InvalidOperationException>(
            () => transport.Send(unknown, [1], NetworkDelivery.Unreliable));
        Assert.Throws<InvalidOperationException>(() => transport.Disconnect(unknown));
        Assert.False(transport.TryGetPeerStatistics(unknown, out _));
    }
}
