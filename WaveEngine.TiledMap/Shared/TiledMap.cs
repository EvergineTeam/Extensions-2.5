#region File Description
//-----------------------------------------------------------------------------
// TiledMap
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TiledSharp;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Load and store all information concerning Tiled Maps (.TMX) files. To create/edit .tmx files you need to use Tiled Map Editor (http://www.mapeditor.org/)
    /// </summary>
    public class TiledMap : Component
    {
        [RequiredComponent]
        private Transform2D transform = null;

        /// <summary>
        /// The tmx file path
        /// </summary>
        private string tmxPath;

        /// <summary>
        /// The tmx parsed instance
        /// </summary>
        private TmxMap tmxMap;

        /// <summary>
        /// The list of tilesets
        /// </summary>
        private List<Tileset> tilesets;

        /// <summary>
        /// The layer type
        /// </summary>
        private Type layerType;

        /// <summary>
        /// The sampler mode.
        /// </summary>
        private AddressMode samplerMode;

        /// <summary>
        /// DrawOrder of the last layer
        /// </summary>
        private float minLayerDrawOrder;

        /// <summary>
        /// DrawOrder of the first layer
        /// </summary>
        private float maxLayerDrawOrder;

        /// <summary>
        /// Tiled map layers dictionary
        /// </summary>
        private Dictionary<string, TiledMapLayer> tileLayers;

        /// <summary>
        /// object layers dictionary
        /// </summary>
        private Dictionary<string, TiledMapObjectLayer> objectLayers;

        #region Properties
        /// <summary>
        /// Gets the Path of the Tiled Map TMX file
        /// </summary>
        public string TmxPath
        {
            get { return this.tmxPath; }
        }

        /// <summary>
        /// Gets the tileset list
        /// </summary>
        public IEnumerable<Tileset> Tilesets
        {
            get { return this.tilesets; }
        }

        /// <summary>
        /// Gets or sets the draw order of the last layer.
        /// </summary>
        public float MinLayerDrawOrder
        {
            get
            {
                return this.minLayerDrawOrder;
            }

            set
            {
                this.minLayerDrawOrder = value;
                this.UpdateLayerDrawOrders();
            }
        }

        /// <summary>
        /// Gets or sets the draw order of the first layer.
        /// </summary>
        public float MaxLayerDrawOrder
        {
            get
            {
                return this.maxLayerDrawOrder;
            }

            set
            {
                this.maxLayerDrawOrder = value;
                this.UpdateLayerDrawOrders();
            }
        }

        /// <summary>
        /// Gets the tile layer dictionary
        /// </summary>
        public ReadOnlyDictionary<string, TiledMapLayer> TileLayers { get; private set; }

        /// <summary>
        /// Gets the object layer dictionary
        /// </summary>
        public ReadOnlyDictionary<string, TiledMapObjectLayer> ObjectLayers { get; private set; }

        /// <summary>
        /// Gets the TMX format version, generally 1.0.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Map orientation. Tiled supports "orthogonal", "isometric", "staggered" and "hexagonal" (since 0.11.0) at the moment.
        /// </summary>
        public TiledMapOrientationType Orientation { get; private set; }

        /// <summary>
        /// The order in which tiles on tile layers are rendered. 
        /// Valid values are right-down (the default), right-up, left-down and left-up. 
        /// In all cases, the map is drawn row-by-row. 
        /// (since 0.10, but only supported for orthogonal maps at the moment)
        /// </summary>
        public TiledMapRenderOrderType RenderOrder { get; private set; }

        /// <summary>
        /// Stagger Axis. For staggered maps, indicates if the tiles are ordered in X axis or Y axis.
        /// </summary>
        public TiledMapStaggerAxisType StaggerAxis { get; private set; }

        /// <summary>
        /// Stagger Index. For staggered maps, indicates if the tiles index is odd or even.
        /// </summary>
        public TiledMapStaggerIndexType StaggerIndex { get; private set; }

        /// <summary>
        /// The tile side length for hexagonal maps.
        /// </summary>
        public int HexSideLength { get; private set; }

        /// <summary>
        /// The map width in tiles.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The map height in tiles.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The width of a tile.
        /// </summary>
        public int TileWidth { get; private set; }

        /// <summary>
        /// The height of a tile.
        /// </summary>
        public int TileHeight { get; private set; }

        /// <summary>
        /// The background color of the map. (since 0.9.0)
        /// </summary>
        public Color BackgroundColor { get; private set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        /// <param name="tmxPath">TMX file path.</param>
        public TiledMap(string tmxPath)
            : this(tmxPath, DefaultLayers.Alpha, AddressMode.PointClamp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        /// <param name="tmxPath">TMX file path.</param>
        /// <param name="layerType">Render layer associated to this Tiled Map</param>
        public TiledMap(string tmxPath, Type layerType)
            : this(tmxPath, layerType, AddressMode.PointClamp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        /// <param name="tmxPath">TMX file path.</param>
        /// <param name="layerType">Render layer associated to this Tiled Map</param>
        /// <param name="samplerMode">The sampler mode.</param>
        public TiledMap(string tmxPath, Type layerType, AddressMode samplerMode)
        {
            if (string.IsNullOrEmpty(tmxPath))
            {
                throw new ArgumentNullException("tmxPath");
            }

            this.tmxPath = tmxPath;
            this.tilesets = new List<Tileset>();
            this.layerType = layerType;
            this.samplerMode = samplerMode;
            this.minLayerDrawOrder = -100;
            this.maxLayerDrawOrder = 100;

            try
            {
                this.tmxMap = new TmxMap(new WaveDocumentLoader(), this.tmxPath);
                this.Version = this.tmxMap.Version;
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid TiledMap format: A problem occurred during parsing the TMX file.", ex);
            }

            this.Orientation = (TiledMapOrientationType)((int)this.tmxMap.Orientation);
            this.RenderOrder = (TiledMapRenderOrderType)((int)this.tmxMap.RenderOrder);
            this.StaggerAxis = (TiledMapStaggerAxisType)((int)this.tmxMap.StaggerAxis);
            this.StaggerIndex = (TiledMapStaggerIndexType)((int)this.tmxMap.StaggerIndex);
            this.Width = this.tmxMap.Width;
            this.Height = this.tmxMap.Height;
            this.TileWidth = this.tmxMap.TileWidth;
            this.TileHeight = this.tmxMap.TileHeight;
            this.BackgroundColor = new Color(this.tmxMap.BackgroundColor.R, this.tmxMap.BackgroundColor.G, this.tmxMap.BackgroundColor.B);

            if (this.tmxMap.HexSideLength.HasValue)
            {
                this.HexSideLength = this.tmxMap.HexSideLength.Value;
            }

            this.tileLayers = new Dictionary<string, TiledMapLayer>();
            this.TileLayers = new ReadOnlyDictionary<string, TiledMapLayer>(this.tileLayers);

            this.objectLayers = new Dictionary<string, TiledMapObjectLayer>();
            this.ObjectLayers = new ReadOnlyDictionary<string, TiledMapObjectLayer>(this.objectLayers);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get a tileset by a Global ID
        /// </summary>
        /// <param name="gid">Th Global ID.</param>
        /// <returns>The associated tileset.</returns>
        public Tileset GetTilesetByGid(int gid)
        {
            if (gid <= 0)
            {
                return null;
            }

            for (int i = 0; i < this.tilesets.Count; i++)
            {
                var tileset = this.tilesets[i];
                if (gid >= tileset.FirstGid && gid <= tileset.LastGid)
                {
                    return tileset;
                }
            }

            return null;
        }

        /// <summary>
        /// Get Tile coordinates (x, y) by world position
        /// </summary>
        /// <param name="position">The world position</param>
        /// <param name="tileX">Out tile X coordinate</param>
        /// <param name="tileY">Out tile Y coordinate</param>
        public void GetTileCoordinatesByWorldPosition(Vector2 position, out int tileX, out int tileY)
        {
            int sideLengthX = 0;
            int sideLengthY = 0;
            bool staggerX = this.StaggerAxis == TiledMapStaggerAxisType.X;
            bool staggerEven = this.StaggerIndex == TiledMapStaggerIndexType.Even;

            if (this.Orientation == TiledMapOrientationType.Hexagonal)
            {
                if (this.StaggerAxis == TiledMapStaggerAxisType.X)
                {
                    sideLengthX = this.HexSideLength;
                }
                else
                {
                    sideLengthY = this.HexSideLength;
                }
            }

            position = Vector3.Transform(position.ToVector3(this.transform.DrawOrder), this.transform.WorldInverseTransform).ToVector2();

            Vector2 referencePosition = new Vector2(
                position.X / (this.TileWidth + sideLengthX),
                position.Y / (this.TileHeight + sideLengthY));

            if (referencePosition.X < 0)
            {
                referencePosition.X -= 1;
            }

            if (referencePosition.Y < 0)
            {
                referencePosition.Y -= 1;
            }

            switch (this.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    tileX = (int)referencePosition.X;
                    tileY = (int)referencePosition.Y;
                    break;

                case TiledMapOrientationType.Isometric:
                    float halfHeight = this.Height * 0.5f;
                    tileX = (int)(-halfHeight + (referencePosition.X + referencePosition.Y));

                    float y = halfHeight + (-referencePosition.X + referencePosition.Y);
                    y = (y >= 0) ? y : y - 1;
                    tileY = (int)y;
                    break;

                case TiledMapOrientationType.Staggered:

                    if (staggerEven)
                    {
                        if (staggerX)
                        {
                            referencePosition.Y -= 0.5f;
                        }
                        else
                        {
                            referencePosition.X -= 0.5f;
                        }
                    }

                    int coordX = (int)(-0.5f + (referencePosition.X + referencePosition.Y));

                    y = 0.5f + (-referencePosition.X + referencePosition.Y);
                    y = (y >= 0) ? y : y - 1;
                    int coordY = (int)y;

                    int evenOffset = staggerEven ? 1 : 0;

                    if (staggerX)
                    {
                        tileX = coordX - coordY;
                        tileY = (coordX + coordY + evenOffset) / 2;
                    }
                    else
                    {
                        tileX = (coordX - coordY + evenOffset) / 2;
                        tileY = coordX + coordY;
                    }
                    break;

                case TiledMapOrientationType.Hexagonal:
                    Vector2 tileCoordinates = Vector2.Zero;
                    var sideOffsetX = (this.TileWidth - sideLengthX) / 2;
                    var sideOffsetY = (this.TileHeight - sideLengthY) / 2;

                    if (staggerX)
                    {
                        position.X -= staggerEven ? this.TileWidth : sideOffsetX;
                    }
                    else
                    {
                        position.Y -= staggerEven ? this.TileHeight : sideOffsetY;
                    }

                    referencePosition = new Vector2(
                        (float)Math.Floor(position.X / (this.TileWidth + sideLengthX)),
                        (float)Math.Floor(position.Y / (this.TileHeight + sideLengthY)));

                    // Relative x and y position on the base square of the grid-aligned tile
                    Vector2 rel = new Vector2(
                        position.X - referencePosition.X * (this.TileWidth + sideLengthX),
                        position.Y - referencePosition.Y * (this.TileHeight + sideLengthY));

                    // Adjust the reference point to the correct tile coordinates
                    // Determine the nearest hexagon tile by the distance to the center
                    Vector2[] centers = new Vector2[4];
                    var columnWidth = sideOffsetX + sideLengthX;
                    var rowHeight = sideOffsetY + sideLengthY;

                    if (staggerX)
                    {
                        int left = sideLengthX / 2;
                        int centerX = left + columnWidth;
                        int centerY = this.TileHeight / 2;

                        centers[0] = new Vector2(left, centerY);
                        centers[1] = new Vector2(centerX, centerY - rowHeight);
                        centers[2] = new Vector2(centerX, centerY + rowHeight);
                        centers[3] = new Vector2(centerX + columnWidth, centerY);

                        referencePosition.X *= 2;

                        if (staggerEven)
                        {
                            referencePosition.X += 1;
                        }
                    }
                    else
                    {
                        int top = sideLengthY / 2;
                        int centerX = this.TileWidth / 2;
                        int centerY = top + rowHeight;

                        centers[0] = new Vector2(centerX, top);
                        centers[1] = new Vector2(centerX - columnWidth, centerY);
                        centers[2] = new Vector2(centerX + columnWidth, centerY);
                        centers[3] = new Vector2(centerX, centerY + rowHeight);

                        referencePosition.Y *= 2;

                        if (staggerEven)
                        {
                            referencePosition.Y += 1;
                        }
                    }

                    int nearest = 0;
                    float minDist = float.MaxValue;

                    for (int i = 0; i < 4; ++i)
                    {
                        Vector2 center = centers[i];
                        float dc = (center - rel).LengthSquared();

                        if (dc < minDist)
                        {
                            minDist = dc;
                            nearest = i;
                        }
                    }

                    var offsetsStaggerX = new Vector2[]{
                        new Vector2( 0,  0),
                        new Vector2(+1, -1),
                        new Vector2(+1,  0),
                        new Vector2(+2,  0),
                    };

                    var offsetsStaggerY = new Vector2[]{
                        new Vector2( 0,  0),
                        new Vector2(-1, +1),
                        new Vector2( 0, +1),
                        new Vector2( 0, +2),
                    };

                    var offsets = staggerX ? offsetsStaggerX : offsetsStaggerY;
                    tileCoordinates = referencePosition + offsets[nearest];

                    tileX = (int)tileCoordinates.X;
                    tileY = (int)tileCoordinates.Y;
                    break;

                default:
                    tileX = 0;
                    tileY = 0;
                    break;
            }
        }

        /// <summary>
        /// Does the stagger x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        internal bool DoStaggerX(int x)
        {
            var isStaggerX = this.StaggerAxis == TiledMapStaggerAxisType.X;
            var staggerEven = this.StaggerIndex == TiledMapStaggerIndexType.Even ? 1 : 0;

            return isStaggerX && ((x & 1) ^ staggerEven) == 1;
        }

        /// <summary>
        /// Does the stagger y.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        internal bool DoStaggerY(int y)
        {
            var isStaggerX = this.StaggerAxis == TiledMapStaggerAxisType.X;
            var staggerEven = this.StaggerIndex == TiledMapStaggerIndexType.Even ? 1 : 0;

            return !isStaggerX && ((y & 1) ^ staggerEven) == 1;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes Tiled Map
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.CreateTilesets();

            this.CreateObjectLayers();
            this.CreateTileLayers();
        }

        /// <summary>
        /// Create tilesets associated to this map.
        /// </summary>
        private void CreateTilesets()
        {
            // Load tilesets
            foreach (var tmxTileset in this.tmxMap.Tilesets)
            {
                this.tilesets.Add(new Tileset(tmxTileset, this));
            }
        }

        /// <summary>
        /// Load object layers
        /// </summary>
        private void CreateObjectLayers()
        {
            foreach (var tmxObjectLayer in this.tmxMap.ObjectGroups)
            {
                var tileMapObjectLayer = new TiledMapObjectLayer(tmxObjectLayer);
                this.objectLayers.Add(tmxObjectLayer.Name, tileMapObjectLayer);
            }
        }

        /// <summary>
        /// Create tile layers
        /// </summary>
        private void CreateTileLayers()
        {
            // Create layers
            foreach (var tmxLayer in this.tmxMap.Layers)
            {
                this.CreateChildTileLayer(tmxLayer);
            }

            this.UpdateLayerDrawOrders();
        }

        /// <summary>
        /// Create the tile layer as a child entity
        /// </summary>
        /// <param name="tmxLayer">the tmx parsed layer.</param>
        private void CreateChildTileLayer(TmxLayer tmxLayer)
        {
            var tileMapLayer = new TiledMapLayer(tmxLayer);
            this.tileLayers.Add(tmxLayer.Name, tileMapLayer);

            Entity layerEntity = new Entity(tmxLayer.Name)
            {
                IsVisible = tmxLayer.Visible
            }
            .AddComponent(new Transform2D()
            {
                Opacity = (float)tmxLayer.Opacity,
                Origin = this.transform.Origin
            })
            .AddComponent(tileMapLayer)
            .AddComponent(new TiledMapLayerRenderer(this.layerType, this.samplerMode));

            this.Owner.AddChild(layerEntity);
        }

        /// <summary>
        /// Update draw order of the layers
        /// </summary>
        private void UpdateLayerDrawOrders()
        {
            float drawOrderStep;
            if (this.tmxMap.Layers.Count > 1)
            {
                drawOrderStep = (this.minLayerDrawOrder - this.maxLayerDrawOrder) / (this.tmxMap.Layers.Count - 1);
            }
            else
            {
                drawOrderStep = 0;
            }

            if (this.Owner == null)
            {
                return;
            }

            int i = 0;
            foreach (var tmxLayer in this.tmxMap.Layers)
            {
                Entity childLayer = this.Owner.FindChild(tmxLayer.Name);
                Transform2D transform = childLayer.FindComponent<Transform2D>();

                transform.LocalDrawOrder = this.maxLayerDrawOrder + (drawOrderStep * i);
                i++;
            }
        }
        #endregion
    }
}
