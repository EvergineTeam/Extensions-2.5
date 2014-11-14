using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Size2
    {
        public static readonly Size2 Zero;
        public static readonly Size2 Empty;
        public int Width;
        public int Height;

        static Size2()
        {
            Zero = new Size2(0, 0);
            Empty = Zero;
        }

        public Size2(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
