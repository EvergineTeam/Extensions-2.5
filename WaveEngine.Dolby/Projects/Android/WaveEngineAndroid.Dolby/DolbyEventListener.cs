#region File Description
//-----------------------------------------------------------------------------
// DolbyEventListener
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using Com.Dolby.Dap;
using System;
#endregion

namespace WaveEngine.Dolby
{
    /// <summary>
    /// Dolby Event Listener
    /// </summary>
    public class DolbyEventListener : Java.Lang.Object, IOnDolbyAudioProcessingEventListener
    {
        /// <summary>
        /// Gets or sets a value indicating wether the client is connected
        /// </summary>
        public bool IsConnectedClient
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets a value indicating wether the audio processing is enabled 
        /// </summary>
        public bool AudioProcessingEnabled
        {
            get; private set;
        }

        /// <summary>
        /// Current Selected Profile
        /// </summary>
        public DolbyAudioProcessing.PROFILE CurrentSelectedProfile
        {
                get; private set;
        }

        /// <summary>
        /// Called when [dolby audio processing client connected].
        /// </summary>
        public void OnDolbyAudioProcessingClientConnected()
        {
            this.IsConnectedClient = true;
        }

        /// <summary>
        /// Called when [dolby audio processing client disconnected].
        /// </summary>
        public void OnDolbyAudioProcessingClientDisconnected()
        {
            this.IsConnectedClient = false;
        }

        /// <summary>
        /// Called when [dolby audio processing enabled].
        /// </summary>
        /// <param name="p0">if set to <c>true</c> [p0].</param>
        public void OnDolbyAudioProcessingEnabled(bool p0)
        {
            this.AudioProcessingEnabled = p0;
        }

        /// <summary>
        /// Called when [dolby audio processing profile selected].
        /// </summary>
        /// <param name="p0">The p0.</param>
        public void OnDolbyAudioProcessingProfileSelected(DolbyAudioProcessing.PROFILE p0)
        {
            this.CurrentSelectedProfile = p0;
        }
    }
}
