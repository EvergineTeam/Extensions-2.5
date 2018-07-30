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
    /// Represent the result of loading a dataset
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_LoadDataSetResult
    {
        /// <summary>
        /// Maxium number of trackable results
        /// </summary>
        public const int MAX_TRACKABLE_RESULTS = 5;

        /// <summary>
        /// Number of trackables stored in the loaded dataset
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int NumTrackables;

        /// <summary>
        /// Array of trackables stored in the loaded dataset
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_TRACKABLE_RESULTS)]
        public QCAR_Trackable[] Trackables;
    }
}
