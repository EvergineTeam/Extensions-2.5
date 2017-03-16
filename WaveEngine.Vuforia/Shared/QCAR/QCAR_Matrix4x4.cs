#region File Description
//-----------------------------------------------------------------------------
// QCAR_Matrix4x4
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
    internal struct QCAR_Matrix4x4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 4)]
        public float[] data;

        /// <summary>
        /// To the engine matrix.
        /// </summary>
        /// <param name="mat">The mat.</param>
        /// <returns>Converted matrix.</returns>
        public WaveEngine.Common.Math.Matrix ToEngineMatrix()
        {
            return new WaveEngine.Common.Math.Matrix()
            {
                M11 = data[0],
                M12 = data[1],
                M13 = data[2],
                M14 = data[3],
                M21 = data[4],
                M22 = data[5],
                M23 = data[6],
                M24 = data[7],
                M31 = data[8],
                M32 = data[9],
                M33 = data[10],
                M34 = data[11],
                M41 = data[12],
                M42 = data[13],
                M43 = data[14],
                M44 = data[15]
            };
        }

    };
}
