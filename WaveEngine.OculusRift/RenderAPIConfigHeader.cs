#region File Description
//-----------------------------------------------------------------------------
// RenderAPIConfigHeader
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Platform-independent part of rendering API-configuration data.
    /// It is a part of ovrRenderAPIConfig, passed to ovrHmd_Configure.
    /// </summary>
    public struct RenderAPIConfigHeader
    {
        /// <summary>
        /// The API
        /// </summary>
        public RenderAPIType API;

        /// <summary>
        /// The rt size
        /// </summary>
        public Size2 RTSize;

        /// <summary>
        /// The multisample
        /// </summary>
        public int Multisample;
    }
}
