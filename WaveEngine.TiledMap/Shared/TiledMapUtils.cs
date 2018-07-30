// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// TiledMapUtils
    /// </summary>
    public static class TiledMapUtils
    {
        /// <summary>
        /// CollisionEntityFromObject
        /// </summary>
        /// <param name="entityName">entityName</param>
        /// <param name="mapObject">mapObject</param>
        /// <returns>Entity</returns>
        public static Entity CollisionEntityFromObject(string entityName, TiledMapObject mapObject)
        {
            Entity entity = null;
            if (mapObject.ObjectType == TiledMapObjectType.Basic)
            {
                if (mapObject.Width > 0 && mapObject.Height > 0)
                {
                    entity = CreateBasicEntity(entityName, mapObject)
                        .AddComponent(new RectangleCollider2D());
                }
            }
            else if (mapObject.ObjectType == TiledMapObjectType.Polyline)
            {
                if (mapObject.Points.Count > 1)
                {
                    entity = new Entity(entityName)
                        .AddComponent(new Transform2D()
                        {
                            LocalPosition = new Vector2(mapObject.X, mapObject.Y),
                            LocalRotation = MathHelper.ToRadians(mapObject.Rotation),
                        })
                        .AddComponent(new EdgeCollider2D()
                        {
                            Vertices = mapObject.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToArray()
                        });
                }
            }
            else
            {
                // Only Basic and PolyLines object types are supported
            }

            return entity;
        }

        private static Entity CreateBasicEntity(string entityName, TiledMapObject mapObject)
        {
            return new Entity(entityName)
                    .AddComponent(new Transform2D()
                    {
                        LocalPosition = new Vector2(mapObject.X, mapObject.Y),
                        Rectangle = new RectangleF(0, 0, mapObject.Width, mapObject.Height),
                        LocalRotation = MathHelper.ToRadians(mapObject.Rotation),
                        Origin = Vector2.Zero
                    });
        }

        /// <summary>
        /// Gets the tile rectangle by tile identifier.
        /// </summary>
        /// <param name="tileset">The tileset.</param>
        /// <param name="tileId">The tile identifier.</param>
        /// <returns>Tile rectangle</returns>
        public static Rectangle GetRectangleTileByID(Tileset tileset, int tileId)
        {
            int rectangleX = tileId % tileset.XTilesCount;
            int rectangleY = (tileId - rectangleX) / tileset.XTilesCount;

            int x = tileset.Margin + ((tileset.TileWidth + tileset.Spacing) * rectangleX);
            int y = tileset.Margin + ((tileset.TileHeight + tileset.Spacing) * rectangleY);

            return new Rectangle(x, y, tileset.TileWidth, tileset.TileHeight);
        }
    }
}
