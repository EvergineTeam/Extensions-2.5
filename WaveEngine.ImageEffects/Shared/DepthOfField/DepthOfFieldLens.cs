#region File Description
//-----------------------------------------------------------------------------
// DepthOfFieldLens
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
    /// Represent a Depth of Field as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class DepthOfFieldLens : Lens
    {
        /// <summary>
        /// The blur downsample factor
        /// </summary>
        private float downSampleFactor;

        #region Properties
        /// <summary>
        /// Gets or sets blur scale, default value is 4.0f.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 20.0f, 0.1f)]
        public float BlurScale
        {
            get
            {
                return (this.material as DepthOfFieldMaterial).BlurScale;
            }

            set
            {
                (this.material as DepthOfFieldMaterial).BlurScale = value;
            }
        }

        /// <summary>
        /// Focus distance of the lens.
        /// </summary>
        [DataMember]        
        public float FocusDistance
        {
            get
            {
                return (this.material as DepthOfFieldMaterial).FocusDistance;
            }

            set
            {
                (this.material as DepthOfFieldMaterial).FocusDistance = value;
            }
        }

        /// <summary>
        /// Blur Downsample factor.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1, 16, 1f)]
        public float DownSampleFactor
        {
            get
            {
                return this.downSampleFactor;
            }

            set
            {
                this.downSampleFactor = value;
            }
        }

        /// <summary>
        /// Focus range of the lens.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 10, 1f)]
        public float FocusRange
        {
            get
            {
                return (this.material as DepthOfFieldMaterial).FocusRange;
            }

            set
            {
                (this.material as DepthOfFieldMaterial).FocusRange = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="DepthOfFieldLens"/> class.
        /// </summary>
        public DepthOfFieldLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.downSampleFactor = 4;
            this.material = new DepthOfFieldMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            if(this.Source == null)
            {
                return;
            }

            var mat = this.material as DepthOfFieldMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            int width = (int)(this.Source.Width / this.downSampleFactor);
            int height = (int)(this.Source.Height / this.downSampleFactor);

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            RenderTarget rt2 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // Down sampler
            mat.Pass = DepthOfFieldMaterial.Passes.DownSampler;
            mat.Texture = this.Source;
            this.RenderToImage(rt1, this.material);

            // Blur
            mat.Pass = DepthOfFieldMaterial.Passes.Blur;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // UpCombine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = DepthOfFieldMaterial.Passes.Combine;
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
