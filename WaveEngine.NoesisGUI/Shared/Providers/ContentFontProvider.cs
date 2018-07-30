// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Noesis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WaveEngine.Framework.Services;

namespace WaveEngine.NoesisGUI.Providers
{
    /// <summary>
    /// Content provider for fonts, using Wave Services.
    /// </summary>
    internal class ContentFontProvider : FontProvider
    {
        private Storage service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFontProvider"/> class.
        /// </summary>
        public ContentFontProvider()
        {
            this.service = WaveServices.Storage;
        }

        /// <summary>
        /// Scans a folder for fonts
        /// </summary>
        /// <param name="folder">The folder to scan.</param>
        public override void ScanFolder(string folder)
        {
            var fileNames = this.service.GetContentFileNames(folder);
            foreach (var fileName in fileNames)
            {
                if (fileName.EndsWith(".ttf") || fileName.EndsWith(".otf"))
                {
                    this.RegisterFont(folder, System.IO.Path.GetFileName(fileName));
                }
            }
        }

        /// <summary>
        /// Opens a font
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="id">The font ID.</param>
        /// <returns>A stream with the loaded content.</returns>
        public override Stream OpenFont(string folder, string id)
        {
            var memStream = new MemoryStream();
            using (var fileStream = this.service.OpenContentFile(System.IO.Path.Combine(folder, id)))
            {
                fileStream.CopyTo(memStream);
            }

            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }
    }
}
