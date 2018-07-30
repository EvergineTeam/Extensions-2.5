// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Render a TiledMap Layer
    /// </summary>
    [DataContract]
    public class TiledMapLayerRenderer : Drawable2D
    {
        /// <summary>
        /// Number of indices per tile
        /// </summary>
        private const int IndicesPerTile = 6;

        /// <summary>
        /// Number of indices per tile
        /// </summary>
        private const int VerticesPerTile = 4;

        /// <summary>
        /// Maximum number of tiles per buffer
        /// </summary>
        private const int MaxTilesPerBuffer = ushort.MaxValue / IndicesPerTile;

        /// <summary>
        /// The associated tiled map
        /// </summary>
        private TiledMap tiledMap;

        /// <summary>
        /// Materials list for each tileset
        /// </summary>
        private List<StandardMaterial> materials;

        /// <summary>
        /// Meshes list
        /// </summary>
        private List<Mesh> meshes;

        /// <summary>
        /// Tiles count
        /// </summary>
        private int nTiles;

        /// <summary>
        /// Vertices data
        /// </summary>
        private VertexPositionColorTexture[][] vertices;

        /// <summary>
        /// The layer vertex buffer
        /// </summary>
        private DynamicVertexBuffer[] vertexBuffer;

        /// <summary>
        /// The layer index buffer
        /// </summary>
        private IndexBuffer[] indexBuffer;

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
        /// The cached origin
        /// </summary>
        private Vector2 cachedOrigin;

        /// <summary>
        /// A transform to apply the origin
        /// </summary>
        private Matrix originTranslation;

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayerRenderer" /> class.
        /// </summary>
        public TiledMapLayerRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayerRenderer" /> class.
        /// </summary>
        /// <param name="layerId">The layer type.</param>
        public TiledMapLayerRenderer(int layerId)
            : base(layerId)
        {
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.materials = new List<StandardMaterial>();
            this.meshes = new List<Mesh>();
            this.originTranslation = Matrix.Identity;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Draw debug lines
        /// </summary>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            Matrix worldTransform = this.originTranslation * this.transform2D.WorldTransform;

            Vector2 startPoint = Vector2.Zero;
            Vector2 endPoint = Vector2.Zero;

            int width = this.tiledMap.Width;
            int height = this.tiledMap.Height;
            int tileWidth = this.tiledMap.TileWidth;
            int tileHeight = this.tiledMap.TileHeight;

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

                        this.DrawDebugLine(ref startPoint, ref endPoint, ref worldTransform);
                    }

                    // Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        float y = i * tileHeight;
                        startPoint.X = 0;
                        startPoint.Y = y;
                        endPoint.X = width * tileWidth;
                        endPoint.Y = y;

                        this.DrawDebugLine(ref startPoint, ref endPoint, ref worldTransform);
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

                        this.DrawDebugLine(ref startPoint, ref endPoint, ref worldTransform);
                    }

                    // Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        startPoint.X = -(i * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        startPoint.Y = i * tileHeight * 0.5f;
                        endPoint.X = ((width - i) * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        endPoint.Y = (width + i) * tileHeight * 0.5f;

                        this.DrawDebugLine(ref startPoint, ref endPoint, ref worldTransform);
                    }

                    break;
                #endregion

                #region Staggered & Hexagonal
                case TiledMapOrientationType.Staggered:
                case TiledMapOrientationType.Hexagonal:
                    int sideLengthX = 0;
                    int sideLengthY = 0;

                    if (this.tiledMap.Orientation == TiledMapOrientationType.Hexagonal)
                    {
                        if (this.tiledMap.StaggerAxis == TiledMapStaggerAxisType.X)
                        {
                            sideLengthX = this.tiledMap.HexSideLength;
                        }
                        else
                        {
                            sideLengthY = this.tiledMap.HexSideLength;
                        }
                    }

                    var sideOffsetX = (tileWidth - sideLengthX) / 2;
                    var sideOffsetY = (tileHeight - sideLengthY) / 2;

                    var columnWidth = sideOffsetX + sideLengthX;
                    var rowHeight = sideOffsetY + sideLengthY;

                    var oct = new Vector2[]
                    {
                        new Vector2(0,                        tileHeight - sideOffsetY),
                        new Vector2(0,                        sideOffsetY),
                        new Vector2(sideOffsetX,              0),
                        new Vector2(tileWidth - sideOffsetX,  0),
                        new Vector2(tileWidth,                sideOffsetY),
                        new Vector2(tileWidth,                tileHeight - sideOffsetY),
                        new Vector2(tileWidth - sideOffsetX,  tileHeight),
                        new Vector2(sideOffsetX,              tileHeight)
                    };

                    var startTile = Vector2.Zero;
                    var startPos = Vector2.Zero;

                    if (this.tiledMap.StaggerAxis == TiledMapStaggerAxisType.X)
                    {
                        for (; /*startPos.X <= rect.right() &&*/ startTile.X < width; startTile.X++)
                        {
                            Vector2 rowTile = startTile;
                            Vector2 rowPos = startPos;

                            if (this.tiledMap.DoStaggerX((int)startTile.X))
                            {
                                rowPos.Y += rowHeight;
                            }

                            for (; /*rowPos.Y <= rect.bottom() && */rowTile.Y < height; rowTile.Y++)
                            {
                                this.DrawDebugLine(rowPos + oct[1], rowPos + oct[2], ref worldTransform);
                                this.DrawDebugLine(rowPos + oct[2], rowPos + oct[3], ref worldTransform);
                                this.DrawDebugLine(rowPos + oct[3], rowPos + oct[4], ref worldTransform);

                                bool isStaggered = this.tiledMap.DoStaggerX((int)startTile.X);
                                bool lastRow = rowTile.Y == height - 1;
                                bool lastColumn = rowTile.X == width - 1;
                                bool bottomLeft = rowTile.X == 0 || (lastRow && isStaggered);
                                bool bottomRight = lastColumn || (lastRow && isStaggered);

                                if (bottomRight)
                                {
                                    this.DrawDebugLine(rowPos + oct[5], rowPos + oct[6], ref worldTransform);
                                }

                                if (lastRow)
                                {
                                    this.DrawDebugLine(rowPos + oct[6], rowPos + oct[7], ref worldTransform);
                                }

                                if (bottomLeft)
                                {
                                    this.DrawDebugLine(rowPos + oct[7], rowPos + oct[0], ref worldTransform);
                                }

                                rowPos.Y += tileHeight + sideLengthY;
                            }

                            startPos.X += columnWidth;
                        }
                    }
                    else
                    {
                        for (; /* startPos.Y <= rect.bottom() &&*/ startTile.Y < height; startTile.Y++)
                        {
                            Vector2 rowTile = startTile;
                            Vector2 rowPos = startPos;

                            if (this.tiledMap.DoStaggerY((int)startTile.Y))
                            {
                                rowPos.X += columnWidth;
                            }

                            for (; /*rowPos.X <= rect.right() &&*/ rowTile.X < width; rowTile.X++)
                            {
                                this.DrawDebugLine(rowPos + oct[0], rowPos + oct[1], ref worldTransform);
                                this.DrawDebugLine(rowPos + oct[1], rowPos + oct[2], ref worldTransform);
                                this.DrawDebugLine(rowPos + oct[3], rowPos + oct[4], ref worldTransform);

                                bool isStaggered = this.tiledMap.DoStaggerY((int)startTile.Y);
                                bool lastRow = rowTile.Y == height - 1;
                                bool lastColumn = rowTile.X == width - 1;
                                bool bottomLeft = lastRow || (rowTile.X == 0 && !isStaggered);
                                bool bottomRight = lastRow || (lastColumn && isStaggered);

                                if (lastColumn)
                                {
                                    this.DrawDebugLine(rowPos + oct[4], rowPos + oct[5], ref worldTransform);
                                }

                                if (bottomRight)
                                {
                                    this.DrawDebugLine(rowPos + oct[5], rowPos + oct[6], ref worldTransform);
                                }

                                if (bottomLeft)
                                {
                                    this.DrawDebugLine(rowPos + oct[7], rowPos + oct[0], ref worldTransform);
                                }

                                rowPos.X += tileWidth + sideLengthX;
                            }

                            startPos.Y += rowHeight;
                        }
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

            if (this.cachedOrigin != this.transform2D.Origin)
            {
                this.cachedOrigin = this.transform2D.Origin;
                this.originTranslation = Matrix.CreateTranslation(new Vector3(
                    -this.transform2D.Rectangle.Width * this.transform2D.Origin.X,
                    -this.transform2D.Rectangle.Height * this.transform2D.Origin.Y,
                    0));
            }

            Matrix worldTransform = this.originTranslation * this.transform2D.WorldTransform;
            float drawOrder = this.transform2D.DrawOrder;

            float opacity = this.Transform2D.GlobalOpacity;
            if (this.RenderManager.ShouldDrawFlag(Framework.Managers.DebugLinesFlags.DebugAlphaOpacity))
            {
                opacity *= DebugAlpha;
            }

            for (int i = 0; i < this.meshes.Count; i++)
            {
                var mesh = this.meshes[i];

                if (this.CullingTest(mesh, ref worldTransform))
                {
                    var material = this.materials[mesh.MaterialIndex];

                    material.Alpha = opacity;
                    material.LayerId = this.LayerId;
                    mesh.ZOrder = drawOrder;
                    this.RenderManager.DrawMesh(mesh, material, ref worldTransform);
                }
            }
        }
        #endregion

        #region Private Methods
        private bool CullingTest(Mesh mesh, ref Matrix worldTransform)
        {
            bool passesTest = true;
            var camera = this.RenderManager.CurrentDrawingCamera2D;

            if (camera != null &&
                mesh.BoundingBox.HasValue)
            {
                var bbox = mesh.BoundingBox.Value;

                bbox.Transform(ref worldTransform);

                // Checks if intersects with the frustum
                passesTest = camera.BoundingFrustum.Intersects(bbox);
            }

            return passesTest;
        }

        private void DrawDebugLine(Vector2 startPoint, Vector2 endPoint, ref Matrix worldTransform)
        {
            this.DrawDebugLine(ref startPoint, ref endPoint, ref worldTransform);
        }

        private void DrawDebugLine(ref Vector2 startPoint, ref Vector2 endPoint, ref Matrix worldTransform)
        {
            Color color = Color.Yellow;
            float drawOrder = this.transform2D.DrawOrder;
            var lineBatch = this.RenderManager.LineBatch2D;

            Vector2.Transform(ref startPoint, ref worldTransform, out startPoint);
            Vector2.Transform(ref endPoint, ref worldTransform, out endPoint);

            lineBatch.DrawLine(ref startPoint, ref endPoint, ref color, drawOrder);
        }

        /// <summary>
        /// Initializes the layer renderer
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.tiledMap = this.Owner.Parent.FindComponent<TiledMap>();
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

                var bufferCount = (int)Math.Ceiling((float)newNTiles / MaxTilesPerBuffer);

                this.vertices = new VertexPositionColorTexture[bufferCount][];
                this.vertexBuffer = new DynamicVertexBuffer[bufferCount];
                this.indexBuffer = new IndexBuffer[bufferCount];

                for (int j = 0; j < bufferCount; j++)
                {
                    int nBufferTiles = Math.Min(MaxTilesPerBuffer, this.nTiles - (j * MaxTilesPerBuffer));

                    // Vertices
                    this.vertices[j] = new VertexPositionColorTexture[nBufferTiles * VerticesPerTile];
                    this.vertexBuffer[j] = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);

                    // Indices
                    ushort[] indices = new ushort[nBufferTiles * IndicesPerTile];
                    for (int i = 0; i < nBufferTiles; i++)
                    {
                        indices[i * 6] = (ushort)(i * 4);
                        indices[(i * 6) + 1] = (ushort)((i * 4) + 1);
                        indices[(i * 6) + 2] = (ushort)((i * 4) + 2);
                        indices[(i * 6) + 3] = (ushort)((i * 4) + 2);
                        indices[(i * 6) + 4] = (ushort)((i * 4) + 3);
                        indices[(i * 6) + 5] = (ushort)(i * 4);
                    }

                    this.indexBuffer[j] = new IndexBuffer(indices);
                }
            }

            int tileIndex = 0;
            int startIndex = 0;
            int currentIndexCount = 0;
            int bufferIndex = 0;
            var boundingBox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
            Tileset currentTileset = null;

            for (int i = 0; i < this.tiledMap.Height; i++)
            {
                for (int j = 0; j < this.tiledMap.Width; j++)
                {
                    int x, y;
                    this.GetCellRenderOrderByIndex(j, i, out x, out y);

                    LayerTile tile = this.tiledMapLayer.GetLayerTileByMapCoordinates(x, y);

                    if (tile == null || tile.Id < 0 || tile.Tileset == null || tile.Tileset.Image == null)
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
                        this.NewMesh(startIndex, currentIndexCount, bufferIndex, currentTileset.Image, ref boundingBox);

                        startIndex = tileIndex;
                        currentIndexCount = 0;
                        currentTileset = tileset;
                        boundingBox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
                    }

                    this.FillTile(currentTileset, tile, bufferIndex, tileIndex, ref boundingBox);

                    tileIndex++;
                    currentIndexCount++;

                    if (tileIndex == MaxTilesPerBuffer)
                    {
                        this.vertexBuffer[bufferIndex].SetData(this.vertices[bufferIndex]);
                        this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer[bufferIndex]);

                        this.NewMesh(startIndex, currentIndexCount, bufferIndex, currentTileset.Image, ref boundingBox);

                        tileIndex = 0;
                        startIndex = 0;
                        currentIndexCount = 0;
                        currentTileset = null;
                        boundingBox = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
                        bufferIndex++;
                    }
                }
            }

            if (currentTileset != null)
            {
                this.vertexBuffer[bufferIndex].SetData(this.vertices[bufferIndex]);
                this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer[bufferIndex]);

                this.NewMesh(startIndex, currentIndexCount, bufferIndex, currentTileset.Image, ref boundingBox);
            }

            this.transform2D.Rectangle = this.tiledMap.CalcRectangle();
        }

        /// <summary>
        /// Get cell render order by the tile index
        /// </summary>
        /// <param name="i">The tile x coordinate</param>
        /// <param name="j">The tile y coordinate</param>
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
        /// <param name="bufferIndex">The buffer index</param>
        /// <param name="tileIndex">Current tileId</param>
        /// <param name="boundingBox">Mesh bounding box</param>
        private void FillTile(Tileset tileset, LayerTile tile, int bufferIndex, int tileIndex, ref BoundingBox boundingBox)
        {
            int textureWidth = tileset.Image.Width;
            int textureHeight = tileset.Image.Height;

            var rect = TiledMapUtils.GetRectangleTileByID(tileset, tile.Id);

            RectangleF tileRectangle = new RectangleF(
                rect.X / (float)textureWidth,
                rect.Y / (float)textureHeight,
                rect.Width / (float)textureWidth,
                rect.Height / (float)textureHeight);

            int vertexId = tileIndex * VerticesPerTile;

            Vector2 position;
            this.tiledMap.GetTilePosition(tile.X, tile.Y, tileset, out position);
            tile.LocalPosition = position;

            var position0 = new Vector3(position.X, position.Y, 0);
            var position1 = new Vector3(position.X + tileset.TileWidth, position.Y, 0);
            var position2 = new Vector3(position.X + tileset.TileWidth, position.Y + tileset.TileHeight, 0);
            var position3 = new Vector3(position.X, position.Y + tileset.TileHeight, 0);

            var textCoord0 = new Vector2(tileRectangle.X, tileRectangle.Y);
            var textCoord1 = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y);
            var textCoord2 = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y + tileRectangle.Height);
            var textCoord3 = new Vector2(tileRectangle.X, tileRectangle.Y + tileRectangle.Height);

            Vector3.Min(ref position0, ref boundingBox.Min, out boundingBox.Min);
            Vector3.Max(ref position2, ref boundingBox.Max, out boundingBox.Max);

            #region Flip calculation
            if (tile.HorizontalFlip)
            {
                var texCoordAux = textCoord0;
                textCoord0 = textCoord1;
                textCoord1 = texCoordAux;

                texCoordAux = textCoord2;
                textCoord2 = textCoord3;
                textCoord3 = texCoordAux;
            }

            if (tile.VerticalFlip)
            {
                var texCoordAux = textCoord0;
                textCoord0 = textCoord3;
                textCoord3 = texCoordAux;

                texCoordAux = textCoord2;
                textCoord2 = textCoord1;
                textCoord1 = texCoordAux;
            }

            if (tile.DiagonalFlip)
            {
                var texCoordAux = textCoord0;
                textCoord0 = textCoord2;
                textCoord2 = texCoordAux;
            }
            #endregion

            // Vertex 0
            this.vertices[bufferIndex][vertexId].Position = position0;
            this.vertices[bufferIndex][vertexId].Color = Color.White;
            this.vertices[bufferIndex][vertexId].TexCoord = textCoord0;
            vertexId++;

            // Vertex 1
            this.vertices[bufferIndex][vertexId].Position = position1;
            this.vertices[bufferIndex][vertexId].Color = Color.White;
            this.vertices[bufferIndex][vertexId].TexCoord = textCoord1;
            vertexId++;

            // Vertex 2
            this.vertices[bufferIndex][vertexId].Position = position2;
            this.vertices[bufferIndex][vertexId].Color = Color.White;
            this.vertices[bufferIndex][vertexId].TexCoord = textCoord2;
            vertexId++;

            // Vertex 3
            this.vertices[bufferIndex][vertexId].Position = position3;
            this.vertices[bufferIndex][vertexId].Color = Color.White;
            this.vertices[bufferIndex][vertexId].TexCoord = textCoord3;
            vertexId++;
        }

        /// <summary>
        /// Creates a new layer mesh
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        /// <param name="count">Count indices</param>
        /// <param name="bufferIndex">The buffer index</param>
        /// <param name="image">Mesh material</param>
        /// <param name="boundingBox">Mesh bounding box</param>
        private void NewMesh(int startIndex, int count, int bufferIndex, Texture2D image, ref BoundingBox boundingBox)
        {
            var mesh = new Mesh(
                0,
                this.vertices.Length,
                startIndex * IndicesPerTile,
                count * 2,
                this.vertexBuffer[bufferIndex],
                this.indexBuffer[bufferIndex],
                PrimitiveType.TriangleList)
            {
                DisableBatch = true,
                MaterialIndex = this.materials.Count,
                BoundingBox = boundingBox,
            };

            var material = new StandardMaterial(this.LayerId, image)
            {
                LightingEnabled = false
            };

            material.Initialize(this.Assets);

            this.materials.Add(material);
            this.meshes.Add(mesh);
        }

        /// <summary>
        /// Clear meshes
        /// </summary>
        private void RemoveBuffers()
        {
            this.vertices = null;
            if (this.vertexBuffer != null)
            {
                for (int i = 0; i < this.vertexBuffer.Length; i++)
                {
                    this.RenderManager.GraphicsDevice.DestroyVertexBuffer(this.vertexBuffer[i]);
                }
            }

            if (this.indexBuffer != null)
            {
                for (int i = 0; i < this.indexBuffer.Length; i++)
                {
                    this.RenderManager.GraphicsDevice.DestroyIndexBuffer(this.indexBuffer[i]);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Delete buffers
            this.RemoveBuffers();
        }
        #endregion
    }
}
