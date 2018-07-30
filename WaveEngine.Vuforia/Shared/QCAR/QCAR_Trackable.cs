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
    /// Represents a Vuforia trackable definition
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_Trackable
    {
        /// <summary>
        /// Maximum number of characteres for a trackable name
        /// </summary>
        public const int MAX_TRACK_NAME_SIZE = 64;

        /// <summary>
        /// Identifier of the trackable
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int Id;

        /// <summary>
        /// Name if the trackable
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TRACK_NAME_SIZE)]
        public string TrackName;

        /// <summary>
        /// Trackable type
        /// </summary>
        public TargetTypes TargetType;
    }
}
