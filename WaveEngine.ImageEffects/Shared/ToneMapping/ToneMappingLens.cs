#region File Description
//-----------------------------------------------------------------------------
// ToneMappingLens
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
    /// Represent a toneMapping image effect.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class ToneMappingLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the operator, default value is Linear.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [DataMember]
        public ToneMappingMaterial.OperatorType Operator
        {
            get
            {
                return (this.material as ToneMappingMaterial).Operator;
            }

            set
            {
                (this.material as ToneMappingMaterial).Operator = value;
            }
        }

        /// <summary>
        /// Gets or sets the gamma,  default value is 2.2f.
        /// </summary>
        /// <value>
        /// The gamma.
        /// </value>
        [DataMember]
        public float Gamma
        {
            get
            {
                return (this.material as ToneMappingMaterial).Gamma;
            }

            set
            {
                (this.material as ToneMappingMaterial).Gamma = value;
            }
        }

        /// <summary>
        /// Gets or sets the exposure, default value is 1.5f.
        /// </summary>
        /// <value>
        /// The exposure.
        /// </value>
        [DataMember]
        public float Exposure
        {
            get
            {
                return (this.material as ToneMappingMaterial).Exposure;
            }

            set
            {
                (this.material as ToneMappingMaterial).Exposure = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ToneMappingLens"/> class.
        /// </summary>
        public ToneMappingLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new ToneMappingMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as ToneMappingMaterial;
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
