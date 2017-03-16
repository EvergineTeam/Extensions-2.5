#region File Description
//-----------------------------------------------------------------------------
// AdjacencyMatrix
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Represents an adjacency Matrix
    /// </summary>
    /// <typeparam name="T">Type of nodes in the adjacency matrixc</typeparam>
    public class AdjacencyMatrix<T>
    {
        #region Variables
        /// <summary>
        /// The adjacencty matrix
        /// </summary>
        private Dictionary<T, List<Adjacency<T>>> adjacenctyMatrix;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the nodes count.
        /// </summary>
        /// <value>
        /// The nodes count.
        /// </value>
        public int NodesCount
        {
            get
            {
                return this.adjacenctyMatrix.Keys.Count;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjacencyMatrix{T}" /> class.
        /// </summary>
        public AdjacencyMatrix()
        {
            this.adjacenctyMatrix = new Dictionary<T, List<Adjacency<T>>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the adjacent.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="adjacent">The adjacent.</param>
        /// <param name="weight">The weight.</param>
        public void AddAdjacent(T node, T adjacent, int weight)
        {
            this.AddNodeIfNeeded(adjacent);

            this.AddNodeIfNeeded(node);

            this.AddAdjacency(node, adjacent, weight);

            this.AddAdjacency(adjacent, node, weight);            
        }

        /// <summary>
        /// Removes the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void RemoveNode(T node)
        {
            this.adjacenctyMatrix.Remove(node);

            foreach (T key in this.adjacenctyMatrix.Keys)
            {
                this.adjacenctyMatrix[key].RemoveAll(c => c.Node.Equals(node));
            }
        }

        /// <summary>
        /// Gets the adjacents.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Adjacents of the node</returns>
        public List<Adjacency<T>> GetAdjacents(T node)
        {
            if (this.adjacenctyMatrix.ContainsKey(node))
            {
                return this.adjacenctyMatrix[node];
            }
            else
            {
                return new List<Adjacency<T>>();
            }
        }

        /// <summary>
        /// Gets the bigger arist.
        /// </summary>
        /// <returns>The bigger arist</returns>
        public int GetBiggerArist()
        {
            int biggerArist = int.MinValue;
            foreach (var key in this.adjacenctyMatrix.Keys)
            {
                foreach (var adjacent in this.adjacenctyMatrix[key])
                {
                    if (adjacent.Weight > biggerArist)
                    {
                        biggerArist = adjacent.Weight;
                    }
                }
            }

            return biggerArist;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the less than zero node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="ArgumentException">When node is less than zero</exception>
        private void CheckLessThanZeroNode(int node)
        {
            if (node < 0)
            {
                throw new ArgumentException(string.Format("Node index {0} can not be less than zero.", node));
            }
        }

        /// <summary>
        /// Adds the node if needed.
        /// </summary>
        /// <param name="node">The node.</param>
        private void AddNodeIfNeeded(T node)
        {
            if (!this.adjacenctyMatrix.ContainsKey(node))
            {
                this.adjacenctyMatrix.Add(node, new List<Adjacency<T>>());
            }
        }

        /// <summary>
        /// Adds the adjacency.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="adjacent">The adjacent.</param>
        /// <param name="weight">The weight.</param>
        private void AddAdjacency(T node, T adjacent, int weight)
        {
            var adjacencies = this.adjacenctyMatrix[node];

            var adjacentToUpdate = adjacencies.Where(a => a.Node.Equals(adjacent)).FirstOrDefault();

            if (adjacentToUpdate != null)
            {
                adjacentToUpdate.Weight = weight;
            }
            else
            {
                this.adjacenctyMatrix[node].Add(new Adjacency<T>() { Node = adjacent, Weight = weight });
            }
        }
        #endregion
    }
}
