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
    /// Represent a 4x4 Vuforia matrix
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_Matrix4x4
    {
        /// <summary>
        /// Raw data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 4)]
        public float[] data;

        /// <summary>
        /// Convert the Vuforia matrix to an engine matrix.
        /// </summary>
        /// <returns>Converted matrix.</returns>
        public WaveEngine.Common.Math.Matrix ToEngineMatrix()
        {
            return new WaveEngine.Common.Math.Matrix()
            {
                M11 = this.data[0],
                M12 = this.data[1],
                M13 = this.data[2],
                M14 = this.data[3],
                M21 = this.data[4],
                M22 = this.data[5],
                M23 = this.data[6],
                M24 = this.data[7],
                M31 = this.data[8],
                M32 = this.data[9],
                M33 = this.data[10],
                M34 = this.data[11],
                M41 = this.data[12],
                M42 = this.data[13],
                M43 = this.data[14],
                M44 = this.data[15]
            };
        }
    }
}
