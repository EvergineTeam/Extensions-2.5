// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia platform specific integration service
    /// </summary>
    internal abstract class ARServiceBase
    {
        private static readonly Matrix ProjectionCorrectionMatrix = Matrix.CreateRotationX(MathHelper.Pi);

        #region P/Invoke

        /// <summary>
        /// Dll name used in the P/Inkoves
        /// </summary>
        protected const string DllName =
#if ANDROID
        "libVuforiaAdapter.so";
#elif IOS
        "__Internal";
#else
        "VuforiaAdapter.dll";

#endif

        [DllImport(DllName)]
        private extern static void QCAR_init(string licenseKey, VuforiaInitializedCallback.InitCallback callback);

        [DllImport(DllName)]
        private extern static void QCAR_getVideoInfo(ref int frameWidth, ref int frameHeight, ref QCAR_VideoMesh videoMesh);

        [DllImport(DllName)]
        private extern static bool QCAR_shutDown();

        [DllImport(DllName)]
        private extern static ARState QCAR_getState();

        [DllImport(DllName)]
        private extern static void QCAR_setOrientation(int frameWidth, int frameHeight, QCAR_Orientation orientation);

        [DllImport(DllName)]
        private extern static void QCAR_setHint(QCAR_Hint hint, int value);

        [DllImport(DllName)]
        private extern static int QCAR_loadDataSet(string dataSetPath, bool extendedTracking, ref QCAR_LoadDataSetResult result);

        [DllImport(DllName)]
        private extern static void QCAR_startTrack(VuforiaStartTrackCallback.StartTrackCallback callback);

        [DllImport(DllName)]
        private extern static bool QCAR_stopTrack();

        [DllImport(DllName)]
        private extern static void QCAR_getCameraProjection(float nearPlane, float farPlane, ref QCAR_Matrix4x4 result);

        [DllImport(DllName)]
        private extern static void QCAR_update(ref QCAR_UpdateResult result);
        #endregion

        #region Variables
        private bool retrieveCameraTexture;
        private bool shouldRefreshBackgroundCameraMesh;

        private List<TrackableResult> trackableResults;
        private Mesh backgroundCameraMesh;
        private int maxSimultaneousImageTargets;
        private int maxSimultaneousObjectTargets;

        private QCAR_VideoMesh vuforiaVideoMesh;

        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// The current Vuforia orientation
        /// </summary>
        protected QCAR_Orientation currentOrientation;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the dataset in use.
        /// </summary>
        public DataSet Dataset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Trackable objects currently being tracked.
        /// </summary>
        public IEnumerable<TrackableResult> TrackableResults
        {
            get
            {
                return this.trackableResults;
            }
        }

        /// <summary>
        /// Gets the Vuforia service state.
        /// </summary>
        /// <value>The Vuforia service state.</value>
        public ARState State
        {
            get
            {
                return QCAR_getState();
            }
        }

        /// <summary>
        /// Gets the camera texture.
        /// </summary>
        /// <value>The camera texture.</value>
        public Texture CameraTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the mesh for the background camera
        /// </summary>
        public Mesh BackgroundCameraMesh
        {
            get
            {
                return this.backgroundCameraMesh;
            }
        }

        /// <summary>
        /// Gets or sets how many image targets to detect and track at the same time
        /// </summary>
        /// <remarks>
        /// Tells the tracker how many image shall be processed
        /// at most at the same time.E.g. if an app will never require
        /// tracking more than two targets, this value should be set to 2.
        /// Default is: 1.
        /// </remarks>
        public int MaxSimultaneousImageTargets
        {
            get
            {
                return this.maxSimultaneousImageTargets;
            }

            set
            {
                if (this.maxSimultaneousImageTargets != value)
                {
                    this.maxSimultaneousImageTargets = value;

                    if (this.State != ARState.Tracking)
                    {
                        QCAR_setHint(QCAR_Hint.HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets how many object targets to detect and track at the same time
        /// </summary>
        /// <remarks>
        /// Tells the tracker how many 3D objects shall be processed
        /// at most at the same time.E.g. if an app will never require
        /// tracking more than 1 target, this value should be set to 1.
        /// Default is: 1.
        /// </remarks>
        public int MaxSimultaneousObjectTargets
        {
            get
            {
                return this.maxSimultaneousObjectTargets;
            }

            set
            {
                if (this.maxSimultaneousObjectTargets != value)
                {
                    this.maxSimultaneousObjectTargets = value;

                    if (this.State != ARState.Tracking)
                    {
                        QCAR_setHint(QCAR_Hint.HINT_MAX_SIMULTANEOUS_OBJECT_TARGETS, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the world center result if it is detected.
        /// </summary>
        public TrackableResult WorldCenterResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets world center setting on the ARCamera
        /// </summary>
        public ARTrackableBehavior WorldCenterTrackable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets defines how the relative transformation that is returned by the service is applied.
        /// Either the camera is moved in the scene with respect to a "world center" or all the targets are moved with respect to the camera.
        /// </summary>
        public WorldCenterMode WorldCenterMode
        {
            get;
            set;
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ARServiceBase"/> class.
        /// </summary>
        public ARServiceBase()
        {
            this.trackableResults = new List<TrackableResult>();

            this.maxSimultaneousImageTargets = 1;
            this.maxSimultaneousObjectTargets = 1;

            this.graphicsDevice = WaveServices.GraphicsDevice;
        }

        /// <summary>
        /// Initializes the Vuforia service
        /// </summary>
        /// <param name="licenseKey">The license key</param>
        /// <returns>
        ///   <c>true</c> if the initialization was succeed; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> Initialize(string licenseKey)
        {
            var result = await this.InternalInitialize(licenseKey);

            if (result)
            {
                var platform = WaveServices.Platform;
                platform.OnDisplayOrientationChanged += this.Platform_OnDisplayOrientationChanged;
                platform.OnScreenSizeChanged += this.Platform_OnScreenSizeChanged;
            }

            return result;
        }

        /// <summary>
        /// Internals the initialize.
        /// </summary>
        /// <param name="licenseKey">The license key.</param>
        /// <returns>A boolean indicating whether the service was initialized</returns>
        protected virtual Task<bool> InternalInitialize(string licenseKey)
        {
            var vuforiaInitCallback = new VuforiaInitializedCallback(null);
            QCAR_init(licenseKey, vuforiaInitCallback.CallBack);

            return vuforiaInitCallback.Task;
        }

        /// <summary>
        /// Called before the startTracking native method is called
        /// </summary>
        protected virtual void InternalBeforeStartTracking()
        {
        }
        #endregion

        /// <summary>
        /// Loads a new dataSet. If any other dataSet is loaded, it will be deactivated and unloaded before load the new one.
        /// </summary>
        /// <param name="dataSetPath">The dataset path</param>
        /// <param name="extendedTracking">A value indicating whether extended tracking feature is enabled for all dataSet trackables.</param>
        /// <returns><c>true</c>, if the dataset has been loaded, <c>false</c> otherwise.</returns>
        public bool LoadDataSet(string dataSetPath, bool extendedTracking)
        {
            var loadResult = default(QCAR_LoadDataSetResult);
            var result = QCAR_loadDataSet(dataSetPath, extendedTracking, ref loadResult) == 0;

            if (result)
            {
                this.Dataset = new DataSet(dataSetPath);
                this.Dataset.Trackables = loadResult.Trackables
                                                    .Take(loadResult.NumTrackables)
                                                    .Select(t => TargetFactory.CreateTarget(t))
                                                    .ToList();
            }

            return result;
        }

        /// <summary>
        /// Shut down Vuforia Service
        /// </summary>
        /// <returns><c>true</c>, if the service was shutted down correctly, <c>false</c> otherwise.</returns>
        public bool ShutDown()
        {
            var platform = WaveServices.Platform;
            platform.OnDisplayOrientationChanged -= this.Platform_OnDisplayOrientationChanged;
            platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;

            return QCAR_shutDown();
        }

        /// <summary>
        /// Starts the Vuforia target tracking.
        /// </summary>
        /// <param name="retrieveCameraTexture">if set to <c>true</c>, the service will update the camera video texture.</param>
        /// <returns>
        ///   <c>true</c>, if tracking was started, <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> StartTracking(bool retrieveCameraTexture)
        {
            this.retrieveCameraTexture = retrieveCameraTexture;

            this.UpdateOrientation();

            var vuforiaStartTrackCallback = new VuforiaStartTrackCallback((bool result) =>
            {
                if (result)
                {
                    QCAR_setHint(QCAR_Hint.HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS, this.maxSimultaneousImageTargets);
                    QCAR_setHint(QCAR_Hint.HINT_MAX_SIMULTANEOUS_OBJECT_TARGETS, this.maxSimultaneousObjectTargets);

                    this.DestroyCameraTexture();
                    if (retrieveCameraTexture)
                    {
                        this.vuforiaVideoMesh = new QCAR_VideoMesh();
                        int textureWidth = 0, textureHeight = 0;
                        QCAR_getVideoInfo(ref textureWidth, ref textureHeight, ref this.vuforiaVideoMesh);

                        if (textureWidth == 0 || textureHeight == 0)
                        {
                            throw new InvalidOperationException("Invalid camera texture size");
                        }

                        var vertexBuffer = new DynamicVertexBuffer(VertexPositionTexture.VertexFormat);
                        vertexBuffer.SetData(this.vuforiaVideoMesh.Vertices);
                        this.graphicsDevice.BindVertexBuffer(vertexBuffer);

                        var indexBuffer = new IndexBuffer(this.vuforiaVideoMesh.Indices);
                        this.graphicsDevice.BindIndexBuffer(indexBuffer);

                        this.backgroundCameraMesh = new Mesh(vertexBuffer, indexBuffer, PrimitiveType.TriangleList);

                        this.CameraTexture = this.CreateCameraTexture(textureWidth, textureHeight);
                    }

                    this.UpdateOrientation();
                }
            });

            this.InternalBeforeStartTracking();

            QCAR_startTrack(vuforiaStartTrackCallback.CallBack);

            return await vuforiaStartTrackCallback.Task;
        }

        /// <summary>
        /// Stops the Vuforia target tracking.
        /// </summary>
        /// <returns><c>true</c>, if tracking was stopped, <c>false</c> otherwise.</returns>
        public virtual bool StopTracking()
        {
            this.DestroyCameraTexture();

            return QCAR_stopTrack();
        }

        /// <summary>
        /// Gets the camera projection matrix.
        /// </summary>
        /// <returns>The camera projection matrix.</returns>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public Matrix GetCameraProjection(float nearPlane, float farPlane)
        {
            var result = default(QCAR_Matrix4x4);

            QCAR_getCameraProjection(nearPlane, farPlane, ref result);

            var projection = ProjectionCorrectionMatrix * result.ToEngineMatrix();

            return projection;
        }

        /// <summary>
        /// Update the service
        /// </summary>
        /// <param name="gameTime">The game timestan elapsed from the latest update</param>
        public void Update(TimeSpan gameTime)
        {
            if (this.State != ARState.Tracking)
            {
                return;
            }

            if (this.retrieveCameraTexture)
            {
                this.UpdateCameraTexture();
            }

            var updateResult = default(QCAR_UpdateResult);
            QCAR_update(ref updateResult);

            if (this.shouldRefreshBackgroundCameraMesh &&
                this.UpdateBackgroundCameraMesh(updateResult))
            {
                this.shouldRefreshBackgroundCameraMesh = false;
            }

            this.trackableResults = updateResult.TrackableResults
                                                .Take(updateResult.NumTrackableResults)
                                                .Select(t => TargetFactory.CreateTrackableResult(t, this.Dataset))
                                                .ToList();
        }

        private bool UpdateBackgroundCameraMesh(QCAR_UpdateResult updateResult)
        {
            if (this.backgroundCameraMesh == null)
            {
                return false;
            }

            var videoTextureProjection = updateResult.VideoBackgroundProjection.ToEngineMatrix();
            this.AdjustVideoTextureProjection(ref videoTextureProjection);
            var vertexBuffer = this.backgroundCameraMesh.VertexBuffer;
            var vertices = this.vuforiaVideoMesh.Vertices.ToArray();
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3.Transform(ref vertices[i].Position, ref videoTextureProjection, out vertices[i].Position);
                vertices[i].Position.Z = 0.5f;
            }

            vertexBuffer.SetData(vertices);
            this.graphicsDevice.BindVertexBuffer(vertexBuffer);
            return true;
        }

        private void DestroyCameraTexture()
        {
            if (this.CameraTexture != null)
            {
                var renderTarget = this.CameraTexture as RenderTarget;
                if (renderTarget != null)
                {
                    WaveServices.GraphicsDevice.RenderTargets.DestroyRenderTarget(renderTarget);
                }
                else
                {
                    WaveServices.GraphicsDevice.Textures.DestroyTexture(this.CameraTexture);
                }

                this.CameraTexture = null;
            }
        }

        /// <summary>
        /// Update QCAR orientation
        /// </summary>
        private void UpdateOrientation()
        {
            switch (WaveServices.Platform.DisplayOrientation)
            {
                default:
                case DisplayOrientation.LandscapeLeft:
                    this.currentOrientation = QCAR_Orientation.ORIENTATION_LANDSCAPE_LEFT;
                    break;

                case DisplayOrientation.LandscapeRight:
                    this.currentOrientation = QCAR_Orientation.ORIENTATION_LANDSCAPE_RIGHT;
                    break;

                case DisplayOrientation.Portrait:
                    this.currentOrientation = QCAR_Orientation.ORIENTATION_PORTRAIT;
                    break;

                case DisplayOrientation.PortraitFlipped:
                    this.currentOrientation = QCAR_Orientation.ORIENTATION_PORTRAIT_UPSIDEDOWN;
                    break;
            }

            if (this.State == ARState.Tracking)
            {
                var platform = WaveServices.Platform;
                QCAR_setOrientation(platform.ScreenWidth, platform.ScreenHeight, this.currentOrientation);
                this.shouldRefreshBackgroundCameraMesh = this.retrieveCameraTexture;
            }
        }

        private void Platform_OnDisplayOrientationChanged(object sender, DisplayOrientation e)
        {
            this.UpdateOrientation();
        }

        private void Platform_OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.UpdateOrientation();
        }

        /// <summary>
        /// Makes platform specific adjustments of the video texture projection matrix
        /// </summary>
        /// <param name="videoTextureProjection">The video texture projection matrix provided by Vuforia</param>
        protected virtual void AdjustVideoTextureProjection(ref Matrix videoTextureProjection)
        {
        }

        /// <summary>
        /// Creates the camera texture.
        /// </summary>
        /// <param name="textureWidth">Width of the texture.</param>
        /// <param name="textureHeight">Height of the texture.</param>
        /// <returns>The camera texture</returns>
        protected abstract Texture CreateCameraTexture(int textureWidth, int textureHeight);

        /// <summary>
        /// Updates the camera texture.
        /// </summary>
        protected abstract void UpdateCameraTexture();
    }
}
