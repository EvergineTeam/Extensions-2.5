#region File Description
//-----------------------------------------------------------------------------
// InvertLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a invert image post processing.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class InvertLens : Lens
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvertLens"/> class.
        /// </summary>
        public InvertLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new InvertMaterial();
        }
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as InvertMaterial;
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
    }
}
