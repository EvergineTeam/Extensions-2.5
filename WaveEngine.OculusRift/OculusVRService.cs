// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using OculusWrap;
using System;
using WaveEngine.DirectX;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.OculusRift.Input;
using WaveEngine.Common.Input;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    public class OculusVRService : UpdatableService
    {
        /// <summary>
        /// The left hand index
        /// </summary>
        private static readonly int LeftHandIndex = 0;

        /// <summary>
        /// The right hand index
        /// </summary>
        private static readonly int RightHandIndex = 1;

        /// <summary>
        /// The application
        /// </summary>
        private OculusVRApplication ovrApplication;

        /// <summary>
        /// The generic controller state
        /// </summary>
        private GamePadState gamePadstate;

        /// <summary>
        /// The empty controller array
        /// </summary>
        private VRGenericControllerState[] emptyControllersArray = new VRGenericControllerState[0];

        /// <summary>
        /// The generic controller array
        /// </summary>
        private VRGenericControllerState[] genericControllersArray = new VRGenericControllerState[2];

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
        /// Gets the eye properties
        /// </summary>
        public VREye[] EyesProperties
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("OVR is not available. See console output to find the reason");
                }

                return this.ovrApplication.EyesProperties;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        public VRPose TrackerCameraPose
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
        /// Gets the left controller index
        /// </summary>
        public int LeftControllerIndex
        {
            get
            {
                return this.OculusTouchControllerState.IsConnected ? LeftHandIndex : -1;
            }
        }

        /// <summary>
        /// Gets the right controller index
        /// </summary>
        public int RightControllerIndex
        {
            get
            {
                return this.OculusTouchControllerState.IsConnected ? RightHandIndex : -1;
            }
        }

        /// <summary>
        /// Gets the controller state list
        /// </summary>
        public VRGenericControllerState[] GenericControllerStates
        {
            get
            {
                return this.OculusTouchControllerState.IsConnected ? this.genericControllersArray : this.emptyControllersArray;
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

        /// <summary>
        /// Gets the Oculus Remote state.
        /// </summary>
        public OculusRemoteState OculusRemoteState { get; private set; }

        /// <summary>
        /// Gets the Oculus Touch controller state.
        /// </summary>
        public OculusTouchControllerState OculusTouchControllerState { get; private set; }

        /// <summary>
        /// Gets the state of the generic controller.
        /// </summary>
        /// <value>
        /// The state of the generic controller.
        /// </value>
        public GamePadState GamePadState
        {
            get
            {
                return this.gamePadstate;
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

            this.OculusRemoteState = new OculusRemoteState();
            this.OculusTouchControllerState = new OculusTouchControllerState();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the oculus input states and the Touch controllers poses.
        /// </summary>
        /// <param name="gameTime">The elapsed game time since the last update.</param>
        public override void Update(TimeSpan gameTime)
        {
            this.OculusRemoteState.Update(this.Hmd);
            this.OculusTouchControllerState.Update(this.Hmd);

            this.UpdateGenericControllers();
            this.UpdateGamePad();
        }

        /// <summary>
        /// Updates generic controller state.
        /// </summary>
        private void UpdateGenericControllers()
        {
            this.OculusTouchControllerState.ToLeftGenericController(out this.genericControllersArray[LeftHandIndex]);
            this.OculusTouchControllerState.ToRightGenericController(out this.genericControllersArray[RightHandIndex]);

            if (this.OculusTouchControllerState.IsConnected)
            {
                this.genericControllersArray[LeftHandIndex].Pose = this.ovrApplication.LeftControllerPose;
                this.genericControllersArray[RightHandIndex].Pose = this.ovrApplication.RightControllerPose;
            }
        }

        /// <summary>
        /// Updates generic controller state.
        /// </summary>
        private void UpdateGamePad()
        {
            this.gamePadstate.IsConnected = false;
            if (this.OculusTouchControllerState.IsConnected)
            {
                this.OculusTouchControllerState.ToGamePadState(out this.gamePadstate);
            }
            else if (this.OculusRemoteState.IsConnected)
            {
                this.OculusRemoteState.ToGamePadState(out this.gamePadstate);
            }
        }

        /// <summary>
        /// Changes the tracking origin to floor height.
        /// </summary>
        /// <param name="floor">Whether to place the tracking origin at floor height.</param>
        public void SetTrackingOriginAtFloorHeight(bool floor)
        {
            this.Hmd.SetTrackingOriginType(floor ? OVRTypes.TrackingOrigin.FloorLevel : OVRTypes.TrackingOrigin.EyeLevel);
        }
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
