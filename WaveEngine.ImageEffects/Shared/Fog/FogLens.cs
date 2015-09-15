#region File Description
//-----------------------------------------------------------------------------
// FogLens
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Fog as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class FogLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the fog based technique, default value is Exponential.
        /// </summary>
        [DataMember]
        public FogMaterial.Techniques Technique
        {
            get
            {
                return (this.material as FogMaterial).Technique;
            }

            set
            {
                (this.material as FogMaterial).Technique = value;
            }
        }

        /// <summary>
        /// Gets or sets Fog Color, default color is (0.5f, 0.6f, 0.7f)l.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        [DataMember]
        public Color FogColor
        {
            get
            {
                return (this.material as FogMaterial).FogColor;
            }

            set
            {
                (this.material as FogMaterial).FogColor = value;
            }
        }

        /// <summary>
        /// Gets or sets fog density, default value is 10.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1f, 100.0f, 2f)]
        public float FogDensity
        {
            get
            {
                return (this.material as FogMaterial).FogDensity;
            }

            set
            {
                (this.material as FogMaterial).FogDensity = value;
            }
        }

        /// <summary>
        /// Gets or sets start linear fog, default value is 0.02f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float StartFog
        {
            get
            {
                return (this.material as FogMaterial).StartFog;
            }

            set
            {
                (this.material as FogMaterial).StartFog = value;
            }
        }

        /// <summary>
        /// Gets or sets fog density, default value is 0.8f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float EndFog
        {
            get
            {
                return (this.material as FogMaterial).EndFog;
            }

            set
            {
                (this.material as FogMaterial).EndFog = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="FogLens"/> class.
        /// </summary>
        public FogLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new FogMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = (this.material as FogMaterial);
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
