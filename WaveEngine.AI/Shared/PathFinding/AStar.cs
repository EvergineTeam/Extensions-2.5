#region File Description
//-----------------------------------------------------------------------------
// AStar
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Path Finding A Star Algorithm
    /// </summary>
    /// <typeparam name="T">Type of nodes in graph</typeparam>
    public class AStar<T> : PathFindingAlgorithm<T>
    {
        #region Variables
        /// <summary>
        /// The adjacency matrix
        /// </summary>
        public AdjacencyMatrix<T> AdjacencyMatrix;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>The next position from start to end</returns>
        public override T GetNextPosition(T start, T end)
        {
            return default(T);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>The path from start to end</returns>
        public override List<T> GetPath(T start, T end)
        {
            Dictionary<T, T> camefrom = new Dictionary<T, T>();
            Dictionary<T, int> costSoFar = new Dictionary<T, int>();
            bool pathFound = false;

            var frontier = new PriorityQueue<PathFindingQueueNode<T>>(this.AdjacencyMatrix.NodesCount);
            frontier.Enqueue(new PathFindingQueueNode<T>() { Node = end }, 0);

            costSoFar.Add(end, 0);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Node.Equals(start))
                {
                    pathFound = true;
                    break;
                }

                var adjacents = this.AdjacencyMatrix.GetAdjacents(current.Node);

                foreach (var next in adjacents)
                {
                    int newCost = costSoFar[current.Node] + next.Weight;

                    if (!costSoFar.ContainsKey(next.Node)
                        || newCost < costSoFar[next.Node])
                    {
                        costSoFar[next.Node] = newCost;

                        int priority = newCost;

                        frontier.Enqueue(new PathFindingQueueNode<T>() { Node = next.Node }, priority);

                        camefrom[next.Node] = current.Node;
                    }
                }
            }

            return this.CreatePath(camefrom, start, end, pathFound);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the path.
        /// </summary>
        /// <param name="camefrom">The camefrom.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="pathFound">if set to <c>true</c> [path found].</param>
        /// <returns>
        /// The path to follow
        /// </returns>
        private List<T> CreatePath(Dictionary<T, T> camefrom, T start, T end, bool pathFound)
        {
            List<T> path = new List<T>();

            if (pathFound && camefrom.Count > 0)
            {
                if (camefrom.Count == 1)
                {
                    path.Add(end);
                }
                else
                {
                    T current = start;

                    while (!current.Equals(end))
                    {
                        current = camefrom[current];
                        path.Add(current);
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Adds the or update camefrom.
        /// </summary>
        /// <param name="camefrom">The camefrom.</param>
        /// <param name="current">The current.</param>
        /// <param name="next">The next.</param>
        private void AddOrUpdateCamefrom(Dictionary<T, T> camefrom, PathFindingQueueNode<T> current, Adjacency<T> next)
        {
            if (camefrom.ContainsKey(next.Node))
            {
                camefrom[next.Node] = current.Node;
            }
            else
            {
                camefrom.Add(next.Node, current.Node);
            }
        }

        /// <summary>
        /// Adds the or update cost so far.
        /// </summary>
        /// <param name="costSoFar">The cost so far.</param>
        /// <param name="next">The next.</param>
        /// <param name="newCost">The new cost.</param>
        private void AddOrUpdateCostSoFar(Dictionary<T, int> costSoFar, Adjacency<T> next, int newCost)
        {
            if (costSoFar.ContainsKey(next.Node))
            {
                costSoFar[next.Node] = newCost;
            }
            else
            {
                costSoFar.Add(next.Node, newCost);
            }
        }
        #endregion
    }
}
