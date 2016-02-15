#region File Description
//-----------------------------------------------------------------------------
// ARCameraComponent
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
#endregion

namespace WaveEngine.Vuforia
{
    [DataContract]
    public class ARCameraComponent : Camera3D
    {
        private VuforiaService arService;

        public ARCameraComponent()
            : base()
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.ClearFlags = Common.Graphics.ClearFlags.DepthAndStencil;
        }

        #region implemented abstract members of Camera

        protected override void Initialize()
        {
            base.Initialize();

            this.arService = WaveServices.GetService<VuforiaService>();
        }

        protected override void RefreshDimensions()
        {
            base.RefreshDimensions();

            if (this.arService != null)
            {
                this.aspectRatio = (float)platformService.ScreenWidth / (float)platformService.ScreenHeight;
                this.width = this.viewportWidth = platformService.ScreenWidth;
                this.height = this.viewportHeight = platformService.ScreenHeight;
            }
        }

        protected override void RefreshView()
        {
            if (this.arService == null)
            {
                base.RefreshView();
            }
        }

        protected override void RefreshProjection()
        {
            if (this.arService == null)
            {
                base.RefreshProjection();
            }
        }

        protected override void Render(TimeSpan gameTime)
        {
            if (this.arService != null 
                && this.arService.IsSupported 
                && this.arService.State == ARState.TRACKING)
            {
                this.view = this.arService.Pose;
                this.Transform.Position = this.arService.PoseInv.Translation;

                this.upVector = this.arService.Pose.Up;
                this.Transform.LookAt(this.Transform.Position + (this.arService.PoseInv.Backward * this.farPlane), this.upVector);

                this.projection = this.arService.GetCameraProjection(this.nearPlane, this.farPlane);

                this.projectionRenderTarget = this.projection;
                this.dirtyViewProjection = true;

                if (platformService.AdapterType != WaveEngine.Common.AdapterType.DirectX)
                {
                    this.projectionRenderTarget.M22 = -this.projectionRenderTarget.M22;
                }
            }

            base.Render(gameTime);
        }
        #endregion
    }
}

