﻿// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// Represent a Fast Blur as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class FastBlurLens : Lens
    {
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBlurLens"/> class.
        /// </summary>
        public FastBlurLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new FastBlurMaterial();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets blur scale, default value is 4.0f.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 20.0f, 0.1f)]
        public float BlurScale
        {
            get
            {
                return (this.material as FastBlurMaterial).BlurScale;
            }

            set
            {
                (this.material as FastBlurMaterial).BlurScale = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as FastBlurMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

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
