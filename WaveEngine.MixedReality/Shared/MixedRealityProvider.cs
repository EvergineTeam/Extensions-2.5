// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Common.VR;
using WaveEngine.Components.VR;
using WaveEngine.Framework.Services;
using WaveEngine.MixedReality.Interaction;
#endregion

namespace WaveEngine.MixedReality
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    [DataContract]
    public class MixedRealityProvider : VRProvider
    {
        /// <summary>
        /// The application
        /// </summary>
        private MixedRealityService mixedRealityService;
        private SpatialInputService spatialInputService;

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
                return this.mixedRealityService != null && this.mixedRealityService.IsConnected;
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
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return this.mixedRealityService.EyeProperties;
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
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return VRPose.DefaultPose;
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
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return this.spatialInputService.LeftControllerIndex;
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
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return this.spatialInputService.RightControllerIndex;
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
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return this.spatialInputService.GenericControllerStates;
            }
        }

        /// <summary>
        /// Gets the state of the generic controller.
        /// </summary>
        /// <value>
        /// The state of the generic controller.
        /// </value>
        [DontRenderProperty]
        public override GamePadState GamepadState
        {
            get;
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedRealityProvider"/> class.
        /// </summary>
        public MixedRealityProvider()
        {
            this.UpdateOrder = 0;
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
                this.mixedRealityService.WorldToTrackingSpace = this.cameraRig.TrackingSpaceTransform.WorldInverseTransform;

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

        /// <summary>
        /// DisableStabilizationPlane
        /// </summary>
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

            this.mixedRealityService = WaveServices.GetService<MixedRealityService>();
            this.spatialInputService = WaveServices.GetService<SpatialInputService>();
        }
        #endregion
    }
}
