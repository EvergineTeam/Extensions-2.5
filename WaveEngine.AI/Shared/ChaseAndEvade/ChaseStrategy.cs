// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Chase strategy base class
    /// </summary>
    public abstract class ChaseStrategy : Component
    {
        /// <summary>
        /// Checks if the target is detected
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>True if target is in range, false in other case</returns>
        public abstract bool TargetDetected(TimeSpan timeSpan);

        /// <summary>
        /// Chases the target
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public abstract void Chase(TimeSpan timeSpan);
    }
}
