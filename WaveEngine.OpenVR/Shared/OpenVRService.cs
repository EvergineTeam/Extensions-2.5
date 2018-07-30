// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.OpenVR.Input;
using WaveEngine.OpenVR.Helpers;
using Valve.VR;
using WaveEngine.Common.VR;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Input;
using ValveOpenVR = Valve.VR.OpenVR;
#endregion

namespace WaveEngine.OpenVR
{
    public class OpenVRService : UpdatableService
    {
        /// <summary>
        /// The application
        /// </summary>
        private OpenVRApplication ovrApplication;

        private TrackedDevicePose_t[] renderPoses;
        private VRPose[] poses;

        private TrackedDevicePose_t[] gamePoses;

        private List<OpenVRTrackingReference> trackers;
        private List<OpenVRTrackingReference> baseStations;
        private List<OpenVRControllerState> controllers;
        private List<VRGenericControllerState> genericControllers;

        private OpenVRTrackingReference[] trackersArray;
        private OpenVRTrackingReference[] baseStationsArray;
        private OpenVRControllerState[] controllersArray;
        private VRGenericControllerState[] genericControllersArray;

        private Dictionary<int, OpenVRControllerState> controllersPool;
        private Dictionary<int, OpenVRTrackingReference> baseStationsPool;
        private Dictionary<int, OpenVRTrackingReference> trackersPool;

        private GamePadState gamePadState;

        #region Properties

        public CVROverlay Overlay
        {
            get
            {
                return ValveOpenVR.Overlay;
            }
        }

        public bool VRInitializing { get; private set; }

        public bool VRCalibrating { get; private set; }

        public bool VROutOfRange { get; private set; }

        /// <summary>
        /// Gets the eye properties
        /// </summary>
        public VREye[] EyesProperties { get; private set; }

        /// <summary>
        /// Gets the tracker poses
        /// </summary>
        public OpenVRTrackingReference[] Trackers => this.trackersArray;

        /// <summary>
        /// Gets the tracker camera poses
        /// </summary>
        public OpenVRTrackingReference[] BaseStations => this.baseStationsArray;

        /// <summary>
        /// Gets the controller states
        /// </summary>
        public OpenVRControllerState[] Controllers => this.controllersArray;

        /// <summary>
        /// Gets the generic controller states
        /// </summary>
        public VRGenericControllerState[] GenericControllers => this.genericControllersArray;

        /// <summary>
        /// Gets a gamepad state mapped using controllers
        /// </summary>
        public GamePadState GamePadState => this.gamePadState;

        /// <summary>
        /// Gets the left controller index
        /// </summary>
        public int LeftControllerIndex
        {
            get; private set;
        }

        /// <summary>
        /// Gets the right controller index
        /// </summary>
        public int RightControllerIndex
        {
            get; private set;
        }

        public OpenVRControllerState LeftController
        {
            get
            {
                if (this.LeftControllerIndex >= 0 && this.controllersArray.Length > this.LeftControllerIndex)
                {
                    return this.controllersArray[this.LeftControllerIndex];
                }

                return null;
            }
        }

        public OpenVRControllerState RightController
        {
            get
            {
                if (this.RightControllerIndex >= 0 && this.controllersArray.Length > this.RightControllerIndex)
                {
                    return this.controllersArray[this.RightControllerIndex];
                }

                return null;
            }
        }

        public string TrackingSystemName
        {
            get
            {
                return this.GetStringProperty(ValveOpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_TrackingSystemName_String);
            }
        }

        public string ModelNumber
        {
            get
            {
                return this.GetStringProperty(ValveOpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_ModelNumber_String);
            }
        }

        public string SerialNumber
        {
            get
            {
                return this.GetStringProperty(ValveOpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_SerialNumber_String);
            }
        }

        public float SecondsFromVsyncToPhotons
        {
            get
            {
                return this.GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);
            }
        }

        public float DisplayFrequency
        {
            get
            {
                return this.GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);
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
        /// Initializes a new instance of the <see cref="OpenVRService"/> class.
        /// </summary>
        /// <param name="ovrApplication">The OpenVR application.</param>
        public OpenVRService()
        {
            this.renderPoses = new TrackedDevicePose_t[ValveOpenVR.k_unMaxTrackedDeviceCount];
            this.gamePoses = new TrackedDevicePose_t[0];
            this.poses = new VRPose[ValveOpenVR.k_unMaxTrackedDeviceCount];

            this.controllers = new List<OpenVRControllerState>();
            this.trackers = new List<OpenVRTrackingReference>();
            this.baseStations = new List<OpenVRTrackingReference>();
            this.genericControllers = new List<VRGenericControllerState>();

            this.controllersPool = new Dictionary<int, OpenVRControllerState>();
            this.trackersPool = new Dictionary<int, OpenVRTrackingReference>();
            this.baseStationsPool = new Dictionary<int, OpenVRTrackingReference>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.ovrApplication = Game.Current.Application as OpenVRApplication;

            if (this.ovrApplication.HmdDetected)
            {
                var eyeTextures = this.ovrApplication.EyeTextures;
                this.EyesProperties = new VREye[3];
                for (int i = 0; i < this.EyesProperties.Length; i++)
                {
                    var vrEye = new VREye();

                    if (i < eyeTextures.Length)
                    {
                        vrEye.Texture = eyeTextures[i];
                    }

                    this.EyesProperties[i] = vrEye;
                }
            }
        }
        #endregion

        #region Public Method
        public override void Update(TimeSpan gameTime)
        {
            if (this.IsConnected)
            {
                if (ValveOpenVR.Compositor != null)
                {
                    var errorResult = ValveOpenVR.Compositor.WaitGetPoses(this.renderPoses, this.gamePoses);
                    OpenVRHelper.ReportCompositeError(errorResult);
                }
                else
                {
                    this.ovrApplication.Hmd.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, this.renderPoses);
                }

                this.UpdateHMD();
                this.UpdateDevices();
                this.UpdateControllers();

                ////else
                ////{
                ////    var hmd = this.ovrApplication.Hmd;
                ////    hmd.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, this.renderPoses);
                ////}
            }
        }

        private void UpdateHMD()
        {
            var hmd = this.ovrApplication.Hmd;

            if (this.renderPoses.Length > ValveOpenVR.k_unTrackedDeviceIndex_Hmd)
            {
                var result = this.renderPoses[ValveOpenVR.k_unTrackedDeviceIndex_Hmd].eTrackingResult;

                this.VRInitializing = result == ETrackingResult.Uninitialized;

                this.VRCalibrating =
                    result == ETrackingResult.Calibrating_InProgress ||
                    result == ETrackingResult.Calibrating_OutOfRange;

                this.VROutOfRange =
                    result == ETrackingResult.Running_OutOfRange ||
                    result == ETrackingResult.Calibrating_OutOfRange;

                if (result == ETrackingResult.Running_OK)
                {
                    var hmdPose = this.renderPoses[ValveOpenVR.k_unTrackedDeviceIndex_Hmd];
                    Matrix hmdPoseMatrix;
                    hmdPose.ToMatrix(out hmdPoseMatrix);

                    VRPose centerEyePose;
                    hmdPoseMatrix.ToVRPose(out centerEyePose);
                    this.EyesProperties[(int)VREyeType.CenterEye].Pose = centerEyePose;

                    for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                    {
                        var eyeProperties = this.EyesProperties[eyeIndex];
                        var eyeTexture = eyeProperties.Texture;

                        Matrix eyeProjectionMatrix;
                        hmd.GetProjectionMatrix((EVREye)eyeIndex, eyeTexture.NearPlane, eyeTexture.FarPlane).ToMatrix(out eyeProjectionMatrix);
                        eyeProperties.Projection = eyeProjectionMatrix;

                        Matrix eyePoseMatrix;
                        hmd.GetEyeToHeadTransform((EVREye)eyeIndex).ToMatrix(out eyePoseMatrix);
                        eyePoseMatrix *= hmdPoseMatrix;

                        VRPose eyePose;
                        eyePoseMatrix.ToVRPose(out eyePose);
                        eyeProperties.Pose = eyePose;
                    }
                }
            }
        }

        private void UpdateDevices()
        {
            var hmd = this.ovrApplication.Hmd;

            this.LeftControllerIndex = -1;
            this.RightControllerIndex = -1;

            var left = (int)hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            var right = (int)hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

            this.trackers.Clear();
            this.controllers.Clear();
            this.baseStations.Clear();

            for (uint i = 0; i < ValveOpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                var rp = this.renderPoses[i];

                if (rp.bDeviceIsConnected && rp.bPoseIsValid)
                {
                    rp.ToVRPose(out this.poses[i]);

                    var deviceClass = hmd.GetTrackedDeviceClass(i);

                    switch (deviceClass)
                    {
                        case ETrackedDeviceClass.GenericTracker:
                            var tracker = this.GetTrackersFromPool((int)i);
                            tracker.Update((int)i, this.poses[i]);

                            this.trackers.Add(tracker);
                            break;

                        case ETrackedDeviceClass.Controller:

                            var state = new VRControllerState_t();
                            if (this.ovrApplication.Hmd.GetControllerState(i, ref state, (uint)Marshal.SizeOf(state)))
                            {
                                VRControllerRole role;

                                // Update hand role
                                if (i == left)
                                {
                                    role = VRControllerRole.LeftHand;
                                    this.LeftControllerIndex = this.controllers.Count;
                                }
                                else if (i == right)
                                {
                                    role = VRControllerRole.RightHand;
                                    this.RightControllerIndex = this.controllers.Count;
                                }
                                else
                                {
                                    role = VRControllerRole.Undefined;
                                }

                                var controllerState = this.GetControllerFromPool((int)i);
                                controllerState.Update((int)i, role, ref state, ref this.poses[i]);

                                this.controllers.Add(controllerState);
                            }

                            break;

                        case ETrackedDeviceClass.TrackingReference:
                            var trackingReference = this.GetBaseStationFromPool((int)i);
                            trackingReference.Update((int)i, this.poses[i]);

                            this.baseStations.Add(trackingReference);

                            break;
                    }
                }
            }

            this.trackersArray = this.trackers.ToArray();
            this.baseStationsArray = this.baseStations.ToArray();
            this.controllersArray = this.controllers.ToArray();

            Array.Resize(ref this.genericControllersArray, this.controllersArray.Length);
            for (int i = 0; i < this.controllersArray.Length; i++)
            {
                this.controllersArray[i].ToGenericController(out this.genericControllersArray[i]);
            }
        }

        private OpenVRControllerState GetControllerFromPool(int id)
        {
            OpenVRControllerState result;
            if (!this.controllersPool.TryGetValue(id, out result))
            {
                result = new OpenVRControllerState();
                this.controllersPool.Add(id, result);
            }

            return result;
        }

        private OpenVRTrackingReference GetBaseStationFromPool(int id)
        {
            OpenVRTrackingReference result;
            if (!this.baseStationsPool.TryGetValue(id, out result))
            {
                result = new OpenVRTrackingReference();
                this.baseStationsPool.Add(id, result);
            }

            return result;
        }

        private OpenVRTrackingReference GetTrackersFromPool(int id)
        {
            OpenVRTrackingReference result;
            if (!this.trackersPool.TryGetValue(id, out result))
            {
                result = new OpenVRTrackingReference();
                this.trackersPool.Add(id, result);
            }

            return result;
        }

        private void UpdateControllers()
        {
            this.gamePadState = new GamePadState();

            // Fill Gamepad state
            if (this.LeftControllerIndex >= 0 && this.LeftControllerIndex < this.controllers.Count)
            {
                this.controllers[this.LeftControllerIndex].ToLeftGamepad(ref this.gamePadState);
            }

            if (this.RightControllerIndex >= 0 && this.LeftControllerIndex < this.controllers.Count)
            {
                this.controllers[this.RightControllerIndex].ToRightGamepad(ref this.gamePadState);
            }
        }

        #endregion

        #region Private Methods
        private string GetStringProperty(uint deviceIndex, ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            var hmd = this.ovrApplication.Hmd;
            var capactiy = hmd.GetStringTrackedDeviceProperty(deviceIndex, prop, null, 0, ref error);
            if (capactiy > 1)
            {
                var result = new System.Text.StringBuilder((int)capactiy);
                hmd.GetStringTrackedDeviceProperty(ValveOpenVR.k_unTrackedDeviceIndex_Hmd, prop, result, capactiy, ref error);
                return result.ToString();
            }

            return (error != ETrackedPropertyError.TrackedProp_Success) ? error.ToString() : "<unknown>";
        }

        private float GetFloatProperty(ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            var hmd = this.ovrApplication.Hmd;
            return hmd.GetFloatTrackedDeviceProperty(ValveOpenVR.k_unTrackedDeviceIndex_Hmd, prop, ref error);
        }
        #endregion
    }
}
