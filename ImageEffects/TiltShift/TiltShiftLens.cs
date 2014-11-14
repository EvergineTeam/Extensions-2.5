#region File Description
//-----------------------------------------------------------------------------
// TiltShiftLens
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a TiltShiftLens as postprocessing filter.
    /// </summary>
    public class TiltShiftLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the tilt shift Power, default value is 3.
        /// </summary>
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
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="TiltShiftMaterial"/> class.
        /// </summary>
        public TiltShiftLens()
        {
            this.material = new TiltShiftMaterial();
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
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            int width = this.Source.Width / 4;
            int height = this.Source.Height / 4;

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
