#region File Description
//-----------------------------------------------------------------------------
// ScreenOverlayMaterial
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
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
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// ScreenOverlayMaterial effect.
    /// </summary>
    public class ScreenOverlayMaterial : Material
    {
        /// <summary>
        /// Blend mode between textures.
        /// </summary>
        public enum BlendMode
        {
            /// <summary>
            /// Additive.
            /// </summary>
            Additive,

            /// <summary>
            /// Screen blend.
            /// </summary>
            ScreenBlend,

            /// <summary>
            /// Multiply.
            /// </summary>
            Multiply,

            /// <summary>
            /// Overlay.
            /// </summary>
            Overlay,

            /// <summary>
            /// Alpha blend.
            /// </summary>
            AlphaBlend
        }

        /// <summary>
        /// The BlendMode
        /// </summary>
        public BlendMode OverlayMode;

        /// <summary>
        /// The intesity
        /// </summary>
        public float Intensity;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The LUT texture
        /// </summary>
        private Texture overlayTexture;

        /// <summary>
        /// The lut texture path
        /// </summary>
        private string overlayTexturePath;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Additive", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ScreenOverlaypsScreenOverlayAdditive", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("ScreenBlend", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ScreenOverlaypsScreenOverlayScreenBlend", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Multiply", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ScreenOverlaypsScreenOverlayMultiply", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Overlay", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ScreenOverlaypsScreenOverlayOverlay", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("AlphaBlend", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ScreenOverlaypsScreenOverlayAlphaBlend", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct ScreenOverlayEffectParameters
        {
            [FieldOffset(0)]
            public float Intensity;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private ScreenOverlayEffectParameters shaderParameters;

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
        /// Gets or sets the OverlayTexture.
        /// </summary>
        public Texture OverlayTexture
        {
            get
            {
                return this.overlayTexture;
            }

            set
            {
                this.overlayTexture = value;
            }
        }

        /// <summary>
        /// Gets or sets the overlay texture path.
        /// </summary>
        /// <value>
        /// The overlay texture path.
        /// </value>
        public string OverlayTexturePath
        {
            get
            {
                return this.overlayTexturePath;
            }

            set
            {
                this.overlayTexturePath = value;

                this.RefreshOverlayTexture();
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
                int index = (int)this.OverlayMode;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenOverlayMaterial"/> class.
        /// </summary>
        public ScreenOverlayMaterial(string overlayTexturePath = null)
            : base(DefaultLayers.Opaque)
        {
            this.OverlayMode = BlendMode.Multiply;
            this.overlayTexturePath = overlayTexturePath;
            this.SamplerMode = AddressMode.LinearClamp;
            this.Intensity = 1.0f;

            this.shaderParameters = new ScreenOverlayEffectParameters();
            this.shaderParameters.Intensity = this.Intensity;
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

            this.RefreshOverlayTexture();
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
                this.shaderParameters.Intensity = this.Intensity;
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                if (this.overlayTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.overlayTexture, 1);
                }
            }
        }
        #endregion

        #region Private

        /// <summary>
        /// Refreshes the overlay texture.
        /// </summary>
        private void RefreshOverlayTexture()
        {
            if (this.assetsContainer == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.overlayTexturePath))
            {
                this.overlayTexture = this.assetsContainer.LoadAsset<Texture2D>(this.overlayTexturePath);
            }
            else
            {
                var assembly = this.GetMemberAssembly();
                var currentNamespace = assembly.GetName().Name;

                var textureResourcePath = currentNamespace + ".ScreenOverlay.Gradient.wpk";
                var textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);

                this.overlayTexture = this.assetsContainer.LoadAsset<Texture2D>(textureResourcePath, textureStream);
            }
        }
        #endregion
    }
}
