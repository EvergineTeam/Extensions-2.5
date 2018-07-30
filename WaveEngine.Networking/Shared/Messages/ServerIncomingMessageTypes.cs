// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// Incoming message types received by the server
    /// </summary>
    internal enum ServerIncomingMessageTypes : byte
    {
        // TODO: Document this
#pragma warning disable SA1602 // Enumeration items must be documented
        SetPlayerProperties,
        SetRoomProperties,

        CreateRoomRequest,
        JoinOrCreateRoomRequest,
        JoinRoomRequest,
        LeaveRoomRequest,

        UserDataToHost,
        UserDataToRoom,
        UserDataToOtherClient,
#pragma warning restore SA1602 // Enumeration items must be documented
    }
}
