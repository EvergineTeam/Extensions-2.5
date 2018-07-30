// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Runtime.Serialization;
using WaveEngine.Networking.Client;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides <see cref="INetworkRoom.CustomProperties"/> of the current room neccessary for
    /// <see cref="NetworkPropertySync{K, V}"/> components
    /// </summary>
    [DataContract]
    public class NetworkRoomProvider : NetworkCustomPropertiesProvider
    {
        #region Initialize

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override NetworkPropertiesTable GetCustomProperties(MatchmakingClientService matchmakingClientService)
        {
            return matchmakingClientService.CurrentRoom?.CustomProperties;
        }

        #endregion
    }
}
