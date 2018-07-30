// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Globalization;
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// This class represent a network endpoint.
    /// </summary>
    public class NetworkEndpoint
    {
        #region Properties

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkEndpoint"/> class.
        /// </summary>
        /// <param name="address">The IP address</param>
        /// <param name="port">The port</param>
        public NetworkEndpoint(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as NetworkEndpoint;
            return other != null && other.Address.Equals(this.Address) && other.Port.Equals(this.Port);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 29) + this.Address.GetHashCode();
            hash = (hash * 29) + this.Port.GetHashCode();
            return hash;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Address}:{this.Port}";
        }

        /// <summary>
        /// Converts an IP address string with port to a <see cref="NetworkEndpoint"/> instance.
        /// </summary>
        /// <param name="epString">A string  that contains an IP address in dotted-quad notation for IPv4 followed by colon and the port number</param>
        /// <returns>A <see cref="NetworkEndpoint"/> instance.</returns>
        public static NetworkEndpoint Parse(string epString)
        {
            string[] ep = epString.Split(':');
            if (ep.Length < 2)
            {
                throw new FormatException("Invalid endpoint format");
            }

            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }

            return new NetworkEndpoint(ep[0], port);
        }

        #endregion
    }
}
