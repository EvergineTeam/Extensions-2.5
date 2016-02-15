#region File Description
//-----------------------------------------------------------------------------
// RadialBlurMaterial
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// RadialBlur effect.
    /// </summary>
    public class RadialBlurMaterial : Material
    {
        /// <summary>
        /// The nsamples
        /// </summary>
        public int Nsamples 
        {
            get 
            { 
                return this.nsamples; 
            }

            set 
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidOperationException("Out of range, [0 - 30].");
                }

                this.nsamples = value;
            }
        }

        /// <summary>
        /// The blur width
        /// </summary>
        public float BlurWidth;

        /// <summary>
        /// The center
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// The nsamples
        /// </summary>
        private int nsamples;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("RadialBlur", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "RadialBlurpsRadialBlur", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct RadialBlurEffectParameters
        {
            [FieldOffset(0)]
            public int Nsamples;

            [FieldOffset(4)]
            public float BlurWidth;

            [FieldOffset(8)]
            public Vector2 Center;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private RadialBlurEffectParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture Texture
        {
            get
            {
                return this.texture;
            }

            set
            {
                this.texture = value;
            }
        }

        /// <summary>
        /// Gets the current technique.
        /// </summary>
        /// <value>
        /// The current technique.
        /// </value>
        public override string CurrentTechnique
        {
            get
            {
                return techniques[0].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanlinesMaterial"/> class.
        /// </summary>
        public RadialBlurMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.Nsamples = 10;
            this.BlurWidth = 0.1f;
            this.Center = new Vector2(0.5f);

            this.shaderParameters = new RadialBlurEffectParameters();
            this.shaderParameters.Nsamples = this.Nsamples;
            this.shaderParameters.BlurWidth = this.BlurWidth;
            this.shaderParameters.Center = this.Center;
            this.Parameters = this.shaderParameters;

            this.InitializeTechniques(techniques);
        }

        /// <summary>
        /// Initializes the specified assets.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public override void Initialize(WaveEngine.Framework.Services.AssetsContainer assets)
        {
            base.Initialize(assets);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Applies the pass.
        /// </summary>
        /// <param name="cached">The efect is cached.</param>
        public override void SetParameters(bool cached)
        {
            if (!cached)
            {
                this.shaderParameters.Nsamples = this.Nsamples;
                this.shaderParameters.BlurWidth = this.BlurWidth;
                this.shaderParameters.Center = this.Center;
                this.Parameters = this.shaderParameters;

                if (this.Texture != null)
                {
                    this.graphicsDevice.SetTexture(this.Texture, 0);
                }
            }
        }
        #endregion
    }
}
