#region File Description
//-----------------------------------------------------------------------------
// PathFindingQueueNode
//
// Copyright © 2015 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Path Finding Node for queue
    /// </summary>
    /// <typeparam name="T">Type of nodes</typeparam>
    public class PathFindingQueueNode<T> : PriorityQueueNode
    {
        #region Properties
        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public T Node { get; set; }

        #endregion

        #region Initialize
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
