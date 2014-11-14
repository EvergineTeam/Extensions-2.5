using System;
using System.Runtime.InteropServices;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11ConfigData
    {
        public RenderAPIConfigHeader Header;
        public IntPtr pDevice;
        public IntPtr pDeviceContext;
        public IntPtr pBackBufferRT;
        public IntPtr pSwapChain;
    }
}
