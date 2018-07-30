// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.MixedReality.SpatialMapping;
#endregion

namespace WaveEngine.MixedReality.Toolkit
{
    /// <summary>
    /// GazeCollision
    /// </summary>
    [DataContract]
    public class GazeCollision : Component
    {
        /// <summary>
        /// gazeBehavior
        /// </summary>
        [RequiredComponent]
        private GazeBehavior gazeBehavior = null;

        /// <summary>
        /// CursorHit
        /// </summary>
        public struct CursorHit
        {
            /// <summary>
            /// Hit
            /// </summary>
            public bool Hit;

            /// <summary>
            /// Position
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Target
            /// </summary>
            public Vector3 Target;
        }

        private SpatialMappingService spatialMappingService;

        #region Properties

        /// <summary>
        /// Gets or sets surfaceOffset
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Offset value in meters between the surface and cursor")]
        public float SurfaceOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether normalAligned
        /// </summary>
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
        /// <param name="rayDistance">The ray distance to test</param>
        /// <returns>Cursor hit</returns>
        public CursorHit CheckCursorCollision(ref Ray ray, float rayDistance)
        {
            HitResult3D result = this.Owner.Scene.PhysicsManager.Simulation3D.RayCast(ref ray, rayDistance);

            CursorHit cursorHit = new CursorHit();
            if (result.Succeeded)
            {
                cursorHit.Hit = true;
                cursorHit.Position = result.Point - (ray.Direction * this.SurfaceOffset);
                cursorHit.Target = result.Point + result.Normal;
            }

            return cursorHit;
        }
    }
}
