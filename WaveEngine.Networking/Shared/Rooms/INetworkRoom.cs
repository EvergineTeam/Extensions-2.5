// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Networking.Players;
#endregion

namespace WaveEngine.Networking.Rooms
{
    /// <summary>
    /// This interface represents a network room
    /// </summary>
    public interface INetworkRoom : IRoomInfo
    {
        #region Properties

        /// <summary>
        /// Gets the Id of the player who's the master of this Room.
        /// Note: This changes when the current master leaves the room.
        /// </summary>
        int MasterClientId { get; }

        /// <summary>
        /// Gets a value indicating whether the room is listed in its lobby.
        /// </summary>
        /// <remarks>
        /// Rooms can be created invisible, or changed to invisible.
        /// </remarks>
        bool IsVisible { get; }

        /// <summary>
        /// Gets the custom properties of the room.
        /// Those properties are synchronized by the server automatically.
        /// </summary>
        /// <remarks>
        /// All players inside the room can change these properties.
        /// Setting properties in this table updates the server and other players.
        ///
        /// Keys in the table are bytes to reduce network traffic. It is recommended
        /// to use a custom enumerator for better clarity.
        /// </remarks>
        /// <example>
        /// <code>
        /// var matchmakingClient = WaveEngine.GetService&lt;MatchmakingClientService&gt;();
        /// var roomProperties = matchmakingClient.CurrentRoom.CustomProperties;
        /// roomProperties.Set((byte)CustomEnum.MapLevel, "cs_italy");
        /// var mapLevel = roomProperties.GetString((byte)CustomEnum.MapLevel);
        /// </code>
        /// </example>
        NetworkPropertiesTable CustomProperties { get; }

        /// <summary>
        /// Gets the "list" of all the players who are in that room (including the local player). Only updated while inside a Room.
        /// </summary>
        IEnumerable<INetworkPlayer> AllPlayers { get; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the master client id changes
        /// </summary>
        event EventHandler MasterClientIdChanged;

        #endregion
    }
}
