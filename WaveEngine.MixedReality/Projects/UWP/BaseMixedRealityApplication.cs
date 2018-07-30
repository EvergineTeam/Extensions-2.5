// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
using WaveEngine.MixedReality.Internals;
using WaveEngine.MixedReality.Utilities;
using WaveEngine.DirectX;
using Windows.UI.Input.Spatial;
using Windows.UI.Xaml;
using System.Collections.Generic;
using WaveEngine.MixedReality.Interaction;
#endregion

namespace WaveEngine.MixedReality
{
    /// <summary>
    /// Updates, renders, and presents holographic content using Direct3D.
    /// </summary>
    public class BaseMixedRealityApplication : Adapter.BaseApplication, IMixedRealityApplication, IDisposable
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
        public SpatialStationaryFrameOfReference ReferenceFrame;

        /// <summary>
        /// Eye properties
        /// </summary>
        private VREye[] eyesProperties;

        /// <summary>
        /// The head ray
        /// </summary>
        private Ray headRay;

        /// <summary>
        /// Wave MixedReality service
        /// </summary>
        private MixedRealityService mixedRealityService;

        /// <summary>
        /// Render Target Manager
        /// </summary>
        private RenderTargetManager renderTargetManager;

        /// <summary>
        /// Specific adapter
        /// </summary>
        private Adapter.Adapter adapter;

        /// <summary>
        /// BackBuffer handles
        /// </summary>
        private Dictionary<int, VREyeTexture[]> backBufferHandles;

        #region Properties

        /// <summary>
        /// Gets the eye poses for VR Camera
        /// </summary>
        public VREye[] EyesProperties
        {
            get
            {
                return this.eyesProperties;
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
        /// Gets or sets a value indicating whether gets or sets a value a indicating whether the application is in fullscreen
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

        /// <summary>
        /// Gets view
        /// </summary>
        public override UIElement View
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Intialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMixedRealityApplication"/> class.
        /// Loads and initializes application assets when the application is loaded.
        /// </summary>
        public BaseMixedRealityApplication()
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
            this.adapter = this.Adapter as Adapter.Adapter;
            this.renderTargetManager = this.Adapter.Graphics.RenderTargetManager as RenderTargetManager;

            this.mixedRealityService = new MixedRealityService(this);

            Framework.Services.WaveServices.RegisterService(this.mixedRealityService);
            Framework.Services.WaveServices.RegisterService(new SpatialInputService());

            this.eyesProperties = new VREye[3];

            for (int i = 0; i < 3; i++)
            {
                this.eyesProperties[i] = new VREye();
            }


            this.backBufferHandles = new Dictionary<int, VREyeTexture[]>();
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

            var eyeTexture = this.eyesProperties[0].Texture;
            this.deviceResources.UpdateCameraClipDistance(eyeTexture.NearPlane, eyeTexture.FarPlane);

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
                if (this.mixedRealityService.FocusPosition.HasValue)
                {
                    var position = this.mixedRealityService.FocusPosition.Value;

                    if (!this.mixedRealityService.FocusNormal.HasValue)
                    {
                        renderingParameters.SetFocusPoint(currentCoordinateSystem, new System.Numerics.Vector3(position.X, position.Y, position.Z));
                    }
                    else
                    {
                        var normal = this.mixedRealityService.FocusNormal.Value;

                        if (!this.mixedRealityService.FocusVelocity.HasValue)
                        {
                            renderingParameters.SetFocusPoint(
                                currentCoordinateSystem,
                                new System.Numerics.Vector3(position.X, position.Y, position.Z),
                                new System.Numerics.Vector3(normal.X, normal.Y, normal.Z));
                        }
                        else
                        {
                            var velocity = this.mixedRealityService.FocusVelocity.Value;

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

                        var eyeProperties = this.eyesProperties[i];
                        var eyePose = eyeProperties.Pose;
                        eyePose.Position = view.Translation;
                        Quaternion.CreateFromRotationMatrix(ref view, out eyePose.Orientation);
                        eyeProperties.Pose = eyePose;
                        eyeProperties.Projection = projectionMatrix;
                    }

                    var leftEyePose = this.eyesProperties[(int)VREyeType.LeftEye].Pose;
                    var rightEyePose = this.eyesProperties[(int)VREyeType.RightEye].Pose;
                    var centerEyeProperties = this.eyesProperties[(int)VREyeType.CenterEye];

                    var centerEyePose = centerEyeProperties.Pose;
                    centerEyePose.Position = Vector3.Lerp(leftEyePose.Position, rightEyePose.Position, 0.5f);
                    centerEyePose.Orientation = Quaternion.Lerp(leftEyePose.Orientation, rightEyePose.Orientation, 0.5f);
                    centerEyeProperties.Pose = centerEyePose;
                }
            }

            this.Render();

            this.deviceResources.Present(ref holographicFrame);
        }

        /// <summary>
        /// Notifies renderers that device resources need to be released.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        public void OnDeviceLost(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Notifies renderers that device resources may now be recreated.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        public void OnDeviceRestored(object sender, EventArgs e)
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
                this.deviceResources.AddHolographicCamera(holographicCamera);

                // Holographic frame predictions will not include any information about this camera until
                // the deferral is completed.
                deferral.Complete();
            });

            task1.Start();
        }

        /// <summary>
        /// Camera removed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The args</param>
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

                int backBufferPointer = (int)cameraResources.BackBufferTexture2D.NativePointer;

                VREyeTexture[] vrEyeTexture;
                if (this.backBufferHandles.TryGetValue(backBufferPointer, out vrEyeTexture))
                {
                    if (newWidth != this.adapter.Width || newHeight != this.adapter.Height)
                    {
                        // Free renderTarget
                        for (int r = 0; r < vrEyeTexture.Length; r++)
                        {
                            this.renderTargetManager.DestroyDepthTexture(vrEyeTexture[r].RenderTarget.DepthTexture);
                            this.renderTargetManager.DestroyRenderTarget(vrEyeTexture[r].RenderTarget);
                        }

                        this.CreateRenderTargets(cameraResources, newWidth, newHeight, backBufferPointer, this.adapter, this.renderTargetManager);

                        for (int r = 0; r < vrEyeTexture.Length; r++)
                        {
                            this.backBufferHandles[backBufferPointer][r] = this.eyesProperties[r].Texture;
                        }
                    }
                    else
                    {
                        var lenght = cameraResources.IsRenderingStereoscopic ? 2 : 1;

                        for (int i = 0; i < lenght; i++)
                        {
                            this.eyesProperties[i].Texture = vrEyeTexture[i];
                        }
                        var eyeRT = this.eyesProperties[0].Texture.RenderTarget;
                        var dxRT = this.renderTargetManager.TargetFromHandle<DXRenderTarget>(eyeRT.TextureHandle);
                        this.adapter.GraphicsDevice.BackBuffer = dxRT.TargetView;
                    }
                }
                else
                {
                    this.CreateRenderTargets(cameraResources, newWidth, newHeight, backBufferPointer, this.adapter, this.renderTargetManager);

                    VREyeTexture[] textures = new VREyeTexture[]
                    {
                        this.eyesProperties[0].Texture,
                        this.eyesProperties[1].Texture
                    };

                    this.backBufferHandles.Add(backBufferPointer, textures);
                }
            }
        }

        /// <summary>
        /// Create Render targets
        /// </summary>
        /// <param name="cameraResources">camera Resources</param>
        /// <param name="newWidth">render width</param>
        /// <param name="newHeight">render height</param>
        /// <param name="backBufferPointer">target handle</param>
        /// <param name="adapter">adapter instance</param>
        /// <param name="renderTargetManager">render Target Manager</param>
        private void CreateRenderTargets(CameraResources cameraResources, int newWidth, int newHeight, int backBufferPointer, Adapter.Adapter adapter, RenderTargetManager renderTargetManager)
        {
            adapter.SetSize(newWidth, newHeight);

            DepthTexture depthTexture = renderTargetManager.CreateDepthTexture(newWidth, newHeight);

            var lenght = cameraResources.IsRenderingStereoscopic ? 2 : 1;

            for (int i = 0; i < lenght; i++)
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

                var eyeTexture = new VREyeTexture()
                {
                    Viewport = new Viewport(0, 0, 1, 1),
                    NearPlane = 0.01f,
                    FarPlane = 1000,
                    RenderTarget = renderTarget
                };

                this.eyesProperties[i].Texture = eyeTexture;
            }

            var eyeRT = this.eyesProperties[0].Texture.RenderTarget;
            var dxRT = renderTargetManager.TargetFromHandle<DXRenderTarget>(eyeRT.TextureHandle);
            adapter.GraphicsDevice.BackBuffer = dxRT.TargetView;
        }

        private void OnLocatabilityChanged(SpatialLocator sender, object args)
        {
            switch (sender.Locatability)
            {
                case SpatialLocatability.Unavailable:
                    // Holograms cannot be rendered.
                    {
                        string message = "Warning! Positional tracking is " + sender.Locatability + ".";
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
