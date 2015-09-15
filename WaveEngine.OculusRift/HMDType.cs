#region File Description
//-----------------------------------------------------------------------------
// HMDType
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// HMD type.
    /// </summary>
    public enum HMDType
    {
        /// <summary>
        /// None device.
        /// </summary>
        None = 0,

        /// <summary>
        /// DK1 device.
        /// </summary>
        DK1 = 3,

        /// <summary>
        /// DKHD device.
        /// </summary>
        DKHD = 4,

        /// <summary>
        /// DK2 device.
        /// </summary>
        DK2 = 6,

        /// <summary>
        ///  Some HMD other then the one in the enumeration.
        /// </summary>
        Other = 7  
    }
}
