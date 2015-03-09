#region File Description
//-----------------------------------------------------------------------------
// EvadeStrategy
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// The evade strategy base Component
    /// </summary>
    public abstract class EvadeStrategy : Component
    {
        /// <summary>
        /// Executes the evade strategy.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public abstract void Evade(TimeSpan gameTime);

        /// <summary>
        /// Contains the logic that decide to evade.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>True if evasion is needed, false in other case</returns>
        public abstract bool NeedToEvade(TimeSpan gameTime);
    }
}
