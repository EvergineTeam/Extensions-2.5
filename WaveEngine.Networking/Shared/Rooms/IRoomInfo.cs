// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
#endregion

namespace WaveEngine.Networking.Rooms
{
    /// <summary>
    /// Defines the basic information of a network room
    /// </summary>
    public interface IRoomInfo
    {
        #region Properties

        /// <summary>
        /// Gets the name of a room. Unique identifier for a room/match (per AppId + game-Version).
        /// </summary>
        /// <remarks>
        /// The name can't be changed once it's set by the matchmaking server.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Gets the count of players currently in room.
        /// </summary>
        byte PlayerCount { get; }

        /// <summary>
        /// Gets a value indicating whether the room is full and therefore, no more players can join it.
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Gets a set of string properties that are in the RoomInfo of the Lobby.
        /// This list is defined when creating the room and can't be changed afterwards.
        /// </summary>
        /// <remarks>
        /// You could name properties that are not set from the beginning. Those will be synchronized with the lobby when added later on.
        /// </remarks>
        HashSet<string> PropertiesListedInLobby { get; }

        /// <summary>
        /// Gets the limit of players for this room. This property is shown in lobby, too.
        /// If the room is full (players count == maxplayers), joining this room will fail.
        /// </summary>
        /// <remarks>
        /// If the player has joined the room, the setter will update the server and all clients.
        /// </remarks>
        byte MaxPlayers { get; }

        #endregion
    }
}
