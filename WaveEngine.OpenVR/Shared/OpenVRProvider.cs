// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Components.VR;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR;
using WaveEngine.OpenVR.Input;
#endregion

namespace WaveEngine.OpenVR
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    [DataContract]
    public class OpenVRProvider : VRProvider
    {
        /// <summary>
        /// The application
        /// </summary>
        private OpenVRService openVRService;

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
                return this.openVRService != null && this.openVRService.IsConnected;
            }
        }

        /// <summary>
        /// Gets the eye properties
        /// </summary>
        [DontRenderProperty]
        public override VREye[] EyesProperties
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.EyesProperties;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        [DontRenderProperty]
        public override VRPose TrackerCameraPose
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                if (this.openVRService.BaseStations.Length > 0)
                {
                    return this.openVRService.BaseStations[0].Pose;
                }
                else
                {
                    return default(VRPose);
                }
            }
        }

        /// <summary>
        /// Gets the left controller index
        /// </summary>
        [DontRenderProperty]
        public override int LeftControllerIndex
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.LeftControllerIndex;
            }
        }

        /// <summary>
        /// Gets the right controller index
        /// </summary>
        [DontRenderProperty]
        public override int RightControllerIndex
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.RightControllerIndex;
            }
        }

        /// <summary>
        /// Gets the controller state list
        /// </summary>
        public override VRGenericControllerState[] ControllerStates
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.GenericControllers;
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
                    this.openVRService.ShowHMDMirrorTexture = this.showHMDMirrorTexture;
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

                return this.openVRService.HMDMirrorRenderTarget;
            }
        }

        /// <summary>
        /// Gets the state of the HTC Vive controller.
        /// </summary>
        /// <value>
        /// The state of the HTC Vive controller.
        /// </value>
        public OpenVRControllerState[] OpenVRControllers
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.Controllers;
            }
        }

        public override GamePadState GamepadState
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.openVRService.GamePadState;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenVRProvider"/> class.
        /// </summary>
        public OpenVRProvider()
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
                    this.cameraRig.LeftEyeCamera.SetCustomProjection(this.EyesProperties[(int)VREyeType.LeftEye].Projection);
                }

                if (this.cameraRig.RightEyeCamera != null)
                {
                    this.cameraRig.RightEyeCamera.SetCustomProjection(this.EyesProperties[(int)VREyeType.RightEye].Projection);
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

            this.openVRService = WaveServices.GetService<OpenVRService>();

            if (this.IsConnected)
            {
                this.openVRService.ShowHMDMirrorTexture = this.showHMDMirrorTexture;
            }
        }
        #endregion
    }
}
