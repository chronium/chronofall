namespace ChronoFall.Network.Transport;

public enum NetworkDelivery
{
    Unreliable,
    ReliableUnordered,
    Sequenced,
    ReliableOrdered,
    ReliableSequenced,
}
