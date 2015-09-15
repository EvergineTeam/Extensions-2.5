#region File Description
//-----------------------------------------------------------------------------
// Host
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System.Net;
using Lidgren.Network;

namespace WaveEngine.Networking
{
    /// <summary>
    /// This class represent a host.
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        public Host()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class from a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public Host(NetIncomingMessage message)
        {
            this.Address = message.SenderEndPoint.Address.ToString();
            this.Port = message.SenderEndPoint.Port;
        }
    }
}