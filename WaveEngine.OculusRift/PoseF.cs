#region File Description
//-----------------------------------------------------------------------------
// PoseF
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Position and orientation together.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseF
    {
        /// <summary>
        /// The orientation
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        /// The position
        /// </summary>
        public Vector3 Position;
    }
}
