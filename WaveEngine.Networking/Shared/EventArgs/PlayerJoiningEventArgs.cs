// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Server.Players;
using WaveEngine.Networking.Server.Rooms;
#endregion

namespace WaveEngine.Networking.Server
{
    /// <summary>
    /// Represents the arguments of the message received when a player is joining to a room.
    /// </summary>
    public class PlayerJoiningEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the joining request has been rejected.
        /// </summary>
        public bool IsRejected { get; private set; }

        /// <summary>
        /// Gets the room that player is trying to join.
        /// </summary>
        public ServerRoom Room { get; private set; }

        /// <summary>
        /// Gets the player that is joining.
        /// </summary>
        public ServerPlayer Player { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerJoiningEventArgs" /> class.
        /// </summary>
        /// <param name="room">The room that player is trying to join.</param>
        /// <param name="player">tThe player that is joining.</param>
        public PlayerJoiningEventArgs(ServerRoom room, ServerPlayer player)
        {
            this.Room = room;
            this.Player = player;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mark the joining request as rejected.
        /// </summary>
        public void Reject()
        {
            this.IsRejected = true;
        }

        #endregion
    }
}
