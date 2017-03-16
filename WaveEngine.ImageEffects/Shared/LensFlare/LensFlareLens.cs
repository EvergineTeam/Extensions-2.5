#region File Description
//-----------------------------------------------------------------------------
// LensFlareLens
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Lens Flare as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class LensFlareLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets bias of downsampler, default value is -0.9f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.1f, 2.0f, 0.01f)]
        public float Bias
        {
            get
            {
                return (this.material as LensFlareMaterial).Bias;
            }

            set
            {
                (this.material as LensFlareMaterial).Bias = value;
            }
        }

        /// <summary>
        /// Gets or sets scale of downsampler, default value is 7.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1.0f, 12.0f, 0.2f)]
        public float Scale
        {
            get
            {
                return (this.material as LensFlareMaterial).Scale;
            }

            set
            {
                (this.material as LensFlareMaterial).Scale = value;
            }
        }

        /// <summary>
        /// Gets or sets GhostDispersal, default value is 0.37f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float GhostDispersal
        {
            get
            {
                return (this.material as LensFlareMaterial).GhostDispersal;
            }

            set
            {
                (this.material as LensFlareMaterial).GhostDispersal = value;
            }
        }
        
        /// <summary>
        /// Gets or sets HaloWidth, default value is 0.47f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float HaloWidth
        {
            get
            {
                return (this.material as LensFlareMaterial).HaloWidth;
            }

            set
            {
                (this.material as LensFlareMaterial).HaloWidth = value;
            }
        }
        
        /// <summary>
        /// Gets or sets Distortion, default value is 1.5f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.1f, 2.5f, 0.1f)]
        public float Distortion
        {
            get
            {
                return (this.material as LensFlareMaterial).Distortion;
            }

            set
            {
                (this.material as LensFlareMaterial).Distortion = value;
            }
        }

        /// <summary>
        /// Gets or sets Intensity, default value is 5f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1f, 10f, 0.5f)]
        public float Intensity
        {
            get
            {
                return (this.material as LensFlareMaterial).Intensity;
            }

            set
            {
                (this.material as LensFlareMaterial).Intensity = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="LensFlareLens"/> class.
        /// </summary>
        public LensFlareLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new LensFlareMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as LensFlareMaterial;

            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            int width = this.Source.Width / 4;
            int height = this.Source.Height / 4;

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            RenderTarget rt2 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // Down sampler
            mat.Texture = this.Source;
            mat.Pass = LensFlareMaterial.Passes.DownSampler;
            this.RenderToImage(rt1, this.material);

            // LensFlare
            mat.Pass = LensFlareMaterial.Passes.LensFlare;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // Blur
            mat.Pass = LensFlareMaterial.Passes.Blur;
            mat.Texture = rt2;
            this.RenderToImage(rt1, this.material);

            // Combine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = LensFlareMaterial.Passes.Combine;
            mat.Texture = this.Source;
            mat.LensFlareTexture = rt1;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
            mat.LensFlareTexture = null;

            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt1);
            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt2);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
