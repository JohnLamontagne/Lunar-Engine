namespace Lunar.Core.Net
{
    /// <summary>Represents a remote peer in the transport-agnostic networking layer.</summary>
    public interface IConnection
    {
        /// <summary>Stable identifier for this peer for the lifetime of the connection.</summary>
        long UniqueIdentifier { get; }

        /// <summary>Send a packet of the given type with the requested delivery semantics.</summary>
        void SendPacket(PacketType packetType, Packet packet, DeliveryMethod deliveryMethod);

        /// <summary>Disconnect with an optional reason string.</summary>
        void Disconnect(string reason = "");
    }
}
