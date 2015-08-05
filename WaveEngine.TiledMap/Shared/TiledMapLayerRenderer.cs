#region File Description
//-----------------------------------------------------------------------------
// TiledMapLayerRenderer
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Render a TiledMap Layer
    /// </summary>
    public class TiledMapLayerRenderer : Drawable2D
    {
        /// <summary>
        /// Number of indices per tile
        /// </summary>
        private const int indicesPerTile = 6;

        /// <summary>
        /// Number of indices per tile
        /// </summary>
        private const int verticesPerTile = 4;

        /// <summary>
        /// The associated tiled map
        /// </summary>
        private TiledMap tiledMap;

        /// <summary>
        /// Meshes list associated to its material
        /// </summary>
        private List<Tuple<BasicMaterial2D, Mesh>> meshes;

        /// <summary>
        /// Tiles count
        /// </summary>
        private int nTiles;

        /// <summary>
        /// Vertices data
        /// </summary>
        VertexPositionColorTexture[] vertices;

        /// <summary>
        /// The layer vertex buffer
        /// </summary>
        private DynamicVertexBuffer vertexBuffer;

        /// <summary>
        /// The layer index buffer
        /// </summary>
        private IndexBuffer indexBuffer;

        /// <summary>
        /// This component requires a Transfrom2D
        /// </summary>
        [RequiredComponent]
        private Transform2D transform2D = null;

        /// <summary>
        /// This component requires a TiledMapLayer component.
        /// </summary>
        [RequiredComponent]
        private TiledMapLayer tiledMapLayer = null;

        /// <summary>
        /// The sampler mode
        /// </summary>
        private AddressMode samplerMode;

        /// <summary>
        /// Gets the sampler mode
        /// </summary>
        public AddressMode SamplerMode
        {
            get
            {
                return this.samplerMode;
            }

            set
            {
                this.samplerMode = value;

                // restore all materials
                foreach (var tuple in this.meshes)
                {
                    tuple.Item1.SamplerMode = this.samplerMode;
                }
            }
        }

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayerRenderer" /> class.
        /// </summary>
        public TiledMapLayerRenderer()
            : this(DefaultLayers.Alpha, AddressMode.PointClamp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayerRenderer" /> class.
        /// </summary>
        /// <param name="layerType">The layer type.</param>
        public TiledMapLayerRenderer(Type layerType)
            : this(layerType, AddressMode.PointClamp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayerRenderer" /> class.
        /// </summary>
        /// <param name="layerType">The layer type.</param>
        /// <param name="samplerMode">The sampler mode.</param>
        public TiledMapLayerRenderer(Type layerType, AddressMode samplerMode)
            : base(layerType)
        {
            this.meshes = new List<Tuple<BasicMaterial2D, Mesh>>();
            this.samplerMode = samplerMode;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Draw debug lines
        /// </summary>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            Color color = Color.Yellow;
            float drawOrder = this.transform2D.DrawOrder;
            var lineBatch = this.RenderManager.LineBatch2D;
            Vector2 startPoint = Vector2.Zero;
            Vector2 endPoint = Vector2.Zero;

            int width = this.tiledMap.Width;
            int height = this.tiledMap.Height;
            int tileWidth = this.tiledMap.TileWidth;
            int tileHeight = this.tiledMap.TileHeight;

            Matrix worldTransform = this.transform2D.WorldTransform;

            lineBatch.DrawPointVM(Vector2.Zero, 10, color, drawOrder);

            switch (this.tiledMap.Orientation)
            {
                #region Orthogonal
                case TiledMapOrientationType.Orthogonal:
                    // Vertical lines
                    for (int i = 0; i <= width; i++)
                    {
                        float x = i * tileWidth;
                        startPoint.X = x;
                        startPoint.Y = 0;
                        endPoint.X = x;
                        endPoint.Y = height * tileHeight;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);

                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }

                    // Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        float y = i * tileHeight;
                        startPoint.X = 0;
                        startPoint.Y = y;
                        endPoint.X = width * tileWidth;
                        endPoint.Y = y;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);
                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }
                    break;
                #endregion

                #region Isometric
                case TiledMapOrientationType.Isometric:

                    // Vertical lines
                    for (int i = 0; i <= width; i++)
                    {
                        startPoint.X = (i * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        startPoint.Y = i * tileHeight * 0.5f;
                        endPoint.X = ((i - height) * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        endPoint.Y = (i + height) * tileHeight * 0.5f;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);
                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }

                    //// Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        startPoint.X = -(i * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        startPoint.Y = i * tileHeight * 0.5f;
                        endPoint.X = ((width - i) * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        endPoint.Y = (width + i) * tileHeight * 0.5f;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);
                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }

                    break;
                #endregion

                #region Staggered
                case TiledMapOrientationType.Staggered:
                    int fixHeight = (height - 1) / 2 + 1;

                    for (int i = 0; i < (width + fixHeight); i++)
                    {
                        startPoint.X = 0;
                        startPoint.Y = 0.5f + i;

                        endPoint.X = width;
                        endPoint.Y = startPoint.Y - width;

                        if (startPoint.Y > fixHeight)
                        {
                            startPoint.X = startPoint.Y - fixHeight;
                            startPoint.Y = fixHeight;
                        }

                        if (endPoint.Y < 0)
                        {
                            endPoint.X = width + endPoint.Y;
                            endPoint.Y = 0;
                        }

                        startPoint.X *= tileWidth;
                        startPoint.Y *= tileHeight;
                        endPoint.X *= tileWidth;
                        endPoint.Y *= tileHeight;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);
                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }

                    for (int i = 0; i < (width + fixHeight); i++)
                    {
                        startPoint.X = width;
                        startPoint.Y = 0.5f + i;

                        endPoint.X = 0;
                        endPoint.Y = startPoint.Y - width;

                        if (endPoint.Y < 0)
                        {
                            endPoint.X = width - startPoint.Y;
                            endPoint.Y = 0;
                        }

                        if (startPoint.Y > fixHeight)
                        {
                            startPoint.X = width - (startPoint.Y - fixHeight);
                            startPoint.Y = width;
                        }

                        startPoint.X *= tileWidth;
                        startPoint.Y *= tileHeight;
                        endPoint.X *= tileWidth;
                        endPoint.Y *= tileHeight;

                        Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
                        Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);
                        lineBatch.DrawLineVM(ref startPoint, ref endPoint, ref color, drawOrder);
                    }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// Draws the layer meshes
        /// </summary>
        /// <param name="gameTime">the current time</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.tiledMapLayer.NeedRefresh)
            {
                this.RefreshMeshes();
                this.tiledMapLayer.NeedRefresh = false;
            }

            Matrix worldTransform = this.transform2D.WorldTransform;
            float drawOrder = this.transform2D.DrawOrder;

            for (int i = 0; i < this.meshes.Count; i++)
            {
                var tuple = this.meshes[i];
                var material = tuple.Item1;
                var mesh = tuple.Item2;

                material.TintColor = Color.White * this.transform2D.Opacity;
                mesh.ZOrder = drawOrder;
                this.RenderManager.DrawMesh(mesh, material, ref worldTransform);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the layer renderer
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.tiledMap = this.tiledMapLayer.TiledMap;
        }

        /// <summary>
        /// Refresh layer meshes.
        /// </summary>
        private void RefreshMeshes()
        {
            this.meshes.Clear();

            // If the tiles count is changed, clear the previous buffers an init a new ones
            int newNTiles = this.tiledMapLayer.Tiles.Count();
            if (newNTiles != this.nTiles)
            {
                this.RemoveBuffers();
                this.nTiles = newNTiles;

                // Vertices                
                this.vertices = new VertexPositionColorTexture[this.nTiles * verticesPerTile];
                this.vertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);

                // Indices
                ushort[] indices = new ushort[this.nTiles * indicesPerTile];
                for (int i = 0; i < this.nTiles; i++)
                {
                    indices[i * 6] = (ushort)(i * 4);
                    indices[(i * 6) + 1] = (ushort)((i * 4) + 1);
                    indices[(i * 6) + 2] = (ushort)((i * 4) + 2);
                    indices[(i * 6) + 3] = (ushort)((i * 4) + 2);
                    indices[(i * 6) + 4] = (ushort)((i * 4) + 3);
                    indices[(i * 6) + 5] = (ushort)(i * 4);
                }
                this.indexBuffer = new IndexBuffer(indices);
            }

            int tileIndex = 0;
            int startIndex = 0;
            int currentIndexCount = 0;
            Tileset currentTileset = null;

            for (int i = 0; i < this.tiledMap.Height; i++)
            {
                for (int j = 0; j < this.tiledMap.Width; j++)
                {

                    int x, y;
                    this.GetCellRenderOrderByIndex(j, i, out x, out y);

                    LayerTile tile = this.tiledMapLayer.GetLayerTileByMapCoordinates(x, y);

                    if (tile == null || tile.Gid <= 0 || tile.Tileset == null || tile.Tileset.Image == null)
                    {
                        continue;
                    }

                    Tileset tileset = tile.Tileset;

                    if (currentTileset == null)
                    {
                        currentTileset = tileset;
                    }
                    else if (tileset != currentTileset)
                    {
                        this.NewMesh(startIndex, currentIndexCount, currentTileset.Image);

                        startIndex = tileIndex;
                        currentIndexCount = 0;
                        currentTileset = tileset;
                    }

                    this.FillTile(currentTileset, tile, tileIndex);

                    tileIndex++;
                    currentIndexCount++;
                }
            }

            if (currentTileset != null)
            {
                this.vertexBuffer.SetData(this.vertices);
                this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer);

                this.NewMesh(startIndex, currentIndexCount, currentTileset.Image);
            }
        }

        /// <summary>
        /// Get cell render order by the tile index
        /// </summary>
        /// <param name="i">The tile x coord</param>
        /// <param name="j">The tile y coord</param>
        /// <param name="x">The tile x render order</param>
        /// <param name="y">The tile y render order</param>
        private void GetCellRenderOrderByIndex(int i, int j, out int x, out int y)
        {
            switch (this.tiledMap.RenderOrder)
            {
                default:
                case TiledMapRenderOrderType.Right_Down:
                    x = i; 
                    y = j;
                    break;
                case TiledMapRenderOrderType.Right_Up:
                    x = i;
                    y = this.tiledMap.Height - j - 1;
                    break;
                case TiledMapRenderOrderType.Left_Down:
                    x = this.tiledMap.Width - i - 1;
                    y = j;
                    break;
                case TiledMapRenderOrderType.Left_Up:
                    x = this.tiledMap.Width - i - 1;
                    y = this.tiledMap.Height - j - 1;
                    break;
            }
        }

        /// <summary>
        /// Fill VertexBuffer with a tile
        /// </summary>
        /// <param name="tileset">The tileset.</param>
        /// <param name="tile">The tile information.</param>
        /// <param name="tileIndex">Current tileId</param>
        private void FillTile(Tileset tileset, LayerTile tile, int tileIndex)
        {
            int textureWidth = tileset.Image.Width;
            int textureHeight = tileset.Image.Height;
            int tilesetTileWidth = tileset.TileWidth;
            int tilesetTileHeight = tileset.TileHeight;
            int spacing = tileset.Spacing;
            int margin = tileset.Margin;
            int tileGid = tile.Gid;
            int sheetIndex = tileGid - tileset.FirstGid;

            int rectangleX = (sheetIndex % tileset.XTilesCount);
            int rectangleY = ((sheetIndex - rectangleX) / tileset.XTilesCount);

            Rectangle rect = new Rectangle(
                margin + (tilesetTileWidth + spacing) * rectangleX,
                margin + (tilesetTileHeight + spacing) * rectangleY,
                tilesetTileWidth,
                tilesetTileHeight);

            RectangleF tileRectangle = new RectangleF(
                rect.X / (float)textureWidth,
                rect.Y / (float)textureHeight,
                rect.Width / (float)textureWidth,
                rect.Height / (float)textureHeight);

            int vertexId = tileIndex * verticesPerTile;

            Vector2 position;
            this.GetTilePosition(tile, tileset, out position);

            #region Flip calculation - Original version
            /*
            float texCoordXStart, texCoordXEnd;
            float texCoordYStart, texCoordYEnd;

            if (tile.HorizontalFlip)
            {
                texCoordXStart = tileRectangle.X + tileRectangle.Width;
                texCoordXEnd = tileRectangle.X;
                texCoordYStart = tileRectangle.Y;
                texCoordYEnd = tileRectangle.Y + tileRectangle.Height;
            }
            else if (tile.VerticalFlip)
            {
                texCoordXStart = tileRectangle.X;
                texCoordXEnd = tileRectangle.X + tileRectangle.Width;
                texCoordYStart = tileRectangle.Y + tileRectangle.Height;
                texCoordYEnd = tileRectangle.Y;

            }
            else if (tile.DiagonalFlip)
            {
                texCoordXStart = tileRectangle.X + tileRectangle.Width;
                texCoordXEnd = tileRectangle.X;
                texCoordYStart = tileRectangle.Y + tileRectangle.Height;
                texCoordYEnd = tileRectangle.Y;
            }
            else
            {
                texCoordXStart = tileRectangle.X;
                texCoordXEnd = tileRectangle.X + tileRectangle.Width;
                texCoordYStart = tileRectangle.Y;
                texCoordYEnd = tileRectangle.Y + tileRectangle.Height;
            }

            // Vertex 0
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = new Vector2(texCoordXStart, texCoordYStart);
            vertexId++;
            
            // Vertex 1
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = new Vector2(texCoordXEnd, texCoordYStart);
            vertexId++;
            
            // Vertex 2
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = new Vector2(texCoordXEnd, texCoordYEnd);
            vertexId++;
            
            // Vertex 3
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = new Vector2(texCoordXStart, texCoordYEnd);
            vertexId++;
            */
            #endregion Flip calculation - Original version

            #region Flip calculation - Fixed version
            var topLeft = new Vector2(tileRectangle.X, tileRectangle.Y);
            var topRight = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y);
            var bottomRight = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y + tileRectangle.Height);
            var bottomLeft = new Vector2(tileRectangle.X, tileRectangle.Y + tileRectangle.Height);

            if (tile.DiagonalFlip)
            {
                var temp = topRight;
                topRight = bottomLeft;
                bottomLeft = temp;
            }

            if (tile.HorizontalFlip)
            {
                var temp = topLeft;
                topLeft = topRight;
                topRight = temp;
                temp = bottomRight;
                bottomRight = bottomLeft;
                bottomLeft = temp;
            }

            if (tile.VerticalFlip)
            {
                var temp = topLeft;
                topLeft = bottomLeft;
                bottomLeft = temp;
                temp = topRight;
                topRight = bottomRight;
                bottomRight = temp;
            }

            // Vertex 0
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = topLeft;
            vertexId++;

            // Vertex 1
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = topRight;
            vertexId++;

            // Vertex 2
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = bottomRight;
            vertexId++;

            // Vertex 3
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = bottomLeft;
            vertexId++;
            #endregion Flip calculation - Fixed version
        }

        /// <summary>
        /// Obtains the tile position
        /// </summary>
        /// <param name="tile">The tile</param>
        /// <param name="tileset">The tileset</param>
        /// <param name="position">The tile position</param>
        private void GetTilePosition(LayerTile tile, Tileset tileset, out Vector2 position)
        {
            switch (this.tiledMap.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    position = new Vector2(
                        tile.X * this.tiledMap.TileWidth,
                        tile.Y * this.tiledMap.TileHeight);
                    break;

                case TiledMapOrientationType.Isometric:
                    position = new Vector2(
                        ((tile.X - tile.Y) * this.tiledMap.TileWidth * 0.5f) + (this.tiledMap.Height * this.tiledMap.TileWidth * 0.5f) - this.tiledMap.TileWidth * 0.5f,
                        ((tile.X + tile.Y) * this.tiledMap.TileHeight * 0.5f));
                    break;

                case TiledMapOrientationType.Staggered:
                    position = new Vector2(
                        (tile.X + ((tile.Y % 2) * 0.5f)) * this.tiledMap.TileWidth,
                        tile.Y * 0.5f * this.tiledMap.TileHeight 
                        );

                    break;
                default:
                    position = Vector2.Zero;
                    break;
            }

            position.Y = position.Y + this.tiledMap.TileHeight - tileset.TileHeight;
        }

        /// <summary>
        /// Creates a new layer mesh
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        /// <param name="count">Count indices</param>
        /// <param name="image">Mesh material</param>
        private void NewMesh(int startIndex, int count, Texture2D image)
        {
            Mesh mesh = new Mesh(
                0,
                this.vertices.Length,
                startIndex * indicesPerTile,
                count * 2,
                this.vertexBuffer,
                this.indexBuffer,
                PrimitiveType.TriangleList)
                {
                    DisableBatch = true
                };

            BasicMaterial2D material = new BasicMaterial2D(image, this.LayerType)
            {
                SamplerMode = this.samplerMode
            };

            material.Initialize(this.Assets);

            this.meshes.Add(new Tuple<BasicMaterial2D, Mesh>(material, mesh));
        }

        /// <summary>
        /// Clear meshes
        /// </summary>
        private void RemoveBuffers()
        {
            this.vertices = null;
            if (this.vertexBuffer != null)
            {
                this.RenderManager.GraphicsDevice.DestroyVertexBuffer(this.vertexBuffer);
            }

            if (this.indexBuffer != null)
            {
                this.RenderManager.GraphicsDevice.DestroyIndexBuffer(this.indexBuffer);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
