#region File Description
//-----------------------------------------------------------------------------
// ColorCorrectionMaterial
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
using System.Reflection;
using WaveEngine.Common.IO;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// ColorCorrectionMaterial effect.
    /// </summary>
    public class ColorCorrectionMaterial : Material
    {
        /// <summary>
        /// Color Space type
        /// </summary>
        public enum ColorSpaceType
        {
            /// <summary>
            /// The default
            /// </summary>
            Default,

            /// <summary>
            /// The linear
            /// </summary>
            Linear,
        }

        /// <summary>
        /// The color space
        /// </summary>
        public ColorSpaceType ColorSpace;

        /// <summary>
        /// The scale
        /// </summary>
        private float scale;

        /// <summary>
        /// The offset
        /// </summary>
        private float offset;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The LUT texture
        /// </summary>
        private Texture lutTexture;

        /// <summary>
        /// The lut texture path
        /// </summary>
        private string lutTexturePath;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("ColorCorrection", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ColorCorrectionpsColorCorrection", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("ColorCorrectionLinear", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ColorCorrectionpsColorCorrectionLinear", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct ColorCorrectionEffectParameters
        {
            [FieldOffset(0)]
            public float Scale;

            [FieldOffset(4)]
            public float Offset;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private ColorCorrectionEffectParameters shaderParameters;

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
        /// Gets or sets the LUTTexture.
        /// </summary>
        public Texture LUTTexture
        {
            get
            {
                return this.lutTexture;
            }

            set
            {
                this.lutTexture = value;
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
                int index = (int)this.ColorSpace;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorCorrectionMaterial"/> class.
        /// </summary>
        public ColorCorrectionMaterial()
            : this(string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanlinesMaterial"/> class.
        /// </summary>
        public ColorCorrectionMaterial(string LUTTexture)
            : base(DefaultLayers.Opaque)
        {
            this.ColorSpace = ColorSpaceType.Default;
            this.lutTexturePath = LUTTexture;
            this.SamplerMode = AddressMode.LinearClamp;
            this.scale = (16f - 1f) / 16f;
            this.offset = 1.0f / (2.0f * 16f);

            this.shaderParameters = new ColorCorrectionEffectParameters();
            this.shaderParameters.Scale = this.scale;
            this.shaderParameters.Offset = this.offset;
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

            if (!string.IsNullOrEmpty(this.lutTexturePath))
            {
                this.lutTexture = assets.LoadAsset<Texture2D>(this.lutTexturePath);
            }
            else
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                var currentNamespace = assembly.GetName().Name;

                var textureResourcePath = currentNamespace + ".RGBTable16x1.wpk";
                var textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);

                this.lutTexture = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);
            }
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
                this.shaderParameters.Scale = this.scale;
                this.shaderParameters.Offset = this.offset;
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                if (this.lutTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.lutTexture, 1);
                }
            }
        }
        #endregion
    }
}
