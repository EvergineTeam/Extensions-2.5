#region File Description
//-----------------------------------------------------------------------------
// CameraResources
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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

namespace WaveEngine.Hololens.Internals
{
    /// <summary>
    /// Constant buffer used to send hologram position transform to the shader pipeline.
    /// </summary>
    internal struct ViewProjectionConstantBuffer
    {
        public Matrix4x4 viewProjection;
    }

    /// <summary>
    /// Manages DirectX device resources that are specific to a holographic camera, such as the
    /// back buffer, ViewProjection constant buffer, and viewport.
    /// </summary>
    public class CameraResources : Disposer
    {
        // Direct3D rendering objects. Required for 3D.
        Texture2D d3dBackBuffer;

        // Device resource to store view and projection matrices.
        SharpDX.Direct3D11.Buffer viewProjectionConstantBuffer;

        // Direct3D rendering properties.
        SharpDX.DXGI.Format dxgiFormat;
        RawViewportF d3dViewport;
        Size d3dRenderTargetSize;

        // Indicates whether the camera supports stereoscopic rendering.
        bool isStereo = false;

        // Pointer to the holographic camera these resources are for.
        HolographicCamera holographicCamera = null;

        #region Properties

        public Texture2D BackBufferTexture2D
        {
            get { return d3dBackBuffer; }
        }

        public Size RenderTargetSize
        {
            get { return d3dRenderTargetSize; }
        }
        public bool IsRenderingStereoscopic
        {
            get { return isStereo; }
        }

        public HolographicCamera HolographicCamera
        {
            get { return holographicCamera; }
        }

        #endregion

        #region Initialize
        public CameraResources(HolographicCamera holographicCamera)
        {
            this.holographicCamera = holographicCamera;
            isStereo = holographicCamera.IsStereo;
            d3dRenderTargetSize = holographicCamera.RenderTargetSize;

            d3dViewport.Height = (float)d3dRenderTargetSize.Height;
            d3dViewport.Width = (float)d3dRenderTargetSize.Width;
            d3dViewport.X = 0;
            d3dViewport.Y = 0;
            d3dViewport.MinDepth = 0;
            d3dViewport.MaxDepth = 1;
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
            HolographicCameraRenderingParameters cameraParameters
            )
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
            if ((null == d3dBackBuffer) || (d3dBackBuffer.NativePointer != cameraBackBuffer.NativePointer))
            {
                // This can change every frame as the system moves to the next buffer in the
                // swap chain. This mode of operation will occur when certain rendering modes
                // are activated.
                d3dBackBuffer = cameraBackBuffer;

                // Get the DXGI format for the back buffer.
                // This information can be accessed by the app using CameraResources::GetBackBufferDXGIFormat().
                Texture2DDescription backBufferDesc = BackBufferTexture2D.Description;
                dxgiFormat = backBufferDesc.Format;

                // Check for render target size changes.
                Size currentSize = holographicCamera.RenderTargetSize;
                if (d3dRenderTargetSize != currentSize)
                {
                    // Set render target size.
                    d3dRenderTargetSize = HolographicCamera.RenderTargetSize;
                }
            }

            // Create the constant buffer, if needed.
            if (null == viewProjectionConstantBuffer)
            {
                // Create a constant buffer to store view and projection matrices for the camera.
                ViewProjectionConstantBuffer viewProjectionConstantBufferData = new ViewProjectionConstantBuffer();
                viewProjectionConstantBuffer = this.ToDispose(SharpDX.Direct3D11.Buffer.Create(
                    device,
                    BindFlags.ConstantBuffer,
                    ref viewProjectionConstantBufferData));
            }
        }

        /// <summary>
        /// Releases resources associated with a holographic display back buffer.
        /// </summary>
        public void ReleaseResourcesForBackBuffer(DeviceResources deviceResources)
        {
            var context = deviceResources.D3DDeviceContext;

            this.RemoveAndDispose(ref d3dBackBuffer);

            const int D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT = 8;
            RenderTargetView[] nullViews = new RenderTargetView[D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT];

            // Ensure system references to the back buffer are released by clearing the render
            // target from the graphics pipeline state, and then flushing the Direct3D context.
            context.OutputMerger.SetRenderTargets(null, nullViews);
            context.Flush();
        }

        public void ReleaseAllDeviceResources(DeviceResources deviceResources)
        {
            ReleaseResourcesForBackBuffer(deviceResources);
            this.RemoveAndDispose(ref viewProjectionConstantBuffer);
        }
        #endregion
    }
}
