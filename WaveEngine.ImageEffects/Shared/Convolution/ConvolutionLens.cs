﻿#region File Description
//-----------------------------------------------------------------------------
// ConvolutionLens
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
    /// Represent a Convolution Matrix as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class ConvolutionLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the filter, Laplace by default.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        [DataMember]
        public ConvolutionMaterial.FilterType Filter
        {
            get
            {
                return (this.material as ConvolutionMaterial).Filter;
            }

            set
            {
                (this.material as ConvolutionMaterial).Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ mul, default value is 1.0f.
        /// </summary>
        /// <value>
        /// The reduce_ mul.
        /// </value>
        [DataMember]        
        public float Scale
        {
            get
            {
                return (this.material as ConvolutionMaterial).Scale;
            }

            set
            {
                (this.material as ConvolutionMaterial).Scale = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingLens"/> class.
        /// </summary>
        public ConvolutionLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new ConvolutionMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = (this.material as ConvolutionMaterial);
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
