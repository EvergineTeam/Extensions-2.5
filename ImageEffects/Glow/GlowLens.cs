#region File Description
//-----------------------------------------------------------------------------
// GlowLens
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a glow as postprocessing filter.
    /// </summary>
    public class GlowLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets down up scale, default value is 8.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        public float GlowScale
        {
            get
            {
                return (this.material as GlowMaterial).GlowScale;
            }

            set
            {
                (this.material as GlowMaterial).GlowScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the intensity, default value is 3f.
        /// </summary>
        /// <value>
        /// The intensity.
        /// </value>
        public float Intensity
        {
            get
            {
                return (this.material as GlowMaterial).Intensity;
            }

            set
            {
                (this.material as GlowMaterial).Intensity = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GlowLens"/> class.
        /// </summary>
        public GlowLens()
        {
            this.material = new GlowMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as GlowMaterial;
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
            mat.Pass = GlowMaterial.Passes.DownSampler;
            this.RenderToImage(rt1, this.material);

            // Bloom
            mat.Pass = GlowMaterial.Passes.Blur;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // UpCombine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = GlowMaterial.Passes.UpCombine;
            mat.Texture = this.Source;
            mat.Texture1 = rt2;
            this.RenderToImage(this.Destination, this.material);

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
