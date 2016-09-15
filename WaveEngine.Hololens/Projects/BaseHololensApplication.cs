#region File Description
//-----------------------------------------------------------------------------
// BaseHololensApplication
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using Windows.Graphics.Holographic;
using Windows.Perception.Spatial;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Linq;
using WaveEngine.Common.VR;
using WaveEngine.Common.Graphics;
using SharpDX.Direct3D11;
using WaveEngine.Common.Math;
using Windows.Perception.Spatial.Surfaces;
using WaveEngine.Hololens.Internals;
using WaveEngine.Hololens.Utilities;
using WaveEngine.DirectX;
using Windows.UI.Input.Spatial;
#endregion

namespace WaveEngine.Hololens
{
    /// <summary>
    /// Updates, renders, and presents holographic content using Direct3D.
    /// </summary>
    public class BaseHololensApplication : Adapter.BaseApplication, IHololensApplication, IDisposable
    {
        /// <summary>
        /// Cached reference to device resources.
        /// </summary>
        private DeviceResources deviceResources;

        /// <summary>
        /// Represents the holographic space around the user.
        /// </summary>
        private HolographicSpace holographicSpace;

        /// <summary>
        /// SpatialLocator that is attached to the primary camera.
        /// </summary>
        private SpatialLocator locator;

        /// <summary>
        /// A reference frame attached to the holographic camera. 
        /// </summary>
        internal SpatialStationaryFrameOfReference ReferenceFrame;

        /// <summary>
        /// Eye textures
        /// </summary>
        private VREyeTexture[] eyeTextures;

        /// <summary>
        /// Eye poses
        /// </summary>
        private VREyePose[] eyePoses;

        /// <summary>
        /// The head ray
        /// </summary>
        private Ray headRay;

        /// <summary>
        /// Wave Hololens service
        /// </summary>
        private HololensService hololensService;

        #region Properties
        /// <summary>
        /// Gets the eye texture information for VR Camera
        /// </summary>
        public VREyeTexture[] EyeTextures
        {
            get
            {
                return this.eyeTextures;
            }
        }

        /// <summary>
        /// Gets the eye poses for VR Camera
        /// </summary>
        public VREyePose[] EyePoses
        {
            get
            {
                return this.eyePoses;
            }
        }

        /// <summary>
        /// Gets the head ray
        /// </summary>
        public Ray HeadRay
        {
            get
            {
                return this.headRay;
            }
        }

        /// <summary>
        /// Gets or sets a value a indicating whether the application is in fullscreen
        /// </summary>
        public override bool FullScreen
        {
            get
            {
                return true;
            }

            set
            {
            }
        }
        #endregion

        #region Intialize
        /// <summary>
        /// Loads and initializes application assets when the application is loaded.
        /// </summary>        
        public BaseHololensApplication()
        {
            this.deviceResources = new DeviceResources();

            // Register to be notified if the Direct3D device is lost.
            this.deviceResources.DeviceLost += this.OnDeviceLost;
            this.deviceResources.DeviceRestored += this.OnDeviceRestored;
        }

        /// <summary>
        /// Application initialization
        /// </summary>
        public override void Initialize()
        {
            var graphicsDevice = new GraphicsDevice()
            {
                DeviceDirect3D = this.deviceResources.D3DDevice,
                ContextDirect3D = this.deviceResources.D3DDeviceContext,
                DxgiDeviceManager = this.deviceResources.DxgiDeviceManager
            };

            this.Adapter = new Adapter.Adapter(this, graphicsDevice);

            this.hololensService = new HololensService(this);
            Framework.Services.WaveServices.RegisterService(this.hololensService);

            this.eyePoses = new VREyePose[3];

            this.eyePoses[0] = this.eyePoses[1] = this.eyePoses[2] = new VREyePose()
            {
                Orientation = Quaternion.Identity,
                Position = Vector3.Zero,
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Draw frame
        /// </summary>
        /// <param name="elapsedTime">The elapsed time</param>
        public override void Draw(TimeSpan elapsedTime)
        {
        }

        /// <summary>
        /// Update frame
        /// </summary>
        /// <param name="elapsedTime">The elapsed time</param>
        public override void Update(TimeSpan elapsedTime)
        {
        }

        /// <summary>
        /// The app is suspending
        /// </summary>
        public override void OnSuspending()
        {
            base.OnSuspending();

            this.deviceResources.Trim();
        }

        /// <summary>
        /// Set holographic space
        /// </summary>
        /// <param name="holographicSpace">The holographic space</param>
        public void SetHolographicSpace(HolographicSpace holographicSpace)
        {
            // The DeviceResources class uses the preferred DXGI adapter ID from the holographic
            // space (when available) to create a Direct3D device. The HolographicSpace
            // uses this ID3D11Device to create and manage device-based resources such as
            // swap chains.
            this.deviceResources.SetHolographicSpace(holographicSpace);

            this.holographicSpace = holographicSpace;

            // Use the default SpatialLocator to track the motion of the device.
            this.locator = SpatialLocator.GetDefault();

            // Be able to respond to changes in the positional tracking state.
            this.locator.LocatabilityChanged += this.OnLocatabilityChanged;

            // Respond to camera added events by creating any resources that are specific
            // to that camera, such as the back buffer render target view.
            // When we add an event handler for CameraAdded, the API layer will avoid putting
            // the new camera in new HolographicFrames until we complete the deferral we created
            // for that handler, or return from the handler without creating a deferral. This
            // allows the app to take more than one frame to finish creating resources and
            // loading assets for the new holographic camera.
            // This function should be registered before the app creates any HolographicFrames.
            holographicSpace.CameraAdded += this.OnCameraAdded;

            // Respond to camera removed events by releasing resources that were created for that
            // camera.
            // When the app receives a CameraRemoved event, it releases all references to the back
            // buffer right away. This includes render target views, Direct2D target bitmaps, and so on.
            // The app must also ensure that the back buffer is not attached as a render target, as
            // shown in DeviceResources.ReleaseResourcesForBackBuffer.
            holographicSpace.CameraRemoved += this.OnCameraRemoved;

            // The simplest way to render world-locked holograms is to create a stationary reference frame
            // when the app is launched. This is roughly analogous to creating a "world" coordinate system
            // with the origin placed at the device's position as the app is launched.
            this.ReferenceFrame = this.locator.CreateStationaryFrameOfReferenceAtCurrentLocation();
        }
            
        /// <summary>
        /// Dispose unmanaged resources
        /// </summary>
        public void Dispose()
        {
            this.deviceResources.Dispose();
            this.deviceResources = null;

            this.Adapter.Dispose();
            this.Adapter = null;
        }

        /// <summary>
        /// Renders the current frame to each holographic display, according to the 
        /// current application and spatial positioning state. Returns true if the 
        /// frame was rendered to at least one display.
        /// </summary>
        public void UpdateAndDraw()
        {                                                
            HolographicFrame holographicFrame = this.holographicSpace.CreateNextFrame();

            // Get a prediction of where holographic cameras will be when this frame
            // is presented.
            HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;


            // Back buffers can change from frame to frame. Validate each buffer, and recreate
            // resource views and depth buffers as needed.
            this.deviceResources.EnsureCameraResources(holographicFrame, prediction);

            this.UpdateEyeProperties();

            // Up-to-date frame predictions enhance the effectiveness of image stablization and
            // allow more accurate positioning of holograms.
            holographicFrame.UpdateCurrentPrediction();


            // Get a prediction of where holographic cameras will be when this frame
            // is presented.
            prediction = holographicFrame.CurrentPrediction;

            // Next, we get a coordinate system from the attached frame of reference that is
            // associated with the current frame. Later, this coordinate system is used for
            // for creating the stereo view matrices when rendering the sample content.
            SpatialCoordinateSystem currentCoordinateSystem = this.ReferenceFrame.CoordinateSystem;

            this.deviceResources.UpdateCameraClipDistance(this.eyeTextures[0].NearPlane, this.eyeTextures[0].FarPlane);

            holographicFrame.UpdateCurrentPrediction();
            prediction = holographicFrame.CurrentPrediction;

            foreach (var cameraPose in prediction.CameraPoses)
            {

                // The HolographicCameraRenderingParameters class provides access to set
                // the image stabilization parameters.
                HolographicCameraRenderingParameters renderingParameters = holographicFrame.GetRenderingParameters(cameraPose);

                // SetFocusPoint informs the system about a specific point in your scene to
                // prioritize for image stabilization. The focus point is set independently
                // for each holographic camera.
                // You should set the focus point near the content that the user is looking at.
                // In this example, we put the focus point at the center of the sample hologram,
                // since that is the only hologram available for the user to focus on.
                // You can also set the relative velocity and facing of that content; the sample
                // hologram is at a fixed point so we only need to indicate its position.
                if (this.hololensService.FocusPosition.HasValue)
                {
                    var position = this.hololensService.FocusPosition.Value;

                    if (!this.hololensService.FocusNormal.HasValue)
                    {
                        renderingParameters.SetFocusPoint(currentCoordinateSystem, new System.Numerics.Vector3(position.X, position.Y, position.Z));
                    }
                    else
                    {
                        var normal = this.hololensService.FocusNormal.Value;

                        if (!this.hololensService.FocusVelocity.HasValue)
                        {
                            renderingParameters.SetFocusPoint(
                                currentCoordinateSystem,
                                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                                new System.Numerics.Vector3(normal.X, normal.Y, normal.Z));
                        }
                        else
                        {
                            var velocity = this.hololensService.FocusVelocity.Value;

                            renderingParameters.SetFocusPoint(
                                currentCoordinateSystem,
                                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                                new System.Numerics.Vector3(normal.X, normal.Y, normal.Z),
                                new System.Numerics.Vector3(velocity.X, velocity.Y, velocity.Z));
                        }
                    }
                }

                var pointerPose = SpatialPointerPose.TryGetAtTimestamp(this.ReferenceFrame.CoordinateSystem, prediction.Timestamp);
                if (pointerPose != null)
                {
                    pointerPose.Head.Position.ToWave(out this.headRay.Position);
                    pointerPose.Head.ForwardDirection.ToWave(out this.headRay.Direction);
                }

                var viewTransaform = cameraPose.TryGetViewTransform(this.ReferenceFrame.CoordinateSystem);
                var projectionTransform = cameraPose.ProjectionTransform;

                if (viewTransaform.HasValue)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Matrix viewMatrix;
                        Matrix projectionMatrix;

                        if (i == (int)VREyeType.LeftEye)
                        {
                            viewTransaform.Value.Left.ToWave(out viewMatrix);
                            projectionTransform.Left.ToWave(out projectionMatrix);
                        }
                        else
                        {
                            viewTransaform.Value.Right.ToWave(out viewMatrix);
                            projectionTransform.Right.ToWave(out projectionMatrix);
                        }

                        Matrix view;
                        Matrix.Invert(ref viewMatrix, out view);

                        this.eyePoses[i].Position = view.Translation;
                        Quaternion quaternion;
                        Quaternion.CreateFromRotationMatrix(ref view, out quaternion);
                        this.eyePoses[i].Orientation = quaternion;
                        this.eyePoses[i].Projection = projectionMatrix;
                    }

                    this.eyePoses[2].Position = Vector3.Lerp(this.eyePoses[0].Position, this.eyePoses[1].Position, 0.5f);
                    this.eyePoses[2].Orientation = this.eyePoses[0].Orientation;
                }
            }

            this.Render();

            this.deviceResources.Present(ref holographicFrame);
        }

        /// <summary>
        /// Notifies renderers that device resources need to be released.
        /// </summary>
        public void OnDeviceLost(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Notifies renderers that device resources may now be recreated.
        /// </summary>
        public void OnDeviceRestored(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Camera added
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The args</param>
        public void OnCameraAdded(HolographicSpace sender, HolographicSpaceCameraAddedEventArgs args)
        {
            Deferral deferral = args.GetDeferral();
            HolographicCamera holographicCamera = args.Camera;

            Task task1 = new Task(() =>
            {
                // Create device-based resources for the holographic camera and add it to the list of
                // cameras used for updates and rendering. Notes:
                //   * Since this function may be called at any time, the AddHolographicCamera function
                //     waits until it can get a lock on the set of holographic camera resources before
                //     adding the new camera. At 60 frames per second this wait should not take long.
                //   * A subsequent Update will take the back buffer from the RenderingParameters of this
                //     camera's CameraPose and use it to create the ID3D11RenderTargetView for this camera.
                //     Content can then be rendered for the HolographicCamera.
                deviceResources.AddHolographicCamera(holographicCamera);

                // Holographic frame predictions will not include any information about this camera until
                // the deferral is completed.
                deferral.Complete();
            });

            task1.Start();
        }

        public void OnCameraRemoved(HolographicSpace sender, HolographicSpaceCameraRemovedEventArgs args)
        {
            Task task2 = new Task(() =>
            {
            });
            task2.Start();

            // Before letting this callback return, ensure that all references to the back buffer 
            // are released.
            // Since this function may be called at any time, the RemoveHolographicCamera function
            // waits until it can get a lock on the set of holographic camera resources before
            // deallocating resources for this camera. At 60 frames per second this wait should
            // not take long.
            this.deviceResources.RemoveHolographicCamera(args.Camera);
        }
        #endregion

        #region Private Methods
        private void UpdateEyeProperties()
        {
            var cameraResources = this.deviceResources.cameraResourcesDictionary.Values.FirstOrDefault();
            if (cameraResources != null)
            {
                int newWidth = (int)cameraResources.RenderTargetSize.Width;
                int newHeight = (int)cameraResources.RenderTargetSize.Height;

                var adapter = this.Adapter as Adapter.Adapter;

                if (newWidth != adapter.Width || newHeight != adapter.Height)
                {
                    adapter.SetSize(newWidth, newHeight);
                    RenderTargetManager renderTargetManager = this.Adapter.Graphics.RenderTargetManager as RenderTargetManager;


                    DepthTexture depthTexture = renderTargetManager.CreateDepthTexture(newWidth, newHeight);

                    this.eyeTextures = new VREyeTexture[cameraResources.IsRenderingStereoscopic ? 2 : 1];

                    for (int i = 0; i < this.eyeTextures.Length; i++)
                    {
                        RenderTargetViewDescription rtViewDescription = new RenderTargetViewDescription()
                        {
                            Format = cameraResources.BackBufferTexture2D.Description.Format,
                            Dimension = RenderTargetViewDimension.Texture2DArray,
                            Texture2DArray = new RenderTargetViewDescription.Texture2DArrayResource()
                            {
                                FirstArraySlice = i,
                                ArraySize = 1,
                                MipSlice = 0
                            }
                        };

                        var renderTarget = renderTargetManager.CreateRenderTarget(cameraResources.BackBufferTexture2D, rtViewDescription);
                        renderTarget.DepthTexture = depthTexture;

                        VREyeTexture eyeTexture = new VREyeTexture()
                        {
                            Viewport = new Viewport(0, 0, 1, 1),
                            NearPlane = 0.01f,
                            FarPlane = 1000,
                            RenderTarget = renderTarget
                        };

                        this.eyeTextures[i] = eyeTexture;
                    }

                    var dxRT = renderTargetManager.TargetFromHandle<DXRenderTarget>(this.EyeTextures[0].RenderTarget.TextureHandle);
                    adapter.GraphicsDevice.BackBuffer = dxRT.TargetView;
                }
            }
        }

        private void OnLocatabilityChanged(SpatialLocator sender, Object args)
        {
            switch (sender.Locatability)
            {
                case SpatialLocatability.Unavailable:
                    // Holograms cannot be rendered.
                    {
                        String message = "Warning! Positional tracking is " + sender.Locatability + ".";
                        Debug.WriteLine(message);
                    }
                    break;

                // In the following three cases, it is still possible to place holograms using a
                // SpatialLocatorAttachedFrameOfReference.
                case SpatialLocatability.PositionalTrackingActivating:
                // The system is preparing to use positional tracking.

                case SpatialLocatability.OrientationOnly:
                // Positional tracking has not been activated.

                case SpatialLocatability.PositionalTrackingInhibited:
                    // Positional tracking is temporarily inhibited. User action may be required
                    // in order to restore positional tracking.
                    break;

                case SpatialLocatability.PositionalTrackingActive:
                    // Positional tracking is active. World-locked content can be rendered.
                    break;
            }
        }
        #endregion
    }
}
