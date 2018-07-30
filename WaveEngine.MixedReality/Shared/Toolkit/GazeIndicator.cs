// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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

namespace WaveEngine.MixedReality.Toolkit
{
    /// <summary>
    /// GazeIndicator
    /// </summary>
    [DataContract]
    public class GazeIndicator : Component
    {
        [RequiredComponent]
        private GazeBehavior gazeBehavior = null;

        private Entity indicator;
        private Transform3D indicatorTransform;

        private Vector3 planeNormal;

        private Vector3 cameraPosition;

        private Vector3 v;

        private Vector3 d;

        private Vector3 projectedPoint;

        private Vector3 indicatorDirection;

        private Vector3 indicatorPosition;

        #region Properties

        /// <summary>
        /// Gets or sets indicatorPrefab
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Prefab, Tooltip = "Prefab instantiate as indicator arrow.")]
        public string IndicatorPrefab { get; set; }

        /// <summary>
        /// Gets or sets targetPoint
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "A single 3D point to follow")]
        public Vector3 TargetPoint { get; set; }

        /// <summary>
        /// Gets or sets radialOffset
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Radial offset distance between cursor and indicator.")]
        public float RadialOffset { get; set; }

        /// <summary>
        /// Gets or sets radialVisibilityDistance
        /// </summary>
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
        /// <param name="cursorPosition">cursorPosition</param>
        /// <param name="cursorScale">cursorScale</param>
        public void UpdateIndicator(Vector3 cursorPosition, Vector3 cursorScale)
        {
            var cameraTransform = this.RenderManager.ActiveCamera3D.Transform;
            this.cameraPosition = cameraTransform.Position;

            // Calculate plane
            this.planeNormal = this.cameraPosition - cursorPosition;
            this.planeNormal.Normalize();

            // Project point
            this.v = this.TargetPoint - cursorPosition;
            float distance;
            Vector3.Dot(ref this.v, ref this.planeNormal, out distance);
            this.d = this.planeNormal * distance;
            this.projectedPoint = this.TargetPoint - this.d;

            // Calcualte indicator
            this.indicatorDirection = this.projectedPoint - cursorPosition;

            var frustumContained = this.RenderManager.ActiveCamera3D.BoundingFrustum.Contains(this.TargetPoint);
            this.indicator.IsVisible = frustumContained != ContainmentType.Contains;

            this.indicatorDirection.Normalize();

            this.indicatorPosition = cursorPosition + (this.indicatorDirection * this.RadialOffset * cursorScale.Z);
            this.indicatorTransform.Position = this.indicatorPosition;

            // Indicator Orientation
            this.indicatorTransform.LookAt(this.cameraPosition, this.indicatorDirection);
        }
    }
}
