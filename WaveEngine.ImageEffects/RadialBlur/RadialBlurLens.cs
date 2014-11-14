#region File Description
//-----------------------------------------------------------------------------
// RadialBblurLens
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a RadialBblur as postprocessing filter.
    /// </summary>
    public class RadialBlurLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the nsamples, default value is 10.
        /// </summary>
        /// <value>
        /// The nsamples.
        /// </value>
        public int Nsamples
        {
            get
            {
                return (this.material as RadialBlurMaterial).Nsamples;
            }

            set
            {
                (this.material as RadialBlurMaterial).Nsamples = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the blur, default value is 0.1f.
        /// </summary>
        /// <value>
        /// The width of the blur.
        /// </value>
        public float BlurWidth
        {
            get
            {
                return (this.material as RadialBlurMaterial).BlurWidth;
            }

            set
            {
                (this.material as RadialBlurMaterial).BlurWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the center, default value is (0.5f, 0.5f).
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        public Vector2 Center
        {
            get
            {
                return (this.material as RadialBlurMaterial).Center;
            }

            set
            {
                (this.material as RadialBlurMaterial).Center = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialBlurLens"/> class.
        /// </summary>
        public RadialBlurLens()
        {
            this.material = new RadialBlurMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as RadialBlurMaterial).Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);
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
