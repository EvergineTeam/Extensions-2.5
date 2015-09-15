#region File Description
//-----------------------------------------------------------------------------
// FovPort
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Parameters of lens
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FovPort
    {
        /// <summary>
        /// Up tan
        /// </summary>
        public float UpTan;

        /// <summary>
        /// Down tan
        /// </summary>
        public float DownTan;

        /// <summary>
        /// The left tan
        /// </summary>
        public float LeftTan;

        /// <summary>
        /// The right tan
        /// </summary>
        public float RightTan;
    }
}
