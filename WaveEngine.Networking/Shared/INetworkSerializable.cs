// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// This interface defines a network serializable object
    /// </summary>
    public interface INetworkSerializable
    {
        #region Public Methods

        /// <summary>
        /// Read this instance fields from a network buffer.
        /// </summary>
        /// <param name="buffer">The network buffer</param>
        void Read(NetBuffer buffer);

        /// <summary>
        /// Writes this instance fields to a network buffer.
        /// </summary>
        /// <param name="buffer">The network buffer</param>
        void Write(NetBuffer buffer);

        #endregion
    }
}
