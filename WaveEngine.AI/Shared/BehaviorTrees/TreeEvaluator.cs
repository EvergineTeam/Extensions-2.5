#region File Description
//-----------------------------------------------------------------------------
// TreeEvaluator
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
    /// Represents a class that evaluates a behavior tree
    /// </summary>
    /// <typeparam name="T">The type of elements a behavior tree will contain</typeparam>
    public class TreeEvaluator<T> where T : NodeInfo
    {
        /// <summary>
        /// The current node
        /// </summary>
        public Node<T> CurrentNode;

        /// <summary>
        /// Evaluates the given tree and find the current node.
        /// </summary>
        /// <param name="nodeInfo">The node information.</param>
        /// <param name="tree">The tree behavior.</param>
        public void Evaluate(T nodeInfo, Node<T> tree)
        {
            var stack = new Stack<Node<T>>();

            stack.Push(tree);

            while (stack.Any())
            {
                var current = stack.Pop();
                if (current.Evaluate(nodeInfo))
                {
                    if (current.Children != null)
                    {
                        for (int i = current.Children.Count - 1; i >= 0; i--)
                        {
                            stack.Push(current.Children[i]);
                        }
                    }
                    else
                    {
                        this.CurrentNode = current;
                        //// do not use stat.clear(), we use break to abort the while when correct node has been reached
                        break;
                    }
                }
            }
        }
    }
}
