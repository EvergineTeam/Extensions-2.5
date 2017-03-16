#region File Description
//-----------------------------------------------------------------------------
// CameraResolution
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
    /// This interface represent the camera properties.
    /// </summary>
    public interface ICameraResolution
    {
        /// <summary>
        /// The height resolution.
        /// </summary>
        uint Height { get;}

        /// <summary>
        /// The width resolution.
        /// </summary>
        uint Width { get; }

        /// <summary>
        ///  The refresh rate.
        /// </summary>
        uint RefreshRate { get;}
    }
}
