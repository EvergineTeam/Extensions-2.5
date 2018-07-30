// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Factory to main network components
    /// </summary>
    public class NetworkFactory : INetworkFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates the network server.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <param name="pingInterval">Ping interval in seconds.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        /// <returns>
        /// A new network server.
        /// </returns>
        public INetworkServer CreateNetworkServer(string applicationIdentifier, int port, float pingInterval, float connectionTimeout)
        {
            return new NetworkServer(applicationIdentifier, port, pingInterval, connectionTimeout);
        }

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="pingInterval">Ping interval in seconds.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        /// <returns>
        /// A new network client
        /// </returns>
        public INetworkClient CreateNetworkClient(string applicationIdentifier, float pingInterval, float connectionTimeout)
        {
            return new NetworkClient(applicationIdentifier, pingInterval, connectionTimeout);
        }

        #endregion
    }
}
