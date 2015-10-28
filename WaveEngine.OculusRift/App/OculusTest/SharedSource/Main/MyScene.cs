#region Using Statements
using System;
using WaveEngine.Cardboard;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Common.VR;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Components.VR;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.LeapMotion;
using WaveEngine.LeapMotion.Behaviors;
using WaveEngine.LeapMotion.Drawables;
using WaveEngine.Materials;
using WaveEngine.OculusRift;
////using WaveEngine.LeapMotion;
////using WaveEngine.LeapMotion.Behaviors;
////using WaveEngine.LeapMotion.Drawables;
////using WaveEngine.OculusRift;
#endregion

namespace OculusTest
{
    public class MyScene : Scene
    {
        private ToggleSwitch toggleSwitch;
        private VRCameraRig cameraRig;
        private LeapMotionRig leapMotionRig;

        VideoInfo[] videos;
        private VideoInfo currentVideo;


        protected override void CreateScene()
        {
            Matrix m = Matrix.Identity;

            Vector3 forward = m.Forward;
            Vector3 backward = m.Backward;


            //WaveServices.ScreenContextManager.SetDiagnosticsActive(true);

            this.Load(WaveContent.Scenes.MyScene);

            var vrCamera = this.EntityManager.Find("VRCamera");

            this.cameraRig = vrCamera.FindComponent<VRCameraRig>();
            cameraRig.Monoscopic = false;
            cameraRig.VRMode = VRMode.AttachedMode;
            cameraRig.BackgroundColor = Color.CornflowerBlue;

            var cardboard = new CardboardVRProvider()
            {
                IsBarrelDistortionEnabled = true,
                IsNeckDisplacementEnabled = true,                
            };
            vrCamera.AddComponent(cardboard);

            FreeCamera3D freeCamera3D = new FreeCamera3D("freeCam", Vector3.One * 4, Vector3.Zero);
            var cam3D = freeCamera3D.Entity.FindComponent<Camera3D>();
            cam3D.Viewport = new Viewport(0, 0, 0.2f, 0.2f);
            cam3D.ClearFlags = ClearFlags.DepthAndStencil;
            cam3D.CameraOrder = 10;

            this.EntityManager.Add(freeCamera3D);

            ////var oculus = new OculusVRProvider()
            ////{
            ////    ShowHMDMirrorTexture = true
            ////};

            ////vrCamera.AddComponent(oculus);

            var leapMotionService = new LeapMotionService();
            WaveServices.RegisterService(leapMotionService);
            leapMotionService.StartSensor(LeapFeatures.Hands | LeapFeatures.HMDMode);

            ////this.toggleSwitch = new ToggleSwitch();
            ////this.toggleSwitch.Toggled += toggleSwitch_Toggled;

            ////this.EntityManager.Add(this.toggleSwitch);

            ////videos = new VideoInfo[2];


            ////WaveServices.VideoPlayer.Play(videos[0]);
            ////WaveServices.VideoPlayer.IsLooped = true;

            ////var screenMaterial = this.Assets.LoadModel<MaterialModel>(WaveContent.Assets.Material.TVScreenMaterial);
            ////(screenMaterial.Material as StandardMaterial).Diffuse = WaveServices.VideoPlayer.VideoTexture;
        }

        ////void toggleSwitch_Toggled(object sender, EventArgs e)
        ////{
        ////    if (this.currentVideo != null)
        ////    {
        ////        this.currentVideo.Dispose();
        ////        this.currentVideo = null;
        ////    }

        ////    if (toggleSwitch.IsOn)
        ////    {
        ////        videos[1] = WaveServices.VideoPlayer.VideoInfoFromPath("http://d3v31wdy1eeefh.cloudfront.net/9e224c24-a5de-43d5-b1d7-0cfa0713d710/videos/02-1080p_HD_f_1.mp4");
        ////    }
        ////    else
        ////    {
        ////        videos[0] = WaveServices.VideoPlayer.VideoInfoFromPath("http://d3v31wdy1eeefh.cloudfront.net/e306b645-8d89-46f3-9b84-7003a634114a/videos/02-1080p_HD_f_1.mp4");
        ////    }

        ////    this.currentVideo = videos[toggleSwitch.IsOn ? 1 : 0];

        ////    WaveServices.VideoPlayer.Play(this.currentVideo);
        ////}


        protected override void Start()
        {
            base.Start();

            var vrCamera = this.EntityManager.Find("VRCamera");
            var vrCameraRig = vrCamera.FindComponent<VRCameraRig>();
            vrCameraRig.AttachedCamera.FieldOfView = MathHelper.ToRadians(80);
            
            Entity leapMotion = new Entity("LeapMotion")
            .AddComponent(new Transform3D())
            .AddComponent(this.leapMotionRig = new LeapMotionRig())            
            ;


            //this.leapMotionRig.LeftHandRig.IndexTransform

            vrCameraRig.CenterEyeAnchor.AddChild(leapMotion);
                       

            this.leapMotionRig.LeftHandRig.IndexAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.MiddleAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.PinkyAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.RingAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.ThumbAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.ElBowAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.WristAnchor.AddComponent(new MyDebugDrawable());
            ////this.leapMotionRig.LeftHandRig.PalmAnchor.AddComponent(new MyDebugDrawable());
        }
    }

    public class MyDebugDrawable : Drawable3D
    {
        [RequiredComponent]
        public Transform3D transform;

        public override void Draw(TimeSpan gameTime)
        {
            this.RenderManager.LineBatch3D.DrawPoint(this.transform.Position, 0.01f, Color.Yellow);
            

        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
