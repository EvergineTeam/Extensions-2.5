#region File Description
//-----------------------------------------------------------------------------
// ScreenOverlayLens
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
    /// Represent a ScreenOverlayLens as postprocessing filter.
    /// </summary>
    public class ScreenOverlayLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ColorSpace, default value is ColorCorrectionMaterial.ColorSpaceType.Default;
        /// </summary>
        public ScreenOverlayMaterial.BlendMode Mode
        {
            get
            {
                return (this.material as ScreenOverlayMaterial).OverlayMode;
            }

            set
            {
                (this.material as ScreenOverlayMaterial).OverlayMode = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanlinesLens" /> class.
        /// </summary>
        /// <param name="overlayTexturePath">The overlay texture path.</param>
        public ScreenOverlayLens(string overlayTexturePath)
        {
            this.material = new ScreenOverlayMaterial(overlayTexturePath);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as ScreenOverlayMaterial).Texture = this.Source;
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
