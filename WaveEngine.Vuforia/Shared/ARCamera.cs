#region File Description
//-----------------------------------------------------------------------------
// ARCamera
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Cameras;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// FixedCamera decorate class
    /// </summary>
    public class ARCamera : FixedCamera3D
    {
        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ARCamera" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ARCamera(string name)
            : base(name, Vector3.Zero, Vector3.Zero)
        {
            this.entity.RemoveComponent<Camera3D>();
            this.entity.AddComponent(new ARCameraComponent());
        }
        #endregion
    }
}