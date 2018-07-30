// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Represent a class that will contain the information a Tree behavior need to evaluate and execute a node.
    /// </summary>
    public class NodeInfo
    {
        /// <summary>
        /// The game time
        /// </summary>
        private TimeSpan gameTime;

        /// <summary>
        /// The evaluate tree
        /// </summary>
        private bool evaluateTree;

        /// <summary>
        /// Gets or sets the game time.
        /// </summary>
        /// <value>
        /// The game time.
        /// </value>
        public virtual TimeSpan GameTime
        {
            get { return this.gameTime; }
            set { this.gameTime = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree must be reevaluated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tree must be reevaluated; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EvaluateTree
        {
            get { return this.evaluateTree; }
            set { this.evaluateTree = value; }
        }
    }
}
