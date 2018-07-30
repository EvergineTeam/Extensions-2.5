// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.MixedReality.Media
{
    /// <summary>
    /// This interface represent the camera properties.
    /// </summary>
    public interface ICameraResolution
    {
        /// <summary>
        /// Gets the height resolution.
        /// </summary>
        uint Height { get; }

        /// <summary>
        /// Gets the width resolution.
        /// </summary>
        uint Width { get; }

        /// <summary>
        ///  Gets the refresh rate.
        /// </summary>
        uint RefreshRate { get; }
    }
}
