#region File Description
//-----------------------------------------------------------------------------
// BloomLens
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
    /// Represent a Bloom as postprocessing filter.
    /// </summary>
    public class BloomLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the bloom threshold, default value is 0.4f.
        /// </summary>
        /// <value>
        /// The bloom threshold.
        /// </value>
        public float BloomThreshold
        {
            get
            {
                return (this.material as BloomMaterial).BloomThreshold;
            }

            set
            {
                (this.material as BloomMaterial).BloomThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets down up scale, default value is 8.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        public float BloomScale
        {
            get
            {
                return (this.material as BloomMaterial).BloomScale;
            }

            set
            {
                (this.material as BloomMaterial).BloomScale = value;
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
                return (this.material as BloomMaterial).Intensity;
            }

            set
            {
                (this.material as BloomMaterial).Intensity = value;
            }
        }

        /// <summary>
        /// Gets or sets the bloom tint.
        /// </summary>
        /// <value>
        /// The bloom tint.
        /// </value>
        public Vector3 BloomTint
        {
            get
            {
                return (this.material as BloomMaterial).BloomTint;
            }

            set
            {
                (this.material as BloomMaterial).BloomTint = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BloomLens"/> class.
        /// </summary>
        public BloomLens()
        {
            this.material = new BloomMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as BloomMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            int width = this.Source.Width / 4;
            int height = this.Source.Height / 4;

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            RenderTarget rt2 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);

            // Down sampler
            mat.Texture = this.Source;
            mat.Pass = BloomMaterial.Passes.DownSampler;
            this.RenderToImage(rt1, this.material);

            // Bloom
            mat.Pass = BloomMaterial.Passes.Bloom;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            //// UpCombine
            mat.Pass = BloomMaterial.Passes.UpCombine;
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
