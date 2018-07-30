// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Networking.Client;
using WaveEngine.Networking.Players;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides the <see cref="INetworkPlayer"/> and its <see cref="INetworkPlayer.CustomProperties"/>
    /// neccessary for <see cref="NetworkPropertySync{K, V}"/> components
    /// </summary>
    [DataContract]
    public class NetworkPlayerProvider : NetworkCustomPropertiesProvider
    {
        [DataMember]
        private int playerId;

        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the target player. If set to -1 the target will be the local player.
        /// </summary>
        [RenderPropertyAsInput(
            MinLimit = -1,
            Tooltip = "The identifier of the target player. If set to -1 the target will be the local player")]
        public int PlayerId
        {
            get
            {
                return this.playerId;
            }

            set
            {
                if (this.playerId != value)
                {
                    this.playerId = value;

                    if (this.isInitialized)
                    {
                        this.UpdateCustomPropertiesReference();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the target player
        /// </summary>
        [DontRenderProperty]
        public INetworkPlayer Player { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the <see cref="Player"/> property is changed. Previous player is received as parameter.
        /// </summary>
        public event EventHandler<INetworkPlayer> PlayerRefreshed;

        #endregion

        #region Initialize

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.playerId = -1;
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override NetworkPropertiesTable GetCustomProperties(MatchmakingClientService matchmakingClientService)
        {
            INetworkPlayer newPlayer;
            if (this.playerId == -1)
            {
                newPlayer = matchmakingClientService.LocalPlayer;
            }
            else
            {
                newPlayer = matchmakingClientService.CurrentRoom?.AllPlayers?.FirstOrDefault(player => player.Id == this.playerId);
            }

            var previousPlayer = this.Player;
            if (previousPlayer != newPlayer)
            {
                this.Player = newPlayer;
                this.PlayerRefreshed?.Invoke(this, previousPlayer);
            }

            return this.Player.CustomProperties;
        }

        #endregion
    }
}
