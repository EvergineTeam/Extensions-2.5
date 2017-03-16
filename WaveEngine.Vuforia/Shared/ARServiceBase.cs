#region File Description
//-----------------------------------------------------------------------------
// ARServiceBase
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Vuforia.QCAR;
using NetTask = System.Threading.Tasks.Task;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia platform specific integration service
    /// </summary>
    public abstract class ARServiceBase
    {
        #region P/Invoke
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
        private extern static void QCAR_setOrientation(int frameWidth, int frameHeight, AROrientation orientation);

        [DllImport(DllName)]
        private extern static int QCAR_initialize(string dataSetPath, bool extendedTracking);

        [DllImport(DllName)]
        private extern static void QCAR_startTrack(VuforiaStartTackCallback.StartTrackCallback callback);

        [DllImport(DllName)]
        private extern static bool QCAR_stopTrack();

        [DllImport(DllName)]
        private extern static void QCAR_update(ref QCAR_TrackResult result);

        [DllImport(DllName)]
        private extern static void QCAR_getCameraProjection(float nearPlane, float farPlane, ref QCAR_Matrix4x4 result);
        #endregion

        #region Variables
        private static readonly Matrix poseCorrectionRotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
        private Task<bool> initializationTask;
        private string currentTrackName;
        private bool retrieveCameraTexture;  
        
        // The current orientation      
        protected AROrientation currentOrientation;
        private Mesh videoMesh;

        /// <summary>
        /// The camera projection matrix 
        /// </summary>
        protected Matrix videoTextureProjection;
        #endregion

        #region Properties
        public ARState State
        {
            get
            {
                return QCAR_getState();
            }
        }

        public string CurrentTrackName
        {
            get
            {
                return this.currentTrackName;
            }

            private set
            {
                if (this.currentTrackName != value)
                {
                    this.currentTrackName = value;

                    if (this.TrackNameChanged != null)
                    {
                        this.TrackNameChanged(this, this.currentTrackName);
                    }
                }
            }
        }

        public Matrix Pose
        {
            get;
            private set;
        }

        public Matrix PoseInv
        {
            get;
            private set;
        }

        public Matrix Projection
        {
            get;
            private set;
        }

        public Texture CameraTexture
        {
            get;
            private set;
        }

        public Matrix CameraProjectionMatrix
        {
            get
            {
                return this.videoTextureProjection;
            }
        }

        public Mesh VoideoMesh
        {
            get
            {
                return this.videoMesh;
            }
        }
        #endregion

        #region Events
        public event EventHandler<string> TrackNameChanged;
        #endregion

        #region Initialize
        #endregion

        public async Task<bool> Initialize(string licenseKey, string dataSetPath, bool extendedTracking)
        {
            var tcs = new TaskCompletionSource<bool>();
            this.initializationTask = tcs.Task;

            var result = await this.InternalInitialize(licenseKey);

            if (result)
            {
                var platform = WaveServices.Platform;
                platform.OnDisplayOrientationChanged += this.Platform_OnDisplayOrientationChanged;
                platform.OnScreenSizeChanged += this.Platform_OnScreenSizeChanged;

                result = QCAR_initialize(dataSetPath, extendedTracking) == 0;
            }

            tcs.SetResult(result);

            return result;
        }

        public bool ShutDown()
        {
            var platform = WaveServices.Platform;
            platform.OnDisplayOrientationChanged -= this.Platform_OnDisplayOrientationChanged;
            platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;

            return QCAR_shutDown();
        }

        public async Task<bool> StartTrack(bool retrieveCameraTexture)
        {
            var initResult = true;

            if(this.initializationTask != null)
            {
                initResult = await this.initializationTask;
            }

            if (!initResult || this.State != ARState.INITIALIZED)
            {
                return false;
            }

            this.UpdateOrientation();

            var vuforiaStartTrackCallback = new VuforiaStartTackCallback((bool result) =>
            {
                if (result && retrieveCameraTexture)
                {
                    QCAR_VideoMesh vuforiaVideoMesh = new QCAR_VideoMesh();
                    int textureWidth = 0, textureHeight = 0;
                    QCAR_getVideoInfo(ref textureWidth, ref textureHeight, ref vuforiaVideoMesh);

                    if (textureWidth == 0 || textureHeight == 0)
                    {
                        throw new InvalidOperationException("Invalid camera texture size");
                    }
                    
                    VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionTexture.VertexFormat);
                    vertexBuffer.SetData(vuforiaVideoMesh.Vertices);

                    IndexBuffer indexBuffer = new IndexBuffer(vuforiaVideoMesh.Indices);

                    this.videoMesh = new Mesh(vertexBuffer, indexBuffer, PrimitiveType.TriangleList);

                    this.UpdateOrientation();

                    this.CameraTexture = this.CreateCameraTexture(textureWidth, textureHeight);

                    this.retrieveCameraTexture = true;
                }
            });

            QCAR_startTrack(vuforiaStartTrackCallback.CallBack);

            return await vuforiaStartTrackCallback.Task;
        }

        public virtual bool StopTrack()
        {
            this.DestroyCameraTexture();

            return QCAR_stopTrack();
        }

        public Matrix GetCameraProjection(float nearPlane, float farPlane)
        {
            var result = default(QCAR_Matrix4x4);

            QCAR_getCameraProjection(nearPlane, farPlane, ref result);

            return result.ToEngineMatrix();
        }

        public virtual void Update(TimeSpan gameTime)
        {
            if (this.State != ARState.TRACKING)
            {
                return;
            }

            if (this.retrieveCameraTexture)
            {
                this.UpdateCameraTexture();
            }

            var result = default(QCAR_TrackResult);
            QCAR_update(ref result);
                        
            this.CurrentTrackName = result.IsTracking ? result.TrackName : null;

            this.videoTextureProjection = result.VideoBackgroundProjection.ToEngineMatrix();           

            if (result.IsTracking)
            {
                this.Pose = poseCorrectionRotationMatrix * result.TrackPose.ToEngineMatrix();
                this.PoseInv = Matrix.Invert(this.Pose);
            }
        }

        private void DestroyCameraTexture()
        {
            if (this.CameraTexture != null)
            {
                var renderTarget = this.CameraTexture as RenderTarget;
                if (renderTarget != null)
                {
                    WaveServices.GraphicsDevice.RenderTargets.DestroyRenderTarget((RenderTarget)this.CameraTexture);
                }
                else
                {
                    WaveServices.GraphicsDevice.Textures.DestroyTexture(this.CameraTexture);
                }

                this.CameraTexture = null;
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
        /// Update QCAR orientation
        /// </summary>
        private void UpdateOrientation()
        {
            switch (WaveServices.Platform.DisplayOrientation)
            {
                default:
                case DisplayOrientation.LandscapeLeft:
                    this.currentOrientation = AROrientation.ORIENTATION_LANDSCAPE_LEFT;
                    break;

                case DisplayOrientation.LandscapeRight:
                    this.currentOrientation = AROrientation.ORIENTATION_LANDSCAPE_RIGHT;
                    break;

                case DisplayOrientation.Portrait:
                    this.currentOrientation = AROrientation.ORIENTATION_PORTRAIT;
                    break;

                case DisplayOrientation.PortraitFlipped:
                    this.currentOrientation = AROrientation.ORIENTATION_PORTRAIT_UPSIDEDOWN;
                    break;
            }

            if (this.State == ARState.TRACKING)
            {
                var platform = WaveServices.Platform;
                QCAR_setOrientation(platform.ScreenWidth, platform.ScreenHeight, this.currentOrientation);
            }
        }

        /// <summary>
        /// Internals the initialize.
        /// </summary>
        /// <param name="licenseKey">The license key.</param>
        /// <returns></returns>
        protected virtual Task<bool> InternalInitialize(string licenseKey)
        {
            var vuforiaInitCallback = new VuforiaInitializedCallback(null);
            QCAR_init(licenseKey, vuforiaInitCallback.CallBack);

            return vuforiaInitCallback.Task;
        }

        /// <summary>
        /// Creates the camera texture.
        /// </summary>
        /// <param name="textureWidth">Width of the texture.</param>
        /// <param name="textureHeight">Height of the texture.</param>
        /// <returns></returns>
        protected abstract Texture CreateCameraTexture(int textureWidth, int textureHeight);

        /// <summary>
        /// Updates the camera texture.
        /// </summary>
        protected abstract void UpdateCameraTexture();
    }
}