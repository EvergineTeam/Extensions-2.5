#region File Description
//-----------------------------------------------------------------------------
// Size2
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Represent a size struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Size2
    {
        /// <summary>
        /// The zero
        /// </summary>
        public static readonly Size2 Zero;

        /// <summary>
        /// The empty
        /// </summary>
        public static readonly Size2 Empty;

        /// <summary>
        /// The width
        /// </summary>
        public int Width;

        /// <summary>
        /// The height
        /// </summary>
        public int Height;

        /// <summary>
        /// Initializes static members of the <see cref="Size2"/> struct.
        /// </summary>
        static Size2()
        {
            Zero = new Size2(0, 0);
            Empty = Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size2"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size2(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
