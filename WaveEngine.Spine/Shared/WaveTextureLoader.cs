// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Spine;
using System.IO;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Spine
{
    /// <summary>
    /// Loader for wpk textures
    /// </summary>
    internal class WaveTextureLoader : TextureLoader
    {
        /// <summary>
        /// The assets
        /// </summary>
        private AssetsContainer assets;

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveTextureLoader" /> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public WaveTextureLoader(AssetsContainer assets)
        {
            this.assets = assets;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="path">The path.</param>
        public void Load(AtlasPage page, string path)
        {
            var material = new StandardMaterial()
            {
                LightingEnabled = false,
                Diffuse1Path = path,
                VertexColorEnable = true
            };

            material.Initialize(this.assets);

            var texture = material.Diffuse1;
            page.rendererObject = material;
            page.width = texture.Width;
            page.height = texture.Height;
        }

        /// <summary>
        /// Unloads the specified texture.
        /// </summary>
        /// <param name="material">The texture.</param>
        public void Unload(object material)
        {
            var mat = material as StandardMaterial;
            mat.Diffuse1Path = null;
        }
        #endregion
    }
}
