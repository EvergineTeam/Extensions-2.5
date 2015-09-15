#region File Description
//-----------------------------------------------------------------------------
// D3D11ConfigData
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
    /// D3D11ConfigData hold all InPtr needed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11ConfigData
    {
        /// <summary>
        /// The header
        /// </summary>
        public RenderAPIConfigHeader Header;

        /// <summary>
        /// The device
        /// </summary>
        public IntPtr Device;

        /// <summary>
        /// The device context
        /// </summary>
        public IntPtr DeviceContext;

        /// <summary>
        /// The back buffer rt
        /// </summary>
        public IntPtr BackBufferRT;

        /// <summary>
        /// The swap chain
        /// </summary>
        public IntPtr SwapChain;
    }
}
