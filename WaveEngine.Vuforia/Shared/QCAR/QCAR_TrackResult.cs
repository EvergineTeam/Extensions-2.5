#region File Description
//-----------------------------------------------------------------------------
// QCAR_TrackResult
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace WaveEngine.Vuforia.QCAR
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_TrackResult
    {
        public const int TRACK_NAME_SIZE = 32;

        [MarshalAs(UnmanagedType.I1)]
        public bool IsTracking;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TRACK_NAME_SIZE)]
        public string TrackName;

        public QCAR_Matrix4x4 TrackPose;
        public QCAR_Matrix4x4 VideoBackgroundProjection;
    };

}
