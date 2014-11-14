using System;
using System.Runtime.InteropServices;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11TextureData
    {
        public TextureHeader Header;
        public IntPtr pTexture;
        public IntPtr pSRView;
        public IntPtr pPlatformData0;
        public IntPtr pPlatformData1;
        public IntPtr pPlatformData2;
        public IntPtr pPlatformData3;
        public IntPtr pPlatformData4;
        public IntPtr pPlatformData5;
    }
}
