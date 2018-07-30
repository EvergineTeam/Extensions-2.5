

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Vuforia
{
    internal class ARServiceAndroid : ARServiceBase
    {
        #region P/Invoke
        [DllImport(DllName)]
        private extern static void QCAR_setInitState();

        [DllImport(DllName)]
        private extern static void QCAR_setVideoTexture(int texturePtr);

        [DllImport(DllName)]
        private extern static void QCAR_updateVideoTexture();
        #endregion

        protected override Task<bool> InternalInitialize(string licenseKey)
        {
            Java.Lang.JavaSystem.LoadLibrary("Vuforia");
            Java.Lang.JavaSystem.LoadLibrary("VuforiaAdapter");

            var adapter = Game.Current.Application.Adapter as WaveEngine.Adapter.Adapter;
            var activity = adapter.Activity;

            Com.Vuforia.Vuforia.SetInitParameters(activity, Com.Vuforia.Vuforia.Gl20, licenseKey);
            Com.Vuforia.Vuforia.Init();
            QCAR_setInitState();

            return Task.FromResult(true);
        }

        protected override Texture CreateCameraTexture(int textureWidth, int textureHeight)
        {
            var adapter = Game.Current.Application.Adapter as Adapter.Adapter;
            
            var textureManager = adapter.Graphics.TextureManager;
            var cameraTexture = new Texture2D()
            {
                Width = textureWidth,
                Height = textureHeight,
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
    }
}