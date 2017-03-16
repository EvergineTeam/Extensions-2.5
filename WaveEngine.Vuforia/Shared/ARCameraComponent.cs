#region File Description
//-----------------------------------------------------------------------------
// ARCameraComponent
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using System.Linq;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Vuforia
{
    [DataContract]
    public class ARCameraComponent : Camera3D
    {
        /// <summary>
        /// The Vuforia service
        /// </summary>        
        protected VuforiaService vuforiaService;

        [DataMember]
        private bool renderVideoCameraBackground;

        /// <summary>
        /// The background video quad
        /// </summary>
        private Mesh backgroundVideoQuad;

        /// <summary>
        /// The background video material
        /// </summary>
        private StandardMaterial backgroundVideoMaterial;

        /// <summary>
        /// Gets or sets a value indicating whether the camera video should be rendered in the background.
        /// </summary>
        /// <value>
        /// <c>true</c> if the camera video should be rendered in the background; otherwise, <c>false</c>.
        /// </value>
        public bool RenderVideoCameraBackground
        {
            get
            {
                return this.renderVideoCameraBackground;
            }

            set
            {
                this.renderVideoCameraBackground = value;
            }
        }

        /// <summary>
        /// Sets the defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.renderVideoCameraBackground = true;
        }

        #region implemented abstract members of Camera

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.vuforiaService = WaveServices.GetService<VuforiaService>();
        }

        /// <summary>
        /// Refreshes the dimensions.
        /// </summary>
        protected override void RefreshDimensions()
        {
            base.RefreshDimensions();

            if (this.vuforiaService != null)
            {
                this.aspectRatio = (float)platformService.ScreenWidth / (float)platformService.ScreenHeight;
                this.width = this.viewportWidth = platformService.ScreenWidth;
                this.height = this.viewportHeight = platformService.ScreenHeight;
            }
        }

        /// <summary>
        /// Refreshes the view.
        /// </summary>
        protected override void RefreshView()
        {
            if (this.vuforiaService == null)
            {
                base.RefreshView();
            }
        }

        /// <summary>
        /// Refreshes the projection.
        /// </summary>
        protected override void RefreshProjection()
        {
            if (this.vuforiaService == null)
            {
                base.RefreshProjection();
            }
        }

        /// <summary>
        /// Renders the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Render(TimeSpan gameTime)
        {
            if (this.vuforiaService != null
                && this.vuforiaService.IsSupported
                && this.vuforiaService.State == ARState.TRACKING)
            {
                if (this.backgroundVideoQuad == null)
                {
                    this.InitializeVideoMeshAndMaterial();
                }

                this.view = this.vuforiaService.Pose;
                this.Transform.Position = this.vuforiaService.PoseInv.Translation;

                this.upVector = this.vuforiaService.Pose.Up;
                this.Transform.LookAt(this.Transform.Position + (this.vuforiaService.PoseInv.Backward * this.farPlane), this.upVector);

                this.projection = this.vuforiaService.GetCameraProjection(this.nearPlane, this.farPlane);

                this.projectionRenderTarget = this.projection;
                this.dirtyViewProjection = true;

                if (platformService.AdapterType != WaveEngine.Common.AdapterType.DirectX)
                {
                    this.projectionRenderTarget.M22 = -this.projectionRenderTarget.M22;
                }

                if (this.renderVideoCameraBackground && this.vuforiaService.CameraTexture != null)
                {
                    this.backgroundVideoMaterial.Diffuse = this.vuforiaService.CameraTexture;

                    var clearFlags = this.ClearFlags;
                    this.ClearFlags = ClearFlags.DepthAndStencil;
                    this.RenderBackgroundImage();
                    base.Render(gameTime);
                    this.ClearFlags = clearFlags;
                }
                else
                {
                    base.Render(gameTime);
                }
            }
            else
            {
                base.Render(gameTime);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.backgroundVideoQuad != null)
                    {
                        WaveServices.GraphicsDevice.DestroyIndexBuffer(this.backgroundVideoQuad.IndexBuffer);
                        WaveServices.GraphicsDevice.DestroyVertexBuffer(this.backgroundVideoQuad.VertexBuffer);
                        this.backgroundVideoQuad = null;
                    }

                    if (this.backgroundVideoMaterial != null)
                    {
                        this.backgroundVideoMaterial = null;
                    }
                }
            }
        }
        #endregion

        #region Background Video
        /// <summary>
        /// Initializes the video mesh and material.
        /// </summary>
        private void InitializeVideoMeshAndMaterial()
        {
            if (this.vuforiaService.VideoMesh == null)
            {
                return;
            }

            var lastLayer = this.RenderManager.Layers.FirstOrDefault();

            this.backgroundVideoMaterial = new StandardMaterial()
            {
                LayerType = lastLayer.GetType(),
                LightingEnabled = false,
                DeferredLightingPass = DeferredLightingPass.ForwardPass
            };

            this.backgroundVideoMaterial.Initialize(this.Assets);

            this.backgroundVideoQuad = this.vuforiaService.VideoMesh;

            var graphicsDevice = WaveServices.GraphicsDevice;
            graphicsDevice.BindVertexBuffer(this.backgroundVideoQuad.VertexBuffer);
            graphicsDevice.BindIndexBuffer(this.backgroundVideoQuad.IndexBuffer);

        }

        /// <summary>
        /// Renders the background image.
        /// </summary>
        protected void RenderBackgroundImage()
        {
            var clearColor = Color.Black;
            this.RenderManager.GraphicsDevice.Clear(ref clearColor, ClearFlags.All, 1);

            this.backgroundVideoMaterial.Matrices.WorldViewProj = this.vuforiaService.CameraProjectionMatrix;

            var quad = this.backgroundVideoQuad;
            this.backgroundVideoMaterial.Apply(null);

            this.RenderManager.GraphicsDevice.DrawVertexBuffer(quad.NumVertices, quad.NumPrimitives, quad.PrimitiveType, quad.VertexBuffer, quad.IndexBuffer);
        }
        #endregion
    }
}

