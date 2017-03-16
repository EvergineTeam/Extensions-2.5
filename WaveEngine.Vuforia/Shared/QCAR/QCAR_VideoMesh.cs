#region File Description
//-----------------------------------------------------------------------------
// QCAR_VideoMesh
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
using WaveEngine.Common.Graphics.VertexFormats;
#endregion

namespace WaveEngine.Vuforia.QCAR
{
    /// <summary>
    /// The video mesh
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_VideoMesh
    {
        /// <summary>
        /// The vertex 1
        /// </summary>
        VertexPositionTexture v1;

        /// <summary>
        /// The vertex 2
        /// </summary>
        VertexPositionTexture v2;

        /// <summary>
        /// The vertex 3
        /// </summary>
        VertexPositionTexture v3;

        /// <summary>
        /// The vertex 4
        /// </summary>
        VertexPositionTexture v4;

        /// <summary>
        /// The indices
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public ushort[] Indices;


        /// <summary>
        /// To array
        /// </summary>
        /// <returns>The vertex array</returns>
        public VertexPositionTexture[] Vertices
        {
            get
            {
                return new VertexPositionTexture[] { v1, v2, v3, v4 };
            }
        }
    };

}
