#region File Description
//-----------------------------------------------------------------------------
// ToneMappingMaterial
//
// Copyright © 2014 Wave Corporation
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
    /// ToneMapping effect.
    /// </summary>
    public class ToneMappingMaterial : Material
    {
        /// <summary>
        /// Operator type
        /// </summary>
        public enum OperatorType
        {
            /// <summary>
            /// The filmic
            /// </summary>
            Filmic,

            /// <summary>
            /// The linear
            /// </summary>
            Linear,

            /// <summary>
            /// The luma based reinhard
            /// </summary>
            LumaBasedReinhard,

            /// <summary>
            /// The white preserving luma based reinhard
            /// </summary>
            WhitePreservingLumaBasedReinhard,

            /// <summary>
            /// The photography
            /// </summary>
            Photography,

            /// <summary>
            /// The rombin da house
            /// </summary>
            RombinDaHouse,

            /// <summary>
            /// The simple reinhard
            /// </summary>
            SimpleReinhard,

            /// <summary>
            /// The uncharted2
            /// </summary>
            Uncharted2,
        }

        /// <summary>
        /// The operator
        /// </summary>
        public OperatorType Operator;

        /// <summary>
        /// The number tiles
        /// </summary>
        public float Gamma;

        /// <summary>
        /// The exposure
        /// </summary>
        public float Exposure;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Filmic", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsFilmic", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Linear", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsLinear", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("LumaBasedReinhard", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsLumaBasedReinhard", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("WhitePreservingLumaBasedReinhard", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsWhitePreservingLumaBasedReinhard", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Photography", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsPhotography", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("RombinDaHouse", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsRombinDaHouse", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("SimpleReinhard", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsSimpleReinhard", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Uncharted2", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ToneMappingpsUncharted2", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct ToneMappingEffectParameters
        {
            [FieldOffset(0)]
            public float Gamma;

            [FieldOffset(4)]
            public float Exposure;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private ToneMappingEffectParameters shaderParameters;

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
                if (value == null)
                {
                    throw new NullReferenceException("Texture cannot be null.");
                }

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
                int index = (int)this.Operator;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="ToneMappingMaterial"/> class.
        /// </summary>
        public ToneMappingMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.Operator = OperatorType.Linear;
            this.Gamma = 2.2f;
            this.Exposure = 1.0f;

            this.shaderParameters = new ToneMappingEffectParameters();
            UpdateShaderParamenters();

            this.InitializeTechniques(techniques);
        }

        private void UpdateShaderParamenters()
        {
            this.shaderParameters.Gamma = this.Gamma;
            this.shaderParameters.Exposure = this.Exposure;
            this.Parameters = this.shaderParameters;
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
                this.shaderParameters.Gamma = this.Gamma;
                this.shaderParameters.Exposure = this.Exposure;
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
