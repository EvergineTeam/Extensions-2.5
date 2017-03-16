#region File Description
//-----------------------------------------------------------------------------
// NetworkSyncComponent
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// Base class for netwrok sync components
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Networking")]
    public abstract class NetworkSyncComponent : Component
    {
        /// <summary>
        /// Reads the synchronize data.
        /// </summary>
        /// <param name="binaryReader">The binary reader.</param>
        public abstract void ReadSyncData(IncomingMessage binaryReader);

        /// <summary>
        /// Needs the send synchronize data.
        /// </summary>
        /// <returns>If component need to sync</returns>
        public abstract bool NeedSendSyncData();

        /// <summary>
        /// Writes the synchronize data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public abstract void WriteSyncData(OutgoingMessage writer);
    }
}
