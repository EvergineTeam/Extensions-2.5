// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using ARKit;
using CoreVideo;
using System;
using WaveEngine.Common.Math;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Framework.Graphics;
using WaveEngine.ARMobile.Materials;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// The ARKit specific service for ARMobile extension
    /// </summary>
    public class ARKitService : ARMobileService
    {
        /// <summary>
        /// The orientation adjust matrix
        /// </summary>
        private Matrix swapMatrix = Matrix.CreateRotationZ(MathHelper.Pi);

        /// <summary>
        /// The current frame
        /// </summary>
        private ARFrame currentFrame;

        private int sizeY;

        private int sizeUV;

        private YUVMaterial yuvMaterial;

        private ARSession arkitSession;

        private GraphicsDevice graphicsDevice;

        private Platform platform;

        private ARConfiguration aRConfiguration;

        private bool texturesInitialized;

        private bool orientationChanged;

        private Texture2D cameraTextureY;

        private Texture2D cameraTextureUV;

        private VertexPositionTexture[] vertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARKitService"/> class.
        /// </summary>
        public ARKitService()
            : base()
        {
            this.graphicsDevice = WaveServices.GraphicsDevice;
            this.platform = WaveServices.Platform;
            this.platform.OnDisplayOrientationChanged += this.Platform_OnDisplayOrientationChanged;

            this.backgroundCameraMaterial = this.yuvMaterial = new YUVMaterial();

            this.RefreshConfiguration();
        }

        /// <inheritdoc />
        public override Task<bool> StartTracking(ARMobileStartOptions startOptions)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                throw new NotSupportedException("ARKit requires iOS11 or higher");
            }

            if (this.arkitSession == null)
            {
                this.arkitSession = new ARSession();
                this.arkitSession.Delegate = new ARKitSessionDelegate(this);
            }
            else if ((startOptions & ARMobileStartOptions.ResetTracking) != 0 ||
                     (startOptions & ARMobileStartOptions.RemoveExistingAnchors) != 0)
            {
                this.ClearAllAnchors();
            }

            this.arkitSession.Run(this.aRConfiguration, startOptions.ToARKit());

            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public override void PauseTracking()
        {
            this.arkitSession?.Pause();
        }

        /// <inheritdoc />
        public override bool HitTest(Vector2 screenPosition, ARMobileHitType hitType, out ARMobileHitTestResult[] results)
        {
            var point = new CGPoint(screenPosition.X / this.platform.ScreenWidth, screenPosition.Y / this.platform.ScreenHeight);
            var arHitTestResultType = hitType.ToARKit();

            var arKitResults = this.arkitSession.CurrentFrame?.HitTest(point, arHitTestResultType);

            if (arKitResults != null)
            {
                results = new ARMobileHitTestResult[arKitResults.Length];

                for (int i = 0; i < arKitResults.Length; i++)
                {
                    var arkitResult = arKitResults[i];

                    var hitResult = new ARMobileHitTestResult()
                    {
                        Distance = (float)arkitResult.Distance,
                        HitType = arkitResult.Type.ToWave(),
                    };

                    arkitResult.WorldTransform.ToWave(out hitResult.WorldTransform);

                    var anchor = arkitResult.Anchor;
                    if (anchor != null)
                    {
                        var anchorId = anchor.Identifier.ToWave();
                        hitResult.Anchor = this.FindAnchor(anchorId);
                    }

                    results[i] = hitResult;
                }

                return arKitResults.Length > 0;
            }

            results = null;

            return false;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (this.platform != null)
            {
                this.platform.OnDisplayOrientationChanged -= this.Platform_OnDisplayOrientationChanged;
            }

            this.cameraTextureY?.Unload();
            this.cameraTextureUV?.Unload();
            this.cameraTextureY = null;
            this.cameraTextureUV = null;

            if (this.backgroundCameraMesh != null)
            {
                this.graphicsDevice.DestroyIndexBuffer(this.backgroundCameraMesh.IndexBuffer);
                this.graphicsDevice.DestroyVertexBuffer(this.backgroundCameraMesh.VertexBuffer);

                this.backgroundCameraMesh = null;
            }

            this.arkitSession?.Dispose();
            this.arkitSession = null;
        }

        /// <summary>
        /// Proccess a frame
        /// </summary>
        /// <param name="frame">The AR frame to be proccesed</param>
        internal unsafe void ProcessFrame(ARFrame frame)
        {
            this.currentFrame?.Dispose();

            this.currentFrame = frame;

            this.UpdateTextures(this.currentFrame);

            if (this.PointCloudEnabled &&
                this.currentFrame.RawFeaturePoints != null)
            {
                var points = this.currentFrame.RawFeaturePoints.Points;

                var nP = points.Length;
                var size = (this.internalPointCloud != null) ? this.internalPointCloud.Length : 0;

                if (this.internalPointCloud != null)
                {
                    if (nP != size)
                    {
                        Array.Resize(ref this.internalPointCloud, nP);
                    }
                }
                else
                {
                    this.internalPointCloud = new Vector3[nP];
                }

                for (int i = 0; i < nP; i++)
                {
                    points[i].ToWave(out this.internalPointCloud[i]);
                }
            }
            else
            {
                this.internalPointCloud = null;
            }

            // Updates the camera transform matrix
            this.currentFrame.Camera.Transform.ToWave(out this.cameraTransform);

            var orientation = this.platform.DisplayOrientation.ToUIKit();
            var viewportSize = new CoreGraphics.CGSize(this.platform.ScreenWidth, this.platform.ScreenHeight);

            if (this.orientationChanged)
            {
                this.orientationChanged = false;

                // If display rotation changed (also includes view size change), we need to re-query the UV
                // coordinates for the screen rectangle, as they may have changed as well.
                var vertexBuffer = this.backgroundCameraMesh.VertexBuffer;
                this.UpdateQuadTextCoords(this.vertices, vertexBuffer, this.currentFrame);
                this.graphicsDevice.BindVertexBuffer(vertexBuffer);
            }

            var activeCamera = this.ActiveCamera;
            float nearPlane = activeCamera?.NearPlane ?? 0.1f;
            float farPlane = activeCamera?.FarPlane ?? 1000f;

            var proj = this.currentFrame.Camera.GetProjectionMatrix(orientation, viewportSize, nearPlane, farPlane);
            proj.ToWave(out this.cameraProjection);

            if (this.platform.DisplayOrientation == Common.Input.DisplayOrientation.LandscapeRight)
            {
                Matrix.Multiply(ref this.cameraProjection, ref this.swapMatrix, out this.cameraProjection);
            }

            // Refreshes the light estimation
            var lightEstimate = this.currentFrame.LightEstimate;

            if (lightEstimate != null)
            {
                if (this.lightEstimation == null)
                {
                    this.lightEstimation = new ARMobileLightEstimation();
                }

                this.lightEstimation.UpdateLumensIntensity((float)lightEstimate.AmbientIntensity);
                this.lightEstimation.UpdateTemperature((float)lightEstimate.AmbientColorTemperature);
            }
            else
            {
                this.lightEstimation = null;
            }
        }

        /// <inheritdoc />
        internal override void RefreshConfiguration()
        {
            if (this.TrackPosition)
            {
                this.aRConfiguration = new ARWorldTrackingConfiguration()
                {
                    LightEstimationEnabled = this.LightEstimationEnabled,
                    PlaneDetection = this.PlaneDetection.ToARKit(),
                    WorldAlignment = this.WorldAlignment.ToARKit()
                };

                this.isSupported = ARWorldTrackingConfiguration.IsSupported;
            }
            else
            {
                this.aRConfiguration = new AROrientationTrackingConfiguration()
                {
                    LightEstimationEnabled = this.LightEstimationEnabled,
                    WorldAlignment = this.WorldAlignment.ToARKit()
                };

                this.isSupported = AROrientationTrackingConfiguration.IsSupported;
            }

            this.Reset();
        }

        /// <inheritdoc />
        internal override void Reset()
        {
            if (this.isSupported &&
                this.TrackingState != WaveEngine.Components.AR.ARTrackingState.NotAvailable)
            {
                this.arkitSession.Pause();
                this.arkitSession.Run(this.aRConfiguration, ARSessionRunOptions.ResetTracking);
            }
        }

        private void CreateVideoMesh(CVPixelBuffer img)
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

        private void UpdateQuadTextCoords(VertexPositionTexture[] vertices, VertexBuffer vertexBuffer, ARFrame frame)
        {
            var orientation = this.platform.DisplayOrientation.ToUIKit();
            var viewportSize = new CGSize(this.platform.ScreenWidth, this.platform.ScreenHeight);

            var displayTransform = frame.GetDisplayTransform(orientation, viewportSize).Invert();

            displayTransform.TransformPoint(new CGPoint(1, 0)).ToWave(ref vertices[0].TexCoord);
            displayTransform.TransformPoint(new CGPoint(0, 0)).ToWave(ref vertices[1].TexCoord);
            displayTransform.TransformPoint(new CGPoint(0, 1)).ToWave(ref vertices[2].TexCoord);
            displayTransform.TransformPoint(new CGPoint(1, 1)).ToWave(ref vertices[3].TexCoord);

            vertexBuffer.SetData(vertices);
            this.graphicsDevice.BindVertexBuffer(vertexBuffer);
        }

        private void CreateTextures(CVPixelBuffer img)
        {
            var imageWidth = (int)img.Width;
            var imageHeight = (int)img.Height;
            var widthUV = (int)img.GetWidthOfPlane(1);
            var heightUV = (int)img.GetHeightOfPlane(1);
            this.sizeY = (int)img.GetBytesPerRowOfPlane(0) * imageHeight;
            this.sizeUV = (int)img.GetBytesPerRowOfPlane(1) * heightUV;

            this.cameraTextureY = new Texture2D()
            {
                Format = PixelFormat.R8,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = imageWidth,
                Height = imageHeight,
                Levels = 1
            };

            this.cameraTextureUV = new Texture2D()
            {
                Format = PixelFormat.R8G8,
                Usage = TextureUsage.Dynamic,
                CpuAccess = TextureCpuAccess.Write,
                Width = widthUV,
                Height = heightUV,
                Levels = 1,
            };

            this.graphicsDevice.Textures.UploadTexture(this.cameraTextureY);
            this.graphicsDevice.Textures.UploadTexture(this.cameraTextureUV);

            this.yuvMaterial.LuminanceTexture = this.cameraTextureY;
            this.yuvMaterial.ChromaTexture = this.cameraTextureUV;
        }

        private void UpdateTextures(ARFrame frame)
        {
            try
            {
                using (var img = frame.CapturedImage)
                {
                    if (!this.texturesInitialized)
                    {
                        this.CreateVideoMesh(img);
                        this.CreateTextures(img);
                        this.texturesInitialized = true;
                    }

                    var yPtr = img.GetBaseAddress(0);
                    var uvPtr = img.GetBaseAddress(1);

                    if (yPtr != IntPtr.Zero &&
                        uvPtr != IntPtr.Zero)
                    {
                        this.graphicsDevice.Textures.SetData(this.cameraTextureY, yPtr, this.sizeY);
                        this.graphicsDevice.Textures.SetData(this.cameraTextureUV, uvPtr, this.sizeUV);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Platform_OnDisplayOrientationChanged(object sender, Common.Input.DisplayOrientation orientation)
        {
            this.orientationChanged = true;
        }
    }
}
