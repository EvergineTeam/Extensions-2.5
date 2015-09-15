#region File Description
//-----------------------------------------------------------------------------
// TiltShiftLens
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
    /// Represent a TiltShiftLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class TiltShiftLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the blur scale, default value is 1.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 5.0f, 0.1f)]
        public float BlurScale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the blur downsample scale, default value is 4.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1, 8, 0.1f)]
        public float DownSampleScale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tilt shift Power, default value is 3.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 10.0f, 0.1f)]
        public float Power
        {
            get
            {
                return (this.material as TiltShiftMaterial).Power;
            }

            set
            {
                (this.material as TiltShiftMaterial).Power = value;
            }
        }

        /// <summary>
        /// Gets or sets the tilt shift Y position, default value is 0.5.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(-0.5f, 1.5f, 0.1f)]
        public float TiltPosition
        {
            get
            {
                return (this.material as TiltShiftMaterial).TiltPosition;
            }

            set
            {
                (this.material as TiltShiftMaterial).TiltPosition = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="TiltShiftMaterial"/> class.
        /// </summary>
        public TiltShiftLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new TiltShiftMaterial();
            this.BlurScale = 1;
            this.DownSampleScale = 4;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as TiltShiftMaterial;
            mat.TexcoordOffset.X = this.BlurScale / this.Source.Width;
            mat.TexcoordOffset.Y = this.BlurScale / this.Source.Height;

            int width = (int)(this.Source.Width / this.DownSampleScale);
            int height = (int)(this.Source.Height / this.DownSampleScale);

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            RenderTarget rt2 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // Down sampler
            mat.Texture = this.Source;
            mat.Pass = TiltShiftMaterial.Passes.Simple;
            this.RenderToImage(rt1, this.material);

            // Fast Blur
            mat.Pass = TiltShiftMaterial.Passes.FastBlur;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // Tiltshift
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = TiltShiftMaterial.Passes.TiltShift;
            mat.Texture = this.Source;
            mat.Texture1 = rt2;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
            mat.Texture1 = null;

            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt1);
            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt2);
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
