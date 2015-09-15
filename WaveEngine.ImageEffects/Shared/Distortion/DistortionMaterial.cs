#region File Description
//-----------------------------------------------------------------------------
// DistortionMaterial
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
    /// DistortionMaterial effect.
    /// </summary>
    public class DistortionMaterial : Material
    {
        /// <summary>
        /// The Power
        /// </summary>
        public float Power;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The normal
        /// </summary>
        private Texture normal;

        /// <summary>
        /// The normal texture path
        /// </summary>
        private string normalTexturePath;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Distortion", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "DistortionpsDistortion", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct DistortionEffectParameters
        {
            [FieldOffset(0)]
            public float Power;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private DistortionEffectParameters shaderParameters;

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
        /// Gets or sets the normal.
        /// </summary>
        /// <value>
        /// The normal.
        /// </value>
        public Texture Normal
        {
            get
            {
                return this.normal;
            }

            set
            {
                this.normal = value;
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
        /// Initializes a new instance of the <see cref="DistortionMaterial"/> class.
        /// </summary>
        public DistortionMaterial(string normalTexturePath)
            : base(DefaultLayers.Opaque)
        {
            this.normalTexturePath = normalTexturePath;
            this.SamplerMode = AddressMode.LinearClamp;
            this.Power = 0.05f;

            this.shaderParameters = new DistortionEffectParameters();
            this.shaderParameters.Power = this.Power;
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

            if (!string.IsNullOrEmpty(this.normalTexturePath))
            {
                this.normal = assets.LoadAsset<Texture2D>(this.normalTexturePath);
            }
            else
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                var currentNamespace = assembly.GetName().Name;

                var textureResourcePath = currentNamespace + ".DistortionNormals.wpk";
                var textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);

                this.normal = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);
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
                this.shaderParameters.Power = this.Power;
                this.Parameters = this.shaderParameters;

                if (this.Texture != null)
                {
                    this.graphicsDevice.SetTexture(this.Texture, 0);
                }

                if (this.Normal != null)
                {
                    this.graphicsDevice.SetTexture(this.normal, 1);
                }
            }
        }
        #endregion
    }
}
