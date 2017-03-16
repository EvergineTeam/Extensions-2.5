#region File Description
//-----------------------------------------------------------------------------
// TiledMapLayerRenderer
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
        private List<Tuple<StandardMaterial, Mesh>> meshes;

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
        /// The cached origin
        /// </summary>
        private Vector2 cachedOrigin;

        /// <summary>
        /// A transform to apply the origin
        /// </summary>
        private Matrix originTranslation;

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
            this.samplerMode = samplerMode;
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.samplerMode = AddressMode.PointClamp;
            this.meshes = new List<Tuple<StandardMaterial, Mesh>>();
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

            Vector2 startPoint = Vector2.Zero;
            Vector2 endPoint = Vector2.Zero;

            int width = this.tiledMap.Width;
            int height = this.tiledMap.Height;
            int tileWidth = this.tiledMap.TileWidth;
            int tileHeight = this.tiledMap.TileHeight;

            RenderManager.LineBatch2D.DrawPoint(Vector2.Zero, 10, Color.Red, -10);

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

                        this.DrawDebugLine(ref startPoint, ref endPoint);
                    }

                    // Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        float y = i * tileHeight;
                        startPoint.X = 0;
                        startPoint.Y = y;
                        endPoint.X = width * tileWidth;
                        endPoint.Y = y;

                        this.DrawDebugLine(ref startPoint, ref endPoint);
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

                        this.DrawDebugLine(ref startPoint, ref endPoint);
                    }

                    // Horizontal lines
                    for (int i = 0; i <= height; i++)
                    {
                        startPoint.X = -(i * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        startPoint.Y = i * tileHeight * 0.5f;
                        endPoint.X = ((width - i) * tileWidth * 0.5f) + (height * tileWidth * 0.5f);
                        endPoint.Y = (width + i) * tileHeight * 0.5f;

                        this.DrawDebugLine(ref startPoint, ref endPoint);
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
                                rowPos.Y += rowHeight;

                            for (; /*rowPos.Y <= rect.bottom() && */rowTile.Y < height; rowTile.Y++)
                            {
                                this.DrawDebugLine(rowPos + oct[1], rowPos + oct[2]);
                                this.DrawDebugLine(rowPos + oct[2], rowPos + oct[3]);
                                this.DrawDebugLine(rowPos + oct[3], rowPos + oct[4]);

                                bool isStaggered = this.tiledMap.DoStaggerX((int)startTile.X);
                                bool lastRow = rowTile.Y == height - 1;
                                bool lastColumn = rowTile.X == width - 1;
                                bool bottomLeft = rowTile.X == 0 || (lastRow && isStaggered);
                                bool bottomRight = lastColumn || (lastRow && isStaggered);

                                if (bottomRight)
                                    this.DrawDebugLine(rowPos + oct[5], rowPos + oct[6]);
                                if (lastRow)
                                    this.DrawDebugLine(rowPos + oct[6], rowPos + oct[7]);
                                if (bottomLeft)
                                    this.DrawDebugLine(rowPos + oct[7], rowPos + oct[0]);

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
                                rowPos.X += columnWidth;

                            for (; /*rowPos.X <= rect.right() &&*/ rowTile.X < width; rowTile.X++)
                            {
                                this.DrawDebugLine(rowPos + oct[0], rowPos + oct[1]);
                                this.DrawDebugLine(rowPos + oct[1], rowPos + oct[2]);
                                this.DrawDebugLine(rowPos + oct[3], rowPos + oct[4]);

                                bool isStaggered = this.tiledMap.DoStaggerY((int)startTile.Y);
                                bool lastRow = rowTile.Y == height - 1;
                                bool lastColumn = rowTile.X == width - 1;
                                bool bottomLeft = lastRow || (rowTile.X == 0 && !isStaggered);
                                bool bottomRight = lastRow || (lastColumn && isStaggered);

                                if (lastColumn)
                                    this.DrawDebugLine(rowPos + oct[4], rowPos + oct[5]);
                                if (bottomRight)
                                    this.DrawDebugLine(rowPos + oct[5], rowPos + oct[6]);
                                if (bottomLeft)
                                    this.DrawDebugLine(rowPos + oct[7], rowPos + oct[0]);

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
                    0
                    ));
            }

            Matrix worldTransform = this.originTranslation * this.transform2D.WorldTransform;
            float drawOrder = this.transform2D.DrawOrder;
            float opacity = this.RenderManager.DebugLines ? DebugAlpha : this.transform2D.GlobalOpacity;

            for (int i = 0; i < this.meshes.Count; i++)
            {
                var tuple = this.meshes[i];
                var material = tuple.Item1;
                var mesh = tuple.Item2;
                
                material.Alpha = opacity;
                material.LayerType = this.LayerType;
                material.SamplerMode = this.samplerMode;
                mesh.ZOrder = drawOrder;
                this.RenderManager.DrawMesh(mesh, material, ref worldTransform);
            }
        }
        #endregion

        #region Private Methods
        private void DrawDebugLine(Vector2 startPoint, Vector2 endPoint)
        {
            this.DrawDebugLine(ref startPoint, ref endPoint);
        }

        private void DrawDebugLine(ref Vector2 startPoint, ref Vector2 endPoint)
        {
            Color color = Color.Yellow;
            float drawOrder = this.transform2D.DrawOrder;
            var lineBatch = this.RenderManager.LineBatch2D;
            Matrix worldTransform = this.transform2D.WorldTransform;

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

            this.transform2D.Rectangle = this.tiledMap.CalcRectangle();
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

            var rect = TiledMapUtils.GetRectangleTileByID(tileset, tile.Id);

            RectangleF tileRectangle = new RectangleF(
                rect.X / (float)textureWidth,
                rect.Y / (float)textureHeight,
                rect.Width / (float)textureWidth,
                rect.Height / (float)textureHeight);

            int vertexId = tileIndex * verticesPerTile;

            Vector2 position;
            this.tiledMap.GetTilePosition(tile.X, tile.Y, tileset, out position);
            tile.LocalPosition = position;

            var textCoord0 = new Vector2(tileRectangle.X, tileRectangle.Y);
            var textCoord1 = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y);
            var textCoord2 = new Vector2(tileRectangle.X + tileRectangle.Width, tileRectangle.Y + tileRectangle.Height);
            var textCoord3 = new Vector2(tileRectangle.X, tileRectangle.Y + tileRectangle.Height);

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
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = textCoord0;
            vertexId++;

            // Vertex 1
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = textCoord1;
            vertexId++;

            // Vertex 2
            this.vertices[vertexId].Position = new Vector3(position.X + tileset.TileWidth, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = textCoord2;
            vertexId++;

            // Vertex 3
            this.vertices[vertexId].Position = new Vector3(position.X, position.Y + tileset.TileHeight, 0);
            this.vertices[vertexId].Color = Color.White;
            this.vertices[vertexId].TexCoord = textCoord3;
            vertexId++;
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

            StandardMaterial material = new StandardMaterial(this.LayerType, image)
            {
                LightingEnabled = false,
                SamplerMode = this.samplerMode
            };

            material.Initialize(this.Assets);

            this.meshes.Add(new Tuple<StandardMaterial, Mesh>(material, mesh));
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
            // Delete buffers
            this.RemoveBuffers();
        }
        #endregion
    }
}
