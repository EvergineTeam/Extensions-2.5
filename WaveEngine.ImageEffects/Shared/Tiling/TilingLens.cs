#region File Description
//-----------------------------------------------------------------------------
// TilingLens
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
    /// Represent an tiling as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class TilingLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the span_max, default value is Vector3(0.7f, 0.7f, 0.7f).
        /// </summary>
        /// <value>
        /// The span_max.
        /// </value>
        [DataMember]
        public Vector3 EdgeColor
        {
            get
            {
                return (this.material as TilingMaterial).EdgeColor;
            }

            set
            {
                (this.material as TilingMaterial).EdgeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ mul, default value 75f.
        /// </summary>
        /// <value>
        /// The reduce_ mul.
        /// </value>
        [DataMember]
        public float NumTiles
        {
            get
            {
                return (this.material as TilingMaterial).NumTiles;
            }

            set
            {
                (this.material as TilingMaterial).NumTiles = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ minimum, default value 0.15f.
        /// </summary>
        /// <value>
        /// The reduce_ minimum.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.01f)]
        public float Threshhold
        {
            get
            {
                return (this.material as TilingMaterial).Threshhold;
            }

            set
            {
                (this.material as TilingMaterial).Threshhold = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingLens"/> class.
        /// </summary>
        public TilingLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new TilingMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as TilingMaterial;
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
