#region File Description
//-----------------------------------------------------------------------------
// LeapMotionService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using Leap;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.LeapMotion
{
    /// <summary>
    /// This waveengine service make easy to connect with LeapMotion device.
    /// </summary>
    public class LeapMotionService : UpdatableService
    {
        /// <summary>
        /// Default image with of LeapMotion device.
        /// </summary>
        public static readonly int LeapImageWith = 640;

        /// <summary>
        /// Default image height of LeapMotion device.
        /// </summary>
        public static readonly int LeapImageHeight = 240;

        /// <summary>
        /// Raise an event when the circle gesture is detected.
        /// </summary>
        public event EventHandler<LeapGestureEventArgs> OnGestureCircle;

        /// <summary>
        /// Raise an event when the tap gesture is detected.
        /// </summary>
        public event EventHandler<LeapGestureEventArgs> OnGestureTap;

        /// <summary>
        /// Raise an event when the screen tap gesture is detected.
        /// </summary>
        public event EventHandler<LeapGestureEventArgs> OnGestureScreenTap;

        /// <summary>
        /// Rase an event when the swipe gesture is detected.
        /// </summary>
        public event EventHandler<LeapGestureEventArgs> OnGestureSwipe;

        /// <summary>
        /// Reference to leap motion controller.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// The hands list.
        /// </summary>
        private HandList hands;

        /// <summary>
        /// Reference to right hand.
        /// </summary>
        private Hand rightHand;

        /// <summary>
        /// Reference to left hand.
        /// </summary>
        private Hand leftHand;

        /// <summary>
        /// Reference to current frame.
        /// </summary>
        private Frame currentFrame;

        /// <summary>
        /// Depth texture.
        /// </summary>
        private Texture2D depthTexture;

        /// <summary>
        /// Reference to graphis device.
        /// </summary>
        private GraphicsDevice graphicsDevice = WaveServices.GraphicsDevice;

        #region Properties

        /// <summary>
        /// Gets the current frame.
        /// </summary>
        public Frame CurrentFrame
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.WriteLine("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.currentFrame;
            }

            private set
            {
                this.currentFrame = value;
            }
        }

        /// <summary>
        /// Gets the Hand list.
        /// </summary>
        public HandList Hands
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.Write("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.hands;
            }
        }

        /// <summary>
        /// Gets the right hand.
        /// </summary>
        public Hand RightHand
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.Write("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.rightHand;
            }
        }

        /// <summary>
        /// Gets the left hand.
        /// </summary>
        public Hand LeftHand
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.Write("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.leftHand;
            }
        }

        /// <summary>
        /// Gets the leap motion controller.
        /// </summary>
        public Controller Controller
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.Write("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.controller;
            }
        }

        /// <summary>
        /// Gets the current activate features.
        /// </summary>
        public LeapFeatures CurrentFeatures { get; private set; }

        /// <summary>
        /// Gets the depth texture.
        /// </summary>
        public Texture2D DepthTexture
        {
            get
            {
                if (!this.IsReady)
                {
                    Console.Write("LeapMotion is not ready. First, you need to call to the start method.");
                    return null;
                }

                return this.depthTexture;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the controller is ready.
        /// </summary>
        public bool IsReady { get; private set; }
        #endregion

        #region Initialize
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Initializes static members of the <see cref="LeapMotionService"/> class.
        /// </summary>
        static LeapMotionService()
        {
            LoadNativeLibrary("Leap.dll");
            LoadNativeLibrary("LeapCSharp.dll");
        }

        /// <summary>
        /// Load a native library embedded in the current assembly.
        /// </summary>
        /// <param name="fileName">Library name.</param>
        private static void LoadNativeLibrary(string fileName)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);

            if (!File.Exists(path) || (LoadLibrary(path) == IntPtr.Zero))
            {
                path = Path.Combine(Path.GetTempPath(), fileName);

                try
                {
                    Type type = typeof(LeapMotionService);
                    string name = type.Namespace + "." + fileName;
                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name).CopyTo(stream);
                    }
                }
                catch
                {
                }

                LoadLibrary(path);
            }
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Starts to capture from leapmotion device.
        /// </summary>
        /// <param name="features">The active features for this instance. <see cref="LeapFeatures"/></param>
        /// <returns>Whether the device is ready.</returns>
        public bool StartSensor(LeapFeatures features)
        {
            this.CurrentFeatures = features;
            this.controller = new Controller();

            if (this.controller == null)
            {
                this.IsReady = false;
                Console.WriteLine("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");
            }
            else
            {
                if (features.HasFlag(LeapFeatures.CameraImages))
                {
                    if (!this.controller.IsPolicySet(Controller.PolicyFlag.POLICY_IMAGES))
                    {
                        this.controller.SetPolicy(Controller.PolicyFlag.POLICY_IMAGES);
                    }

                    this.InitializeTexture2D(LeapImageWith, LeapImageHeight, out this.depthTexture);
                }

                if (features.HasFlag(LeapFeatures.HMDMode))
                {
                    LeapExtensions.HWDModeEnable = true;

                    if (!this.controller.IsPolicySet(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD))
                    {
                        this.controller.SetPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                    }
                }

                if (features.HasFlag(LeapFeatures.Gestures))
                {
                    this.controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
                    this.controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
                    this.controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
                    this.controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
                }

                this.IsReady = true;
            }

            this.Update(TimeSpan.Zero);

            return this.IsReady;
        }

        /// <summary>
        /// Stop capture.
        /// </summary>
        public void StopSensor()
        {
            this.controller.Dispose();
            this.CurrentFeatures = LeapFeatures.None;
            this.controller = null;
            this.hands = null;
            this.currentFrame = null;

            this.graphicsDevice.Textures.DestroyTexture(this.depthTexture);
        }

        /// <summary>
        /// Update the service.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public override void Update(TimeSpan gameTime)
        {
            if (this.IsReady)
            {
                this.CurrentFrame = this.controller.Frame();

                if (this.CurrentFeatures.HasFlag(LeapFeatures.Hands))
                {
                    this.hands = this.CurrentFrame.Hands;

                    this.leftHand = null;
                    this.rightHand = null;

                    foreach (var hand in this.hands)
                    {
                        if (hand.IsValid)
                        {
                            if (hand.IsLeft)
                            {
                                if (this.leftHand == null)
                                {
                                    this.leftHand = hand;
                                }
                                else
                                {
                                    this.rightHand = hand;
                                }
                            }
                            else if (hand.IsRight)
                            {
                                if (this.rightHand == null)
                                {
                                    this.rightHand = hand;
                                }
                                else
                                {
                                    this.leftHand = hand;
                                }
                            }
                        }
                    }
                }

                if (this.CurrentFeatures.HasFlag(LeapFeatures.Gestures))
                {
                    foreach (var gesture in this.CurrentFrame.Gestures())
                    {
                        if (gesture.State == Gesture.GestureState.STATESTOP)
                        {
                            switch (gesture.Type)
                            {
                                case Gesture.GestureType.TYPECIRCLE:
                                    if (this.OnGestureCircle != null)
                                    {
                                        this.OnGestureCircle(this.currentFrame, new LeapGestureEventArgs(gesture));
                                    }

                                    break;
                                case Gesture.GestureType.TYPEKEYTAP:
                                    if (this.OnGestureTap != null)
                                    {
                                        this.OnGestureTap(this.currentFrame, new LeapGestureEventArgs(gesture));
                                    }

                                    break;
                                case Gesture.GestureType.TYPESCREENTAP:
                                    if (this.OnGestureScreenTap != null)
                                    {
                                        this.OnGestureScreenTap(this.currentFrame, new LeapGestureEventArgs(gesture));
                                    }

                                    break;
                                case Gesture.GestureType.TYPESWIPE:
                                    if (this.OnGestureSwipe != null)
                                    {
                                        this.OnGestureSwipe(this.currentFrame, new LeapGestureEventArgs(gesture));
                                    }

                                    break;
                                default:
                                    // handle unrecognized gestures
                                    break;
                            }
                        }
                    }
                }

                if (this.CurrentFeatures.HasFlag(LeapFeatures.CameraImages))
                {
                    var image = this.currentFrame.Images[0];

                    if (image.IsValid)
                    {
                        this.graphicsDevice.Textures.SetData(this.depthTexture, image.DataPointer(), image.Width * image.Height);
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize a texture 2D
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="texture">Reference to the new texture.</param>
        private void InitializeTexture2D(int width, int height, out Texture2D texture)
        {
            texture = new Texture2D()
            {
                Format = PixelFormat.R8,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = width,
                Height = height,
                Levels = 1
            };

            this.graphicsDevice.Textures.UploadTexture(texture);
        }

        /// <summary>
        /// Termine this service.
        /// </summary>
        protected override void Terminate()
        {
            this.StopSensor();
        }

        #endregion
    }
}
