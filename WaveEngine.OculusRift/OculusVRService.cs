#region File Description
//-----------------------------------------------------------------------------
// OculusVRService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OculusWrap;
using System;
using WaveEngine.Adapter.Graphics;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    public class OculusVRService : Service
    {
        /// <summary>
        /// The application
        /// </summary>
        private OculusVRApplication ovrApplication;

        #region Properties
        /// <summary>
        /// Gets the low level acces to the Oculus Rift instance.
        /// </summary>
        public Wrap Oculus
        {
            get
            {
                return this.ovrApplication.Oculus;
            }
        }

        /// <summary>
        /// Gets the low level acces to the Oculus Rift HMD instance.
        /// </summary>
        public Hmd Hmd
        {
            get
            {
                return this.ovrApplication.Hmd;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the composed image of the HMD will be rendered onto the screen.
        /// </summary>
        public bool ShowHMDMirrorTexture
        {
            get
            {
                return this.ovrApplication.ShowHMDMirrorTexture;
            }

            set
            {
                this.ovrApplication.ShowHMDMirrorTexture = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.ovrApplication != null && this.ovrApplication.IsConnected;
            }            
        }

        /// <summary>
        /// Gets the eye poses
        /// </summary>
        public VREyePose[] EyePoses
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrApplication.EyePoses;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        public VREyePose TrackerCameraPose
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrApplication.TrackerCameraPose;
            }
        }

        /// <summary>
        /// Gets the eye textures information.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        internal OculusVREyeTexture[] EyeTextures
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrApplication.EyeTextures;
            }
        }

        /// <summary>
        /// Gets the HMD mirror texture
        /// </summary>
        public RenderTarget HMDMirrorRenderTarget
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrApplication.HMDMirrorRenderTarget;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="OculusVRService"/> class.
        /// </summary>
        /// <param name="ovrApplication">The OVR application.</param>
        public OculusVRService(OculusVRApplication ovrApplication)
        {
            this.ovrApplication = ovrApplication;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        /// <exception cref="System.Exception">Invalid configure rendering</exception>
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
    }
}
