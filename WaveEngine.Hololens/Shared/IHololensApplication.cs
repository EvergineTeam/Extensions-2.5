#region File Description
//-----------------------------------------------------------------------------
// IHololensApplication
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
#endregion

namespace WaveEngine.Hololens
{
    /// <summary>
    /// Interface that represents an hololens application. This class is used in HololensService
    /// </summary>
    public interface IHololensApplication
    {
        /// <summary>
        /// Gets the eye poses for VR Camera
        /// </summary>
        VREyePose[] EyePoses { get; }

        /// <summary>
        /// Gets the eye texture information for VR Camera
        /// </summary>
        VREyeTexture[] EyeTextures { get; }

        /// <summary>
        /// Gets the head ray
        /// </summary>
        Ray HeadRay { get; }
    }
}