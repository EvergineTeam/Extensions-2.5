#region File Description
//-----------------------------------------------------------------------------
// FishEyeLens
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
    /// Represent a Sepia tonemapping post processing.
    /// </summary>
    public class FishEyeLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the strength x, default value is 0.02f.
        /// </summary>
        public float StrengthX { get; set; }

        /// <summary>
        /// Gets or sets the strength y, default value is 0.02f.
        /// </summary>
        public float StrengthY { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FishEyeLens"/> class.
        /// </summary>
        public FishEyeLens()
        {
            this.material = new FishEyeMaterial();
            this.StrengthX = 0.02f;
            this.StrengthY = 0.02f;
        }

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            float aspectRatio = this.Source.Width / this.Source.Height;

            var mat = (this.material as FishEyeMaterial);
            mat.Intensity = new Vector2(StrengthX * aspectRatio, StrengthY * aspectRatio);
            mat.Texture = this.Source;
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
