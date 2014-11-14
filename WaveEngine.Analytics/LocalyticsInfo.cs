#region File Description
//-----------------------------------------------------------------------------
// LocalyticsInfo
//
// Copyright © 2014 Wave Corporation
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
    /// This class represent the basic info necesary to connect with localytics.
    /// </summary>
    public class LocalyticsInfo : AnalyticsInfo
    {
        /// <summary>
        /// Gets the app key.
        /// </summary>
        /// <value>
        /// The app key.
        /// </value>
        public string AppKey { get; private set; }

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalyticsInfo" /> class.
        /// </summary>
        /// <param name="appId">The app id from http://www.localytics.com/.</param>
        public LocalyticsInfo(string appId)
            : base(typeof(Localytics))
        {
            this.AppKey = appId;
        }
        #endregion
    }
}
