// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using SharpDX.DXGI;
using Valve.VR;
using WaveEngine.Adapter;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.DirectX;
using WaveEngine.Framework.Services;
using WaveEngine.OpenVR.Helpers;
using ValveOpenVR = Valve.VR.OpenVR;
using WaveGraphics = WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.OpenVR
{
    public class OpenVRApplication : Application
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// MSAA Sample count
        /// </summary>
        protected int msaaSampleCount;

        /// <summary>
        /// The left eye texture bounds
        /// </summary>
        private VRTextureBounds_t leftEyeTextureBounds;

        /// <summary>
        /// The right eye texture bounds
        /// </summary>
        private VRTextureBounds_t rightEyeTextureBounds;

        /// <summary>
        /// The Swap texture
        /// </summary>
        private Texture_t swapTexture;

        /// <summary>
        /// The Swap render target
        /// </summary>
        private RenderTarget swapRenderTarget;

        /// <summary>
        /// MSAA renderTarget
        /// </summary>
        private RenderTarget msaaRenderTarget;

        /// <summary>
        /// Sprite batch used to render mirror texture
        /// </summary>
        private WaveGraphics.SpriteBatch mirrorSpriteBatch;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        internal bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the HDM
        /// </summary>
        internal CVRSystem Hmd { get; private set; }

        /// <summary>
        /// Gets the eye textures information.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        internal VREyeTexture[] EyeTextures { get; private set; }

        /// <summary>
        /// Gets the mirror texture used to draw the combined texture
        /// </summary>
        internal RenderTarget HMDMirrorRenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the composed image of the HMD will be rendered onto the screen.
        /// </summary>
        internal bool ShowHMDMirrorTexture { get; set; }

        /// <summary>
        /// Gets or sets the OpenVR application type (VRApplication_Scene by default)
        /// </summary>
        public EVRApplicationType OpenVRApplicationType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the HMD is presented in the system
        /// </summary>
        protected internal bool HmdDetected { get; private set; }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="OpenVRApplication" /> class.
        /// </summary>
        static OpenVRApplication()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenVRApplication" /> class.
        /// </summary>
        public OpenVRApplication()
            : base()
        {
            this.ShowHMDMirrorTexture = true;
            this.msaaSampleCount = 1;
            this.IsFixedTimeStep = false;
            this.OpenVRApplicationType = EVRApplicationType.VRApplication_Scene;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Configures the specified windows handler.
        /// </summary>
        /// <param name="windowsHandler">The windows handler.</param>
        public override void Configure(IntPtr windowsHandler)
        {
            base.Configure(windowsHandler);

            this.LoadNativeLibrary();

            var error = EVRInitError.None;

            this.HmdDetected = true;

            this.Hmd = ValveOpenVR.Init(ref error, this.OpenVRApplicationType);
            if (error != EVRInitError.None)
            {
                OpenVRHelper.ReportInitError(error);
                this.Hmd = null;
                this.HmdDetected = false;
            }

            // Verify common interfaces are valid.
            ValveOpenVR.GetGenericInterface(ValveOpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
            {
                OpenVRHelper.ReportInitError(error);
                this.HmdDetected = false;
            }

            ValveOpenVR.GetGenericInterface(ValveOpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
            {
                OpenVRHelper.ReportInitError(error);
                this.HmdDetected = false;
            }

            if (this.Hmd == null)
            {
                ValveOpenVR.Shutdown();
                return;
            }
        }

        private void LoadNativeLibrary()
        {
            var filename = "openvr_api.dll";
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filename);

            if (!File.Exists(path) || (LoadLibrary(path) == IntPtr.Zero))
            {
                path = Path.Combine(Path.GetTempPath(), filename);

                try
                {
                    Type type = typeof(OpenVRService);

                    string name;

                    if (Environment.Is64BitProcess)
                    {
                        name = type.Namespace + ".Libs.x64." + filename;
                    }
                    else
                    {
                        name = type.Namespace + ".Libs.x86." + filename;
                    }

                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name).CopyTo(stream);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                LoadLibrary(path);
            }
        }

        public override void Dispose()
        {
            if (this.context != null)
            {
                this.context.ClearState();
                this.context.Flush();
            }

            ValveOpenVR.Shutdown();

            base.Dispose();
        }
        #endregion

        #region Private Methods
        protected override void BaseInitialize()
        {
            if (this.HmdDetected)
            {
                var rtManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;

                uint recommendedWidth = 0;
                uint recommendedHeight = 0;
                this.Hmd.GetRecommendedRenderTargetSize(ref recommendedWidth, ref recommendedHeight);

                int rtWidth = (int)recommendedWidth * 2;
                int rtHeight = (int)recommendedHeight;

                var eyeDepthTexture = rtManager.CreateDepthTexture(rtWidth, rtHeight, this.msaaSampleCount);

                if (this.msaaSampleCount > 1)
                {
                    // Create MSAA renderTarget
                    this.msaaRenderTarget = rtManager.CreateRenderTarget(rtWidth, rtHeight, PixelFormat.R8G8B8A8, this.msaaSampleCount);
                    this.msaaRenderTarget.DepthTexture = eyeDepthTexture;
                }

                this.swapRenderTarget = rtManager.CreateRenderTarget(rtWidth, rtHeight, PixelFormat.R8G8B8A8);

                if (this.msaaSampleCount == 1)
                {
                    this.swapRenderTarget.DepthTexture = eyeDepthTexture;
                }

                var swapDXRenderTarget = rtManager.TargetFromHandle<DXRenderTarget>(this.swapRenderTarget.TextureHandle);
                this.swapTexture = new Texture_t()
                {
                    handle = swapDXRenderTarget.NativeTexture,
                    eColorSpace = EColorSpace.Auto,
                    eType = ETextureType.DirectX
                };

                this.leftEyeTextureBounds = new VRTextureBounds_t()
                {
                    uMin = 0,
                    uMax = 0.5f,
                    vMin = 0,
                    vMax = 1f
                };

                this.rightEyeTextureBounds = new VRTextureBounds_t()
                {
                    uMin = 0.5f,
                    uMax = 1f,
                    vMin = 0,
                    vMax = 1f
                };

                // Create a set of layers to submit.
                this.EyeTextures = new VREyeTexture[2];

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    var eyeTexture = new VREyeTexture();
                    this.EyeTextures[eyeIndex] = eyeTexture;

                    // Retrieve size and position of the texture for the current eye.
                    eyeTexture.Viewport = new Viewport(
                        recommendedWidth * eyeIndex / (float)this.swapRenderTarget.Width,
                        0,
                        recommendedWidth / (float)this.swapRenderTarget.Width,
                        recommendedHeight / (float)this.swapRenderTarget.Height);

                    eyeTexture.RenderTarget = (this.msaaSampleCount > 1) ? this.msaaRenderTarget : this.swapRenderTarget;
                }

                this.HMDMirrorRenderTarget = this.swapRenderTarget;
            }

            WaveServices.RegisterService(new OpenVRService());

            this.IsConnected = this.Hmd != null;

            base.BaseInitialize();
        }

        protected override void Render()
        {
            base.Render();

            if (this.IsConnected && this.HmdDetected)
            {
                if (this.msaaSampleCount > 1)
                {
                    var rtManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;
                    var dxRt = rtManager.TargetFromHandle<DXRenderTarget>(this.msaaRenderTarget.TextureHandle);
                    var dxSwapRt = rtManager.TargetFromHandle<DXRenderTarget>(this.swapRenderTarget.TextureHandle);

                    this.context.ResolveSubresource(dxRt.Target, 0, dxSwapRt.Target, 0, Format.R8G8B8A8_UNorm);
                }

                EVRCompositorError errorResult;
                errorResult = ValveOpenVR.Compositor.Submit(EVREye.Eye_Left, ref this.swapTexture, ref this.leftEyeTextureBounds, EVRSubmitFlags.Submit_Default);
                OpenVRHelper.ReportCompositeError(errorResult);

                errorResult = ValveOpenVR.Compositor.Submit(EVREye.Eye_Right, ref this.swapTexture, ref this.rightEyeTextureBounds, EVRSubmitFlags.Submit_Default);
                OpenVRHelper.ReportCompositeError(errorResult);

                if (this.ShowHMDMirrorTexture)
                {
                    this.RenderMirrorTexture();
                }
            }
        }

        private void RenderMirrorTexture()
        {
            var graphicsDevice = WaveServices.GraphicsDevice;

            if (graphicsDevice == null)
            {
                return;
            }

            if (this.mirrorSpriteBatch == null)
            {
                this.mirrorSpriteBatch = new WaveGraphics.SpriteBatch(graphicsDevice);
            }

            graphicsDevice.RenderTargets.SetRenderTarget(null);
            var viewport = graphicsDevice.Viewport;
            viewport.X = 0;
            viewport.Y = 0;
            viewport.Width = this.Width;
            viewport.Height = this.Height;
            graphicsDevice.Viewport = viewport;

            var screenRectagle = new Rectangle(0, 0, this.Width, this.Height);

            var clearColor = Color.Black;
            graphicsDevice.Clear(ref clearColor, ClearFlags.All, 1);

            this.mirrorSpriteBatch.Draw(this.swapRenderTarget, screenRectagle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            this.mirrorSpriteBatch.Render();
        }
        #endregion
    }
}
