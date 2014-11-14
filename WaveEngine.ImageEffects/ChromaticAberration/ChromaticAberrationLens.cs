#region File Description
//-----------------------------------------------------------------------------
// ChromaticAberrationLens
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
    /// Represent a ChromaticAberration as postprocessing filter.
    /// </summary>
    public class ChromaticAberrationLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the aberration strength, default value is 10f;
        /// </summary>
        /// <value>
        /// The chromatic aberration.
        /// </value>
        public float AberrationStrength
        {
            get
            {
                return (this.material as ChromaticAberrationMaterial).AberrationStrength;
            }

            set
            {
                (this.material as ChromaticAberrationMaterial).AberrationStrength = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromaticAberrationLens"/> class.
        /// </summary>
        public ChromaticAberrationLens()
        {
            this.material = new ChromaticAberrationMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as ChromaticAberrationMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            mat.Texture = this.Source;
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
