#region File Description
//-----------------------------------------------------------------------------
// VignetteLens
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
    /// Represent a Convolution Matrix as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class VignetteLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the power, default value is 1.0f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 10.0f, 0.1f)]
        public float Power
        {
            get
            {
                return (this.material as VignetteMaterial).Power;
            }

            set
            {
                (this.material as VignetteMaterial).Power = value;
            }
        }

        /// <summary>
        /// Gets or sets the radio, default value is 1.25f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 5.0f, 0.1f)]
        public float Radio
        {
            get
            {
                return (this.material as VignetteMaterial).Radio;
            }

            set
            {
                (this.material as VignetteMaterial).Radio = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingLens"/> class.
        /// </summary>
        public VignetteLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new VignetteMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as VignetteMaterial;
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
