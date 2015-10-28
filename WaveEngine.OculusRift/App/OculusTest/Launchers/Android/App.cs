using System;
using System.Collections.Generic;
using WaveEngine.Framework.Services;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.Views;

namespace OculusTest
{
    [Activity(Label = "OculusTest",
            Icon = "@drawable/icon",
            MainLauncher = true,
            LaunchMode = LaunchMode.SingleTask,
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AndroidActivity : WaveEngine.Adapter.Application
    {
        private OculusTest.Game game;

        public AndroidActivity()
        {
			this.FullScreen = true;

			this.DefaultOrientation = WaveEngine.Common.Input.DisplayOrientation.LandscapeLeft;
            this.SupportedOrientations = WaveEngine.Common.Input.DisplayOrientation.LandscapeLeft | WaveEngine.Common.Input.DisplayOrientation.LandscapeRight;

			// Set the app layout
			this.LayoutId = Resource.Layout.Main;
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            int options = (int)this.Window.DecorView.SystemUiVisibility;
            options |= (int)SystemUiFlags.LowProfile;
            options |= (int)SystemUiFlags.HideNavigation;

            if ((int)Android.OS.Build.VERSION.SdkInt >= 19)
            {
                options |= (int)2048; // SystemUiFlags.Inmersive;
                options |= (int)4096; // SystemUiFlags.ImmersiveSticky;
            }

            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)options;
        }

        public override void Initialize()
        {
            game = new OculusTest.Game();
            game.Initialize(this);

			this.Window.AddFlags(WindowManagerFlags.KeepScreenOn); 
        }

        public override void Update(TimeSpan elapsedTime)
        {
            game.UpdateFrame(elapsedTime);            
        }

        public override void Draw(TimeSpan elapsedTime)
        {
            game.DrawFrame(elapsedTime);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && WaveServices.Platform != null)
            {
                WaveServices.Platform.Exit();
            }

            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnPause()
        {
            if (game != null)
            {
                game.OnDeactivated();
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();            
            if(game != null)
            {
                game.OnActivated();
            }
        }
    }
}

