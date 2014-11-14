#region File Description
//-----------------------------------------------------------------------------
// ScanlinesLens
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
    /// Represent a ScanlinesLens as postprocessing filter.
    /// </summary>
    public class ScanlinesLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the lines factor, default value is 800;
        /// </summary>
        /// <value>
        /// The lines factor.
        /// </value>
        public float LinesFactor
        {
            get
            {
                return (this.material as ScanlinesMaterial).LinesFactor;
            }

            set
            {
                (this.material as ScanlinesMaterial).LinesFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the attenuation, default value is 0.04f.
        /// </summary>
        /// <value>
        /// The attenuation.
        /// </value>
        public float Attenuation
        {
            get
            {
                return (this.material as ScanlinesMaterial).Attenuation;
            }

            set
            {
                (this.material as ScanlinesMaterial).Attenuation = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanlinesLens"/> class.
        /// </summary>
        public ScanlinesLens()
        {
            this.material = new ScanlinesMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as ScanlinesMaterial).Texture = this.Source;
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
