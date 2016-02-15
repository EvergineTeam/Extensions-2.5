#region File Description
//-----------------------------------------------------------------------------
// Node
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Represent the base class a tree behavior node must inherit
    /// </summary>
    /// <typeparam name="T">The type of elements a tree behavior will contain</typeparam>
    public abstract class Node<T> where T : NodeInfo
    {
        /// <summary>
        /// The children of the node
        /// </summary>
        public List<Node<T>> Children;

        /// <summary>
        /// Adds a child to the node
        /// </summary>
        /// <param name="children">The children to be added</param>
        /// <returns>The Node with the children added</returns>
        public Node<T> AddChild(Node<T> children)
        {
            if (this.Children == null)
            {
                this.Children = new List<Node<T>>();
            }

            this.Children.Add(children);
            return this;
        }

        /// <summary>
        /// Executes the logic of the node
        /// </summary>
        /// <param name="nodeInfo">The node information.</param>
        public abstract void Execute(T nodeInfo);

        /// <summary>
        /// Evaluates the node
        /// </summary>
        /// <param name="nodeInfo">The node information.</param>
        /// <returns>
        /// True if evaluation of the node success, false in other case
        /// </returns>
        public abstract bool Evaluate(T nodeInfo);
    }
}
