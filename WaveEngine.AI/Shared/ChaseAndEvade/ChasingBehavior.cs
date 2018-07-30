// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// The chasing behavior for an entity that can chase a target
    /// </summary>
    public class ChasingBehavior : Behavior
    {
        /// <summary>
        /// The strategy
        /// </summary>
        [RequiredComponent(false)]
        public ChaseStrategy Strategy;

        /// <summary>
        /// The move behavior
        /// </summary>
        [RequiredComponent(false)]
        public MovementBase MoveBehavior;

        /// <summary>
        /// The state
        /// </summary>
        public ChaseState State;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChasingBehavior"/> class.
        /// </summary>
        public ChasingBehavior()
            : base()
        {
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            this.EvaluateUpdate(gameTime);
        }

        /// <summary>
        /// Evaluates the update.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public void EvaluateUpdate(TimeSpan timeSpan)
        {
            if (this.Strategy.TargetDetected(timeSpan))
            {
                this.State = ChaseState.Chasing;
                this.Strategy.Chase(timeSpan);
            }
            else
            {
                this.State = ChaseState.Moving;
                this.MoveBehavior.Move(timeSpan);
            }
        }
    }
}
