#region File Description
//-----------------------------------------------------------------------------
// EyeType
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Specifies which eye is being used for rendering.
    /// This type explicitly does not include a third "NoStereo" option, as such is
    /// not required for an HMD-centered API.
    /// </summary>
    public enum EyeType
    {
        /// <summary>
        /// Left eye.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Right eye.
        /// </summary>
        Right = 1,
    }
}
