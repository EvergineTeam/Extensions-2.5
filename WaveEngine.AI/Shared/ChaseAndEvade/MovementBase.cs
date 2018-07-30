// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Movement base component to define a move behavior
    /// </summary>
    public abstract class MovementBase : Component
    {
        /// <summary>
        /// Executes the move behavior.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public abstract void Move(TimeSpan gameTime);
    }
}
