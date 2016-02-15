#region File Description
//-----------------------------------------------------------------------------
// OculusVRCamera3D
//
// Copyright © 2016 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Components.VR;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Base decorator to create a VR Camera
    /// </summary>
    public class OculusVRCamera3D : BaseDecorator
    {
        #region Properties
        /// <summary>
        /// Gets the VRCameraRig component
        /// </summary>
        public VRCameraRig CameraRig
        {
            get
            {
                return this.Entity.FindComponent<VRCameraRig>();
            }
        }

        /// <summary>
        /// Gets the OculusVRProvider component
        /// </summary>
        public OculusVRProvider OculusVRProvider
        {
            get
            {
                return this.Entity.FindComponent<OculusVRProvider>();
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="OculusVRCamera3D" /> class.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <param name="position">The OVR position</param>
        public OculusVRCamera3D(string name, Vector3 position)
        {
            this.Entity = new Entity(name)
            .AddComponent(new Transform3D() { LocalPosition = position })
            .AddComponent(new VRCameraRig())
            .AddComponent(new OculusVRProvider());
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
