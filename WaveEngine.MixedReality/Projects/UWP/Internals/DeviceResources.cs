// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Holographic;
#endregion

namespace WaveEngine.MixedReality.Internals
{
    /// <summary>
    /// Controls all the DirectX device resources.
    /// </summary>
    public class DeviceResources : Disposer
    {
        /// <summary>
        /// Notifies the application that owns DeviceResources when the Direct3D device is lost.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Notifies the application that owns DeviceResources when the Direct3D device is restored.
        /// </summary>
        public event EventHandler DeviceRestored;

        // Direct3D objects.
        private Device3 d3dDevice;
        private DeviceContext3 d3dContext;
        private SharpDX.DXGI.Adapter3 dxgiAdapter;
        private DXGIDeviceManager dxgiDeviceManager;

        // Direct3D interop objects.
        private IDirect3DDevice d3dInteropDevice;

        // Direct2D factories.
        private SharpDX.Direct2D1.Factory2 d2dFactory;
        private SharpDX.DirectWrite.Factory1 dwriteFactory;
        private SharpDX.WIC.ImagingFactory2 wicFactory;

        // The holographic space provides a preferred DXGI adapter ID.
        private HolographicSpace holographicSpace = null;

        // Properties of the Direct3D device currently in use.
        private FeatureLevel d3dFeatureLevel = FeatureLevel.Level_10_0;

        // Whether or not the current Direct3D device supports the optional feature
        // for setting the render target array index from the vertex shader stage.
        private bool d3dDeviceSupportsVprt = false;

        /// <summary>
        /// Back buffer resources, etc. for attached holographic cameras.
        /// </summary>
        public Dictionary<uint, CameraResources> cameraResourcesDictionary = new Dictionary<uint, CameraResources>();
        private object cameraResourcesLock = new object();

        /// <summary>
        /// Swap chain action delegate
        /// </summary>
        /// <param name="cameraResourcesDictionary">The camera resources dictionary</param>
        public delegate void SwapChainAction(Dictionary<uint, CameraResources> cameraResourcesDictionary);

        /// <summary>
        /// Swap chain action delegate
        /// </summary>
        /// <param name="cameraResourcesDictionary">The camera resources dictionary</param>
        /// <returns>A boolean indicating the result of the operation</returns>
        public delegate bool SwapChainActionWithResult(Dictionary<uint, CameraResources> cameraResourcesDictionary);

        #region Properties

        public Device3 D3DDevice
        {
            get { return this.d3dDevice; }
        }

        public DeviceContext3 D3DDeviceContext
        {
            get { return this.d3dContext; }
        }

        public SharpDX.DXGI.Adapter3 DxgiAdapter
        {
            get { return this.dxgiAdapter; }
        }

        public DXGIDeviceManager DxgiDeviceManager
        {
            get { return this.dxgiDeviceManager; }
        }

        public IDirect3DDevice D3DInteropDevice
        {
            get { return this.d3dInteropDevice; }
        }

        public SharpDX.Direct2D1.Factory2 D2DFactory
        {
            get { return this.d2dFactory; }
        }

        public SharpDX.DirectWrite.Factory1 DWriteFactory
        {
            get { return this.dwriteFactory; }
        }

        public SharpDX.WIC.ImagingFactory2 WicImagingFactory
        {
            get { return this.wicFactory; }
        }

        public SharpDX.Direct3D.FeatureLevel D3DDeviceFeatureLevel
        {
            get { return this.d3dFeatureLevel; }
        }

        public bool D3DDeviceSupportsVprt
        {
            get { return this.d3dDeviceSupportsVprt; }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceResources"/> class.
        /// Constructor for DeviceResources.
        /// </summary>
        public DeviceResources()
        {
            this.CreateDeviceIndependentResources();
        }

        /// <summary>
        /// Configures resources that don't depend on the Direct3D device.
        /// </summary>
        private void CreateDeviceIndependentResources()
        {
            // Dispose previous references and set to null
            this.RemoveAndDispose(ref this.d2dFactory);
            this.RemoveAndDispose(ref this.dwriteFactory);
            this.RemoveAndDispose(ref this.wicFactory);

            // Initialize Direct2D resources.
            var debugLevel = SharpDX.Direct2D1.DebugLevel.None;
#if DEBUG
            debugLevel = SharpDX.Direct2D1.DebugLevel.Information;
#endif

            // Initialize the Direct2D Factory.
            this.d2dFactory = this.ToDispose(
                new SharpDX.Direct2D1.Factory2(
                    SharpDX.Direct2D1.FactoryType.SingleThreaded,
                    debugLevel));

            // Initialize the DirectWrite Factory.
            this.dwriteFactory = this.ToDispose(
                new SharpDX.DirectWrite.Factory1(SharpDX.DirectWrite.FactoryType.Shared));

            // Initialize the Windows Imaging Component (WIC) Factory.
            this.wicFactory = this.ToDispose(
                new SharpDX.WIC.ImagingFactory2());
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the holographic space
        /// </summary>
        /// <param name="holographicSpace">The holographic space</param>
        public void SetHolographicSpace(HolographicSpace holographicSpace)
        {
            // Cache the holographic space. Used to re-initalize during device-lost scenarios.
            this.holographicSpace = holographicSpace;

            this.InitializeUsingHolographicSpace();
        }

        /// <summary>
        /// Initializes usinf the holographic space
        /// </summary>
        public void InitializeUsingHolographicSpace()
        {
            // The holographic space might need to determine which adapter supports
            // holograms, in which case it will specify a non-zero PrimaryAdapterId.
            int shiftPos = sizeof(uint);
            ulong id = (ulong)this.holographicSpace.PrimaryAdapterId.LowPart | (((ulong)this.holographicSpace.PrimaryAdapterId.HighPart) << shiftPos);

            // When a primary adapter ID is given to the app, the app should find
            // the corresponding DXGI adapter and use it to create Direct3D devices
            // and device contexts. Otherwise, there is no restriction on the DXGI
            // adapter the app can use.
            if (id != 0)
            {
                // Create the DXGI factory.
                using (var dxgiFactory4 = new SharpDX.DXGI.Factory4())
                {
                    // Retrieve the adapter specified by the holographic space.
                    IntPtr adapterPtr;
                    dxgiFactory4.EnumAdapterByLuid((long)id, InteropStatics.IDXGIAdapter3, out adapterPtr);

                    if (adapterPtr != IntPtr.Zero)
                    {
                        this.dxgiAdapter = new SharpDX.DXGI.Adapter3(adapterPtr);
                    }
                }
            }
            else
            {
                this.RemoveAndDispose(ref this.dxgiAdapter);
            }

            this.CreateDeviceResources();

            this.holographicSpace.SetDirect3D11Device(this.d3dInteropDevice);
        }

        /// <summary>
        /// Validates the back buffer for each HolographicCamera and recreates
        /// resources for back buffers that have changed.
        /// Locks the set of holographic camera resources until the function exits.
        /// </summary>
        /// <param name="frame">The holographic frame</param>
        /// <param name="prediction">The holographic frame prediction</param>
        public void EnsureCameraResources(HolographicFrame frame, HolographicFramePrediction prediction)
        {
            this.UseHolographicCameraResources((Dictionary<uint, CameraResources> cameraResourcesDictionary) =>
            {
                foreach (var pose in prediction.CameraPoses)
                {
                    var renderingParameters = frame.GetRenderingParameters(pose);
                    var cameraResources = cameraResourcesDictionary[pose.HolographicCamera.Id];

                    cameraResources.CreateResourcesForBackBuffer(this, renderingParameters);
                }
            });
        }

        internal void UpdateCameraClipDistance(float nearPlane, float farPlane)
        {
            foreach (var cameraResources in this.cameraResourcesDictionary.Values)
            {
                cameraResources.HolographicCamera.SetNearPlaneDistance(Math.Max(0.1f, nearPlane));
                cameraResources.HolographicCamera.SetFarPlaneDistance(farPlane);
            }
        }

        /// <summary>
        /// Prepares to allocate resources and adds resource views for a camera.
        /// Locks the set of holographic camera resources until the function exits.
        /// </summary>
        public void AddHolographicCamera(HolographicCamera camera)
        {
            this.UseHolographicCameraResources((Dictionary<uint, CameraResources> cameraResourcesDictionary) =>
            {
                cameraResourcesDictionary.Add(camera.Id, new CameraResources(camera));
            });
        }

        // Deallocates resources for a camera and removes the camera from the set.
        // Locks the set of holographic camera resources until the function exits.
        public void RemoveHolographicCamera(HolographicCamera camera)
        {
            this.UseHolographicCameraResources((Dictionary<uint, CameraResources> cameraResourcesDictionary) =>
            {
                CameraResources cameraResources = cameraResourcesDictionary[camera.Id];

                if (cameraResources != null)
                {
                    cameraResources.ReleaseResourcesForBackBuffer(this);
                    cameraResourcesDictionary.Remove(camera.Id);
                }
            });
        }

        /// <summary>
        /// Recreate all device resources and set them back to the current state.
        /// Locks the set of holographic camera resources until the function exits.
        /// </summary>
        public void HandleDeviceLost()
        {
            this.DeviceLost.Invoke(this, null);

            this.UseHolographicCameraResources((Dictionary<uint, CameraResources> cameraResourcesDictionary) =>
            {
                foreach (var pair in cameraResourcesDictionary)
                {
                    CameraResources cameraResources = pair.Value;
                    cameraResources.ReleaseAllDeviceResources(this);
                }
            });

            this.InitializeUsingHolographicSpace();

            this.DeviceRestored.Invoke(this, null);
        }

        /// <summary>
        /// Call this method when the app suspends. It provides a hint to the driver that the app
        /// is entering an idle state and that temporary buffers can be reclaimed for use by other apps.
        /// </summary>
        public void Trim()
        {
            this.d3dContext.ClearState();

            using (var dxgiDevice = this.d3dDevice.QueryInterface<SharpDX.DXGI.Device3>())
            {
                dxgiDevice.Trim();
            }
        }

        /// <summary>
        /// Present the contents of the swap chain to the screen.
        /// Locks the set of holographic camera resources until the function exits.
        /// </summary>
        public void Present(ref HolographicFrame frame)
        {
            // By default, this API waits for the frame to finish before it returns.
            // Holographic apps should wait for the previous frame to finish before
            // starting work on a new frame. This allows for better results from
            // holographic frame predictions.

            ////frame.UpdateCurrentPrediction();
            var presentResult = frame.PresentUsingCurrentPrediction(
                HolographicFramePresentWaitBehavior.WaitForFrameToFinish);

            // The PresentUsingCurrentPrediction API will detect when the graphics device
            // changes or becomes invalid. When this happens, it is considered a Direct3D
            // device lost scenario.
            if (presentResult == HolographicFramePresentResult.DeviceRemoved)
            {
                // The Direct3D device, context, and resources should be recreated.
                this.HandleDeviceLost();
            }
        }

        /// <summary>
        /// Device-based resources for holographic cameras are stored in a std::map. Access this list by providing a
        /// callback to this function, and the std::map will be guarded from add and remove
        /// events until the callback returns. The callback is processed immediately and must
        /// not contain any nested calls to UseHolographicCameraResources.
        /// The callback takes a parameter of type Dictionary uint CameraResources cameraResourcesDictionary
        /// through which the list of cameras will be accessed.
        /// The callback also returns a boolean result.
        /// </summary>
        /// <param name="callback">The callback that will be called once the action is completed.</param>
        /// <returns>A boolean indicating whether the operation was success</returns>
        public bool UseHolographicCameraResources(SwapChainActionWithResult callback)
        {
            bool success = false;
            lock (this.cameraResourcesLock)
            {
                success = callback(this.cameraResourcesDictionary);
            }

            return success;
        }

        /// <summary>
        /// Device-based resources for holographic cameras are stored in a std::map. Access this list by providing a
        /// callback to this function, and the std::map will be guarded from add and remove
        /// events until the callback returns. The callback is processed immediately and must
        /// not contain any nested calls to UseHolographicCameraResources.
        /// The callback takes a parameter of type Dictionary uint CameraResources cameraResourcesDictionary
        /// through which the list of cameras will be accessed.
        /// </summary>
        public void UseHolographicCameraResources(SwapChainAction callback)
        {
            lock (this.cameraResourcesLock)
            {
                callback(this.cameraResourcesDictionary);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Configures the Direct3D device, and stores handles to it and the device context.
        /// </summary>
        private void CreateDeviceResources()
        {
            this.DisposeDeviceAndContext();

            // This flag adds support for surfaces with a different color channel ordering
            // than the API default. It is required for compatibility with Direct2D.
            DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;

#if DEBUG
            // If the project is in a debug build, enable debugging via SDK Layers with this flag.
            creationFlags |= DeviceCreationFlags.Debug;
#endif

            // This array defines the set of DirectX hardware feature levels this app will support.
            // Note the ordering should be preserved.
            // Note that MixedReality supports feature level 11.1. The MixedReality emulator is also capable
            // of running on graphics cards starting with feature level 10.0.
            FeatureLevel[] featureLevels =
            {
                FeatureLevel.Level_12_1,
                FeatureLevel.Level_12_0,
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };

            // Create the Direct3D 11 API device object and a corresponding context.
            try
            {
                if (this.dxgiAdapter != null)
                {
                    using (var device = new Device(this.dxgiAdapter, creationFlags, featureLevels))
                    {
                        // Store pointers to the Direct3D 11.1 API device.
                        this.d3dDevice = this.ToDispose(device.QueryInterface<Device3>());
                    }
                }
                else
                {
                    using (var device = new Device(DriverType.Hardware, creationFlags, featureLevels))
                    {
                        // Store a pointer to the Direct3D device.
                        this.d3dDevice = this.ToDispose(device.QueryInterface<Device3>());
                    }
                }
            }
            catch
            {
                // If the initialization fails, fall back to the WARP device.
                // For more information on WARP, see:
                // http://go.microsoft.com/fwlink/?LinkId=286690
                using (var device = new Device(DriverType.Warp, creationFlags, featureLevels))
                {
                    this.d3dDevice = this.ToDispose(device.QueryInterface<Device3>());
                }
            }

            // Cache the feature level of the device that was created.
            this.d3dFeatureLevel = this.d3dDevice.FeatureLevel;

            // Store a pointer to the Direct3D immediate context.
            this.d3dContext = this.ToDispose(this.d3dDevice.ImmediateContext3);

            // Acquire the DXGI interface for the Direct3D device.
            using (var dxgiDevice = this.d3dDevice.QueryInterface<SharpDX.DXGI.Device3>())
            {
                // Wrap the native device using a WinRT interop object.
                IntPtr pUnknown;
                uint hr = InteropStatics.CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out pUnknown);
                if (hr == 0)
                {
                    this.d3dInteropDevice = (IDirect3DDevice)Marshal.GetObjectForIUnknown(pUnknown);
                    Marshal.Release(pUnknown);
                }

                // Store a pointer to the DXGI adapter.
                // This is for the case of no preferred DXGI adapter, or fallback to WARP.
                this.dxgiAdapter = this.ToDispose(dxgiDevice.Adapter.QueryInterface<SharpDX.DXGI.Adapter3>());
            }

            // Check for device support for the optional feature that allows setting the render target array index from the vertex shader stage.
            var options = this.d3dDevice.CheckD3D113Features3();
            if (options.VPAndRTArrayIndexFromAnyShaderFeedingRasterizer)
            {
                this.d3dDeviceSupportsVprt = true;
            }

            // Startup MediaManager
            MediaManager.Startup();

            // Setup multithread on the Direct3D11 device
            var multithread = this.d3dDevice.QueryInterface<SharpDX.Direct3D.DeviceMultithread>();
            multithread.SetMultithreadProtected(true);

            // Create a DXGI Device Manager
            this.dxgiDeviceManager = new SharpDX.MediaFoundation.DXGIDeviceManager();
            this.dxgiDeviceManager.ResetDevice(this.d3dDevice);
        }

        /// <summary>
        /// Disposes of a device-based resources.
        /// </summary>
        private void DisposeDeviceAndContext()
        {
            // Dispose existing references to Direct3D 11 device and contxt, and set to null.
            this.RemoveAndDispose(ref this.d3dDevice);
            this.RemoveAndDispose(ref this.d3dContext);
            this.RemoveAndDispose(ref this.dxgiAdapter);
            this.RemoveAndDispose(ref this.dxgiDeviceManager);

            // Release the interop device.
            this.d3dInteropDevice = null;
        }

        #endregion
    }
}
