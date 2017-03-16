#region File Description
//-----------------------------------------------------------------------------
// PhotoCaptureResult
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.Hololens.Media
{
    /// <summary>
    /// This class represent the result of a Take a photo using the Photo capture.
    /// </summary>
    public struct PhotoCaptureResult
    {
        /// <summary>
        /// Gets the output file path.
        /// </summary>
        public string OutputPath { get; private set; }

        /// <summary>
        /// Whether the photo was saved or not.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoCaptureResult"/> class.
        /// </summary>
        /// <param name="success">Whether the photo was saved or not.</param>
        /// <param name="outputPath">The file output path.</param>
        public PhotoCaptureResult(bool success, string outputPath)
            : this()
        {
            this.Success = success;
            this.OutputPath = outputPath;
        }
    }
}
