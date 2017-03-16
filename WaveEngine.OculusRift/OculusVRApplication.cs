#region File Description
//-----------------------------------------------------------------------------
// OculusVRApplication
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
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
using static OculusWrap.OVRTypes;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Represent an Oculus VR application in WaveEngine, you need inherit of it for a new WaveEngine Oculus Rift application.
    /// </summary>
    public class OculusVRApplication : Application
    {
        /// <summary>
        /// The Interface ID of the Direct3D Texture2D interface.
        /// </summary>
        private readonly Guid textureInterfaceId = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

        /// <summary>
        /// Default near clip
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
        /// The left controller pose
        /// </summary>
        private VREyePose leftControllerPose;

        /// <summary>
        /// The right controller pose
        /// </summary>
        private VREyePose rightControllerPose;

        /// <summary>
        /// Oculus Rift wrap instance
        /// </summary>
        internal Wrap Oculus;

        /// <summary>
        /// Hmd to eye viewoffsets
        /// </summary>
        private OVRTypes.Vector3f[] hmdToEyeViewOffsets;

        /// <summary>
        /// Oculus eye poses
        /// </summary>
        private OVRTypes.Posef[] oculusEyePoses;

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
        private OculusWrap.LayerEyeFov layerEyeFov;

        /// <summary>
        /// Recommended texture size
        /// </summary>
        private OVRTypes.Sizei[] recommendedTextureSize;

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
        /// The eye texture swap chain
        /// </summary>
        private OculusWrap.TextureSwapChain eyeTextureSwapChain;

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

        /// <summary>
        /// Gets the left controller pose
        /// </summary>
        internal VREyePose LeftControllerPose
        {
            get
            {
                return this.leftControllerPose;
            }
        }

        /// <summary>
        /// Gets the right controller pose
        /// </summary>
        internal VREyePose RightControllerPose
        {
            get
            {
                return this.rightControllerPose;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="OculusVRApplication" /> class.
        /// </summary>
        static OculusVRApplication()
        {
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
            OVRTypes.InitParams initializationParameters = new OVRTypes.InitParams();
            initializationParameters.Flags = OVRTypes.InitFlags.RequestVersion;
            initializationParameters.RequestedMinorVersion = 0;

#if DEBUG
            initializationParameters.Flags |= OVRTypes.InitFlags.Debug;
#endif

            bool success = Oculus.Initialize(initializationParameters);
            if (!success)
            {
                Console.WriteLine("OVR Error: Failed to initialize the Oculus runtime library.");
                return;
            }

            // Use the head mounted display.
            OVRTypes.GraphicsLuid graphicsLuid;
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

            DeviceCreationFlags creationFlags;
#if DEBUG
            creationFlags = DeviceCreationFlags.Debug;
#else
            creationFlags = DeviceCreationFlags.None;
#endif

            if (this.HasVideoSupport)
            {
                try
                {
                    // Startup MediaManager
                    MediaManager.Startup();

                    // Create a DXGI Device Manager
                    this.dxgiDeviceManager = new SharpDX.MediaFoundation.DXGIDeviceManager();
                    this.dxgiDeviceManager.ResetDevice(this.device);

                    creationFlags |= DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport;
                }
                catch(Exception)
                {
                    this.dxgiDeviceManager = null;

                    // Shutdown MediaManager
                    MediaManager.Shutdown();
                }
            }

            // Create Device and SwapChain
            //SharpDX.DXGI.Factory f = new SharpDX.DXGI.Factory1();
            //SharpDX.DXGI.Adapter a = f.GetAdapter(0);
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, creationFlags, supportedLevels, this.desc, out this.device, out this.swapChain);

            // Setup multithread on the Direct3D11 device
            var multithread = this.device.QueryInterface<SharpDX.Direct3D.DeviceMultithread>();
            multithread.SetMultithreadProtected(true);
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

                OVRTypes.Result result;

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
                this.oculusEyePoses = new OVRTypes.Posef[2];
                this.hmdToEyeViewOffsets = new OVRTypes.Vector3f[2];

                result = this.CreateVRSwapTextureSet();
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create swap texture set.");

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    OVRTypes.EyeType eye = (OVRTypes.EyeType)eyeIndex;
                    OculusVREyeTexture eyeTexture = new OculusVREyeTexture();
                    this.eyeTextures[eyeIndex] = eyeTexture;

                    // Retrieve size and position of the texture for the current eye.
                    eyeTexture.FieldOfView = this.Hmd.DefaultEyeFov[eyeIndex];
                    eyeTexture.NearPlane = DefaultNearClip;
                    eyeTexture.FarPlane = DefaultFarClip;
                    eyeTexture.TextureSize = new OVRTypes.Sizei(this.swapRenderTargets[0].Width, this.swapRenderTargets[0].Height);
                    eyeTexture.RenderDescription = this.Hmd.GetRenderDesc(eye, this.Hmd.DefaultEyeFov[eyeIndex]);
                    eyeTexture.HmdToEyeViewOffset = eyeTexture.RenderDescription.HmdToEyeOffset;
                    eyeTexture.ViewportSize.Position = new OVRTypes.Vector2i(this.recommendedTextureSize[0].Width * eyeIndex, 0);
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
                    this.layerEyeFov.ColorTexture[eyeIndex] = this.eyeTextureSwapChain.TextureSwapChainPtr;
                    this.layerEyeFov.Viewport[eyeIndex] = eyeTexture.ViewportSize;
                    this.layerEyeFov.Fov[eyeIndex] = eyeTexture.FieldOfView;
                    this.layerEyeFov.Header.Flags = OVRTypes.LayerFlags.HighQuality;
                }

                // Define the texture used to display the rendered result on the computer monitor.
                OVRTypes.MirrorTextureDesc mirrorTextureDescription = new OVRTypes.MirrorTextureDesc()
                {
                    Format = OVRTypes.TextureFormat.R8G8B8A8_UNORM_SRGB,
                    Width = this.Width,
                    Height = this.Height,
                    MiscFlags = OVRTypes.TextureMiscFlags.None
                };

                OculusWrap.MirrorTexture mirrorTexture;

                // Create the texture used to display the rendered result on the computer monitor.
                result = this.Hmd.CreateMirrorTextureDX(device.NativePointer, mirrorTextureDescription, out mirrorTexture);
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create mirror texture.");

                // Retrieve the Direct3D texture contained in the Oculus MirrorTexture.
                IntPtr mirrorTextureComPtr = IntPtr.Zero;
                result = mirrorTexture.GetBufferDX(textureInterfaceId, out mirrorTextureComPtr);
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to retrieve the texture from the created mirror texture buffer.");

                this.mirrorTexture = new Texture2D(mirrorTextureComPtr);
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
        private OVRTypes.Result CreateVRSwapTextureSet()
        {
            var renderTargetManager = this.adapter.Graphics.RenderTargetManager as RenderTargetManager;

            OVRTypes.Result result;
            this.recommendedTextureSize = new OVRTypes.Sizei[2];
            this.recommendedTextureSize[0] = this.Hmd.GetFovTextureSize(OVRTypes.EyeType.Left, this.Hmd.DefaultEyeFov[0], 1);
            this.recommendedTextureSize[1] = this.Hmd.GetFovTextureSize(OVRTypes.EyeType.Right, this.Hmd.DefaultEyeFov[1], 1);

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
            OVRTypes.TextureSwapChainDesc eyeSwapTextureDescription = new OVRTypes.TextureSwapChainDesc()
            {
                Type = OVRTypes.TextureType.Texture2D,
                Format = OVRTypes.TextureFormat.R8G8B8A8_UNORM_SRGB,
                ArraySize = 1,
                Width = rtWidth,
                Height = rtHeight,
                MipLevels = 1,
                SampleCount = 1,
                StaticImage = 0,
                MiscFlags = OVRTypes.TextureMiscFlags.None,
                BindFlags = OVRTypes.TextureBindFlags.DX_RenderTarget
            };

            // Create a SwapTextureSet, which will contain the textures to render to, for the current eye.
            result = this.Hmd.CreateTextureSwapChainDX(this.device.NativePointer, eyeSwapTextureDescription, out this.eyeTextureSwapChain);
            OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to create swap texture set.");

            // Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
            int textureCount = 0;
            this.eyeTextureSwapChain.GetLength(out textureCount);
            this.swapRenderTargets = new RenderTarget[textureCount];

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
            {
                // Retrieve the Direct3D texture contained in the Oculus TextureSwapChainBuffer.
                IntPtr swapChainTextureComPtr = IntPtr.Zero;
                this.eyeTextureSwapChain.GetBufferDX(textureIndex, textureInterfaceId, out swapChainTextureComPtr);

                this.swapRenderTargets[textureIndex] = renderTargetManager.CreateRenderTarget(swapChainTextureComPtr);
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
            OVRTypes.Result result;
            int currentIndex = 0;

            if (this.IsConnected)
            {
                double frameTiming = this.Hmd.GetPredictedDisplayTime(0);
                OVRTypes.TrackingState trackingState = this.Hmd.GetTrackingState(frameTiming, true);

                // Update tracker camera pose
                var trackerPose = this.Hmd.GetTrackerPose(0);
                trackerPose.Pose.Position.ToVector3(out this.trackerCameraPose.Position);
                trackerPose.Pose.Orientation.ToQuaternion(out this.trackerCameraPose.Orientation);

                // Update controller poses
                var leftControllerPose = trackingState.HandPoses[(int)HandType.Left];
                leftControllerPose.ThePose.Position.ToVector3(out this.leftControllerPose.Position);
                leftControllerPose.ThePose.Orientation.ToQuaternion(out this.leftControllerPose.Orientation);

                var rightControllerPose = trackingState.HandPoses[(int)HandType.Right];
                rightControllerPose.ThePose.Position.ToVector3(out this.rightControllerPose.Position);
                rightControllerPose.ThePose.Orientation.ToQuaternion(out this.rightControllerPose.Orientation);

                // Calculate the position and orientation of each eye.
                this.Oculus.CalcEyePoses(trackingState.HeadPose.ThePose, this.hmdToEyeViewOffsets, ref this.oculusEyePoses);

                // Retrieve the index of the active texture
                result = this.eyeTextureSwapChain.GetCurrentIndex(out currentIndex);
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to retrieve texture swap chain current index.");

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    OculusVREyeTexture eyeTexture = this.eyeTextures[eyeIndex];

                    this.oculusEyePoses[eyeIndex].Position.ToVector3(out this.eyePoses[eyeIndex].Position);
                    this.oculusEyePoses[eyeIndex].Orientation.ToQuaternion(out this.eyePoses[eyeIndex].Orientation);
                    eyeTexture.RenderTarget = (this.msaaSampleCount > 1) ? this.msaaRenderTarget : this.swapRenderTargets[currentIndex];

                    // Get eye projection
                    this.Oculus.Matrix4f_Projection(eyeTexture.FieldOfView, eyeTexture.NearPlane, eyeTexture.FarPlane, OVRTypes.ProjectionModifier.None).ToMatrix(out this.eyePoses[eyeIndex].Projection);

                    this.layerEyeFov.RenderPose[eyeIndex] = this.oculusEyePoses[eyeIndex];
                }

                // Commits any pending changes to the TextureSwapChain, and advances its current index
                result = this.eyeTextureSwapChain.Commit();
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to commit the swap chain texture.");

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
                result = this.Hmd.SubmitFrame(0, this.ovrLayers);
                OculusVRHelpers.WriteErrorDetails(this.Oculus, result, "Failed to submit the frame of the current layers.");

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
            
            Dispose(this.eyeTextureSwapChain);
            
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
