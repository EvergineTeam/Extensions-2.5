// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia orientation structure. This is used by the engine to specify
    /// the application orientation.
    /// </summary>
    internal enum QCAR_Orientation
    {
        /// <summary>
        /// Portrait
        /// </summary>
        ORIENTATION_PORTRAIT = 0,

        /// <summary>
        /// Portrait upside down
        /// </summary>
        ORIENTATION_PORTRAIT_UPSIDEDOWN,

        /// <summary>
        /// Landscape left
        /// </summary>
        ORIENTATION_LANDSCAPE_LEFT,

        /// <summary>
        /// Landscape right
        /// </summary>
        ORIENTATION_LANDSCAPE_RIGHT,
    }
}
