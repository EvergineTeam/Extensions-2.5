// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Noesis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using NoesisTexture = Noesis.Texture;
using WaveTexture = WaveEngine.Common.Graphics.Texture;

namespace WaveEngine.NoesisGUI.Providers
{
    /// <summary>
    /// Content provider for textures, using Wave Services.
    /// </summary>
    internal class ContentTextureProvider : TextureProvider
    {
        private Storage service;

        private ConcurrentDictionary<string, Lazy<WaveTexture>> textures;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTextureProvider"/> class.
        /// </summary>
        public ContentTextureProvider()
        {
            this.service = WaveServices.Storage;
            this.textures = new ConcurrentDictionary<string, Lazy<WaveTexture>>();
        }

        /// <summary>
        /// Gets info for a texture
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="width">The texture width.</param>
        /// <param name="height">The texture height.</param>
        public override void GetTextureInfo(string filename, out uint width, out uint height)
        {
            width = 1;
            height = 1;

            var texture = this.textures.GetOrAdd(filename, new Lazy<WaveTexture>(() => this.InnerLoadTexture(filename))).Value;

            if (texture != null)
            {
                width = (uint)texture.Width;
                height = (uint)texture.Height;
            }
        }

        /// <summary>
        /// Loads a texture
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>The loaded texture.</returns>
        public override NoesisTexture LoadTexture(string filename)
        {
            Lazy<WaveTexture> lazyTexture;
            if (this.textures.TryGetValue(filename, out lazyTexture))
            {
                var texture = lazyTexture.Value;

                if (WaveServices.Platform.AdapterType == AdapterType.DirectX)
                {
                    return NoesisTexture.WrapD3D11Texture(null, texture.NativePointer, texture.Width, texture.Height, texture.Levels, Texture.Format.BGRA8, false);
                }
                else
                {
                    return NoesisTexture.WrapGLTexture(null, texture.NativePointer, texture.Width, texture.Height, texture.Levels, Texture.Format.BGRA8, false);
                }
            }

            return null;
        }

        private WaveTexture InnerLoadTexture(string filename)
        {
            WaveTexture texture = null;

            try
            {
                using (var stream = this.OpenFile(filename))
                {
                    if (stream != null)
                    {
                        texture = Texture2D.FromFile(WaveServices.GraphicsDevice, stream);
                    }
                    else
                    {
                        texture = StaticResources.DefaultTexture;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Exception loading {filename} texture: {e.ToString()}");
            }

            return texture;
        }

        private Stream OpenFile(string filename)
        {
            if (System.IO.Path.IsPathRooted(filename) && File.Exists(filename))
            {
                return File.OpenRead(filename);
            }
            else if (this.service.ExistsContentFile(filename))
            {
                return this.service.OpenContentFile(filename);
            }

            return null;
        }

        /// <summary>
        /// Preload a texture so the provider can find them faster when needed
        /// </summary>
        /// <param name="filename">The file name of the texture to preload</param>
        /// <returns>False if the texture was already loaded, true otherwise</returns>
        internal bool PreloadTexture(string filename)
        {
            var lazy = new Lazy<WaveTexture>(() => this.InnerLoadTexture(filename));

            if (this.textures.TryAdd(filename, lazy))
            {
                // Force the evaluation of the Lazy object
                var value = lazy.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Register a texture with a given identifier
        /// </summary>
        /// <param name="identifier">The texture identifier</param>
        /// <param name="texture">The texture</param>
        /// <returns>False if the texture was already registered, true otherwise</returns>
        internal bool RegisterTexture(string identifier, WaveTexture texture)
        {
            return this.textures.TryAdd(identifier, new Lazy<WaveTexture>(() => texture));
        }
    }
}
