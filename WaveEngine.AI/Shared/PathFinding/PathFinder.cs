#region File Description
//-----------------------------------------------------------------------------
// PathFinder
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.AI.PathFinding;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Path Resolver class
    /// </summary>
    /// <typeparam name="T">Types of nodes</typeparam>
    public class PathFinder<T> : Component
    {
        #region Variables
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The adjacency matrix
        /// </summary>
        private AdjacencyMatrix<T> adjacencyMatrix;

        /// <summary>
        /// The algorithm
        /// </summary>
        [RequiredComponent(false)]
        public PathFindingAlgorithm<T> Algorithm;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the adjacency matrix.
        /// </summary>
        /// <value>
        /// The adjacency matrix.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Adjacency matrix can not be null</exception>
        public AdjacencyMatrix<T> AdjacencyMatrix
        {
            get
            {
                return this.adjacencyMatrix;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Adjacency matrix can not be null");
                }

                this.adjacencyMatrix = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFinder{T}" /> class.
        /// </summary>
        public PathFinder()
            : base("PathFinder" + instances++)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// The next index on the adjacency list to go
        /// </returns>
        public T GetNextPosition(T start, T end)
        {
            return this.Algorithm.GetNextPosition(start, end);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>
        /// Gets the path to follow in the adjacency matrix from the initial position to end position
        /// </returns>
        public List<T> GetPath(T start, T end)
        {
            return this.Algorithm.GetPath(start, end);
        }
        #endregion
    }
}
