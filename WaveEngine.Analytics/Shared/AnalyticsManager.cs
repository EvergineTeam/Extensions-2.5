#region File Description
//-----------------------------------------------------------------------------
// AnalyticsManager
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using WaveEngine.Common;
using System.Diagnostics;
#endregion

namespace WaveEngine.Analytics
{
    /// <summary>
    /// This class management the analytics systems.
    /// </summary>
    public class AnalyticsManager : Service
    {
        /// <summary>
        /// Selected analytics system.
        /// </summary>
        protected AnalyticsSystem analyticsSystem;

        /// <summary>
        /// Handle to adapter.
        /// </summary>
        private IAdapter adapter;

        /// <summary>
        /// Analytics is open.
        /// </summary>
        private bool isOpen;

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen
        {
            get { return this.isOpen; }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsManager"/> class.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        public AnalyticsManager(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the analytics system.
        /// </summary>
        /// <param name="analyticsInfo">The analytics info.</param>        
        public void SetAnalyticsSystem(AnalyticsInfo analyticsInfo)
        {
            var args = new object[] { this.adapter, analyticsInfo };
            this.analyticsSystem = Activator.CreateInstance(analyticsInfo.InternalType, args) as AnalyticsSystem;
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>        
        public void Open()
        {
            if (this.analyticsSystem == null)
            {
                throw new InvalidOperationException("You need set an Analytics System using SetAnalyticsSystem.");
            }

            if (this.isOpen)
            {
                throw new InvalidOperationException("There is a session open right now.");
            }

            this.isOpen = true;
            this.analyticsSystem.Open();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>        
        public void Close()
        {
            if (this.analyticsSystem == null)
            {
                throw new InvalidOperationException("You need set an Analytics System using SetAnalyticsSystem.");
            }

            this.isOpen = false;
            this.analyticsSystem.Close();
        }

        /// <summary>
        /// Uploads this instance.
        /// </summary>        
        public void Upload()
        {
            if (this.analyticsSystem == null)
            {
                throw new InvalidOperationException("You need set an Analytics System using SetAnalyticsSystem.");
            }

            this.analyticsSystem.Upload();
        }

        /// <summary>
        /// Tags the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>       
        public void TagEvent(string eventName, string attribute, string value)
        {
            if (this.analyticsSystem == null)
            {
                throw new InvalidOperationException("You need set an Analytics System using SetAnalyticsSystem.");
            }

            this.analyticsSystem.TagEvent(eventName, attribute, value);
        }

        /// <summary>
        /// Tags the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="attributes">The attributes.</param>        
        public void TagEvent(string eventName, Dictionary<string, string> attributes)
        {
            if (this.analyticsSystem == null)
            {
                throw new InvalidOperationException("You need set an Analytics System using SetAnalyticsSystem.");
            }

            this.analyticsSystem.TagEvent(eventName, attributes);
        }
        #endregion
    }
}
