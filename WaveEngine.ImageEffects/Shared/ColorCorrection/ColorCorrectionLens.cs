#region File Description
//-----------------------------------------------------------------------------
// ColorCorrectionLens
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a ColorCorrectionLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    internal class ColorCorrectionLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ColorSpace, default value is ColorCorrectionMaterial.ColorSpaceType.Default;
        /// </summary>
        [DataMember]
        public ColorCorrectionMaterial.ColorSpaceType ColorSpace
        {
            get
            {
                return (this.material as ColorCorrectionMaterial).ColorSpace;
            }

            set
            {
                (this.material as ColorCorrectionMaterial).ColorSpace = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanlinesLens"/> class.
        /// </summary>
        public ColorCorrectionLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new ColorCorrectionMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as ColorCorrectionMaterial).Texture = this.Source;
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
