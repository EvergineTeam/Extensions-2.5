#region File Description
//-----------------------------------------------------------------------------
// INetworkFactory
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// This class is a factory of main network components.
    /// </summary>
    public interface INetworkFactory
    {
        /// <summary>
        /// Creates the network server.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <returns>
        /// A network server instance.
        /// </returns>
        INetworkServer CreateNetworkServer(string applicationIdentifier, int port);

        /// <summary>
        /// Creates the network client.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <returns>A network client instance.</returns>
        INetworkClient CreateNetworkClient(string applicationIdentifier);
    }
}
