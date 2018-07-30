// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// NeighboursCollection
    /// </summary>
    public abstract class NeighboursCollection : IEnumerable<LayerTile>
    {
        /// <summary>
        /// The tile map layer
        /// </summary>
        protected TiledMapLayer tileMapLayer;

        /// <summary>
        /// The center
        /// </summary>
        protected LayerTile center;

        #region Properties

        /// <summary>
        /// Gets the top neighbour.
        /// </summary>
        /// <value>
        /// The top neighbour.
        /// </value>
        public abstract LayerTile Top { get; }

        /// <summary>
        /// Gets the top right neighbour.
        /// </summary>
        /// <value>
        /// The top right neighbour.
        /// </value>
        public abstract LayerTile TopRight { get; }

        /// <summary>
        /// Gets the right neighbour.
        /// </summary>
        /// <value>
        /// The right neighbour.
        /// </value>
        public abstract LayerTile Right { get; }

        /// <summary>
        /// Gets the bottom right neighbour.
        /// </summary>
        /// <value>
        /// The bottom right neighbour.
        /// </value>
        public abstract LayerTile BottomRight { get; }

        /// <summary>
        /// Gets the bottom neighbour.
        /// </summary>
        /// <value>
        /// The bottom neighbour.
        /// </value>
        public abstract LayerTile Bottom { get; }

        /// <summary>
        /// Gets the bottom left neighbour.
        /// </summary>
        /// <value>
        /// The bottom left neighbour.
        /// </value>
        public abstract LayerTile BottomLeft { get; }

        /// <summary>
        /// Gets the left neighbour.
        /// </summary>
        /// <value>
        /// The left neighbour.
        /// </value>
        public abstract LayerTile Left { get; }

        /// <summary>
        /// Gets the top left neighbour.
        /// </summary>
        /// <value>
        /// The top left neighbour.
        /// </value>
        public abstract LayerTile TopLeft { get; }

        /// <summary>
        /// Gets the <see cref="LayerTile"/> neighbour at the specified index. Starting from top neighbour in clockwise order.
        /// </summary>
        /// <value>
        /// The <see cref="LayerTile"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Neighbour at the specified index. Starting from top neighbour in clockwise order.</returns>
        public LayerTile this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.Top;
                    case 1: return this.TopRight;
                    case 2: return this.Right;
                    case 3: return this.BottomRight;
                    case 4: return this.Bottom;
                    case 5: return this.BottomLeft;
                    case 6: return this.Left;
                    case 7: return this.TopLeft;
                    default: return null;
                }
            }
        }
        #endregion

        #region Initilization

        /// <summary>
        /// Initializes a new instance of the <see cref="NeighboursCollection"/> class.
        /// </summary>
        /// <param name="tileMapLayer">The tile map layer.</param>
        /// <param name="center">The tile reference (center).</param>
        public NeighboursCollection(TiledMapLayer tileMapLayer, LayerTile center)
        {
            this.tileMapLayer = tileMapLayer;
            this.center = center;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator_LayerTile</returns>
        public IEnumerator<LayerTile> GetEnumerator()
        {
            for (int i = 0; i < 8; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
