#region File Description
//-----------------------------------------------------------------------------
// FishEyeLens
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
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
    /// Represent a Sepia tonemapping post processing.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class FishEyeLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the strength x, default value is 0.02f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(-0.50f, 0.50f, 0.01f)]
        public float StrengthX { get; set; }

        /// <summary>
        /// Gets or sets the strength y, default value is 0.02f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(-0.50f, 0.50f, 0.01f)]
        public float StrengthY { get; set; }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="FishEyeLens"/> class.
        /// </summary>
        public FishEyeLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new FishEyeMaterial();
            this.StrengthX = 0.02f;
            this.StrengthY = 0.02f;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            float aspectRatio = this.Source.Width / this.Source.Height;

            var mat = (this.material as FishEyeMaterial);
            mat.Intensity = new Vector2(StrengthX * aspectRatio, StrengthY * aspectRatio);
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
