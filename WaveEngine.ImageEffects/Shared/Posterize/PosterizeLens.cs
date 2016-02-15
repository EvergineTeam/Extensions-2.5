#region File Description
//-----------------------------------------------------------------------------
// PosterizeLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
    /// Represent a PosterizeLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class PosterizeLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Gamma, default value is 0.6f;
        /// </summary>
        [DataMember]
        public float Gamma
        {
            get
            {
                return (this.material as PosterizeMaterial).Gamma;
            }

            set
            {
                (this.material as PosterizeMaterial).Gamma = value;
            }
        }

        /// <summary>
        /// Gets or sets the Regions, default value is 5f.
        /// </summary>
        [DataMember]
        public float Regions
        {
            get
            {
                return (this.material as PosterizeMaterial).Regions;
            }

            set
            {
                (this.material as PosterizeMaterial).Regions = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="PosterizeLens"/> class.
        /// </summary>
        public PosterizeLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new PosterizeMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as PosterizeMaterial;
            mat.Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
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
