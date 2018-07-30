// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// Represent a GaussianBlur as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class GaussianBlurLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets blur factor, by default this value is 1f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.1f, 2.0f, 0.1f)]
        public float Factor
        {
            get
            {
                return (this.material as GaussianBlurMaterial).Factor;
            }

            set
            {
                (this.material as GaussianBlurMaterial).Factor = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlurLens"/> class.
        /// </summary>
        public GaussianBlurLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new GaussianBlurMaterial();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as GaussianBlurMaterial;

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.Source.Width, this.Source.Height);
            mat.Pass = GaussianBlurMaterial.Passes.Horizontal;
            mat.Texture = this.Source;
            this.RenderToImage(rt1, this.material);

            mat.Pass = GaussianBlurMaterial.Passes.Vertical;
            mat.Texture = rt1;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;

            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt1);
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
