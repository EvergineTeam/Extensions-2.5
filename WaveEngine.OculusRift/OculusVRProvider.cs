// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
using WaveEngine.Common.Input;
using WaveEngine.OculusRift.Input;
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

        /// <summary>
        /// Whether the tracking origin is at floor height.
        /// </summary>
        [DataMember]
        private bool trackingOriginAtFloorHeight;

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

                return this.ovrService.EyesProperties;
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

                return this.ovrService.TrackerCameraPose;
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

        /// <summary>
        /// Gets or sets a value indicating whether the tracking origin is at floor height.
        /// </summary>
        /// <value>
        /// Whether the tracking origin is at floor height.
        /// </value>
        public bool TrackingOriginAtFloorHeight
        {
            get
            {
                return this.trackingOriginAtFloorHeight;
            }

            set
            {
                this.trackingOriginAtFloorHeight = value;

                if (this.ovrService != null)
                {
                    this.ovrService.SetTrackingOriginAtFloorHeight(value);
                }
            }
        }

        /// <summary>
        /// Gets the state of the Oculus Touch controller.
        /// </summary>
        /// <value>
        /// The state of the Oculus Touch controller.
        /// </value>
        [DontRenderProperty]
        public OculusTouchControllerState OculusTouchControllerState
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.OculusTouchControllerState;
            }
        }

        /// <summary>
        /// Gets the state of the Oculus Remote controller.
        /// </summary>
        /// <value>
        /// The state of the Oculus Remote controller.
        /// </value>
        [DontRenderProperty]
        public OculusRemoteState OculusRemoteState
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.OculusRemoteState;
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

                return this.ovrService.LeftControllerIndex;
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

                return this.ovrService.RightControllerIndex;
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

                return this.ovrService.GenericControllerStates;
            }
        }

        /// <summary>
        /// Gets the state of the gamepad controller.
        /// </summary>
        /// <value>
        /// The state of the generic controller.
        /// </value>
        [DontRenderProperty]
        public override GamePadState GamepadState
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrService.GamePadState;
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

            this.ovrService = WaveServices.GetService<OculusVRService>();

            if (this.IsConnected)
            {
                this.ovrService.ShowHMDMirrorTexture = this.showHMDMirrorTexture;
                this.TrackingOriginAtFloorHeight = this.trackingOriginAtFloorHeight;
            }
        }
        #endregion
    }
}
