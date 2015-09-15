#region File Description
//-----------------------------------------------------------------------------
// D3D11TextureData
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// D3D11TextureData Hold all the data need to draw in a surface.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11TextureData
    {
        /// <summary>
        /// The header
        /// </summary>
        public TextureHeader Header;

        /// <summary>
        /// The pointer texture
        /// </summary>
        public IntPtr PTexture;

        /// <summary>
        /// The pointer sr view
        /// </summary>
        public IntPtr PSRView;

        /// <summary>
        /// The pointer platform data0
        /// </summary>
        public IntPtr PlatformData0;

        /// <summary>
        /// The pointer platform data1
        /// </summary>
        public IntPtr PlatformData1;

        /// <summary>
        /// The pointer platform data2
        /// </summary>
        public IntPtr PlatformData2;

        /// <summary>
        /// The pointer platform data3
        /// </summary>
        public IntPtr PlatformData3;

        /// <summary>
        /// The pointer platform data4
        /// </summary>
        public IntPtr PlatformData4;

        /// <summary>
        /// The pointer platform data5
        /// </summary>
        public IntPtr PlatformData5;
    }
}
