using System;
using WaveEngine.Adapter.Graphics;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;

namespace WaveEngine.OculusRift
{
    public class OVRService : UpdatableService
    {
        private HMD hmd;
        private EyeRenderDesc[] eyeRenderDesc;
        private WaveEngine.Adapter.Graphics.GraphicsDevice graphicsDevice;
        private WaveEngine.Adapter.Application application;
        private PoseF[] renderPose;
        private D3D11TextureData[] eyeTexture;
        private RenderTarget renderTarget;
        private bool firstUpdate;
        
        #region Properties
        public bool IsConnected
        {
            get;
            private set;
        }

        public RenderTarget RenderTarget 
        {
            get
            {
                return this.renderTarget;
            }
        }

        public EyeRenderDesc LeftEyeRenderDesc
        {
            get 
            {
                return this.eyeRenderDesc[(int)EyeType.Left];
            }
        }

        public EyeRenderDesc RightEyeRenderDesc
        {
            get
            {
                return this.eyeRenderDesc[(int)EyeType.Right];
            }
        }

        public PoseF LeftEyeRenderPose
        {
            get
            {
                return this.renderPose[(int)EyeType.Left];
            }
        }

        public PoseF RightEyeRenderPose
        {
            get
            {
                return this.renderPose[(int)EyeType.Right];
            }
        }
        #endregion

        #region Initialize
        public OVRService(IApplication application)
        {
            this.application = (WaveEngine.Adapter.Application)application;
            this.graphicsDevice = this.application.GraphicsDevice;
            this.renderPose = new PoseF[2];
        }

        protected override void Initialize()
        {
            hmd = null;

            // Initialize OVR
            OVR.Initialize();

            // Create the HMD
            hmd = OVR.HmdCreate(0);
            if(this.IsConnected = (hmd != null))
            {
                this.IsConnected = true;
            }
            else
            {
                hmd = OVR.HmdCreateDebug(HMDType.DK1);
                this.IsConnected = false;
            }
                

            // Attach HMD to windows
            hmd.AttachToWindow(this.application.NativeWindows);

            //var renderTargetSize = hmd.GetDefaultRenderTargetSize(0.5f);

            var renderTargetSize = new Size2(1280, 800);

            // Create our render target
            renderTarget = WaveServices.GraphicsDevice.RenderTargets.CreateRenderTarget(renderTargetSize.Width, renderTargetSize.Height);
            var InternalRenderTarget = application.Adapter.Graphics.RenderTargetManager.TargetFromHandle<DXRenderTarget>(renderTarget.TextureHandle);

            // The viewport sizes
            Rect[] eyeRenderViewport = new Rect[2];
            eyeRenderViewport[0] = new Rect(0, 0, renderTargetSize.Width / 2, renderTargetSize.Height);
            eyeRenderViewport[1] = new Rect((renderTargetSize.Width + 1) / 2, 0, renderTargetSize.Width / 2, renderTargetSize.Height);

            // Create our eye texture data
            eyeTexture = new D3D11TextureData[2];
            eyeTexture[0].Header.API = RenderAPIType.D3D11;
            eyeTexture[0].Header.TextureSize = renderTargetSize;
            eyeTexture[0].Header.RenderViewport = eyeRenderViewport[0];
            eyeTexture[0].pTexture = InternalRenderTarget.NativeTexture;
            eyeTexture[0].pSRView = InternalRenderTarget.NativeResourceView;

            // Right eye uses the same texture, but different rendering viewport
            eyeTexture[1] = eyeTexture[0];
            eyeTexture[1].Header.RenderViewport = eyeRenderViewport[1];

            // Configure d3d11
            D3D11ConfigData d3d11cfg = new D3D11ConfigData();
            d3d11cfg.Header.API = RenderAPIType.D3D11;
            d3d11cfg.Header.RTSize = hmd.Resolution;
            d3d11cfg.Header.Multisample = 1;
            d3d11cfg.pDevice = this.graphicsDevice.NativeDevice;
            d3d11cfg.pDeviceContext = this.graphicsDevice.NativeContext;
            d3d11cfg.pBackBufferRT = this.graphicsDevice.NativeBackBuffer;
            d3d11cfg.pSwapChain = this.graphicsDevice.NativeSwapChain;

            // Configure rendering
            eyeRenderDesc = new EyeRenderDesc[2];
            if (!hmd.ConfigureRendering(d3d11cfg, 
                                        DistortionCapabilities.Chromatic | 
                                        DistortionCapabilities.TimeWarp | 
                                        DistortionCapabilities.Vignette,
                                        hmd.DefaultEyeFov, eyeRenderDesc))
            {
                throw new Exception("Invalid configure rendering");
            }

            // Set enabled capabilities
            hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

            // Configure tracking
            hmd.ConfigureTracking(TrackingCapabilities.Orientation |
                                  TrackingCapabilities.Position |
                                  TrackingCapabilities.MagYawCorrection, TrackingCapabilities.None);

            // Dimiss the Health and Safety Windows
            hmd.DismissHSWDisplay();

            // Get HMD output
            this.application.SetAdapterOutput(hmd.DeviceName);
        }
        #endregion

        #region Public Methods

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
                this.renderPose[i] = hmd.GetEyePose((EyeType)i);
            }
        }

        public Matrix GetProjectionMatrix(EyeType eyeType, float nearPlane = 0.1f, float farPlane = 10000f)
        {
            Matrix result;
            Matrix projection = OVR.MatrixProjection(this.eyeRenderDesc[(int)eyeType].Fov, nearPlane, farPlane, true);
            Matrix.Transpose(ref projection, out result);

            return result;
        }

        public PoseF GetEyePose(EyeType eye)
        {
            return this.renderPose[(int)eye];
        }
        #endregion

        #region Private Methods
        protected override void Terminate()
        {
            if (hmd != null)
            {
                hmd.Dispose();
            }

            OVR.Shutdown();
        }
        #endregion
    }
}
