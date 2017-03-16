#region File Description
//-----------------------------------------------------------------------------
// HololensVRProvider
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.VR;
using WaveEngine.Components.VR;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Hololens
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    [DataContract]
    public class HololensProvider : VRProvider
    {
        /// <summary>
        /// The application
        /// </summary>
        private HololensService hololensService;

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
                return this.hololensService != null && this.hololensService.IsConnected;
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
                    throw new Exception("Hololens is not available. See console output to find the reason");
                }

                return this.hololensService.EyePoses;
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
                    throw new Exception("Hololens is not available. See console output to find the reason");
                }

                return this.hololensService.TrackerCameraPose;
            }
        }

        /// <summary>
        /// Gets the left controller pose
        /// </summary>
        [DontRenderProperty]
        public override VREyePose LeftControllerPose
        {
            get;
        }

        /// <summary>
        /// Gets the right controller pose
        /// </summary>
        [DontRenderProperty]
        public override VREyePose RightControllerPose
        {
            get;
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
                    throw new Exception("Hololens is not available. See console output to find the reason");
                }

                return this.hololensService.EyeTextures;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="HololensProvider"/> class.
        /// </summary>        
        public HololensProvider()
        {
            UpdateOrder = 0;
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
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
                this.hololensService.WorldToTrackingSpace = this.cameraRig.TrackingSpaceTransform.WorldInverseTransform;

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

        public void DisableStabilizationPlane()
        {
            this.DisableStabilizationPlane();
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

            this.hololensService = WaveServices.GetService<HololensService>();
        }
        #endregion
    }
}
