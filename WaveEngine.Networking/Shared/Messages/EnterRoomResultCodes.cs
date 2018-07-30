// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// Incoming enter room result codes received by the client
    /// </summary>
    public enum EnterRoomResultCodes : byte
    {
        /// <summary>
        /// The join or create operation was succeeded.
        /// </summary>
        Succeed,

        /// <summary>
        /// The room specified by the join opeartion does not exists.
        /// </summary>
        RoomNotExists,

        /// <summary>
        /// The room specified by the create operation already exists.
        /// </summary>
        RoomAlreadyExists,

        /// <summary>
        /// The room operation by the join operation is full.
        /// </summary>
        RoomIsFull,

        /// <summary>
        /// The join operation has been rejected by the server.
        /// </summary>
        Rejected,

        /// <summary>
        /// The join or create operation has been aborted by the client.
        /// </summary>
        Aborted
    }
}
