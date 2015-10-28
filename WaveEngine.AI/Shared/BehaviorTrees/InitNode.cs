#region File Description
//-----------------------------------------------------------------------------
// InitNode
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Represent a strongly typed root node for a behavior tree
    /// </summary>
    /// <typeparam name="T">The type of elements a tree behavior will contain</typeparam>
    public sealed class InitNode<T> : Node<T> where T : NodeInfo
    {
        /// <summary>
        /// Executes the logic of the node
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Execute(T gameTime)
        {
        }

        /// <summary>
        /// Evaluates the node
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>True if evaluation of the node success, false in other case</returns>
        public override bool Evaluate(T gameTime)
        {
            return true;
        }
    }
}
