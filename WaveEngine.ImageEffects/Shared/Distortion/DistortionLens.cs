#region File Description
//-----------------------------------------------------------------------------
// DistortionLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a DistortionLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class DistortionLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Power, default value is 0.05f;
        /// </summary>
        /// <value>
        /// The lines factor.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 10.0f, 0.1f)]
        public float Power
        {
            get
            {
                return (this.material as DistortionMaterial).Power;
            }

            set
            {
                (this.material as DistortionMaterial).Power = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="DistortionLens"/> class.
        /// </summary>
        public DistortionLens()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistortionLens" /> class.
        /// </summary>
        /// <param name="normalTexturePath">The normal texture path.</param>
        public DistortionLens(string normalTexturePath)
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new DistortionMaterial(string.Empty);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as DistortionMaterial).Texture = this.Source;
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
