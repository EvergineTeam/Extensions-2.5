#region File Description
//-----------------------------------------------------------------------------
// VRLensLayer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// VR lens mesh layer
    /// </summary>
    public class VRLensLayer : OpaqueLayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VRLensLayer" /> class.
        /// </summary>
        /// <param name="renderManager">Render Manager handler.</param>
        public VRLensLayer(RenderManager renderManager)
            : base(renderManager)
        {
            this.LayerMaskDeaultValue = false;
        }
    }
}
