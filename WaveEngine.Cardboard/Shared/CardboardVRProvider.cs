#region File Description
//-----------------------------------------------------------------------------
// CardboardVRProvider
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using WaveEngine.Cardboard;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.IO;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.VR;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    [DataContract]
    public class CardboardVRProvider : VRProvider, IDisposable
    {
        /// <summary>
        /// The name of the barrel distortion asset
        /// </summary>
        private const string BarrelDistortionModel = "Content/barrelDistortionMesh";

        /// <summary>
        /// The default interpupillary distance in meters
        /// </summary>
        private const float DefaultInterpupillaryDistance = 0.065f;

        /// <summary>
        /// The default interpupillary distance in meters
        /// </summary>
        private static Vector3 DefaultNeckModelFactor = new Vector3(0, 0.15f, -0.08f);

        /// <summary>
        /// Default view of view
        /// </summary>
        public static float DefaultFieldOfView = MathHelper.ToRadians(88);

        /// <summary>
        /// Vectors from neck to eye anchors
        /// </summary>
        private Vector3[] neckAnchorVector;

        /// <summary>
        /// Platform service
        /// </summary>
        private Platform platform;

        /// <summary>
        /// Input service
        /// </summary>
        private Input input;

        /// <summary>
        /// The barrel distortion is enabled
        /// </summary>
        [DataMember]
        private bool isBarrelDistortionEnabled;

        /// <summary>
        /// Eye textures
        /// </summary>
        private VREyeTexture[] eyeTextures;

        /// <summary>
        /// Eye poses
        /// </summary>
        private VREyePose[] eyePoses;

        /// <summary>
        /// Barrel distortion entity mini-scene
        /// </summary>
        private Entity barrelDistortionEntity;

        /// <summary>
        /// Distortion Mesh 
        /// </summary>
        private InternalStaticModel distortionMesh;

        /// <summary>
        /// First update
        /// </summary>
        private bool needRefreshBarrelModel;

        /// <summary>
        /// Cached monoscopic value
        /// </summary>
        private bool cachedMonoscopicValue;

        /// <summary>
        /// Cached VRMode
        /// </summary>
        private VRMode cachedVRMode;

        /// <summary>
        /// Interpupillary distance
        /// </summary>
        [DataMember]
        private float interpupillaryDistance;

        /// <summary>
        /// The neck model factor
        /// </summary>
        [DataMember]
        private Vector3 eyeToNeckDistance;

        /// <summary>
        /// The neck model is enabled
        /// </summary>
        [DataMember]
        private bool isNeckDisplacementEnabled;

        #region Properties
        /// <summary>
        /// Gets a valie indicating whether barrel distortion is enabled
        /// </summary>
        public bool IsBarrelDistortionEnabled
        {
            get
            {
                return this.isBarrelDistortionEnabled;
            }

            set
            {
                this.isBarrelDistortionEnabled = value;
                this.needRefreshBarrelModel = true;
            }
        }

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
                return true;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return this.eyePoses;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return VREyePose.DefaultPose;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return this.eyeTextures;
            }
        }

        /// <summary>
        /// Gets or sets the interpupillary distance (in meters).
        /// </summary>
        public float InterpupillaryDistance
        {
            get
            {
                return this.interpupillaryDistance;
            }
            set
            {
                this.interpupillaryDistance = value;
                this.RefreshNeckModel();
            }
        }


        /// <summary>
        /// Gets or sets the distance between the neck to central eye position
        /// </summary>
        public Vector3 EyeToNeckDistance
        {
            get
            {
                return this.eyeToNeckDistance;
            }
            set
            {
                this.eyeToNeckDistance = value;
                this.RefreshNeckModel();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the neck model simulated position tracking is enabled
        /// </summary>
        public bool IsNeckDisplacementEnabled
        {
            get
            {
                return this.isNeckDisplacementEnabled;
            }
            set
            {
                this.isNeckDisplacementEnabled = value;
                this.RefreshNeckModel();
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Set default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.interpupillaryDistance = DefaultInterpupillaryDistance;
            this.eyeToNeckDistance = DefaultNeckModelFactor;
            this.UpdateOrder = 0;
            this.isBarrelDistortionEnabled = true;
            this.needRefreshBarrelModel = true;
            this.isNeckDisplacementEnabled = true;
            this.platform = WaveServices.Platform;
            this.input = WaveServices.Input;

            // eye textures
            this.eyeTextures = new VREyeTexture[2];
            for (int eyeIndex = 0; eyeIndex < this.eyeTextures.Length; eyeIndex++)
            {
                this.eyeTextures[eyeIndex] = new VREyeTexture();
            }

            // eye poses
            this.eyePoses = new VREyePose[3];
            for (int eyeIndex = 0; eyeIndex < this.eyePoses.Length; eyeIndex++)
            {
                this.eyePoses[eyeIndex] = VREyePose.DefaultPose;
            }

            this.neckAnchorVector = new Vector3[3];
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
            base.Initialize();
            this.platform.OnScreenSizeChanged += this.OnScreenSizeChanged;

            if (this.input.MotionState.IsConnected)
            {
                this.input.StartMotion();
            }

            if (this.cameraRig.LeftEyeCamera != null)
            {
                this.cameraRig.LeftEyeCamera.FieldOfView = DefaultFieldOfView;
            }

            if (this.cameraRig.RightEyeCamera != null)
            {
                this.cameraRig.RightEyeCamera.FieldOfView = DefaultFieldOfView;
            }

            this.RefreshNeckModel();
        }

        /// <summary>
        /// Update the scene
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            if (this.cachedMonoscopicValue != this.cameraRig.Monoscopic)
            {
                this.cachedMonoscopicValue = this.cameraRig.Monoscopic;
                this.needRefreshBarrelModel = true;
            }

            if (this.cachedVRMode != this.cameraRig.VRMode)
            {
                this.cachedVRMode = this.cameraRig.VRMode;
                this.needRefreshBarrelModel = true;
            }

            if (this.needRefreshBarrelModel)
            {
                this.RefreshBarrelDistortionScene();
                this.needRefreshBarrelModel = false;
            }

            this.UpdateEyePoses();
        }

        /// <summary>
        /// The screen size is changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args</param>
        private void OnScreenSizeChanged(object sender, Common.Helpers.SizeEventArgs e)
        {
            this.needRefreshBarrelModel = true;
        }

        /// <summary>
        /// Refresh eye textures
        /// </summary>
        private void RefreshEyeTextures()
        {
            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {
                var eyeTexture = this.eyeTextures[eyeIndex];

                if (eyeTexture.RenderTarget != null && (!this.cachedMonoscopicValue || eyeIndex == 0))
                {
                    WaveServices.GraphicsDevice.RenderTargets.DestroyRenderTarget(eyeTexture.RenderTarget);
                    eyeTexture.RenderTarget = null;
                }
            }

            if (this.cachedVRMode != VRMode.AttachedMode)
            {
                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    var eyeTexture = this.eyeTextures[eyeIndex];

                    if (this.IsBarrelDistortionEnabled)
                    {
                        if (!this.cachedMonoscopicValue || eyeIndex == 0)
                        {
                            eyeTexture.RenderTarget = WaveServices.GraphicsDevice.RenderTargets.CreateRenderTarget(platform.ScreenWidth / 2, platform.ScreenHeight);
                            this.cameraRig.RightEyeCamera.IsActive = true;
                        }
                        else
                        {
                            eyeTexture.RenderTarget = this.EyeTextures[0].RenderTarget;
                            this.cameraRig.RightEyeCamera.IsActive = false;
                        }

                        eyeTexture.Viewport = new Viewport(0, 0, 1, 1);
                    }
                    else
                    {
                        eyeTexture.Viewport = new Viewport(eyeIndex * 0.5f, 0, 0.5f, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the neck model factor
        /// </summary>
        private void RefreshNeckModel()
        {
            Vector3 finalNeckModel = (this.isNeckDisplacementEnabled) ? this.eyeToNeckDistance : Vector3.Zero;
            // Central
            this.neckAnchorVector[2] = finalNeckModel;

            // Left
            this.neckAnchorVector[0] = finalNeckModel - (Vector3.UnitX * (this.interpupillaryDistance / 2));

            // Right
            this.neckAnchorVector[1] = finalNeckModel + (Vector3.UnitX * (this.interpupillaryDistance / 2));
        }

        /// <summary>
        /// Update eye poses
        /// </summary>
        private void UpdateEyePoses()
        {
            Quaternion orientation = this.input.MotionState.IsConnected ? this.input.MotionState.Orientation : Quaternion.Identity;

            for (int eyeIndex = 0; eyeIndex < this.eyePoses.Length; eyeIndex++)
            {
                this.eyePoses[eyeIndex].Orientation = orientation;

                Vector3.Transform(ref this.neckAnchorVector[eyeIndex], ref orientation, out this.eyePoses[eyeIndex].Position);
                this.eyePoses[eyeIndex].Position -= this.neckAnchorVector[2];
            }
        }

        /// <summary>
        /// Refresh the barrel distortion mesh scene
        /// </summary>
        private void RefreshBarrelDistortionScene()
        {
            this.RefreshEyeTextures();

            if (this.isBarrelDistortionEnabled && this.cachedVRMode != VRMode.AttachedMode)
            {
                if (this.barrelDistortionEntity == null)
                {
                    if (this.distortionMesh == null)
                    {
                        var assembly = ReflectionHelper.GetTypeAssembly(typeof(CardboardVRProvider));

                        using (var stream = ResourceLoader.GetEmbeddedResourceStream(assembly, "WaveEngine.Cardboard.BarrelDistortionMesh.wpk"))
                        {
                            this.distortionMesh = this.Assets.LoadAsset<InternalStaticModel>(BarrelDistortionModel, stream);
                        }
                    }

                    if (this.RenderManager.FindLayer(typeof(VRLensLayer)) == null)
                    {
                        this.RenderManager.RegisterLayer(new VRLensLayer(this.RenderManager));
                    }

                    Dictionary<string, Material> materials = new Dictionary<string, Material>();
                    materials.Add("vrMeshLeft", new StandardMaterial() { LightingEnabled = false, Diffuse = this.eyeTextures[0].RenderTarget, LayerType = typeof(VRLensLayer), SamplerMode = AddressMode.LinearClamp });
                    materials.Add("vrMeshRight", new StandardMaterial() { LightingEnabled = false, Diffuse = this.eyeTextures[1].RenderTarget, LayerType = typeof(VRLensLayer), SamplerMode = AddressMode.LinearClamp });

                    MaterialsMap materialsMap = new MaterialsMap(materials);

                    this.barrelDistortionEntity = new Entity("_VRCardboardDistortMesh");

                    Entity distortMeshEntity = new Entity("VRMesh")
                        .AddComponent(new Transform3D())
                        .AddComponent(new Model(BarrelDistortionModel))
                        .AddComponent(materialsMap)
                        .AddComponent(new ModelRenderer());

                    var vrCamera = new FixedCamera3D("VRDistortCamera", new Vector3(0, 1, 0), Vector3.Zero);
                    var camera = vrCamera.Entity.FindComponent<Camera3D>();
                    camera.UpVector = -Vector3.UnitZ;
                    camera.SetCustomProjection(Matrix.CreateOrthographic(1, 1, 0.1f, 100));
                    camera.LayerMaskDefaultValue = false;
                    camera.LayerMask.Add(typeof(VRLensLayer), true);

                    this.barrelDistortionEntity.AddChild(distortMeshEntity);
                    this.barrelDistortionEntity.AddChild(vrCamera.Entity);

                    this.EntityManager.Add(this.barrelDistortionEntity);
                }
            }
            else
            {
                if (this.barrelDistortionEntity != null)
                {
                    this.EntityManager.Remove(this.barrelDistortionEntity);
                    this.barrelDistortionEntity = null;
                    var lensLayer = this.RenderManager.FindLayer(typeof(VRLensLayer));
                    if (lensLayer != null)
                    {
                        this.RenderManager.RemoveLayer(lensLayer);
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.input.MotionState.IsConnected)
            {
                this.input.StopMotion();
            }

            this.platform.OnScreenSizeChanged -= this.OnScreenSizeChanged;
        }
        #endregion

    }
}
