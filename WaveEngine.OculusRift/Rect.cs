using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        private int X;
        private int Y;
        private int Width;
        private int Height;

        public Rect(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
