#region File Description
//-----------------------------------------------------------------------------
// SimpleEvadeStrategy3D
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Simple Evade Strategy implementation for 3D
    /// </summary>
    public class SimpleEvadeStrategy3D : EvadeStrategy
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
        /// The evade from entity
        /// </summary>
        private Entity evadeFromEntity;

        /// <summary>
        /// The evade from transform
        /// </summary>
        private Transform3D evadeFromTransform;

        /// <summary>
        /// The distance
        /// </summary>
        private int distance;

        /// <summary>
        /// The evade velocity
        /// </summary>
        private float evadeVelocity;

        /// <summary>
        /// Gets or sets the evade from entity.
        /// </summary>
        /// <value>
        /// The evade from entity.
        /// </value>
        public Entity EvadeFromEntity
        {
            get
            {
                return this.evadeFromEntity;
            }

            set
            {
                this.evadeFromEntity = value;
                this.GetEvadeEntityTransform();
            }
        }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        public int Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }

        /// <summary>
        /// Gets or sets the evade velocity.
        /// </summary>
        /// <value>
        /// The evade velocity.
        /// </value>
        public float EvadeVelocity
        {
            get { return this.evadeVelocity; }
            set { this.evadeVelocity = value; }
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.GetEvadeEntityTransform();
        }

        /// <summary>
        /// Executes the evade strategy.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Evade(TimeSpan gameTime)
        {
            var color = this.Owner.FindComponent<MaterialsMap>();
            ((StandardMaterial)color.DefaultMaterial).DiffuseColor = Color.Red;

            this.movingComponent.Direction = this.transform.Position - this.evadeFromTransform.Position;
            this.movingComponent.Direction.Normalize();

            this.transform.Position += this.movingComponent.Direction * this.movingComponent.NormalVelocity;
        }

        /// <summary>
        /// Contains the logic that decide to evade.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>True if evasion is needed, false in other case</returns>
        public override bool NeedToEvade(TimeSpan gameTime)
        {
            bool evade = false;

            if (this.evadeFromTransform != null)
            {
                evade = Vector3.Distance(this.transform.Position, this.evadeFromTransform.Position) < this.distance;
            }

            return evade;
        }

        /// <summary>
        /// Gets the evade entity transform.
        /// </summary>        
        private void GetEvadeEntityTransform()
        {
            if (this.evadeFromEntity != null)
            {
                this.evadeFromTransform = this.evadeFromEntity.FindComponent<Transform3D>();
                if (this.evadeFromTransform == null)
                {
                    throw new Exception(this.evadeFromEntity.Name + " entity need a Transform2D component");
                }
            }
        }
    }
}
