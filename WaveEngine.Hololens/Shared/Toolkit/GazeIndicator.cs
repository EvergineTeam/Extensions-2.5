#region File Description
//-----------------------------------------------------------------------------
// GazeIndicator
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
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Hololens.Toolkit
{
    [DataContract]
    public class GazeIndicator : Component
    {
        [RequiredComponent]
        private GazeBehavior gazeBehavior = null;

        private Entity indicator;
        private Transform3D indicatorTransform;

        private Vector3 planeNormal, cameraPosition, v, d, projectedPoint, indicatorDirection, indicatorPosition;

        #region Properties

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Prefab, Tooltip = "Prefab instantiate as indicator arrow.")]
        public string IndicatorPrefab { get; set; }

        [DataMember]
        [RenderProperty(Tooltip = "A single 3D point to follow")]
        public Vector3 TargetPoint { get; set; }

        [DataMember]
        [RenderProperty(Tooltip = "Radial offset distance between cursor and indicator.")]
        public float RadialOffset { get; set; }

        [DataMember]
        public float RadialVisibilityDistance { get; set; }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.TargetPoint = Vector3.Zero;
            this.RadialOffset = 0.02f;
            this.RadialVisibilityDistance = 0.5f;
        }

        /// <summary>
        /// Resolve dependencies method
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Owner.RemoveChild("Indicator");

            if (!string.IsNullOrEmpty(this.IndicatorPrefab))
            {
                this.indicator = this.EntityManager.Instantiate(this.IndicatorPrefab);
                this.indicator.IsSerializable = false;
                this.indicatorTransform = this.indicator.FindComponent<Transform3D>();
                this.Owner.AddChild(this.indicator);
            }
            else
            {
                Debug.WriteLine("You must to add a Indicator prefab");
            }
        }

        /// <summary>
        /// Update Indicator method
        /// </summary>        
        public void UpdateIndicator(Vector3 cursorPosition, Vector3 cursorScale)
        {
            var cameraTransform = this.RenderManager.ActiveCamera3D.Transform;
            this.cameraPosition = cameraTransform.Position;

            // Calculate plane
            this.planeNormal = cameraPosition - cursorPosition;
            planeNormal.Normalize();

            // Project point
            this.v = this.TargetPoint - cursorPosition;
            float distance;
            Vector3.Dot(ref v, ref this.planeNormal, out distance);
            this.d = this.planeNormal * distance;
            this.projectedPoint = this.TargetPoint - d;

            // Calcualte indicator            
            this.indicatorDirection = projectedPoint - cursorPosition;                        

            var frustumContained = this.RenderManager.ActiveCamera3D.BoundingFrustum.Contains(this.TargetPoint);
            this.indicator.IsVisible = frustumContained != ContainmentType.Contains;            

            indicatorDirection.Normalize();

            this.indicatorPosition = cursorPosition + (indicatorDirection * this.RadialOffset * cursorScale.Z);
            this.indicatorTransform.Position = indicatorPosition;

            // Indicator Orientation                                             
            this.indicatorTransform.LookAt(cameraPosition, indicatorDirection);
        }
    }
}
