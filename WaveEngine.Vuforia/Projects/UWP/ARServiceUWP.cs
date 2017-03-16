#region File Description
//-----------------------------------------------------------------------------
// ARServiceUWP
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
        private static readonly Matrix cameraCorrectionRotationMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

        #region P/Invoke
        [DllImport(DllName)]
        private extern static void QCAR_setVideoTexture(IntPtr texturePtr);

        [DllImport(DllName)]
        private extern static void QCAR_updateVideoTexture(IntPtr devicePtr);
        #endregion

        #region Variables
        private IntPtr dxDevicePtr;

        private RasterizerState rasterizerState;
        private Device dxDevice;
        private DeviceContext dxContext;

        #endregion

        public ARServiceUWP()
            : base()
        {
            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            this.dxDevice = adapter.GraphicsDevice.DeviceDirect3D;
            this.dxContext = adapter.GraphicsDevice.ContextDirect3D;
        }

        /// <summary>
        /// Stops the track.
        /// </summary>
        /// <returns></returns>
        public override bool StopTrack()
        {
            this.dxDevicePtr = IntPtr.Zero;

            return base.StopTrack();
        }

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

        /// <summary>
        /// Creates the camera texture.
        /// </summary>
        /// <param name="textureWidth">Width of the texture.</param>
        /// <param name="textureHeight">Height of the texture.</param>
        /// <returns>The new texture</returns>
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

        /// <summary>
        /// Updates the camera texture.
        /// </summary>
        protected override void UpdateCameraTexture()
        {
            var oldState = this.dxContext.Rasterizer.State;
            this.dxContext.Rasterizer.State = this.rasterizerState;

            QCAR_updateVideoTexture(this.dxDevicePtr);

            this.dxContext.Rasterizer.State = oldState;
        }

        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            this.videoTextureProjection = cameraCorrectionRotationMatrix * this.videoTextureProjection;
        }
    }
}