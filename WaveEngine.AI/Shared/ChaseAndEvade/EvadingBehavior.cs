#region File Description
//-----------------------------------------------------------------------------
// EvadingBehavior
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// The behavior needed for a component that is going to have an evading behavior
    /// </summary>
    public class EvadingBehavior : Behavior
    {
        /// <summary>
        /// The evasion strategy
        /// </summary>
        [RequiredComponent(false)]
        public EvadeStrategy Strategy;

        /// <summary>
        /// The normal movement behavior
        /// </summary>
        [RequiredComponent(false)]
        public MovementBase MoveBehavior;

        /// <summary>
        /// The state
        /// </summary>
        public EvadeState State;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvadingBehavior"/> class.
        /// </summary>
        public EvadingBehavior()
            : base()
        {
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.EvaluateUpdate(gameTime);
        }

        /// <summary>
        /// Evaluates the update.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        internal void EvaluateUpdate(TimeSpan gameTime)
        {
            if (this.Strategy.NeedToEvade(gameTime))
            {
                this.Strategy.Evade(gameTime);
                this.State = EvadeState.Evading;
            }
            else
            {
                this.MoveBehavior.Move(gameTime);
                this.State = EvadeState.Moving;
            }
        }
    }
}
