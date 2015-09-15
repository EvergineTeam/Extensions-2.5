#region File Description
//-----------------------------------------------------------------------------
// DeliveryMethod
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// How the library deals with resends and handling of late messages
    /// </summary>
    public enum DeliveryMethod : byte
    {
        /// <summary>
        /// Indicates an error
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Unreliable, unordered delivery
        /// </summary>
        Unreliable = 1,

        /// <summary>
        /// Unreliable delivery, but automatically dropping late messages
        /// </summary>
        UnreliableSequenced = 2,

        /// <summary>
        /// Reliable delivery, but unordered
        /// </summary>
        ReliableUnordered = 34,

        /// <summary>
        /// Reliable delivery, except for late messages which are dropped
        /// </summary>
        ReliableSequenced = 35,

        /// <summary>
        /// Reliable, ordered delivery
        /// </summary>
        ReliableOrdered = 67,
    }
}
