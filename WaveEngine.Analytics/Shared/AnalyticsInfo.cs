#region File Description
//-----------------------------------------------------------------------------
// AnalyticsInfo
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace WaveEngine.Analytics
{
    /// <summary>
    /// This class represent a basic information for a analytics system.
    /// </summary>
    public abstract class AnalyticsInfo
    {
        /// <summary>
        /// Gets the type of the internal.
        /// </summary>
        /// <value>
        /// The type of the internal.
        /// </value>
        public Type InternalType { get; private set; }

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsInfo"/> class.
        /// </summary>
        /// <param name="internalType">Type of the internal.</param>
        public AnalyticsInfo(Type internalType)
        {
            this.InternalType = internalType;
        }
        #endregion
    }
}
