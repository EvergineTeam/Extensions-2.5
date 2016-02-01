#region File Description
//-----------------------------------------------------------------------------
// OculusVRProvider
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OculusWrap;
using System;
using System.Runtime.Serialization;
using WaveEngine.DirectX;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Components.VR;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    [DataContract]
    public class OculusVRProvider : VRProvider
    {
        /// <summary>
        /// The application
        /// </summary>
        private OculusVRService ovrService;

        /// <summary>
        /// Show OculusRift HMD texture
        /// </summary>
        [DataMember]
        private bool showHMDMirrorTexture;

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        [DontRenderProperty]
        public override bool IsConnected
        {
            get
            {
                return this.ovrService != null && this.ovrService.IsConnected;
            }
        }

        /// <summary>
        /// Gets the eye poses
        /// </summary>
        [DontRenderProperty]
        public override VREyePose[] EyePoses
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.EyePoses;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        [DontRenderProperty]
        public override VREyePose TrackerCameraPose
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.TrackerCameraPose;
            }
        }

        /// <summary>
        /// Gets the eye textures information.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        [DontRenderProperty]
        public override VREyeTexture[] EyeTextures
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.EyeTextures;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the composed image of the HMD will be rendered onto the screen.
        /// </summary>
        public bool ShowHMDMirrorTexture
        {
            get
            {
                return this.showHMDMirrorTexture;
            }

            set
            {
                this.showHMDMirrorTexture = value;

                if (this.IsConnected)
                {
                    this.ovrService.ShowHMDMirrorTexture = this.showHMDMirrorTexture;
                }
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

                return this.ovrService.HMDMirrorRenderTarget;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="OculusVRProvider"/> class.
        /// </summary>        
        public OculusVRProvider()
        {
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.showHMDMirrorTexture = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update the provider
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            if (this.IsConnected)
            {
                if (this.cameraRig.LeftEyeCamera != null)
                {
                    this.cameraRig.LeftEyeCamera.SetCustomProjection(this.EyePoses[(int)VREyeType.LeftEye].Projection);
                }

                if (this.cameraRig.RightEyeCamera != null)
                {
                    this.cameraRig.RightEyeCamera.SetCustomProjection(this.EyePoses[(int)VREyeType.RightEye].Projection);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        /// <exception cref="System.Exception">Invalid configure rendering</exception>
        protected override void Initialize()
        {
            base.Initialize();

            this.ovrService = WaveServices.GetService<OculusVRService>();

            if (this.IsConnected)
            {
                this.ovrService.ShowHMDMirrorTexture = this.showHMDMirrorTexture;
            }
        }
        #endregion
    }
}
