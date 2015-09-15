#region File Description
//-----------------------------------------------------------------------------
// ScanlinesLens
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
    /// Represent a ScanlinesLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class ScanlinesLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the lines factor, default value is 800;
        /// </summary>
        /// <value>
        /// The lines factor.
        /// </value>
        [DataMember]
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
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.1f)]
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
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
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
            var mat = this.material as ScanlinesMaterial;
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
