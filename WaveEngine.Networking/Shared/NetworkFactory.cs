#region File Description
//-----------------------------------------------------------------------------
// NetworkFactory
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// Factory to main network components
    /// </summary>
    public class NetworkFactory : INetworkFactory
    {
        /// <summary>
        /// Creates the network server.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <returns>
        /// A new network server.
        /// </returns>
        public INetworkServer CreateNetworkServer(string applicationIdentifier, int port)
        {
            return new NetworkServer(applicationIdentifier, port);
        }

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <returns>A new network client</returns>
        public INetworkClient CreateNetworkClient(string applicationIdentifier)
        {
            return new NetworkClient(applicationIdentifier);
        }
    }
}
