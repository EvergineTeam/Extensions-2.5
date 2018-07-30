// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Players
{
    /// <summary>
    /// This interface defines a network player
    /// </summary>
    public interface INetworkPlayer
    {
        #region Properties

        /// <summary>
        /// Gets an identifier of this player in current room. It's -1 outside of rooms.
        /// </summary>
        /// <remarks>
        /// The Id is assigned per room and only valid in that context. It will change even on leave and re-join.
        /// Ids are never re-used per room.
        /// </remarks>
        int Id { get; }

        /// <summary>
        /// Gets a non-unique nickname of this player. Synced automatically in a room.
        /// </summary>
        /// <remarks>
        /// A player might change his own nickname in a room.
        /// Setting this value updates the server and other players.
        /// </remarks>
        string Nickname { get; }

        /// <summary>
        /// Gets the custom properties of the player.
        /// Those properties are synchronized by the server automatically.
        /// </summary>
        /// <remarks>
        /// Only the local player might change his own custom properties in a room.
        /// Setting properties in this table updates the server and other players.
        ///
        /// Keys in the table are bytes to reduce network traffic. It is recommended
        /// to use a custom enumerator for better clarity.
        /// </remarks>
        /// <example>
        /// <code>
        /// var matchmakingClient = WaveEngine.GetService&lt;MatchmakingClientService&gt;();
        /// var playerProperties = matchmakingClient.LocalPlayer.CustomProperties;
        /// playerProperties.Set((byte)CustomEnum.PlayerPosition, Vector3.Zero);
        /// var position = playerProperties.GetVector3((byte)CustomEnum.PlayerPosition);
        /// </code>
        /// </example>
        NetworkPropertiesTable CustomProperties { get; }

        /// <summary>
        /// Gets or sets an object that can be used to store a reference that's useful to know "by player".
        /// This property is not sync by the server.
        /// </summary>
        /// <remarks>
        /// Example: Set a player's character as Tag by assigning its Entity or EntityPath.
        /// </remarks>
        object TagObject { get; set; }

        /// <summary>
        /// Gets a value indicating whether this player is the Master Client of the current room or not.
        /// </summary>
        /// <remarks>
        /// Can be used as "authoritative" client/player to make decisions, run AI or other.
        /// If the current Master Client leaves the room (leave/disconnect), the server will quickly assign someone else.
        /// If the current Master Client times out (closed app, lost connection, etc), messages sent to this client are
        /// effectively lost for the others!
        /// </remarks>
        bool IsMasterClient { get; }

        /// <summary>
        /// Gets a value indicating whether this player is in the lobby (not in a room).
        /// </summary>
        bool IsInLobby { get; }

        /// <summary>
        /// Gets a value indicating whether this player is the local player.
        /// </summary>
        bool IsLocalPlayer { get; }

        /// <summary>
        /// Gets the room where is the player. It is null if the player is in the lobby.
        /// </summary>
        INetworkRoom Room { get; }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the server updates the player nickname.
        /// </summary>
        event EventHandler OnNicknameChanged;

        /// <summary>
        /// Event raised when the server updates custom properties.
        /// </summary>
        event EventHandler OnCustomPropertiesChanged;

        #endregion
    }
}
