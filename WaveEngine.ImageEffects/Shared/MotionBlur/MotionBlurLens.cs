#region File Description
//-----------------------------------------------------------------------------
// MotionBlurLens
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
    /// Represent a Motion Blur as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class MotionBlurLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the blur lenght, default value is 0.5f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 2.0f, 0.01f)]
        public float BlurLength
        {
            get
            {
                return (this.material as MotionBlurMaterial).BlurLength;
            }

            set
            {
                (this.material as MotionBlurMaterial).BlurLength = value;
            }
        }

        /// <summary>
        /// Motion blur quality, Low by default.
        /// </summary>
        [DataMember]
        public MotionBlurMaterial.EffectQuality Quality
        {
            get
            {
                return (this.material as MotionBlurMaterial).Quality;
            }

            set
            {
                (this.material as MotionBlurMaterial).Quality = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="MotionBlurLens"/> class.
        /// </summary>
        public MotionBlurLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new MotionBlurMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as MotionBlurMaterial;
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
