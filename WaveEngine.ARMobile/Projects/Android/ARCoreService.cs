// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Google.AR.Core;
using Google.AR.Core.Exceptions;
using Java.Nio;
using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
using ARConfig = Google.AR.Core.Config;
using ARCore = Google.AR.Core;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// The ARCore specific service for ARMobile extension
    /// </summary>
    public class ARCoreService : ARMobileService
    {
        private const string TAG = "ARMOBILE";

        private GraphicsDevice graphicsDevice;

        private Platform platform;

        private Session arCoreSession;

        private ARConfig arConfiguration;

        private Frame currentFrame;

        private bool viewportChanged;

        private bool doResumeOnActivated;

        private Texture cameraTexture;

        private FloatBuffer mQuadTexCoord;

        private VertexPositionTexture[] vertices;

        private TaskCompletionSource<bool> initializationTCS;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARCoreService"/> class.
        /// </summary>
        public ARCoreService()
            : base()
        {
            this.backgroundCameraMaterial = new StandardMaterial()
            {
                LayerId = DefaultLayers.Skybox,
                LightingEnabled = false,
            };

            this.graphicsDevice = WaveServices.GraphicsDevice;

            this.platform = WaveServices.Platform;
            this.platform.OnDisplayOrientationChanged += this.Platform_OnDisplayOrientationChanged;
            this.platform.OnScreenSizeChanged += this.Platform_OnScreenSizeChanged;
        }

        protected async override void Initialize()
        {
            base.Initialize();

            this.initializationTCS = new TaskCompletionSource<bool>();

            var adapter = Game.Current.Application.Adapter as WaveEngine.Adapter.Adapter;
            ArCoreApk.Availability availability;
            do
            {
                availability = ArCoreApk.Instance.CheckAvailability(adapter.Activity);

                if (availability.IsTransient)
                {
                    await Task.Delay(200);
                }
            }
            while (availability.IsTransient);

            this.isSupported = availability.IsSupported;
            this.initializationTCS.SetResult(this.isSupported);
        }

        /// <inheritdoc />
        public override void OnDeactivated()
        {
            base.OnDeactivated();

            if (this.currentFrame?.Camera?.TrackingState == Google.AR.Core.TrackingState.Tracking)
            {
                this.arCoreSession.Pause();
                this.doResumeOnActivated = true;
            }
        }

        /// <inheritdoc />
        public override void OnActivated()
        {
            base.OnActivated();

            if (this.doResumeOnActivated)
            {
                this.arCoreSession.Resume();
                this.doResumeOnActivated = false;
            }
        }

        /// <inheritdoc />
        public override async Task<bool> StartTracking(ARMobileStartOptions startOptions)
        {
            bool result = false;

            if (this.arCoreSession != null)
            {
                var hasResetTracking = (startOptions & ARMobileStartOptions.ResetTracking) != 0;

                if (hasResetTracking ||
                    (startOptions & ARMobileStartOptions.RemoveExistingAnchors) != 0)
                {
                    this.ClearAllAnchors();
                }
                else if (!hasResetTracking)
                {
                    this.arCoreSession.Resume();
                    result = true;
                }
            }

            if (!result)
            {
                this.currentFrame?.Dispose();
                this.arCoreSession?.Dispose();
                this.arConfiguration?.Dispose();

                try
                {
                    var adapter = Game.Current.Application.Adapter as WaveEngine.Adapter.Adapter;

                    var isSupported = await this.initializationTCS.Task;
                    if (!isSupported)
                    {
                        throw new Exception("This device does not support AR");
                    }

                    if (ArCoreApk.Instance.RequestInstall(adapter.Activity, true) != ArCoreApk.InstallStatus.Installed)
                    {
                        throw new UnavailableArcoreNotInstalledException();
                    }

                    // ARCore requires camera permissions to operate. If we did not yet obtain runtime
                    // permission on Android M and above, now is a good time to ask the user for it.
                    if (ContextCompat.CheckSelfPermission(adapter.Activity, Android.Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(adapter.Activity, new string[] { Android.Manifest.Permission.Camera }, 0);

                        throw new UnauthorizedAccessException("Camera permission is needed");
                    }

                    this.arCoreSession = new Session(adapter.Activity);

                    // Create default config, check is supported, create session from that config.
                    this.arConfiguration = new ARConfig(this.arCoreSession);
                    if (!this.arCoreSession.IsSupported(this.arConfiguration))
                    {
                        throw new Exception("This device does not support AR");
                    }

                    this.isSupported = true;
                    this.viewportChanged = true;

                    if (this.cameraTexture != null)
                    {
                        this.arCoreSession.SetCameraTextureName((int)this.cameraTexture.TextureHandle);
                    }

                    this.RefreshConfiguration();
                    this.Reset();

                    result = true;
                }
                catch (UnavailableArcoreNotInstalledException)
                {
                    Log.WriteLine(LogPriority.Error, TAG, "Please install ARCore");
                }
                catch (UnavailableApkTooOldException)
                {
                    Log.WriteLine(LogPriority.Error, TAG, "Please update ARCore");
                }
                catch (UnavailableSdkTooOldException)
                {
                    Log.WriteLine(LogPriority.Error, TAG, "Please update this app");
                }
                catch (Java.Lang.Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, TAG, $"This device does not support AR. Error details: {ex.ToString()}");
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, TAG, $"{ex.Message}");
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void PauseTracking()
        {
            this.arCoreSession?.Pause();
            this.TrackingState = WaveEngine.Components.AR.ARTrackingState.NotAvailable;
        }

        /// <inheritdoc />
        public override bool HitTest(Vector2 screenPosition, ARMobileHitType hitType, out ARMobileHitTestResult[] results)
        {
            if (this.currentFrame?.Camera == null ||
                this.currentFrame.Camera.TrackingState != Google.AR.Core.TrackingState.Tracking)
            {
                results = null;
                return false;
            }

            var resultAnchors = new List<ARMobileHitTestResult>();
            foreach (var hit in this.currentFrame.HitTest(screenPosition.X, screenPosition.Y))
            {
                ARMobileHitType? currentHitType = null;
                var trackable = hit.Trackable;

                if ((hitType & ARMobileHitType.FeaturePoint) != 0 &&
                    trackable is ARCore.Point)
                {
                    currentHitType = ARMobileHitType.FeaturePoint;
                }
                else if (trackable is ARCore.Plane)
                {
                    if ((hitType & ARMobileHitType.ExistingPlaneUsingExtent) != 0 &&
                        ((ARCore.Plane)trackable).IsPoseInExtents(hit.HitPose))
                    {
                        currentHitType = ARMobileHitType.ExistingPlaneUsingExtent;
                    }
                    else if ((hitType & ARMobileHitType.ExistingPlaneUsingGeometry) != 0 &&
                        ((ARCore.Plane)trackable).IsPoseInPolygon(hit.HitPose))
                    {
                        currentHitType = ARMobileHitType.ExistingPlaneUsingGeometry;
                    }
                    else if ((hitType & ARMobileHitType.ExistingPlane) != 0 ||
                             (hitType & ARMobileHitType.EstimatedHorizontalPlane) != 0)
                    {
                        currentHitType = ARMobileHitType.ExistingPlane;
                    }
                }

                if (currentHitType.HasValue)
                {
                    var trackableId = trackable.GetGuid();
                    var hitResult = new ARMobileHitTestResult()
                    {
                        Distance = hit.Distance,
                        HitType = currentHitType.Value,
                        Anchor = this.FindAnchor(trackableId)
                    };
                    hit.HitPose.ToWave(out hitResult.WorldTransform);

                    resultAnchors.Add(hitResult);
                }
            }

            results = resultAnchors.ToArray();
            return resultAnchors.Count > 0;
        }

        /// <inheritdoc />
        internal override void RefreshConfiguration()
        {
            if (this.arConfiguration == null)
            {
                return;
            }

            this.arConfiguration.SetUpdateMode(ARConfig.UpdateMode.LatestCameraImage);

            var lightEstimationMode = this.LightEstimationEnabled ? ARConfig.LightEstimationMode.AmbientIntensity : ARConfig.LightEstimationMode.Disabled;
            this.arConfiguration.SetLightEstimationMode(lightEstimationMode);

            ARConfig.PlaneFindingMode planeFingindMode;
            this.PlaneDetection.ToPlaneFindingMode(out planeFingindMode);
            this.arConfiguration.SetPlaneFindingMode(planeFingindMode);

            if (this.arCoreSession.IsSupported(this.arConfiguration))
            {
                this.arCoreSession.Configure(this.arConfiguration);
            }
            else
            {
                Log.WriteLine(LogPriority.Error, TAG, "Invalid configuration for ARCoreSession");
            }
        }

        /// <inheritdoc />
        internal override void Reset()
        {
            if (this.isSupported)
            {
                this.arCoreSession.Pause();

                if (this.arCoreSession != null)
                {
                    this.arCoreSession.Resume();
                }
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            this.cameraTexture = null;

            if (this.backgroundCameraMesh != null)
            {
                WaveServices.GraphicsDevice.DestroyIndexBuffer(this.backgroundCameraMesh.IndexBuffer);
                WaveServices.GraphicsDevice.DestroyVertexBuffer(this.backgroundCameraMesh.VertexBuffer);

                this.backgroundCameraMesh = null;
            }

            this.currentFrame?.Dispose();
            this.currentFrame = null;

            this.arCoreSession?.Dispose();
            this.arCoreSession = null;

            this.arConfiguration?.Dispose();
            this.arConfiguration = null;

            this.platform.OnDisplayOrientationChanged -= this.Platform_OnDisplayOrientationChanged;
            this.platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;
        }

        /// <inheritdoc />
        public override void Update(TimeSpan gameTime)
        {
            if (this.arCoreSession == null)
            {
                return;
            }

            // Notify ARCore session that the view size changed so that the perspective matrix and the video background
            // can be properly adjusted
            this.UpdateDisplayGeometryIfNeeded(this.arCoreSession);

            if (this.cameraTexture == null)
            {
                this.CreateTexture();
            }

            try
            {
                var newFrame = this.arCoreSession.Update();
                if (newFrame.Equals(this.currentFrame))
                {
                    return;
                }

                this.currentFrame?.Dispose();
                this.currentFrame = newFrame;
                var camera = this.currentFrame.Camera;

                if (this.backgroundCameraMesh == null)
                {
                    this.CreateVideoMesh();
                }
                else if (this.currentFrame.HasDisplayGeometryChanged)
                {
                    // If display rotation changed (also includes view size change), we need to re-query the UV
                    // coordinates for the screen rectangle, as they may have changed as well.
                    var vertexBuffer = this.backgroundCameraMesh.VertexBuffer;
                    this.UpdateQuadTextCoords(this.vertices, vertexBuffer, this.currentFrame);
                    this.graphicsDevice.BindVertexBuffer(vertexBuffer);
                }

                if (camera.TrackingState == Google.AR.Core.TrackingState.Paused)
                {
                    this.TrackingState = WaveEngine.Components.AR.ARTrackingState.NotAvailable;
                    return;
                }

                this.TrackingState = WaveEngine.Components.AR.ARTrackingState.Normal;

                // Updates the camera transform matrix
                this.currentFrame.Camera.DisplayOrientedPose.ToWave(out this.cameraTransform);

                var activeCamera = this.ActiveCamera;
                float nearPlane = activeCamera?.NearPlane ?? 0.1f;
                float farPlane = activeCamera?.FarPlane ?? 1000f;

                // Updates the camera projection matrix.
                float[] projmtx = new float[16];
                camera.GetProjectionMatrix(projmtx, 0, nearPlane, farPlane);
                projmtx.ToWave(out this.cameraProjection);

                if (!this.LightEstimationEnabled ||
                    !this.RefreshLightEstimation(this.currentFrame))
                {
                    this.lightEstimation = null;
                }

                if (!this.PointCloudEnabled ||
                    !this.RefreshPointCloud(this.currentFrame))
                {
                    this.internalPointCloud = null;
                }

                if (this.PlaneDetection != PlaneDetectionType.None)
                {
                    this.RefreshTrackables(this.currentFrame);
                }
            }
            catch (System.Exception ex)
            {
                // Avoid crashing the application due to unhandled exceptions.
                Log.Error(TAG, "Exception on the ARCoreSession update", ex);
            }
        }

        private bool RefreshLightEstimation(Frame frame)
        {
            var lightEstimate = frame.LightEstimate;
            var isValidEstimation = lightEstimate.GetState() == LightEstimate.State.Valid;

            if (isValidEstimation)
            {
                if (this.lightEstimation == null)
                {
                    this.lightEstimation = new ARMobileLightEstimation();
                }

                // Compute lighting from average intensity of the image.
                this.lightEstimation.UpdateFactorIntensity(lightEstimate.PixelIntensity);
            }

            return isValidEstimation;
        }

        private bool RefreshPointCloud(Frame frame)
        {
            var pointCloud = frame.AcquirePointCloud();

            if (pointCloud != null)
            {
                var points = pointCloud.Points;
                var numPoints = points.Remaining() / 4; // Four floats: X,Y,Z,confidence.
                var size = this.internalPointCloud?.Length ?? 0;

                if (this.internalPointCloud != null)
                {
                    if (numPoints != size)
                    {
                        Array.Resize(ref this.internalPointCloud, numPoints);
                    }
                }
                else
                {
                    this.internalPointCloud = new Vector3[numPoints];
                }

                for (int i = 0; i < numPoints; i++)
                {
                    this.internalPointCloud[i].X = points.Get();
                    this.internalPointCloud[i].Y = points.Get();
                    this.internalPointCloud[i].Z = points.Get();
                    points.Get(); // Confidence
                }

                // App is responsible for releasing point cloud resources after using it
                pointCloud.Release();
            }

            return pointCloud != null;
        }

        private void RefreshTrackables(Frame frame)
        {
            var newAnchors = new List<ARMobileAnchor>();
            var updatedAnchors = new List<ARMobileAnchor>();
            var removedAnchorsIds = new List<Guid>();
            foreach (ARCore.Plane arCorePlane in frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(ARCore.Plane))))
            {
                if (arCorePlane.TrackingState != ARCore.TrackingState.Tracking ||
                    arCorePlane.SubsumedBy != null)
                {
                    removedAnchorsIds.Add(arCorePlane.GetGuid());
                }
                else
                {
                    var planeId = arCorePlane.GetGuid();
                    var planeAnchor = this.FindAnchor(planeId) as ARMobilePlaneAnchor;

                    if (planeAnchor == null)
                    {
                        planeAnchor = new ARMobilePlaneAnchor() { Id = planeId };
                        newAnchors.Add(planeAnchor);
                    }
                    else
                    {
                        updatedAnchors.Add(planeAnchor);
                    }

                    arCorePlane.Polygon.ToWave(ref planeAnchor.BoundaryPolygon);
                    arCorePlane.CenterPose.ToWave(out planeAnchor.Transform);
                    planeAnchor.Size.X = arCorePlane.ExtentX;
                    planeAnchor.Size.Z = arCorePlane.ExtentZ;
                    planeAnchor.Type = arCorePlane.GetPlaneAnchorType();
                }
            }

            this.AddAnchors(newAnchors);
            this.UpdatedAnchors(updatedAnchors);
            this.RemoveAnchors(removedAnchorsIds);
        }

        /// <summary>
        /// Updates the session display geometry if a change was posted by
        /// <see cref="Framework.Services.Platform.OnScreenSizeChanged"/> event.
        /// This function should be called explicitly before each call to
        /// <see cref="Google.AR.Core.Session.Update"/>. This function will also clear the 'pending update'
        /// (viewportChanged) flag.
        /// </summary>
        /// <param name="session">the <see cref="Google.AR.Core.Session"/> object to update if display geometry changed.</param>
        private void UpdateDisplayGeometryIfNeeded(Google.AR.Core.Session session)
        {
            if (this.viewportChanged)
            {
                SurfaceOrientation displayRotation;
                this.platform.DisplayOrientation.ToSurfaceOrientation(out displayRotation);
                session.SetDisplayGeometry((int)displayRotation, this.platform.ScreenWidth, this.platform.ScreenHeight);
                this.viewportChanged = false;
            }
        }

        private void CreateTexture()
        {
            int[] textures = new int[1];
            GL.GenTextures(1, textures);

            int textureId = textures[0];

            // 0x8d65 came from All.TextureExternalOes
            GL.BindTexture((TextureTarget)0x8d65, textureId);
            GL.TexParameter((TextureTarget)0x8d65, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter((TextureTarget)0x8d65, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.TexParameter((TextureTarget)0x8d65, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter((TextureTarget)0x8d65, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            this.cameraTexture = new VideoTexture()
            {
                Levels = 1,
                Type = TextureType.TextureVideo,
                TextureHandle = (uint)textureId
            };

            ((StandardMaterial)this.backgroundCameraMaterial).Diffuse1 = this.cameraTexture;

            this.arCoreSession.SetCameraTextureName(textureId);
        }

        private void CreateVideoMesh()
        {
            this.vertices = new VertexPositionTexture[4];

            this.vertices[0].Position = new Vector3(-1, -1, 0);
            this.vertices[1].Position = new Vector3(1, -1, 0);
            this.vertices[2].Position = new Vector3(1, 1, 0);
            this.vertices[3].Position = new Vector3(-1, 1, 0);

            var vertexBuffer = new DynamicVertexBuffer(VertexPositionTexture.VertexFormat);

            this.UpdateQuadTextCoords(this.vertices, vertexBuffer, this.currentFrame);

            var indexBuffer = new IndexBuffer(new ushort[] { 0, 1, 2, 2, 3, 0, });
            this.graphicsDevice.BindIndexBuffer(indexBuffer);

            this.backgroundCameraMesh = new Mesh(vertexBuffer, indexBuffer, PrimitiveType.TriangleList);
        }

        private void UpdateQuadTextCoords(VertexPositionTexture[] vertices, VertexBuffer vertexBuffer, Frame frame)
        {
            if (this.mQuadTexCoord == null)
            {
                this.mQuadTexCoord = this.CreateTextCoordsFloatBuffer();
                this.mQuadTexCoord.Put(new float[] { 0, 1, 1, 1, 1, 0, 0, 0 });
                this.mQuadTexCoord.Position(0);
            }

            var mQuadTexCoordTransformed = this.CreateTextCoordsFloatBuffer();

            frame.TransformDisplayUvCoords(this.mQuadTexCoord, mQuadTexCoordTransformed);

            mQuadTexCoordTransformed.Position(0);

            vertices[0].TexCoord.X = mQuadTexCoordTransformed.Get();
            vertices[0].TexCoord.Y = mQuadTexCoordTransformed.Get();

            vertices[1].TexCoord.X = mQuadTexCoordTransformed.Get();
            vertices[1].TexCoord.Y = mQuadTexCoordTransformed.Get();

            vertices[2].TexCoord.X = mQuadTexCoordTransformed.Get();
            vertices[2].TexCoord.Y = mQuadTexCoordTransformed.Get();

            vertices[3].TexCoord.X = mQuadTexCoordTransformed.Get();
            vertices[3].TexCoord.Y = mQuadTexCoordTransformed.Get();

            vertexBuffer.SetData(vertices);
            this.graphicsDevice.BindVertexBuffer(vertexBuffer);
        }

        private FloatBuffer CreateTextCoordsFloatBuffer()
        {
            var bbTexCoords = ByteBuffer.AllocateDirect(4 * 2 * sizeof(float));
            bbTexCoords.Order(ByteOrder.NativeOrder());
            return bbTexCoords.AsFloatBuffer();
        }

        private void Platform_OnDisplayOrientationChanged(object sender, Common.Input.DisplayOrientation orientation)
        {
            this.viewportChanged = true;
        }

        private void Platform_OnScreenSizeChanged(object sender, Common.Helpers.SizeEventArgs size)
        {
            this.viewportChanged = true;
        }
    }
}
