#region File Description
//-----------------------------------------------------------------------------
// GazeCollision
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Hololens.SpatialMapping;
#endregion

namespace WaveEngine.Hololens.Toolkit
{
    [DataContract]
    public class GazeCollision : Component
    {
        [RequiredComponent]
        private GazeBehavior gazeBehavior = null;

        public struct CursorHit
        {
            public bool Hit;
            public Vector3 Position;
            public Vector3 Target;
        };

        private SpatialMappingService spatialMappingService;

        #region Properties
        [DataMember]
        [RenderProperty(Tooltip = "Offset value in meters between the surface and cursor")]
        public float SurfaceOffset { get; set; }

        [DataMember]
        [RenderProperty(Tooltip = "Indicate whether cursor will be aligned with the surfaces or no in otherwise")]
        public bool NormalAligned { get; set; }
        #endregion

        /// <summary>
        /// Default Values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.SurfaceOffset = 0.02f;
            this.NormalAligned = true;
        }

        /// <summary>
        /// Resolve dependencies method
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.spatialMappingService = WaveServices.GetService<SpatialMappingService>();
            if (this.spatialMappingService != null)
            {
                Debug.WriteLine("GazeCollision component require the SpatialMappingService");
            }
        }

        /// <summary>
        /// Check cursor collision
        /// </summary>
        /// <param name="ray">ray cast</param>
        /// <returns>Cursor hit</returns>
        public CursorHit CheckCursorCollision(ref Ray ray)
        {
            RayCastResult3D result;
            this.Owner.Scene.PhysicsManager.RayCast3D(ray, out result);

            CursorHit cursorHit = new CursorHit();
            if (result.HitBody != null)
            {
                cursorHit.Hit = true;
                cursorHit.Position = result.HitData.Location - ray.Direction * this.SurfaceOffset;
                cursorHit.Target = result.HitData.Location + result.HitData.Normal;
            }

            return cursorHit;
        }
    }
}
