

#region Usings Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WaveEngine.Dolby;
using Com.Dolby.Dap;
#endregion

namespace WaveEngineAndroid.Dolby
{
    /// <summary>
    /// Dolby Audio Processing Wave Service
    /// </summary>
    public class DolbyServiceAndroid : WaveEngine.Common.Service, IDolbyService, IDisposable
    {
        /// <summary>
        /// The dolby audio processing object controller
        /// </summary>
        private DolbyAudioProcessing dolbyAudioProcessing;

        /// <summary>
        /// The dolby event listener
        /// </summary>
        private DolbyEventListener dolbyEventListener;

        /// <summary>
        /// The current profile
        /// </summary>
        private DolbyProfile currentProfile = DolbyProfile.MUSIC;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                bool isEnable = false;

                if (this.dolbyAudioProcessing != null && this.dolbyEventListener.IsConnectedClient)
                {
                    isEnable = this.dolbyAudioProcessing.Enabled;
                }

                return isEnable;
            }

            set
            {
                this.EnableDolby(value);
            }
        }

        /// <summary>
        /// Gets or sets the dolby profile.
        /// </summary>
        /// <value>
        /// The dolby profile.
        /// </value>
        public DolbyProfile DolbyProfile
        {
            get
            {
                return this.currentProfile;
            }

            set
            {
                if (this.IsEnabled)
                {
                    this.currentProfile = value;
                    this.dolbyAudioProcessing.SetProfile(this.FromEnum(this.currentProfile));
                }
            }
        }

        /// <summary>
        /// Creates the dolby.
        /// </summary>
        public void StartDolbySession()
        {
            // We need the CreateDolby Method cause on application lifecycle the method Initialize of the Service is only called once.
            // so it is needed to 
            if (dolbyAudioProcessing == null)
            {
                this.dolbyEventListener = new DolbyEventListener();
                this.dolbyAudioProcessing = DolbyAudioProcessing.GetDolbyAudioProcessing(Application.Context, this.FromEnum(this.currentProfile), this.dolbyEventListener);
            }
        }

        /// <summary>
        /// Releases the dolby.
        /// </summary>
        public void ReleaseDolbySession()
        {
            if (this.dolbyAudioProcessing != null)
            {
                this.dolbyAudioProcessing.Release();
            }
        }

        /// <summary>
        /// Restarts the session.
        /// </summary>
        public void RestartDolbySession()
        {
            if (this.dolbyAudioProcessing != null)
            {
                this.dolbyAudioProcessing.RestartSession();
            }
        }

        /// <summary>
        /// Suspends the dolby session.
        /// </summary>
        public void SuspendDolbySession()
        {
            if (this.dolbyAudioProcessing != null)
            {
                this.dolbyAudioProcessing.SuspendSession();
            }
        }

        /// <summary>
        /// Enables the dolby.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        private bool EnableDolby(bool enabled)
        {
            bool isOK = false;

            if (this.dolbyAudioProcessing != null)
            {
                this.dolbyAudioProcessing.Enabled = enabled;
                isOK = true;
            }

            return isOK;
        }

        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.StartDolbySession();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (dolbyAudioProcessing != null)
            {
                this.ReleaseDolbySession();
                this.dolbyAudioProcessing.Dispose();
                this.dolbyAudioProcessing = null;
            }
        }

        /// <summary>
        /// Froms the enum.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <returns></returns>
        private DolbyAudioProcessing.PROFILE FromEnum(DolbyProfile profile)
        {
            DolbyAudioProcessing.PROFILE result;

            switch (profile)
            {
                case DolbyProfile.GAME:
                    result = DolbyAudioProcessing.PROFILE.Game;
                    break;
                case DolbyProfile.MOVIE:
                    result = DolbyAudioProcessing.PROFILE.Movie;
                    break;
                case DolbyProfile.MUSIC:
                    result = DolbyAudioProcessing.PROFILE.Music;
                    break;
                case DolbyProfile.VOICE:
                    result = DolbyAudioProcessing.PROFILE.Voice;
                    break;
                default:
                    result = DolbyAudioProcessing.PROFILE.Music;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Close session.
        /// </summary>
        protected override void Terminate()
        {
            this.ReleaseDolbySession();
        }
    }
}