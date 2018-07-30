// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using WaveEngine.Cardboard;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Input;
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
        /// The name of the custom layer for draw lens
        /// </summary>
        private const string VRLensLayerName = "VRLensLayer";

        /// <summary>
        /// The default interpupillary distance in meters
        /// </summary>
        private const float DefaultInterpupillaryDistance = 0.065f;

        /// <summary>
        /// The default interpupillary distance in meters
        /// </summary>
        private static Vector3 defaultNeckModelFactor = new Vector3(0, 0.15f, -0.08f);

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
        /// The device info
        /// </summary>
        private CardboardDeviceInfo deviceInfo;

        /// <summary>
        /// The barrel distortion is enabled
        /// </summary>
        [DataMember]
        private bool isBarrelDistortionEnabled;

        /// <summary>
        /// Eye properties
        /// </summary>
        private VREye[] eyesProperties;

        /// <summary>
        /// Barrel distortion entity mini-scene
        /// </summary>
        private Entity barrelDistortionEntity;

        /// <summary>
        /// The distortion mesh has been generated
        /// </summary>
        private bool distortionMeshGenerated;

        /// <summary>
        /// Distortion Mesh for left eye
        /// </summary>
        private Mesh distortionMeshLeft;

        /// <summary>
        /// Distortion Mesh for right eye
        /// </summary>
        private Mesh distortionMeshRight;

        /// <summary>
        /// First update
        /// </summary>
        private bool needRefreshBarrelModel;

        /// <summary>
        /// Cached monoscopic value
        /// </summary>
        private bool cachedMonoscopicValue;

        /// <summary>
        /// The left VR distortion mesh material
        /// </summary>
        private StandardMaterial vrMeshLeftMaterial;

        /// <summary>
        /// The right VR distortion mesh material
        /// </summary>
        private StandardMaterial vrMeshRightMaterial;

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
        /// Gets or sets a value indicating whether gets a valie indicating whether barrel distortion is enabled
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
        /// Gets the eye properties
        /// </summary>
        [DontRenderProperty]
        public override VREye[] EyesProperties
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return this.eyesProperties;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return -1;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return -1;
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
                    throw new Exception("Cardboard is not available. See console output to find the reason");
                }

                return null;
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
        /// Gets or sets a value indicating whether gets or sets a value indicating if the neck model simulated position tracking is enabled
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
            this.eyeToNeckDistance = defaultNeckModelFactor;
            this.UpdateOrder = 0;
            this.isBarrelDistortionEnabled = true;
            this.needRefreshBarrelModel = true;
            this.isNeckDisplacementEnabled = true;
            this.platform = WaveServices.Platform;
            this.input = WaveServices.Input;

            // eye properties
            this.eyesProperties = new VREye[3];
            for (int eyeIndex = 0; eyeIndex < this.eyesProperties.Length; eyeIndex++)
            {
                this.eyesProperties[eyeIndex] = new VREye();
                this.eyesProperties[eyeIndex].Texture = new VREyeTexture();
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

            this.deviceInfo = new CardboardDeviceInfo();

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
                var eyeTexture = this.eyesProperties[eyeIndex].Texture;

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
                    var eyeTexture = this.eyesProperties[eyeIndex].Texture;

                    if (this.IsBarrelDistortionEnabled)
                    {
                        if (!this.cachedMonoscopicValue || eyeIndex == 0)
                        {
                            eyeTexture.RenderTarget = WaveServices.GraphicsDevice.RenderTargets.CreateRenderTarget(this.platform.ScreenWidth / 2, this.platform.ScreenHeight);
                            this.cameraRig.RightEyeCamera.IsActive = true;
                        }
                        else
                        {
                            eyeTexture.RenderTarget = this.eyesProperties[0].Texture.RenderTarget;
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

            this.RefreshBarrelDistortionMaterials();
        }

        /// <summary>
        /// Refresh the neck model factor
        /// </summary>
        private void RefreshNeckModel()
        {
            Vector3 finalNeckModel = this.isNeckDisplacementEnabled ? this.eyeToNeckDistance : Vector3.Zero;

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

            for (int eyeIndex = 0; eyeIndex < this.eyesProperties.Length; eyeIndex++)
            {
                var pose = this.eyesProperties[eyeIndex].Pose;
                pose.Orientation = orientation;

                Vector3.Transform(ref this.neckAnchorVector[eyeIndex], ref orientation, out pose.Position);
                pose.Position -= this.neckAnchorVector[2];
                this.eyesProperties[eyeIndex].Pose = pose;
            }
        }

        /// <summary>
        /// Refresh the barrel distortion materials
        /// </summary>
        private void RefreshBarrelDistortionMaterials()
        {
            if (this.vrMeshLeftMaterial != null)
            {
                this.vrMeshLeftMaterial.Diffuse1 = this.eyesProperties[0].Texture.RenderTarget;
            }

            if (this.vrMeshRightMaterial != null)
            {
                this.vrMeshRightMaterial.Diffuse1 = this.eyesProperties[1].Texture.RenderTarget;
                    }
        }

        /// <summary>
        /// Find VRLens layer id
        /// </summary>
        /// <returns>The id of the VRLens layer</returns>
        private int FindVRLensLayerId()
        {
                    var layer = this.RenderManager.FindLayerByName(VRLensLayerName);

                    if (layer == null)
                    {
                        RenderLayerDescription vrLensLayer = new RenderLayerDescription(RasterizeStateEnum.CullBack, BlendStateEnum.Opaque, DepthStencilStateEnum.Write)
                        {
                            Name = VRLensLayerName,
                            DepthRange = DepthRange.Default,
                        };

                        layer = new RenderLayer(vrLensLayer);
                        layer.LayerMaskDefaultValue = false;

                        this.RenderManager.RegisterLayer(layer);
                    }

            return layer.Id;
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
                    if (!this.distortionMeshGenerated)
                    {
                        this.CreateDistortedMeshes();
                    }

                    int layerId = this.FindVRLensLayerId();
                    this.vrMeshLeftMaterial = new StandardMaterial() { LightingEnabled = false, LayerId = layerId };
                    this.vrMeshRightMaterial = new StandardMaterial() { LightingEnabled = false, LayerId = layerId };

                    this.RefreshBarrelDistortionMaterials();

                    this.barrelDistortionEntity = new Entity("_VRCardboardDistortMesh");

                    Entity distortMeshEntityLeft = new Entity("VRMeshLeft")
                        .AddComponent(new Transform3D() { LocalScale = Vector3.One * 0.5f })
                        .AddComponent(new MaterialComponent() { Material = this.vrMeshLeftMaterial })
                        .AddComponent(new CustomMesh() { Mesh = this.distortionMeshLeft })
                        .AddComponent(new MeshRenderer());

                    Entity distortMeshEntityRight = new Entity("VRMeshRight")
                        .AddComponent(new Transform3D() { LocalScale = Vector3.One * 0.5f })
                        .AddComponent(new MaterialComponent() { Material = this.vrMeshRightMaterial })
                        .AddComponent(new CustomMesh() { Mesh = this.distortionMeshRight })
                        .AddComponent(new MeshRenderer());

                    var vrCamera = new FixedCamera3D("VRDistortCamera", new Vector3(0, 1, 0), Vector3.Zero);
                    var camera = vrCamera.Entity.FindComponent<Camera3D>();
                    camera.CameraOrder = float.MaxValue;
                    camera.UpVector = -Vector3.UnitZ;
                    camera.SetCustomProjection(Matrix.CreateOrthographic(1, 1, 0.1f, 100));
                    camera.LayerMaskDefaultValue = false;
                    camera.LayerMask.Add(layerId, true);

                    this.barrelDistortionEntity.AddChild(distortMeshEntityLeft);
                    this.barrelDistortionEntity.AddChild(distortMeshEntityRight);
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
                    var lensLayer = this.RenderManager.FindLayerByName(VRLensLayerName);
                    if (lensLayer != null)
                    {
                        this.RenderManager.RemoveLayer(lensLayer);
                    }
                }
            }
        }

        /// <summary>
        /// Create distorted meshes
        /// </summary>
        private void CreateDistortedMeshes()
        {
            int width = 50;
            int height = 40;

            var vertices = this.ComputeMeshVertices(width, height);
            var indices = this.ComputeMeshIndices(width, height);

            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionTexture.VertexFormat);
            vertexBuffer.SetData(vertices);

            IndexBuffer indexBuffer = new IndexBuffer(indices);

            int halfIndices = indices.Length / (3 * 2);

            this.distortionMeshLeft = new Mesh(0, vertices.Length, 0, halfIndices, vertexBuffer, indexBuffer, PrimitiveType.TriangleList) { Name = "vrLeft" };
            this.distortionMeshRight = new Mesh(0, vertices.Length, halfIndices * 3, halfIndices, vertexBuffer, indexBuffer, PrimitiveType.TriangleList) { Name = "vrRight" };

            this.distortionMeshGenerated = true;
        }

        /// <summary>
        /// Compute mesh vertices distortion
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <returns>The vertices</returns>
        private VertexPositionTexture[] ComputeMeshVertices(int width, int height)
        {
            var vertices = new VertexPositionTexture[2 * width * height];

            var lensFrustum = this.deviceInfo.GetLeftEyeVisibleTanAngles();
            var noLensFrustum = this.deviceInfo.GetLeftEyeNoLensTanAngles();
            var viewport = this.deviceInfo.GetLeftEyeVisibleScreenRect(noLensFrustum);
            var vidx = 0;

            for (var e = 0; e < 2; e++)
            {
                for (var j = 0; j < height; j++)
                {
                    for (var i = 0; i < width; i++, vidx++)
                    {
                        float u = (float)i / (width - 1);
                        float v = (float)j / (height - 1);

                        // Grid points regularly spaced in StreoScreen, and barrel distorted in
                        // the mesh.
                        float s = u;
                        float t = v;
                        float x = MathHelper.Lerp(lensFrustum[0], lensFrustum[2], u);
                        float y = MathHelper.Lerp(lensFrustum[3], lensFrustum[1], v);
                        float d = (float)Math.Sqrt((x * x) + (y * y));
                        float r = this.deviceInfo.Distortion.DistortInverse(d);
                        float p = x * r / d;
                        float q = y * r / d;
                        u = (p - noLensFrustum[0]) / (noLensFrustum[2] - noLensFrustum[0]);
                        v = (q - noLensFrustum[3]) / (noLensFrustum[1] - noLensFrustum[3]);

                        // Convert u,v to mesh screen coordinates.
                        var aspect = this.deviceInfo.Device.WidthMeters / this.deviceInfo.Device.HeightMeters;

                        u = (viewport.X + (u * viewport.Width) - 0.5f) * 2.0f;
                        v = (viewport.Y + (v * viewport.Height) - 0.5f) * 2.0f;

                        VertexPositionTexture vertex = new VertexPositionTexture();
                        vertex.Position.X = u;
                        vertex.Position.Y = 0;
                        vertex.Position.Z = v;
                        vertex.TexCoord.X = s;
                        vertex.TexCoord.Y = t;

                        vertices[vidx] = vertex;
                    }
                }

                var w = lensFrustum[2] - lensFrustum[0];
                lensFrustum[0] = -(w + lensFrustum[0]);
                lensFrustum[2] = w - lensFrustum[2];
                w = noLensFrustum[2] - noLensFrustum[0];
                noLensFrustum[0] = -(w + noLensFrustum[0]);
                noLensFrustum[2] = w - noLensFrustum[2];
                viewport.X = 1 - (viewport.X + viewport.Width);
            }

            return vertices;
        }

        private ushort[] ComputeMeshIndices(int width, int height)
        {
            var indices = new ushort[2 * (width - 1) * (height - 1) * 6];
            var halfwidth = width / 2;
            var halfheight = height / 2;
            var vidx = 0;
            var iidx = 0;
            for (var e = 0; e < 2; e++)
            {
                for (var j = 0; j < height; j++)
                {
                    for (var i = 0; i < width; i++, vidx++)
                    {
                        if (i == 0 || j == 0)
                        {
                            continue;
                        }

                        // Build a quad.  Lower right and upper left quadrants have quads with
                        // the triangle diagonal flipped to get the vignette to interpolate
                        // correctly.
                        if ((i <= halfwidth) == (j <= halfheight))
                        {
                            // Quad diagonal lower left to upper right.
                            indices[iidx++] = (ushort)vidx;
                            indices[iidx++] = (ushort)(vidx - width - 1);
                            indices[iidx++] = (ushort)(vidx - width);
                            indices[iidx++] = (ushort)(vidx - width - 1);
                            indices[iidx++] = (ushort)vidx;
                            indices[iidx++] = (ushort)(vidx - 1);
                        }
                        else
                        {
                            // Quad diagonal upper left to lower right.
                            indices[iidx++] = (ushort)(vidx - 1);
                            indices[iidx++] = (ushort)(vidx - width);
                            indices[iidx++] = (ushort)vidx;
                            indices[iidx++] = (ushort)(vidx - width);
                            indices[iidx++] = (ushort)(vidx - 1);
                            indices[iidx++] = (ushort)(vidx - width - 1);
                        }
                    }
                }
            }

            return indices;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.platform.OnScreenSizeChanged -= this.OnScreenSizeChanged;
        }
        #endregion

    }
}
