// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Connection.Messages
{
    /// <summary>
    /// Message type.
    /// </summary>
    public enum MessageType : byte
    {
        /// <summary>
        /// Data message type.
        /// </summary>
        Data,

        /// <summary>
        /// Synchronization message type.
        /// </summary>
        Synchronization
    }
}
