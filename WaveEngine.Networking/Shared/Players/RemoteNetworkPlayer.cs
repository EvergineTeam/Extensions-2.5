// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Players;
#endregion

namespace WaveEngine.Networking.Client.Players
{
    /// <summary>
    /// This class represents the remote players in the clients
    /// </summary>
    public class RemoteNetworkPlayer : BaseNetworkPlayer
    {
        /// <summary>
        /// A reference to the MatchmakingClientService which is currently keeping the connection and state.
        /// </summary>
        protected MatchmakingClientService matchmakingClientService;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteNetworkPlayer" /> class.
        /// </summary>
        private RemoteNetworkPlayer()
            : base(hasReadOnlyProperties: true)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteNetworkPlayer" /> class based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>A <see cref="RemoteNetworkPlayer" /> instance</returns>
        internal static RemoteNetworkPlayer FromMessage(IncomingMessage message)
        {
            var remotePlayer = new RemoteNetworkPlayer();
            remotePlayer.ReadFromMessage(message);
            return remotePlayer;
        }

        #endregion
    }
}
