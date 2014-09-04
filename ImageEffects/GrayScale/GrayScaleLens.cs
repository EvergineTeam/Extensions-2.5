#region File Description
//-----------------------------------------------------------------------------
// BlackAndWhiteLens
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Black and White postprocessing filter.
    /// </summary>
    public class GrayScaleLens : Lens
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayScaleLens"/> class.
        /// </summary>
        public GrayScaleLens()
            : base()
        {
            this.material = new GrayScaleMaterial();
        }

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as GrayScaleMaterial).Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
