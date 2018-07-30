// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Players;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Client.Players
{
    /// <summary>
    /// This class represents the local player in the clients
    /// </summary>
    public class LocalNetworkPlayer : BaseSyncNetworkPlayer
    {
        #region Properties

        /// <summary>
        /// Gets the room where is the player. It is null if the player is in the lobby.
        /// </summary>
        public new LocalNetworkRoom Room
        {
            get
            {
                return (LocalNetworkRoom)base.Room;
            }
        }

        /// <summary>
        /// Gets the network endpoint of the local player.
        /// </summary>
        public NetworkEndpoint Endpoint { get; internal set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNetworkPlayer" /> class.
        /// </summary>
        public LocalNetworkPlayer()
            : base()
        {
            this.Nickname = $"Player_{Guid.NewGuid()}";
        }

        #endregion
    }
}
