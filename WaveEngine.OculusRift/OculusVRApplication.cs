#region File Description
//-----------------------------------------------------------------------------
// OculusVRApplication
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OculusWrap;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Adapter;
using WaveEngine.DirectX;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Framework.Services;
using SharpDX.MediaFoundation;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Represent an Oculus VR application in WaveEngine, you need inherit of it for a new WaveEngine Oculus Rift application.
    /// </summary>
    public class OculusVRApplication : Application
    {
        /// <summary>
        /// Filename of the DllOVR wrapper file, which wraps the LibOvr.lib in a dll.
        /// </summary>
        public const string DllOvrDll = "DllOvr.dll";

        /// <summary>
        /// Default near clipo¡
        /// </summary>
        private const float DefaultNearClip = 0.1f;

        /// <summary>
        /// Default far clip
        /// </summary>
        public const float DefaultFarClip = 1000;

        /// <summary>
        /// The HMD
        /// </summary>
        internal Hmd Hmd;

        /// <summary>
        /// The eye textures info
        /// </summary>
        private OculusVREyeTexture[] eyeTextures;

        /// <summary>
        /// The render pose
        /// </summary>
        private VREyePose[] eyePoses;

        /// <summary>
        /// The tracker camera pose
        /// </summary>
        private VREyePose trackerCameraPose;

        /// <summary>
        /// Oculus Rift wrap instance
        /// </summary>
        internal Wrap Oculus;

        /// <summary>
        /// Hmd to eye viewoffsets
        /// </summary>
        private OVR.Vector3f[] hmdToEyeViewOffsets;

        /// <summary>
        /// Oculus eye poses
        /// </summary>
        private OVR.Posef[] oculusEyePoses;

        /// <summary>
        /// The DX mirror texture
        /// </summary>
        private Texture2D mirrorTexture;

        /// <summary>
        /// Ovr layers
        /// </summary>
        private Layers ovrLayers;

        /// <summary>
        /// Layer eye fov
        /// </summary>
        private LayerEyeFov layerEyeFov;

        /// <summary>
        /// Recommended texture size
        /// </summary>
        private OVR.Sizei[] recommendedTextureSize;

        /// <summary>
        /// The Swap texture set
        /// </summary>
        private RenderTarget[] swapRenderTargets;

        /// <summary>
        /// MSAA renderTarget
        /// </summary>
        private RenderTarget msaaRenderTarget;

        /// <summary>
        /// The mirror texture used to draw the combined texture
        /// </summary>
        internal RenderTarget HMDMirrorRenderTarget;

        /// <summary>
        /// Sample count
        /// </summary>
        protected int msaaSampleCount;

        /// <summary>
        /// The eye swap texture set
        /// </summary>
        private OculusWrap.D3D11.SwapTextureSet eyeSwapTextureSet;

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
        /// Gets or sets a value indicating whether if the composed image of the HMD will be rendered onto the screen.
        /// </summary>
        public bool ShowHMDMirrorTexture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the eye poses
        /// </summary>
        internal VREyePose[] EyePoses
        {
            get
            {
                return this.eyePoses;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose
        /// </summary>
        internal VREyePose TrackerCameraPose
        {
            get
            {
                return this.trackerCameraPose;
            }
        }

        /// <summary>
        /// Gets the eye textures information.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        internal OculusVREyeTexture[] EyeTextures
        {
            get
            {
                return this.eyeTextures;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="OculusVRApplication" /> class.
        /// </summary>
        static OculusVRApplication()
        {
            string subfolder;

            if (Environment.Is64BitProcess)
            {
                subfolder = "x64";
            }
            else
            {
                subfolder = "x86";
            }

            string outputPath = Path.Combine(subfolder, DllOvrDll);

            if (!File.Exists(outputPath))
            {
                if (!Directory.Exists(subfolder))
                {
                    Directory.CreateDirectory(subfolder);
                }

                var assembly = Assembly.GetAssembly(typeof(OculusVRApplication));
                string resourceName = assembly.GetName().Name + "." + subfolder + "." + DllOvrDll;
                using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(outputPath, System.IO.FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }

                        fileStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OculusVRApplication" /> class.
        /// </summary>
        public OculusVRApplication()
            : base()
        {
            this.ShowHMDMirrorTexture = true;
            this.msaaSampleCount = 1;
            this.IsFixedTimeStep = false;
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

            this.Oculus = new Wrap();

            // Initialize the Oculus runtime.
            bool success = this.Oculus.Initialize();
            if (!success)
            {
                Console.WriteLine("OVR Error: Failed to initialize the Oculus runtime library.");
                return;
            }

            // Use the head mounted display.
            OVR.GraphicsLuid graphicsLuid;
            this.Hmd = this.Oculus.Hmd_Create(out graphicsLuid);
            if (this.Hmd == null)
            {
                Console.WriteLine("OVR Error: Oculus Rift not detected.");
                return;
            }

            if (this.Hmd.ProductName == string.Empty)
            {
                Console.WriteLine("OVR Error: The HMD is not enabled.");
                return;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Base initialization
        /// </summary>
        protected override void BaseInitialize()
        {
            // OVR initialization
            this.OVRInitialization();

            base.BaseInitialize();
        }

        /// <summary>
        /// Create the swapchain
        /// </summary>
        /// <param name="windowsHandler">The windows handle.</param>
        protected override void CreateSwapChain(IntPtr windowsHandler)
        {
            DeviceCreationFlags creationFlags;
#if DEBUG
            creationFlags = DeviceCreationFlags.Debug;
#else
            creationFlags = DeviceCreationFlags.None;
#endif
            creationFlags |= DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport;

            // Define the properties of the swap chain.
            this.desc = new SwapChainDescription()
            {
                BufferCount = 1,
                IsWindowed = !this.FullScreen,
                OutputHandle = windowsHandler,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput | Usage.ShaderInput,
                SwapEffect = SwapEffect.Sequential,
                Flags = SwapChainFlags.AllowModeSwitch,
                ModeDescription = new ModeDescription()
                {
                    Width = this.Width,
                    Height = this.Height,
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(0, 1)
                }
            };

            FeatureLevel[] supportedLevels = new FeatureLevel[]
            {
                FeatureLevel.Level_11_0,
            };

            // Create Device and SwapChain
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, creationFlags, supportedLevels, this.desc, out this.device, out this.swapChain);

            // Startup MediaManager
            MediaManager.Startup();

            // Setup multithread on the Direct3D11 device
            var multithread = this.device.QueryInterface<SharpDX.Direct3D.DeviceMultithread>();
            multithread.SetMultithreadProtected(true);

            // Create a DXGI Device Manager
            this.dxgiDeviceManager = new SharpDX.MediaFoundation.DXGIDeviceManager();
            this.dxgiDeviceManager.ResetDevice(this.device);
        }

        /// <summary>
        /// OVR initialization
        /// </summary>
        private void OVRInitialization()
        {
            try
            {
                this.adapter.GraphicsDevice.IsSrgbModeEnabled = true;
                var renderTargetManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;

                // Specify which head tracking capabilities to enable.
                this.Hmd.SetEnabledCaps(OVR.HmdCaps.DebugDevice);

                // Start the sensor which informs of the Rift's pose and motion
                this.Hmd.ConfigureTracking(OVR.TrackingCaps.ovrTrackingCap_Orientation | OVR.TrackingCaps.ovrTrackingCap_MagYawCorrection | OVR.TrackingCaps.ovrTrackingCap_Position, OVR.TrackingCaps.None);

                OVR.ovrResult result;

                // Retrieve the DXGI device, in order to set the maximum frame latency.
                using (SharpDX.DXGI.Device1 dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device1>())
                {
                    dxgiDevice.MaximumFrameLatency = 1;
                }

                this.ovrLayers = new Layers();
                this.layerEyeFov = this.ovrLayers.AddLayerEyeFov();

                // Create a set of layers to submit.
                this.eyeTextures = new OculusVREyeTexture[2];
                this.eyePoses = new VREyePose[3];
                this.oculusEyePoses = new OVR.Posef[2];
                this.hmdToEyeViewOffsets = new OVR.Vector3f[2];

                result = this.CreateVRSwapTextureSet();
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create swap texture set.");

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    OVR.EyeType eye = (OVR.EyeType)eyeIndex;
                    OculusVREyeTexture eyeTexture = new OculusVREyeTexture();
                    this.eyeTextures[eyeIndex] = eyeTexture;

                    // Retrieve size and position of the texture for the current eye.
                    eyeTexture.FieldOfView = this.Hmd.DefaultEyeFov[eyeIndex];
                    eyeTexture.NearPlane = DefaultNearClip;
                    eyeTexture.FarPlane = DefaultFarClip;
                    eyeTexture.TextureSize = new OVR.Sizei(this.swapRenderTargets[0].Width, this.swapRenderTargets[0].Height);
                    eyeTexture.RenderDescription = this.Hmd.GetRenderDesc(eye, this.Hmd.DefaultEyeFov[eyeIndex]);
                    eyeTexture.HmdToEyeViewOffset = eyeTexture.RenderDescription.HmdToEyeViewOffset;
                    eyeTexture.ViewportSize.Position = new OVR.Vector2i(this.recommendedTextureSize[0].Width * eyeIndex, 0);
                    eyeTexture.ViewportSize.Size = this.recommendedTextureSize[eyeIndex];
                    eyeTexture.Viewport = new Viewport(
                        eyeTexture.ViewportSize.Position.x / (float)this.swapRenderTargets[0].Width,
                        eyeTexture.ViewportSize.Position.y / (float)this.swapRenderTargets[0].Height,
                        eyeTexture.ViewportSize.Size.Width / (float)this.swapRenderTargets[0].Width,
                        eyeTexture.ViewportSize.Size.Height / (float)this.swapRenderTargets[0].Height,
                        0.0f,
                        1.0f);

                    this.hmdToEyeViewOffsets[eyeIndex] = eyeTexture.HmdToEyeViewOffset;

                    // Specify the texture to show on the HMD.
                    this.layerEyeFov.ColorTexture[eyeIndex] = this.eyeSwapTextureSet.SwapTextureSetPtr;
                    this.layerEyeFov.Viewport[eyeIndex] = eyeTexture.ViewportSize;
                    this.layerEyeFov.Fov[eyeIndex] = eyeTexture.FieldOfView;
                    this.layerEyeFov.Header.Flags = OVR.LayerFlags.HighQuality;
                }

                // Define the texture used to display the rendered result on the computer monitor.
                Texture2DDescription mirrorTextureDescription = new Texture2DDescription();
                mirrorTextureDescription.Width = this.Width;
                mirrorTextureDescription.Height = this.Height;
                mirrorTextureDescription.ArraySize = 1;
                mirrorTextureDescription.MipLevels = 1;
                mirrorTextureDescription.Format = Format.R8G8B8A8_UNorm_SRgb;
                mirrorTextureDescription.SampleDescription = new SampleDescription(1, 0);
                mirrorTextureDescription.Usage = ResourceUsage.Default;
                mirrorTextureDescription.CpuAccessFlags = CpuAccessFlags.None;
                mirrorTextureDescription.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;

                // Convert the SharpDX texture description to the native Direct3D texture description.
                OVR.D3D11.D3D11_TEXTURE2D_DESC mirrorTextureDescriptionD3D11 = OculusVRHelpers.CreateTexture2DDescription(mirrorTextureDescription);
                OculusWrap.D3D11.MirrorTexture mirrorTexture;

                // Create the texture used to display the rendered result on the computer monitor.
                result = this.Hmd.CreateMirrorTextureD3D11(device.NativePointer, ref mirrorTextureDescriptionD3D11, OVR.D3D11.SwapTextureSetD3D11Flags.None, out mirrorTexture);
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create mirror texture.");

                this.mirrorTexture = new Texture2D(mirrorTexture.Texture.Texture);
                this.HMDMirrorRenderTarget = renderTargetManager.CreateRenderTarget(this.mirrorTexture.NativePointer);

                WaveServices.RegisterService(new OculusVRService(this));

                this.IsConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Create VR swap texture set
        /// </summary>        
        /// <returns>The result creation vr texture swap</returns>
        private OVR.ovrResult CreateVRSwapTextureSet()
        {
            var renderTargetManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;

            OVR.ovrResult result;
            this.recommendedTextureSize = new OVR.Sizei[2];
            this.recommendedTextureSize[0] = this.Hmd.GetFovTextureSize(OVR.EyeType.Left, this.Hmd.DefaultEyeFov[0], 1);
            this.recommendedTextureSize[1] = this.Hmd.GetFovTextureSize(OVR.EyeType.Right, this.Hmd.DefaultEyeFov[1], 1);

            int rtWidth = this.recommendedTextureSize[0].Width + this.recommendedTextureSize[1].Width;
            int rtHeight = Math.Max(this.recommendedTextureSize[0].Height, this.recommendedTextureSize[1].Height);

            var eyeDepthTexture = renderTargetManager.CreateDepthTexture(rtWidth, rtHeight, this.msaaSampleCount);

            if (this.msaaSampleCount > 1)
            {
                // Create MSAA renderTarget
                this.msaaRenderTarget = renderTargetManager.CreateRenderTarget(rtWidth, rtHeight, PixelFormat.R8G8BA8_sRGB, this.msaaSampleCount);
                this.msaaRenderTarget.DepthTexture = eyeDepthTexture;
            }

            // Define a texture at the size recommended for the eye texture.
            Texture2DDescription eyeSwapTextureDescription = new Texture2DDescription();
            eyeSwapTextureDescription.Width = rtWidth;
            eyeSwapTextureDescription.Height = rtHeight;
            eyeSwapTextureDescription.ArraySize = 1;
            eyeSwapTextureDescription.MipLevels = 1;
            eyeSwapTextureDescription.Format = Format.R8G8B8A8_UNorm_SRgb;
            eyeSwapTextureDescription.SampleDescription = new SampleDescription(1, 0);
            eyeSwapTextureDescription.Usage = ResourceUsage.Default;
            eyeSwapTextureDescription.CpuAccessFlags = CpuAccessFlags.None;
            eyeSwapTextureDescription.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;

            // Convert the SharpDX texture description to the native Direct3D texture description.
            OVR.D3D11.D3D11_TEXTURE2D_DESC swapTextureDescriptionD3D11 = OculusVRHelpers.CreateTexture2DDescription(eyeSwapTextureDescription);

            // Create a SwapTextureSet, which will contain the textures to render to, for the current eye.
            result = this.Hmd.CreateSwapTextureSetD3D11(this.device.NativePointer, ref swapTextureDescriptionD3D11, OVR.D3D11.SwapTextureSetD3D11Flags.None, out this.eyeSwapTextureSet);
            OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create swap texture set.");

            this.swapRenderTargets = new RenderTarget[this.eyeSwapTextureSet.TextureCount];

            // Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
            for (int textureIndex = 0; textureIndex < this.eyeSwapTextureSet.TextureCount; textureIndex++)
            {
                // Retrieve the current textureData object.
                OVR.D3D11.D3D11TextureData textureData = this.eyeSwapTextureSet.Textures[textureIndex];

                this.swapRenderTargets[textureIndex] = renderTargetManager.CreateRenderTarget(textureData.Texture);
                if (this.msaaSampleCount == 1)
                {
                    this.swapRenderTargets[textureIndex].DepthTexture = eyeDepthTexture;
                }
            }

            return result;
        }

        /// <summary>
        /// OVRRender renderer
        /// </summary>
        protected override void Render()
        {
            int currentIndex = 0;

            if (this.IsConnected)
            {
                OVR.FrameTiming frameTiming = this.Hmd.GetFrameTiming(0);
                OVR.TrackingState trackingState = this.Hmd.GetTrackingState(frameTiming.DisplayMidpointSeconds);

                // Calculate the position and orientation of each eye.
                this.Oculus.CalcEyePoses(trackingState.HeadPose.ThePose, this.hmdToEyeViewOffsets, ref this.oculusEyePoses);

                trackingState.CameraPose.Position.ToVector3(out this.trackerCameraPose.Position);
                trackingState.CameraPose.Orientation.ToQuaternion(out this.trackerCameraPose.Orientation);

                currentIndex = this.eyeSwapTextureSet.CurrentIndex++;

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    this.oculusEyePoses[eyeIndex].Position.ToVector3(out this.eyePoses[eyeIndex].Position);
                    this.oculusEyePoses[eyeIndex].Orientation.ToQuaternion(out this.eyePoses[eyeIndex].Orientation);
                    this.eyeTextures[eyeIndex].RenderTarget = (this.msaaSampleCount > 1) ? this.msaaRenderTarget : this.swapRenderTargets[currentIndex];

                    // Get eye projection
                    OVR.ovrMatrix4f_Projection(this.eyeTextures[eyeIndex].FieldOfView, this.eyeTextures[eyeIndex].NearPlane, this.eyeTextures[eyeIndex].FarPlane, OVR.ProjectionModifier.RightHanded).ToMatrix(out this.eyePoses[eyeIndex].Projection);

                    this.layerEyeFov.RenderPose[eyeIndex] = this.oculusEyePoses[eyeIndex];
                }

                // Calc central position
                Vector3.Lerp(ref this.eyePoses[0].Position, ref this.eyePoses[1].Position, 0.5f, out this.eyePoses[2].Position);
                Quaternion.Lerp(ref this.eyePoses[0].Orientation, ref this.eyePoses[1].Orientation, 0.5f, out this.eyePoses[2].Orientation);
            }

            base.Render();

            if (this.IsConnected)
            {
                if (this.msaaSampleCount > 1)
                {
                    var rtManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;
                    var dxRt = rtManager.TargetFromHandle<DXRenderTarget>(this.msaaRenderTarget.TextureHandle);
                    var dxSwapRt = rtManager.TargetFromHandle<DXRenderTarget>(this.swapRenderTargets[currentIndex].TextureHandle);

                    this.context.ResolveSubresource(dxRt.Target, 0, dxSwapRt.Target, 0, Format.R8G8B8A8_UNorm_SRgb);
                }

                // Submit frame to HMD
                this.Hmd.SubmitFrame(0, this.ovrLayers);

                if (this.ShowHMDMirrorTexture)
                {
                    // Show mirror texture into BackBuffer
                    this.device.ImmediateContext.CopyResource(this.mirrorTexture, this.backBuffer);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (this.context != null)
            {
                this.context.ClearState();
                this.context.Flush();
            }

            Dispose(this.mirrorTexture);
            Dispose(this.eyeSwapTextureSet);

            // Disposing the device, before the hmd, will cause the hmd to fail when disposing.
            // Disposing the device, after the hmd, will cause the dispose of the device to fail.
            // It looks as if the hmd steals ownership of the device and destroys it, when it's shutting down.
            // device.Dispose();
            Dispose(this.Hmd);
            Dispose(this.Oculus);

            base.Dispose();
        }

        /// <summary>
        /// Extract embedded resources and copy to output dir
        /// </summary>
        /// <param name="outputDir">Location where you want to copy the resource</param>
        /// <param name="resourceLocation">Namespace (+ dirnames)</param>
        /// <param name="files">List of files within the resourcelocation, you want to copy</param>
        private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, params string[] files)
        {
            foreach (string file in files)
            {
                var filePath = resourceLocation + @"." + file;
                using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
                {
                    var path = System.IO.Path.Combine(outputDir, file);
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }

                        fileStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Dispose the specified object, unless it's a null object.
        /// </summary>
        /// <param name="disposable">Object to dispose.</param>
        private static void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        #endregion
    }
}
