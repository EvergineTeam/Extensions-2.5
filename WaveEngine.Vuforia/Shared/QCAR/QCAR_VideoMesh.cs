// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
        private VertexPositionTexture v1;

        /// <summary>
        /// The vertex 2
        /// </summary>
        private VertexPositionTexture v2;

        /// <summary>
        /// The vertex 3
        /// </summary>
        private VertexPositionTexture v3;

        /// <summary>
        /// The vertex 4
        /// </summary>
        private VertexPositionTexture v4;

        /// <summary>
        /// The indices
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public ushort[] Indices;

        /// <summary>
        /// Gets to array
        /// </summary>
        /// <returns>The vertex array</returns>
        public VertexPositionTexture[] Vertices
        {
            get
            {
                return new VertexPositionTexture[] { this.v1, this.v2, this.v3, this.v4 };
            }
        }
    }
}
