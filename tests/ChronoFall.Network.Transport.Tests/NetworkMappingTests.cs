using ChronoFall.Network.Transport.LiteNetLib;
using global::LiteNetLib;

namespace ChronoFall.Network.Transport.Tests;

public sealed class NetworkMappingTests
{
    [Theory]
    [InlineData(NetworkDelivery.Unreliable, DeliveryMethod.Unreliable)]
    [InlineData(NetworkDelivery.ReliableUnordered, DeliveryMethod.ReliableUnordered)]
    [InlineData(NetworkDelivery.Sequenced, DeliveryMethod.Sequenced)]
    [InlineData(NetworkDelivery.ReliableOrdered, DeliveryMethod.ReliableOrdered)]
    [InlineData(NetworkDelivery.ReliableSequenced, DeliveryMethod.ReliableSequenced)]
    public void EveryDeliveryModeRoundTrips(
        NetworkDelivery shared,
        DeliveryMethod liteNetLib)
    {
        Assert.Equal(liteNetLib, shared.ToLiteNetLib());
        Assert.Equal(shared, liteNetLib.ToNetworkDelivery());
    }

    [Fact]
    public void InvalidDeliveryModesFailExplicitly()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ((NetworkDelivery)int.MaxValue).ToLiteNetLib());
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ((DeliveryMethod)byte.MaxValue).ToNetworkDelivery());
    }

    [Theory]
    [InlineData(DisconnectReason.ConnectionFailed, NetworkDisconnectReason.ConnectionFailed)]
    [InlineData(DisconnectReason.Timeout, NetworkDisconnectReason.Timeout)]
    [InlineData(DisconnectReason.HostUnreachable, NetworkDisconnectReason.HostUnreachable)]
    [InlineData(DisconnectReason.NetworkUnreachable, NetworkDisconnectReason.NetworkUnreachable)]
    [InlineData(DisconnectReason.RemoteConnectionClose, NetworkDisconnectReason.RemoteConnectionClose)]
    [InlineData(DisconnectReason.DisconnectPeerCalled, NetworkDisconnectReason.LocalDisconnect)]
    [InlineData(DisconnectReason.ConnectionRejected, NetworkDisconnectReason.ConnectionRejected)]
    [InlineData(DisconnectReason.InvalidProtocol, NetworkDisconnectReason.InvalidProtocol)]
    [InlineData(DisconnectReason.UnknownHost, NetworkDisconnectReason.UnknownHost)]
    [InlineData(DisconnectReason.Reconnect, NetworkDisconnectReason.Reconnect)]
    [InlineData(DisconnectReason.PeerToPeerConnection, NetworkDisconnectReason.PeerToPeerConnection)]
    [InlineData(DisconnectReason.PeerNotFound, NetworkDisconnectReason.PeerNotFound)]
    public void KnownDisconnectReasonsMapWithoutLeakingLiteNetLib(
        DisconnectReason source,
        NetworkDisconnectReason expected)
    {
        Assert.Equal(expected, source.ToNetworkDisconnectReason());
    }

    [Fact]
    public void UnknownDisconnectReasonMapsToBoundedFallback()
    {
        Assert.Equal(
            NetworkDisconnectReason.Unknown,
            ((DisconnectReason)int.MaxValue).ToNetworkDisconnectReason());
    }
}
