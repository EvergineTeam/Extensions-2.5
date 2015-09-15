#region File Description
//-----------------------------------------------------------------------------
// HexagonalNeighbours
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.TiledMap
{
    public class HexagonalNeighbours : NeighboursCollection
    {
        /// <summary>
        /// The stagger axis
        /// </summary>
        private TiledMapStaggerAxisType staggerAxis;

        /// <summary>
        /// The stagger index
        /// </summary>
        private TiledMapStaggerIndexType staggerIndex;

        #region Properties
        /// <summary>
        /// Gets the top neighbour.
        /// </summary>
        /// <value>
        /// The top neighbour.
        /// </value>
        public override LayerTile Top
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    return null;
                }
                else
                {
                    return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y - 1);
                }
            }
        }

        /// <summary>
        /// Gets the top right neighbour.
        /// </summary>
        /// <value>
        /// The top right neighbour.
        /// </value>
        public override LayerTile TopRight
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    var yParity = this.center.Y % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !yParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && yParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y - 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y - 1);
                    }
                }
                else
                {
                    var xParity = this.center.X % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !xParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && xParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y - 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the right neighbour.
        /// </summary>
        /// <value>
        /// The right neighbour.
        /// </value>
        public override LayerTile Right
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the bottom right neighbour.
        /// </summary>
        /// <value>
        /// The bottom right neighbour.
        /// </value>
        public override LayerTile BottomRight
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    var yParity = this.center.Y % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !yParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && yParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y + 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y + 1);
                    }
                }
                else
                {
                    var xParity = this.center.X % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !xParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && xParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the bottom neighbour.
        /// </summary>
        /// <value>
        /// The bottom neighbour.
        /// </value>
        public override LayerTile Bottom
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    return null;
                }
                else
                {
                    return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y + 1);
                }
            }
        }

        /// <summary>
        /// Gets the bottom left neighbour.
        /// </summary>
        /// <value>
        /// The bottom left neighbour.
        /// </value>
        public override LayerTile BottomLeft
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    var yParity = this.center.Y % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !yParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && yParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y + 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y + 1);
                    }
                }
                else
                {
                    var xParity = this.center.X % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !xParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && xParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the left neighbour.
        /// </summary>
        /// <value>
        /// The left neighbour.
        /// </value>
        public override LayerTile Left
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the top left neighbour.
        /// </summary>
        /// <value>
        /// The top left neighbour.
        /// </value>
        public override LayerTile TopLeft
        {
            get
            {
                if (this.staggerAxis == TiledMapStaggerAxisType.Y)
                {
                    var yParity = this.center.Y % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !yParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && yParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y - 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y - 1);
                    }
                }
                else
                {
                    var xParity = this.center.X % 2 == 0;

                    if ((this.staggerIndex == TiledMapStaggerIndexType.Even && !xParity)
                     || (this.staggerIndex == TiledMapStaggerIndexType.Odd && xParity))
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y - 1);
                    }
                    else
                    {
                        return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y);
                    }
                }
            }
        } 
        #endregion

        #region Initilization
        /// <summary>
        /// Initializes a new instance of the <see cref="HexagonalNeighbours"/> class.
        /// </summary>
        /// <param name="tileMapLayer">The tile map layer.</param>
        /// <param name="center">The center.</param>
        /// <param name="staggerAxis">The stagger axis.</param>
        /// <param name="staggerIndex">Index of the stagger.</param>
        public HexagonalNeighbours(TiledMapLayer tileMapLayer, LayerTile center, TiledMapStaggerAxisType staggerAxis, TiledMapStaggerIndexType staggerIndex)
            : base(tileMapLayer, center)
        {
            this.staggerAxis = staggerAxis;
            this.staggerIndex = staggerIndex;
        } 
        #endregion
    }
}
