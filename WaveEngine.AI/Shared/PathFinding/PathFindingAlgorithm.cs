#region File Description
//-----------------------------------------------------------------------------
// PathFindingAlgorithm
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Path Finding Algorithm interface
    /// </summary>
    /// <typeparam name="T">Type of nodes</typeparam>
    public abstract class PathFindingAlgorithm<T> : Component
    {
        #region Public Methods
        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// The next position from start to end
        /// </returns>
        public abstract T GetNextPosition(T start, T end);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>The path from start to end</returns>
        public abstract List<T> GetPath(T start, T end);
        #endregion
    }
}
