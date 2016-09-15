#region File Description
//-----------------------------------------------------------------------------
// VideoCaptureResult
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
    /// This class represent the result in some videocapture callbacks.
    /// </summary>
    public struct VideoCaptureResult
    {
        /// <summary>
        /// Gets the output file path.
        /// </summary>
        public string OutputPath { get; private set; }

        /// <summary>
        /// Whether the video was saved or not.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureResult"/> class.
        /// </summary>
        /// <param name="success">Whether the video was saved or not.</param>
        /// <param name="outputPath">The file output path.</param>
        public VideoCaptureResult(bool success, string outputPath)
            : this()
        {
            this.Success = success;
            this.OutputPath = outputPath;
        }
    }
}
