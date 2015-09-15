#region File Description
//-----------------------------------------------------------------------------
// Rect
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
    /// Represent a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        /// <summary>
        /// The x
        /// </summary>
        private int x;

        /// <summary>
        /// The y
        /// </summary>
        private int y;

        /// <summary>
        /// The width
        /// </summary>
        private int width;

        /// <summary>
        /// The height
        /// </summary>
        private int height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
