// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Windows.Graphics.Holographic;
using Windows.Foundation;
using Windows.Perception.Spatial;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct3D11;
using System.Numerics;
using Windows.Graphics.DirectX.Direct3D11;
using System.Runtime.InteropServices;
using System;
#endregion

namespace WaveEngine.MixedReality.Internals
{
    /// <summary>
    /// Manages DirectX device resources that are specific to a holographic camera, such as the
    /// back buffer, ViewProjection constant buffer, and viewport.
    /// </summary>
    public class CameraResources : Disposer
    {
        // Direct3D rendering objects. Required for 3D.
        private Texture2D d3dBackBuffer;

        // Device resource to store view and projection matrices.
        private SharpDX.Direct3D11.Buffer viewProjectionConstantBuffer;

        // Direct3D rendering properties.
        private SharpDX.DXGI.Format dxgiFormat;
        private RawViewportF d3dViewport;
        private Size d3dRenderTargetSize;

        // Indicates whether the camera supports stereoscopic rendering.
        private bool isStereo = false;

        // Pointer to the holographic camera these resources are for.
        private HolographicCamera holographicCamera = null;

        #region Properties

        /// <summary>
        /// Gets the back buffer texture.
        /// </summary>
        public Texture2D BackBufferTexture2D
        {
            get { return this.d3dBackBuffer; }
        }

        /// <summary>
        /// Gets the render target size.
        /// </summary>
        public Size RenderTargetSize
        {
            get { return this.d3dRenderTargetSize; }
        }

        /// <summary>
        /// Gets a value indicating whether the stereoscopic rendering feature is active.
        /// </summary>
        public bool IsRenderingStereoscopic
        {
            get { return this.isStereo; }
        }

        /// <summary>
        /// Gets the holographic camera
        /// </summary>
        public HolographicCamera HolographicCamera
        {
            get { return this.holographicCamera; }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraResources"/> class.
        /// </summary>
        /// <param name="holographicCamera">The holographic camera</param>
        public CameraResources(HolographicCamera holographicCamera)
        {
            this.holographicCamera = holographicCamera;
            this.isStereo = holographicCamera.IsStereo;
            this.d3dRenderTargetSize = holographicCamera.RenderTargetSize;

            this.d3dViewport.Height = (float)this.d3dRenderTargetSize.Height;
            this.d3dViewport.Width = (float)this.d3dRenderTargetSize.Width;
            this.d3dViewport.X = 0;
            this.d3dViewport.Y = 0;
            this.d3dViewport.MinDepth = 0;
            this.d3dViewport.MaxDepth = 1;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Updates resources associated with a holographic camera's swap chain.
        /// The app does not access the swap chain directly, but it does create
        /// resource views for the back buffer.
        /// </summary>
        public void CreateResourcesForBackBuffer(
            DeviceResources deviceResources,
            HolographicCameraRenderingParameters cameraParameters)
        {
            var device = deviceResources.D3DDevice;

            // Get the WinRT object representing the holographic camera's back buffer.
            IDirect3DSurface surface = cameraParameters.Direct3D11BackBuffer;

            // Get a DXGI interface for the holographic camera's back buffer.
            // Holographic cameras do not provide the DXGI swap chain, which is owned
            // by the system. The Direct3D back buffer resource is provided using WinRT
            // interop APIs.
            InteropStatics.IDirect3DDxgiInterfaceAccess surfaceDxgiInterfaceAccess = surface as InteropStatics.IDirect3DDxgiInterfaceAccess;
            IntPtr pResource = surfaceDxgiInterfaceAccess.GetInterface(InteropStatics.ID3D11Resource);
            Resource resource = SharpDX.CppObject.FromPointer<Resource>(pResource);
            Marshal.Release(pResource);

            // Get a Direct3D interface for the holographic camera's back buffer.
            Texture2D cameraBackBuffer = resource.QueryInterface<Texture2D>();

            // Determine if the back buffer has changed. If so, ensure that the render target view
            // is for the current back buffer.
            if ((this.d3dBackBuffer == null) || (this.d3dBackBuffer.NativePointer != cameraBackBuffer.NativePointer))
            {
                // This can change every frame as the system moves to the next buffer in the
                // swap chain. This mode of operation will occur when certain rendering modes
                // are activated.
                this.d3dBackBuffer = cameraBackBuffer;

                // Get the DXGI format for the back buffer.
                // This information can be accessed by the app using CameraResources::GetBackBufferDXGIFormat().
                Texture2DDescription backBufferDesc = this.BackBufferTexture2D.Description;
                // backBufferDesc.SampleDescription = new SharpDX.DXGI.SampleDescription(8, 8);
                this.dxgiFormat = backBufferDesc.Format;

                // Check for render target size changes.
                Size currentSize = this.holographicCamera.RenderTargetSize;
                if (this.d3dRenderTargetSize != currentSize)
                {
                    // Set render target size.
                    this.d3dRenderTargetSize = this.HolographicCamera.RenderTargetSize;
                }
            }

            // Create the constant buffer, if needed.
            if (this.viewProjectionConstantBuffer == null)
            {
                // Create a constant buffer to store view and projection matrices for the camera.
                ViewProjectionConstantBuffer viewProjectionConstantBufferData = new ViewProjectionConstantBuffer();
                this.viewProjectionConstantBuffer = this.ToDispose(SharpDX.Direct3D11.Buffer.Create(
                    device,
                    BindFlags.ConstantBuffer,
                    ref viewProjectionConstantBufferData));
            }
        }

        /// <summary>
        /// Releases resources associated with a holographic display back buffer.
        /// </summary>
        /// <param name="deviceResources">The device resources</param>
        public void ReleaseResourcesForBackBuffer(DeviceResources deviceResources)
        {
            var context = deviceResources.D3DDeviceContext;

            this.RemoveAndDispose(ref this.d3dBackBuffer);

            const int D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT = 8;
            RenderTargetView[] nullViews = new RenderTargetView[D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT];

            // Ensure system references to the back buffer are released by clearing the render
            // target from the graphics pipeline state, and then flushing the Direct3D context.
            context.OutputMerger.SetRenderTargets(null, nullViews);
            context.Flush();
        }

        /// <summary>
        /// Releases all device resources.
        /// </summary>
        /// <param name="deviceResources">The device resources</param>
        public void ReleaseAllDeviceResources(DeviceResources deviceResources)
        {
            this.ReleaseResourcesForBackBuffer(deviceResources);
            this.RemoveAndDispose(ref this.viewProjectionConstantBuffer);
        }
        #endregion
    }
}
