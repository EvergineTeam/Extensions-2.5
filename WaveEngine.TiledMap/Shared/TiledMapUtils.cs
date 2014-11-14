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

namespace WaveEngine.TiledMap
{
    public static class TiledMapUtils
    {
        public static Entity CollisionEntityFromObject(string entityName, TiledMapObject mapObject)
        {
            Entity entity = null;
            if (mapObject.ObjectType == TiledMapObjectType.Basic)
            {
                if (mapObject.Width > 0 && mapObject.Height > 0)
                {
                    entity = new Entity(entityName)
                    .AddComponent(new Transform2D()
                    {
                        LocalPosition = new Vector2(mapObject.X, mapObject.Y),
                        Rectangle = new RectangleF(0, 0, mapObject.Width, mapObject.Height),
                        LocalRotation = MathHelper.ToRadians(mapObject.Rotation),
                        Origin = Vector2.Zero
                    })
                    .AddComponent(new RectangleCollider())
                    ;
                }
            }
            else
            {
                // Only Basic object types are supported
            }

            return entity;
        }
    }
}
