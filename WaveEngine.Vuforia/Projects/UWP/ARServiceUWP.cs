// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.DirectX;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia platform specific integration service
    /// </summary>
    internal class ARServiceUWP : ARServiceBase
    {
        private static readonly Matrix CameraCorrectionRotationMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

        #region P/Invoke
        [DllImport(DllName)]
        private extern static void QCAR_setVideoTexture(IntPtr texturePtr);

        [DllImport(DllName)]
        private extern static void QCAR_updateVideoTexture(IntPtr devicePtr);

        [DllImport(DllName)]
        private extern static bool QCAR_setHolographicAppCS(IntPtr appSpecifiedCS);
        #endregion

        #region Variables
        private IntPtr dxDevicePtr;

        private RasterizerState rasterizerState;
        private Device dxDevice;
        private DeviceContext dxContext;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ARServiceUWP"/> class.
        /// </summary>
        public ARServiceUWP()
            : base()
        {
            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            this.dxDevice = adapter.GraphicsDevice.DeviceDirect3D;
            this.dxContext = adapter.GraphicsDevice.ContextDirect3D;
        }

        /// <inheritdoc />
        public override bool StopTracking()
        {
            this.dxDevicePtr = IntPtr.Zero;

            return base.StopTracking();
        }

        /// <inheritdoc />
        protected override Task<bool> InternalInitialize(string licenseKey)
        {
            var rasterizeDescription = new SharpDX.Direct3D11.RasterizerStateDescription()
            {
                CullMode = SharpDX.Direct3D11.CullMode.None,
                FillMode = SharpDX.Direct3D11.FillMode.Solid,
                IsFrontCounterClockwise = false,
                DepthBias = 0,
                SlopeScaledDepthBias = 0,
                DepthBiasClamp = 0.0f,
                IsDepthClipEnabled = true,
                IsScissorEnabled = false,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = false,
            };

            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            this.rasterizerState = new SharpDX.Direct3D11.RasterizerState(adapter.GraphicsDevice.DeviceDirect3D, rasterizeDescription);

            return base.InternalInitialize(licenseKey);
        }

        /// <inheritdoc />
        protected override Texture CreateCameraTexture(int textureWidth, int textureHeight)
        {
            var cameraTexture = new VideoTexture()
            {
                Width = textureWidth,
                Height = textureHeight,
                Levels = 1,
                Faces = 0,
                Format = PixelFormat.R8G8B8A8
            };

            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            var textureManager = adapter.Graphics.TextureManager;
            textureManager.UploadTexture(cameraTexture);
            this.dxDevicePtr = this.dxDevice.NativePointer;

            var dxTexture = textureManager.TextureFromHandle<DXTexture>(cameraTexture.TextureHandle).Texture;
            QCAR_setVideoTexture(dxTexture.NativePointer);

            return cameraTexture;
        }

        /// <inheritdoc />
        protected override void UpdateCameraTexture()
        {
            var oldRasterizerState = this.dxContext.Rasterizer.State;
            var oldDepthStencilState = this.dxContext.OutputMerger.DepthStencilState;
            this.dxContext.Rasterizer.State = this.rasterizerState;

            QCAR_updateVideoTexture(this.dxDevicePtr);

            this.dxContext.Rasterizer.State = oldRasterizerState;
            this.dxContext.OutputMerger.DepthStencilState = oldDepthStencilState;
        }

        /// <inheritdoc />
        protected override void AdjustVideoTextureProjection(ref Matrix videoTextureProjection)
        {
            videoTextureProjection = CameraCorrectionRotationMatrix * videoTextureProjection;
        }
    }
}
