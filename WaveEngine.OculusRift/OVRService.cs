#region File Description
//-----------------------------------------------------------------------------
// OVRService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Adapter.Graphics;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    public class OVRService : UpdatableService
    {
        /// <summary>
        /// The HMD
        /// </summary>
        private HMD hmd;

        /// <summary>
        /// The eye render desc
        /// </summary>
        private EyeRenderDesc[] eyeRenderDesc;

        /// <summary>
        /// The graphics device
        /// </summary>
        private WaveEngine.Adapter.Graphics.GraphicsDevice graphicsDevice;

        /// <summary>
        /// The application
        /// </summary>
        private WaveEngine.Adapter.Application application;

        /// <summary>
        /// The render pose
        /// </summary>
        private PoseF[] renderPose;

        /// <summary>
        /// The eye texture
        /// </summary>
        private D3D11TextureData[] eyeTexture;

        /// <summary>
        /// The render target
        /// </summary>
        private RenderTarget renderTarget;

        /// <summary>
        /// The first update
        /// </summary>
        private bool firstUpdate;

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the render target.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        public RenderTarget RenderTarget
        {
            get
            {
                return this.renderTarget;
            }
        }

        /// <summary>
        /// Gets the left eye render desc.
        /// </summary>
        /// <value>
        /// The left eye render desc.
        /// </value>
        public EyeRenderDesc LeftEyeRenderDesc
        {
            get
            {
                return this.eyeRenderDesc[(int)EyeType.Left];
            }
        }

        /// <summary>
        /// Gets the right eye render desc.
        /// </summary>
        /// <value>
        /// The right eye render desc.
        /// </value>
        public EyeRenderDesc RightEyeRenderDesc
        {
            get
            {
                return this.eyeRenderDesc[(int)EyeType.Right];
            }
        }

        /// <summary>
        /// Gets the left eye render pose.
        /// </summary>
        /// <value>
        /// The left eye render pose.
        /// </value>
        public PoseF LeftEyeRenderPose
        {
            get
            {
                return this.renderPose[(int)EyeType.Left];
            }
        }

        /// <summary>
        /// Gets the right eye render pose.
        /// </summary>
        /// <value>
        /// The right eye render pose.
        /// </value>
        public PoseF RightEyeRenderPose
        {
            get
            {
                return this.renderPose[(int)EyeType.Right];
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="OVRService"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public OVRService(IApplication application)
        {
            this.application = (WaveEngine.Adapter.Application)application;
            this.graphicsDevice = this.application.GraphicsDevice;
            this.renderPose = new PoseF[2];
        }

        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        /// <exception cref="System.Exception">Invalid configure rendering</exception>
        protected override void Initialize()
        {
            this.hmd = null;

            // Initialize OVR
            OVR.Initialize();

            // Create the HMD
            this.hmd = OVR.HmdCreate(0);

            if (this.IsConnected = this.hmd != null)
            {
                this.IsConnected = true;
            }
            else
            {
                this.hmd = OVR.HmdCreateDebug(HMDType.DK1);
                this.IsConnected = false;
            }

            // Attach HMD to windows
            this.hmd.AttachToWindow(this.application.NativeWindows);

            var renderTargetSize = new Size2(1280, 800);  //// var renderTargetSize = hmd.GetDefaultRenderTargetSize(0.5f);

            // Create our render target
            this.renderTarget = WaveServices.GraphicsDevice.RenderTargets.CreateRenderTarget(renderTargetSize.Width, renderTargetSize.Height);
            var internalRenderTarget = this.application.Adapter.Graphics.RenderTargetManager.TargetFromHandle<DXRenderTarget>(this.renderTarget.TextureHandle);

            // The viewport sizes
            Rect[] eyeRenderViewport = new Rect[2];
            eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
            eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, renderTargetSize.Width / 2, renderTargetSize.Height);

            // Create our eye texture data
            this.eyeTexture = new D3D11TextureData[2];
            this.eyeTexture[0].Header.API = RenderAPIType.D3D11;
            this.eyeTexture[0].Header.TextureSize = renderTargetSize;
            this.eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
            this.eyeTexture[0].PTexture = internalRenderTarget.NativeTexture;
            this.eyeTexture[0].PSRView = internalRenderTarget.NativeResourceView;

            // Right eye uses the same texture, but different rendering viewport
            this.eyeTexture[1] = this.eyeTexture[0];
            this.eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];

            // Configure d3d11
            D3D11ConfigData d3d11cfg = new D3D11ConfigData();
            d3d11cfg.Header.API = RenderAPIType.D3D11;
            d3d11cfg.Header.RTSize = this.hmd.Resolution;
            d3d11cfg.Header.Multisample = 1;
            d3d11cfg.Device = this.graphicsDevice.NativeDevice;
            d3d11cfg.DeviceContext = this.graphicsDevice.NativeContext;
            d3d11cfg.BackBufferRT = this.graphicsDevice.NativeBackBuffer;
            d3d11cfg.SwapChain = this.graphicsDevice.NativeSwapChain;

            // Configure rendering
            this.eyeRenderDesc = new EyeRenderDesc[2];

            if (!this.hmd.ConfigureRendering(d3d11cfg, DistortionCapabilities.Chromatic | DistortionCapabilities.TimeWarp | DistortionCapabilities.Vignette, this.hmd.DefaultEyeFov, this.eyeRenderDesc))
            {
                throw new Exception("Invalid configure rendering");
            }

            // Set enabled capabilities
            this.hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

            // Configure tracking
            this.hmd.ConfigureTracking(TrackingCapabilities.Orientation |
                                  TrackingCapabilities.Position |
                                  TrackingCapabilities.MagYawCorrection,
                                  TrackingCapabilities.None);

            // Dimiss the Health and Safety Windows
            this.hmd.DismissHSWDisplay();

            // Get HMD output
            this.application.SetAdapterOutput(this.hmd.DeviceName);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The elapsed game time since the last update.</param>
        public override void Update(TimeSpan gameTime)
        {
            if (!this.firstUpdate)
            {
                this.firstUpdate = true;

                this.application.BeginDraw = () =>
                {
                    hmd.BeginFrame(0);
                };

                this.application.EndDraw = () =>
                {
                    hmd.EndFrame(this.renderPose, eyeTexture);
                };
            }

            for (int i = 0; i < 2; i++)
            {
                this.renderPose[i] = this.hmd.GetEyePose((EyeType)i);
            }
        }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        /// <param name="eyeType">Type of the eye.</param>
        /// <param name="nearPlane">The near plane.</param>
        /// <param name="farPlane">The far plane.</param>
        /// <returns>The result matrix.</returns>
        public Matrix GetProjectionMatrix(EyeType eyeType, float nearPlane = 0.1f, float farPlane = 10000f)
        {
            Matrix result;
            Matrix projection = OVR.MatrixProjection(this.eyeRenderDesc[(int)eyeType].Fov, nearPlane, farPlane, true);
            Matrix.Transpose(ref projection, out result);

            return result;
        }

        /// <summary>
        /// Gets the eye pose.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <returns>The Eye pose.</returns>
        public PoseF GetEyePose(EyeType eye)
        {
            return this.renderPose[(int)eye];
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
            if (this.hmd != null)
            {
                this.hmd.Dispose();
            }

            OVR.Shutdown();
        }
        #endregion
    }
}
