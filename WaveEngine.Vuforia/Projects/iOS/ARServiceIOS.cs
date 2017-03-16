#region File Description
//-----------------------------------------------------------------------------
// ARServiceIOS
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using UIKit;
using Foundation;
using WaveEngine.Common.Graphics;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using System;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Vuforia
{
    internal class ARServiceIOS : ARServiceBase
    {
        /// <summary>
        ///  The camera correction matrix
        /// </summary>
        protected static readonly Matrix cameraCorrectionRotationMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2);

        protected static readonly Matrix cameraCorrectionRotationPortraitUpsideDownMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);

        #region P/Invoke
        [DllImport(DllName)]
        private extern static void QCAR_setVideoTexture(int texturePtr);

        [DllImport(DllName)]
        private extern static void QCAR_updateVideoTexture();
        #endregion
            
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
        
        protected override void UpdateCameraTexture()
        {
            QCAR_updateVideoTexture();
        }

        /// <summary>
        /// Update the service
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            switch(this.currentOrientation)
            {
                case AROrientation.ORIENTATION_PORTRAIT:
                    this.videoTextureProjection = cameraCorrectionRotationMatrix * this.videoTextureProjection;
                    break;
                case AROrientation.ORIENTATION_PORTRAIT_UPSIDEDOWN:
                    this.videoTextureProjection = cameraCorrectionRotationPortraitUpsideDownMatrix * this.videoTextureProjection;
                    break;
                default:
                    break;
            }            
        }
    }
}