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
    /// Content provider for XAMLs, using Wave Services.
    /// </summary>
    internal class ContentXamlProvider : XamlProvider
    {
        private Storage service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentXamlProvider"/> class.
        /// </summary>
        public ContentXamlProvider()
        {
            this.service = WaveServices.Storage;
        }

        /// <summary>
        /// Load a XAML file.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>A stream with the loaded content.</returns>
        public override Stream LoadXaml(string filename)
        {
            var memStream = new MemoryStream();
            using (var fileStream = this.service.OpenContentFile(filename))
            {
                fileStream.CopyTo(memStream);
            }

            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }
    }
}
