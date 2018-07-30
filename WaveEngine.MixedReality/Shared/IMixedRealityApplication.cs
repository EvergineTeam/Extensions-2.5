// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
#endregion

namespace WaveEngine.MixedReality
{
    /// <summary>
    /// Interface that represents an MixedReality application. This class is used in MixedRealityService
    /// </summary>
    public interface IMixedRealityApplication
    {
        /// <summary>
        /// Gets the eye properties for VR Camera
        /// </summary>
        VREye[] EyesProperties { get; }

        /// <summary>
        /// Gets the head ray
        /// </summary>
        Ray HeadRay { get; }
    }
}
