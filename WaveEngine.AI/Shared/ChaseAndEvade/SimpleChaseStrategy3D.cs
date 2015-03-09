#region File Description
//-----------------------------------------------------------------------------
// SimpleChaseStrategy3D
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Simple Chase Strategy implementation for 2D
    /// </summary>
    public class SimpleChaseStrategy3D : ChaseStrategy
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        private Transform3D transform;

        /// <summary>
        /// The moving component
        /// </summary>
        [RequiredComponent]
        private Simple3DMovement movingComponent;

        /// <summary>
        /// The target entity
        /// </summary>
        private Entity targetEntity;

        /// <summary>
        /// The target transform
        /// </summary>
        private Transform3D targetTransform;

        /// <summary>
        /// The detection radious
        /// </summary>
        private float detectionRadious;

        /// <summary>
        /// The direction
        /// </summary>
        private Vector3 direction;

        /// <summary>
        /// The follow velocity
        /// </summary>
        private float followVelocity;

        /// <summary>
        /// Gets or sets the follow velocity.
        /// </summary>
        /// <value>
        /// The follow velocity.
        /// </value>
        public float FollowVelocity
        {
            get { return this.followVelocity; }
            set { this.followVelocity = value; }
        }

        /// <summary>
        /// Gets or sets the detection radious.
        /// </summary>
        /// <value>
        /// The detection radious.
        /// </value>
        public float DetectionRadious
        {
            get { return this.detectionRadious; }
            set { this.detectionRadious = value; }
        }

        /// <summary>
        /// Gets or sets the target entity.
        /// </summary>
        /// <value>
        /// The target entity.
        /// </value>
        public Entity TargetEntity
        {
            get
            {
                return this.targetEntity;
            }

            set
            {
                this.targetEntity = value;
                this.GetTargetTransform();
            }
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
        }

        /// <summary>
        /// Checks if the target is detected
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>True if target is in range, false in other case</returns>
        public override bool TargetDetected(TimeSpan timeSpan)
        {
            bool saw = false;

            if (this.targetEntity != null)
            {
                if (Vector3.Distance(this.targetTransform.Position, this.transform.Position) < this.detectionRadious)
                {
                    saw = true;
                }
            }

            return saw;
        }

        /// <summary>
        /// Chases the target 
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public override void Chase(TimeSpan timeSpan)
        {
            if (this.targetTransform != null)
            {
                this.direction = this.targetTransform.Position - this.transform.Position;

                if (this.direction != Vector3.Zero)
                {
                    this.direction.Normalize();
                }

                this.transform.Position += this.direction * this.followVelocity;
            }
        }

        /// <summary>
        /// Gets the target transform.
        /// </summary>
        private void GetTargetTransform()
        {
            if (this.targetEntity != null)
            {
                this.targetTransform = this.targetEntity.FindComponent<Transform3D>();

                if (this.targetTransform == null)
                {
                    throw new Exception(this.targetEntity.Name + " entity need a Transform3D component");
                }
            }
        }
    }
}
