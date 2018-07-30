// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// Incoming message types received by the client
    /// </summary>
    internal enum ClientIncomingMessageTypes : byte
    {
        // TODO: Document this
#pragma warning disable SA1602 // Enumeration items must be documented
        RefreshRoomsInLobby,
        RefreshRoomInLobbyProperties,
        RefreshCurrentRoomProperties,
        RefreshPlayersInRoom,
        RefreshLocalPlayerProperties,
        RefreshOtherPlayerProperties,

        JoinResponse,
        CreateResponse,
        LeaveResponse,

        UserDataFromHost,
        UserDataFromRoom,
        UserDataFromOtherClient,
#pragma warning restore SA1602 // Enumeration items must be documented
    }
}
