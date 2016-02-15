#region File Description
//-----------------------------------------------------------------------------
// SepiaLens
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
    /// Represent a Sepia tonemapping post processing.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class SepiaLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the image tone, default value is Vector3(0.815f, 0.666f, 0f).
        /// </summary>
        [DataMember]
        public Vector3 ImageTone
        {
            get
            {
                return (this.material as SepiaMaterial).ImageTone;
            }

            set
            {
                (this.material as SepiaMaterial).ImageTone = value;
            }
        }

        /// <summary>
        /// Gets or sets the desaturation, default value is 1.0.
        /// </summary>
        [DataMember]
        public float Desaturation
        {
            get
            {
                return (this.material as SepiaMaterial).Desaturation;
            }

            set
            {
                (this.material as SepiaMaterial).Desaturation = value;
            }
        }

        /// <summary>
        /// Gets or sets the dark tone, default value is Vector3(0.313f, 0.258f, 0f).
        /// </summary>
        [DataMember]
        public Vector3 DarkTone
        {
            get
            {
                return (this.material as SepiaMaterial).DarkTone;
            }

            set
            {
                (this.material as SepiaMaterial).DarkTone = value;
            }
        }

        /// <summary>
        /// Gets or sets the toning, default value is 0.35f.
        /// </summary>
        [DataMember]
        public float Toning
        {
            get
            {
                return (this.material as SepiaMaterial).Toning;
            }

            set
            {
                (this.material as SepiaMaterial).Toning = value;
            }
        }

        /// <summary>
        /// Gets or sets the grey transfer, default value is new Vector3(0.3f, 0.59f, 0.11f).
        /// </summary>
        [DataMember]
        public Vector3 GreyTransfer
        {
            get
            {
                return (this.material as SepiaMaterial).GreyTransfer;
            }

            set
            {
                (this.material as SepiaMaterial).GreyTransfer = value;
            }
        }

        /// <summary>
        /// Gets or sets the global alpha returned, default value is 1.
        /// </summary>
        [DataMember]
        public float GlobalAlpha
        {
            get
            {
                return (this.material as SepiaMaterial).GlobalAlpha;
            }

            set
            {
                (this.material as SepiaMaterial).GlobalAlpha = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SepiaLens"/> class.
        /// </summary>
        public SepiaLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new SepiaMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as SepiaMaterial;
            mat.Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
        }

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
