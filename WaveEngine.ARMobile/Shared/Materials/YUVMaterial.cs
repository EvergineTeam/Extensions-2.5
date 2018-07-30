// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Framework.Graphics;

namespace WaveEngine.ARMobile.Materials
{
    /// <summary>
    /// Material that shows a texture in YUV format
    /// </summary>
    public class YUVMaterial : Material
    {
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique(
                "YUVMaterialTechnique",
                "vsYUVMaterial",
                "psYUVMaterial",
                VertexPositionTexture.VertexFormat),
        };

        /// <summary>
        /// Gets or sets the luminance texture
        /// </summary>
        public Texture LuminanceTexture { get; set; }

        /// <summary>
        /// Gets or sets the luminance texture
        /// </summary>
        public Texture ChromaTexture { get; set; }

        /// <inheritdoc />
        public override string CurrentTechnique
        {
            get { return techniques[0].Name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YUVMaterial"/> class.
        /// </summary>
        public YUVMaterial()
            : base(DefaultLayers.Skybox)
        {
            this.InitializeTechniques(techniques);
        }

        /// <inheritdoc />
        public override void SetParameters(bool cached)
        {
            base.SetParameters(cached);

            if ((this.LuminanceTexture != null) && (this.ChromaTexture != null))
            {
                this.graphicsDevice.SetTexture(this.LuminanceTexture, 0);
                this.graphicsDevice.SetTexture(this.ChromaTexture, 1);
            }
        }
    }
}
