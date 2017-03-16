#region File Description
//-----------------------------------------------------------------------------
// DolbyService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common;
#endregion

namespace WaveEngine.Dolby
{
    /// <summary>
    /// Dolby Integration Service
    /// </summary>
    public class DolbyService : Service
    {
        /// <summary>
        /// Platform Specific Dolby Service
        /// </summary>
        private IDolbyService platformSpecificDolbyService;

        /// <summary>
        /// Gets the IsEnabled property
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                bool res = false;

                if (this.platformSpecificDolbyService != null)
                {
                    return this.platformSpecificDolbyService.IsEnabled;
                }

                return res;
            }

            set
            {
                if (this.platformSpecificDolbyService != null)
                {
                    this.platformSpecificDolbyService.IsEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the DolbyProfile property
        /// </summary>
        public DolbyProfile DolbyProfile
        {
            get
            {
                return this.GetCurrentProfile();
            }
            set
            {
                this.SetCurrentProfile(value);
            }
        }

        /// <summary>
        /// Get Current Profile. MUSIC by default if not supported
        /// </summary>
        private DolbyProfile GetCurrentProfile()
        {
            DolbyProfile res = DolbyProfile.MUSIC;

            if (this.platformSpecificDolbyService != null)
            {
                res = this.platformSpecificDolbyService.DolbyProfile;
            }

            return res;
        }

        /// <summary>
        /// Sets the current profile
        /// </summary>
        /// <param name="profile"></param>
        private void SetCurrentProfile(DolbyProfile profile)
        {
            if (this.platformSpecificDolbyService != null)
            {
                this.platformSpecificDolbyService.DolbyProfile = profile;
            }
        }

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="DolbyService" /> class.
        /// </summary>
        public DolbyService()
        {
            this.platformSpecificDolbyService = null;
#if ANDROID
            this.platformSpecificDolbyService = new WaveEngineAndroid.Dolby.DolbyServiceAndroid();
#endif
        }

        /// <summary>
        /// Initialize service method
        /// </summary>
        protected override void Initialize()
        {
        }
        #endregion

        /// <summary>
        /// Terminate Service method
        /// </summary>
        protected override void Terminate()
        {
            if (this.platformSpecificDolbyService != null)
            {
                this.platformSpecificDolbyService.ReleaseDolbySession();
            }
        }

        /// <summary>
        /// Start Dolby Session
        /// </summary>
        public void StartDolbySession()
        {
            if (this.platformSpecificDolbyService != null)
            {
                this.platformSpecificDolbyService.StartDolbySession();
            }
        }

        /// <summary>
        /// Suspend Dolby Session
        /// </summary>
        public void SuspendDolbySession()
        {
            if (this.platformSpecificDolbyService != null)
            {
                this.platformSpecificDolbyService.SuspendDolbySession();
            }
        }

        /// <summary>
        /// Restart Dolby Session
        /// </summary>
        public void RestartDolbySession()
        {
            if (this.platformSpecificDolbyService != null)
            {
                this.platformSpecificDolbyService.RestartDolbySession();
            }
        }
    }
}
