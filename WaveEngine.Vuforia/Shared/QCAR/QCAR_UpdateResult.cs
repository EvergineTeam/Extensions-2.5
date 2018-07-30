// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace WaveEngine.Vuforia.QCAR
{
    /// <summary>
    /// Represent the result of the Vuforia tracker update
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_UpdateResult
    {
        /// <summary>
        /// Maximun number of trackable results
        /// </summary>
        public const int MAX_TRACKABLE_RESULTS = 5;

        /// <summary>
        /// Projection matrix to use when projecting the video background
        /// </summary>
        public QCAR_Matrix4x4 VideoBackgroundProjection;

        /// <summary>
        /// Number of current trackable results detected during the update
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int NumTrackableResults;

        /// <summary>
        /// Array of current trackable results detected during the update
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_TRACKABLE_RESULTS)]
        public QCAR_TrackableResult[] TrackableResults;
    }
}
