// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using WaveEngine.Common;
#endregion

namespace WaveEngine.Analytics
{
    /// <summary>
    /// This class represent a generic analytics system.
    /// </summary>
    public abstract class AnalyticsSystem
    {
        #region Properties
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsSystem"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        public AnalyticsSystem(AnalyticsInfo info)
        {
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Uploads this instance.
        /// </summary>
        public abstract void Upload();

        /// <summary>
        /// Tags the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public abstract void TagEvent(string eventName, string attribute, string value);

        /// <summary>
        /// Tags the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="attributes">The attributes.</param>
        public abstract void TagEvent(string eventName, Dictionary<string, string> attributes);

        #endregion
    }
}
