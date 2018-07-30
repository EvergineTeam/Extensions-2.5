// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Networking.Client;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides the <see cref="NetworkPropertiesTable"/> neccessary for
    /// <see cref="NetworkPropertySync{K, V}"/> components
    /// </summary>
    [DataContract]
    public abstract class NetworkCustomPropertiesProvider : Component, IDisposable
    {
        /// <summary>
        /// The <see cref="MatchmakingClientService"/> internal reference
        /// </summary>
        [RequiredService]
        private MatchmakingClientService matchmakingClientService = null;

        /// <summary>
        /// Gets the <see cref="NetworkPropertiesTable"/>
        /// </summary>
        [DontRenderProperty]
        public NetworkPropertiesTable CustomProperties
        {
            get;
            private set;
        }

        #region Events

        /// <summary>
        /// Occurs when the <see cref="CustomProperties"/> property is changed. Previous properties table is received
        /// as parameter.
        /// </summary>
        public event EventHandler<NetworkPropertiesTable> CustomPropertiesRefreshed;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            this.UnsubscribeNetworkEvents();
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.matchmakingClientService.StateChanged += this.MatchmakingClient_StateChanged;
            this.UpdateCustomPropertiesReference();
        }

        /// <inheritdoc />
        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.UnsubscribeNetworkEvents();
        }

        /// <summary>
        /// Updates the custom properties reference
        /// </summary>
        protected void UpdateCustomPropertiesReference()
        {
            var newTable = this.GetCustomProperties(this.matchmakingClientService);

            var previousTable = this.CustomProperties;
            if (previousTable != newTable)
            {
                this.CustomProperties = newTable;
                this.CustomPropertiesRefreshed?.Invoke(this, previousTable);
            }
        }

        /// <summary>
        /// Gets the <see cref="NetworkPropertiesTable"/> instance
        /// </summary>
        /// <param name="matchmakingClientService">The <see cref="MatchmakingClientService"/> reference</param>
        /// <returns>The <see cref="NetworkPropertiesTable"/> instance</returns>
        protected abstract NetworkPropertiesTable GetCustomProperties(MatchmakingClientService matchmakingClientService);

        private void MatchmakingClient_StateChanged(object sender, ClientStates state)
        {
            if (state == ClientStates.Joined)
            {
                this.UpdateCustomPropertiesReference();
            }
        }

        private void UnsubscribeNetworkEvents()
        {
            if (this.matchmakingClientService != null)
            {
                this.matchmakingClientService.StateChanged -= this.MatchmakingClient_StateChanged;
            }
        }

        #endregion
    }
}
