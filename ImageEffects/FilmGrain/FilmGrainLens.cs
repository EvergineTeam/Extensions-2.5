#region File Description
//-----------------------------------------------------------------------------
// FilmGrainLens
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
    /// Represent a Film grain as postprocessing filter.
    /// </summary>
    public class FilmGrainLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the grain intensity minimum.
        /// </summary>
        /// <value>
        /// The grain intensity minimum.
        /// </value>
        public float GrainIntensityMin
        {
            get
            {
                return (this.material as FilmGrainMaterial).GrainIntensityMin;
            }

            set
            {
                (this.material as FilmGrainMaterial).GrainIntensityMin = value;
            }
        }

        /// <summary>
        /// Gets or sets the grain intensity maximum.
        /// </summary>
        /// <value>
        /// The grain intensity maximum.
        /// </value>
        public float GrainIntensityMax
        {
            get
            {
                return (this.material as FilmGrainMaterial).GrainIntensityMax;
            }

            set
            {
                (this.material as FilmGrainMaterial).GrainIntensityMax = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the grain.
        /// </summary>
        /// <value>
        /// The size of the grain.
        /// </value>
        public float GrainSize
        {
            get
            {
                return (this.material as FilmGrainMaterial).GrainSize;
            }

            set
            {
                (this.material as FilmGrainMaterial).GrainSize = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmGrainLens"/> class.
        /// </summary>
        public FilmGrainLens() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilmGrainLens"/> class.
        /// </summary>
        public FilmGrainLens(string pathGrainTexture)
        {
            this.material = new FilmGrainMaterial(pathGrainTexture);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as FilmGrainMaterial;
            mat.ScreenWidth = this.Source.Width;
            mat.ScreenHeigth = this.Source.Height;

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
