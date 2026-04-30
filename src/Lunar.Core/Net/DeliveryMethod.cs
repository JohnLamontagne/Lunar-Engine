namespace Lunar.Core.Net
{
    /// <summary>Reliability/ordering guarantees for outbound packets.</summary>
    public enum DeliveryMethod
    {
        /// <summary>Fire-and-forget. May arrive out of order or be dropped.</summary>
        Unreliable = 0,

        /// <summary>Each packet arrives exactly once but may be reordered.</summary>
        ReliableUnordered = 1,

        /// <summary>Packets arrive exactly once and in send order.</summary>
        ReliableOrdered = 2,

        /// <summary>Reliable, but only the latest packet on a channel is delivered; older ones discarded.</summary>
        ReliableSequenced = 3,

        /// <summary>Unreliable, but only the latest packet on a channel is delivered; older ones discarded.</summary>
        Sequenced = 4
    }
}
