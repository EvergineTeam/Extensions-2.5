// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// This class is a factory of main network components.
    /// </summary>
    public interface INetworkFactory
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
        /// A network server instance.
        /// </returns>
        INetworkServer CreateNetworkServer(string applicationIdentifier, int port, float pingInterval, float connectionTimeout);

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="pingInterval">Ping interval in seconds.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        /// <returns>
        /// A network client instance.
        /// </returns>
        INetworkClient CreateNetworkClient(string applicationIdentifier, float pingInterval, float connectionTimeout);

        #endregion
    }
}
