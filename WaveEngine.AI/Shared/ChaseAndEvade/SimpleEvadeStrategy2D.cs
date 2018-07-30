// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Simple Chase Strategy implementation for 3D
    /// </summary>
    public class SimpleEvadeStrategy2D : EvadeStrategy
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform;

        /// <summary>
        /// The sprite
        /// </summary>
        [RequiredComponent]
        private Sprite sprite;

        /// <summary>
        /// The moving component
        /// </summary>
        [RequiredComponent]
        private Simple2DMovement movingComponent;

        /// <summary>
        /// The evade velocity
        /// </summary>
        private float evadeVelocity;

        /// <summary>
        /// The evade from transform
        /// </summary>
        private Transform2D evadeFromTransform;

        /// <summary>
        /// The distance
        /// </summary>
        private int distance;

        /// <summary>
        /// The evade from entity
        /// </summary>
        private Entity evadeFromEntity;

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
        /// Initializes a new instance of the <see cref="SimpleEvadeStrategy2D"/> class.
        /// </summary>
        public SimpleEvadeStrategy2D()
            : this(null, 200, 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEvadeStrategy2D"/> class.
        /// </summary>
        /// <param name="evadeFrom">The evade from.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="evadeVelocity">The evade velocity.</param>
        public SimpleEvadeStrategy2D(Entity evadeFrom, int distance, float evadeVelocity)
        {
            this.distance = distance;
            this.evadeFromEntity = evadeFrom;
            this.evadeVelocity = evadeVelocity;
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
        /// Gets the evade entity transform.
        /// </summary>
        private void GetEvadeEntityTransform()
        {
            if (this.evadeFromEntity != null)
            {
                this.evadeFromTransform = this.evadeFromEntity.FindComponent<Transform2D>();
                if (this.evadeFromTransform == null)
                {
                    throw new Exception(this.evadeFromEntity.Name + " entity need a Transform2D component");
                }
            }
        }

        /// <summary>
        /// Executes the evade strategy.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Evade(TimeSpan gameTime)
        {
            if (this.evadeFromTransform != null)
            {
                this.movingComponent.Direction = this.Transform.Position - this.evadeFromTransform.Position;

                if (this.movingComponent.Direction != Vector2.Zero)
                {
                    this.movingComponent.Direction.Normalize();
                }

                this.movingComponent.LookTo(this.Transform.Position + this.movingComponent.Direction, .15f * this.evadeVelocity);
                this.Transform.Position += this.movingComponent.Direction * this.evadeVelocity;
            }
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
                evade = Vector2.Distance(this.Transform.Position, this.evadeFromTransform.Position) < this.distance;
            }

            return evade;
        }
    }
}
