#region File Description
//-----------------------------------------------------------------------------
// TreeValidator
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Represent the tree validator class
    /// </summary>
    public class TreeValidator
    {
        /// <summary>
        /// Validatesif the given tree is correct.
        /// </summary>
        /// <typeparam name="T">The type of elements a behavior tree will contain</typeparam>
        /// <param name="tree">The tree.</param>
        /// <exception cref="System.ArgumentException">
        /// Root node must be a InitNode
        /// or
        /// Root node must have children
        /// </exception>
        public virtual void Validate<T>(Node<T> tree) where T : NodeInfo
        {
            if (!(tree is InitNode<T>))
            {
                throw new ArgumentException("Root node must be a InitNode");
            }

            if (tree.Children == null || !tree.Children.Any())
            {
                throw new ArgumentException("Root node must have children");
            }
        }
    }
}
