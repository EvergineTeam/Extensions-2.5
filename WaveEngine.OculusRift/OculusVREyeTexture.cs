#region File Description
//-----------------------------------------------------------------------------
// OculusVREyeTexture
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OculusWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.VR;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Contains all the fields used by each eye.
    /// </summary>
    internal class OculusVREyeTexture : VREyeTexture
    {
        /// <summary>
        /// Field of view
        /// </summary>
        internal OVRTypes.FovPort FieldOfView;

        /// <summary>
        /// Texture size
        /// </summary>
        internal OVRTypes.Sizei TextureSize;

        /// <summary>
        /// Viewport size
        /// </summary>
        internal OVRTypes.Recti ViewportSize;

        /// <summary>
        /// Eye Render description
        /// </summary>
        internal OVRTypes.EyeRenderDesc RenderDescription;

        /// <summary>
        /// View offset
        /// </summary>
        internal OVRTypes.Vector3f HmdToEyeViewOffset;
    }
}
