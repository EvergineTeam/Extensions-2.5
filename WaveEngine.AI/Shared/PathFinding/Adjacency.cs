#region File Description
//-----------------------------------------------------------------------------
// Adjacency
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace WaveEngine.AI.PathFinding
{
    /// <summary>
    /// Adjacency class for adjacencies matrix
    /// </summary>
    /// <typeparam name="T">Node t</typeparam>
    public class Adjacency<T>
    {
        /// <summary>
        /// The node
        /// </summary>
        public T Node;

        /// <summary>
        /// The weight
        /// </summary>
        public int Weight;
    }
}
