#region File Description
//-----------------------------------------------------------------------------
// NetworkSyncType
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network sync type
    /// </summary>
    internal enum NetworkSyncType
    {
        /// <summary>
        /// The start
        /// </summary>
        Start,

        /// <summary>
        /// The update
        /// </summary>
        Update,

        /// <summary>
        /// The create
        /// </summary>
        Create,

        /// <summary>
        /// The remove
        /// </summary>
        Remove
    }
}
