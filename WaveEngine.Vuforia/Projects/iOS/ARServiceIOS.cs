// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Vuforia
{
    internal class ARServiceIOS : ARServiceBase
    {
        /// <summary>
        ///  The camera correction matrix
        /// </summary>
        protected static readonly Matrix CameraCorrectionRotationMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2);

        protected static readonly Matrix CameraCorrectionRotationPortraitUpsideDownMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

        #region P/Invoke
        [DllImport(DllName)]
        private extern static void QCAR_setVideoTexture(int texturePtr);

        [DllImport(DllName)]
        private extern static void QCAR_updateVideoTexture();
        #endregion

        /// <inheritdoc />
        protected override Texture CreateCameraTexture(int textureWidth, int textureHeight)
        {
            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            var renderTargetManager = adapter.Graphics.RenderTargetManager;

            var textureManager = adapter.Graphics.TextureManager;
            var cameraTexture = new Texture2D()
            {
                Format = PixelFormat.R8G8B8A8,
                Levels = 1
            };
            textureManager.UploadTexture(cameraTexture);
            var texturePtr = textureManager.TextureFromHandle<uint>(cameraTexture.TextureHandle);

            QCAR_setVideoTexture((int)texturePtr);
            return cameraTexture;
        }

        /// <inheritdoc />
        protected override void UpdateCameraTexture()
        {
            QCAR_updateVideoTexture();
        }

        /// <inheritdoc />
        protected override void AdjustVideoTextureProjection(ref Matrix videoTextureProjection)
        {
            switch (this.currentOrientation)
            {
                case QCAR_Orientation.ORIENTATION_PORTRAIT:
                    videoTextureProjection = CameraCorrectionRotationMatrix * videoTextureProjection;
                    break;
                case QCAR_Orientation.ORIENTATION_PORTRAIT_UPSIDEDOWN:
                    videoTextureProjection = CameraCorrectionRotationPortraitUpsideDownMatrix * videoTextureProjection;
                    break;
                default:
                    break;
            }
        }
    }
}
