#region File Description
//-----------------------------------------------------------------------------
// BloomLens
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
    /// Represent a Bloom as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class BloomLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the bloom threshold, default value is 0.4f.
        /// </summary>
        /// <value>
        /// The bloom threshold.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.01f)]
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
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 20.0f, 0.1f)]
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
        [DataMember]
        [RenderPropertyAsFInput(0)]
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
        [DataMember]
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
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

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
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // Down sampler
            mat.Pass = BloomMaterial.Passes.DownSampler;
            mat.Texture = this.Source;
            this.RenderToImage(rt1, this.material);

            // Bloom
            mat.Pass = BloomMaterial.Passes.Bloom;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // UpCombine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = BloomMaterial.Passes.UpCombine;
            mat.Texture = this.Source;
            mat.Texture1 = rt2;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
            mat.Texture1 = null;

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
