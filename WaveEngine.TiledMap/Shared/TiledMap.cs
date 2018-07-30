// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TiledSharp;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
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
    [DataContract]
    public class TiledMap : Component
    {
        private const string DefaultAssetsPath = @"Content\Assets\";
        private const string TileImageTag = "TileImageLayer";
        private const string TileLayerTag = "TileLayer";

        /// <summary>
        /// The transform 2D
        /// </summary>
        [RequiredComponent]
        private Transform2D transform = null;

        /// <summary>
        /// The tmx file path
        /// </summary>
        [DataMember]
        private string tmxPath;

        /// <summary>
        /// The tmx stream
        /// </summary>
        private Stream tmxStream;

        /// <summary>
        /// The tmx stream assets path (used with tmxStream)
        /// </summary>
        private string tmxStreamAssetsPath;

        /// <summary>
        /// The tmx parsed instance
        /// </summary>
        internal TmxMap TmxMap;

        /// <summary>
        /// The list of tilesets
        /// </summary>
        private List<Tileset> tilesets;

        /// <summary>
        /// DrawOrder of the last layer
        /// </summary>
        [DataMember]
        private float minLayerDrawOrder;

        /// <summary>
        /// DrawOrder of the first layer
        /// </summary>
        [DataMember]
        private float maxLayerDrawOrder;

        /// <summary>
        /// Tiled map layers dictionary
        /// </summary>
        private Dictionary<string, TiledMapLayer> tileLayers;

        /// <summary>
        /// object layers dictionary
        /// </summary>
        private Dictionary<string, TiledMapObjectLayer> objectLayers;

        /// <summary>
        /// image layers dictionary
        /// </summary>
        private Dictionary<string, TiledMapImageLayer> imageLayers;

        #region Properties

        /// <summary>
        /// Gets or sets the Path of the Tiled Map TMX file
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Unknown, ".tmx")]
        public string TmxPath
        {
            get
            {
                return this.tmxPath;
            }

            set
            {
                this.tmxPath = value;
                this.tmxStream = null;
                this.tmxStreamAssetsPath = null;
                if (this.isInitialized)
                {
                    this.ReloadTmxFile();
                }
            }
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
        public IReadOnlyDictionary<string, TiledMapLayer> TileLayers
        {
            get
            {
                return this.tileLayers;
            }
        }

        /// <summary>
        /// Gets the object layer dictionary
        /// </summary>
        public IReadOnlyDictionary<string, TiledMapObjectLayer> ObjectLayers
        {
            get
            {
                return this.objectLayers;
            }
        }

        /// <summary>
        /// Gets the image layer dictionary
        /// </summary>
        public IReadOnlyDictionary<string, TiledMapImageLayer> ImageLayers
        {
            get
            {
                return this.imageLayers;
            }
        }

        /// <summary>
        /// Gets the map file properties.
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties
        {
            get
            {
                return this.TmxMap.Properties;
            }
        }

        /// <summary>
        /// Gets the TMX format version, generally 1.0.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets map orientation. Tiled supports "orthogonal", "isometric", "staggered" and "hexagonal" (since 0.11.0) at the moment.
        /// </summary>
        public TiledMapOrientationType Orientation { get; private set; }

        /// <summary>
        /// Gets the order in which tiles on tile layers are rendered.
        /// Valid values are right-down (the default), right-up, left-down and left-up.
        /// In all cases, the map is drawn row-by-row.
        /// (since 0.10, but only supported for orthogonal maps at the moment)
        /// </summary>
        public TiledMapRenderOrderType RenderOrder { get; private set; }

        /// <summary>
        /// Gets stagger Axis. For staggered maps, indicates if the tiles are ordered in X axis or Y axis.
        /// </summary>
        public TiledMapStaggerAxisType StaggerAxis { get; private set; }

        /// <summary>
        /// Gets stagger Index. For staggered maps, indicates if the tiles index is odd or even.
        /// </summary>
        public TiledMapStaggerIndexType StaggerIndex { get; private set; }

        /// <summary>
        /// Gets the tile side length for hexagonal maps.
        /// </summary>
        public int HexSideLength { get; private set; }

        /// <summary>
        /// Gets the map width in tiles.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the map height in tiles.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width of a tile.
        /// </summary>
        public int TileWidth { get; private set; }

        /// <summary>
        /// Gets the height of a tile.
        /// </summary>
        public int TileHeight { get; private set; }

        /// <summary>
        /// Gets the background color of the map. (since 0.9.0)
        /// </summary>
        public Color BackgroundColor { get; private set; }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        public TiledMap()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        /// <param name="tmxPath">TMX file path.</param>
        public TiledMap(string tmxPath)
        {
            if (string.IsNullOrEmpty(tmxPath))
            {
                throw new ArgumentNullException("tmxPath");
            }

            this.tmxPath = tmxPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMap" /> class.
        /// </summary>
        /// <param name="tmxStream">TMX stream.</param>
        /// <param name="tmxStreamAssetsPath">TMX stream assets path.</param>
        public TiledMap(Stream tmxStream, string tmxStreamAssetsPath = DefaultAssetsPath)
        {
            if (tmxStream == null)
            {
                throw new ArgumentNullException("tmxStream");
            }

            if (string.IsNullOrWhiteSpace(tmxStreamAssetsPath))
            {
                throw new ArgumentException("tmxStreamAssetsPath");
            }

            this.tmxStream = tmxStream;
            this.tmxStreamAssetsPath = tmxStreamAssetsPath;
        }

        /// <summary>
        /// Sets default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.minLayerDrawOrder = -100;
            this.maxLayerDrawOrder = 100;

            this.tilesets = new List<Tileset>();
            this.tileLayers = new Dictionary<string, TiledMapLayer>();
            this.objectLayers = new Dictionary<string, TiledMapObjectLayer>();
            this.imageLayers = new Dictionary<string, TiledMapImageLayer>();
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
        /// Obtains the tile position
        /// </summary>
        /// <param name="x">The X tile coord.</param>
        /// <param name="y">The Y tile coord.</param>
        /// <param name="tileset">The tileset.</param>
        /// <param name="position">The tile position</param>
        internal void GetTilePosition(int x, int y, Tileset tileset, out Vector2 position)
        {
            if (this.TmxMap == null)
            {
                position = Vector2.Zero;
                return;
            }

            switch (this.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    position = new Vector2(
                        x * this.TileWidth,
                        y * this.TileHeight);
                    break;

                case TiledMapOrientationType.Isometric:
                    position = new Vector2(
                        ((x - y) * this.TileWidth * 0.5f) + (this.Height * this.TileWidth * 0.5f) - (this.TileWidth * 0.5f),
                        (x + y) * this.TileHeight * 0.5f);
                    break;

                case TiledMapOrientationType.Staggered:
                case TiledMapOrientationType.Hexagonal:

                    int sideLengthX = 0;
                    int sideLengthY = 0;

                    float rowSize = 0;
                    float columSize = 0;
                    int staggerIndexSign = this.StaggerIndex == TiledMapStaggerIndexType.Odd ? 1 : -1;
                    var staggerAxisOffset = this.StaggerIndex == TiledMapStaggerIndexType.Even ? 0.5f : 0;

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

                    if (this.StaggerAxis == TiledMapStaggerAxisType.X)
                    {
                        rowSize = (x / 2) + ((x % 2) * 0.5f);
                        columSize = staggerAxisOffset + y + ((x % 2) * 0.5f * staggerIndexSign);
                    }
                    else
                    {
                        rowSize = staggerAxisOffset + x + ((y % 2) * 0.5f * staggerIndexSign);
                        columSize = y * 0.5f;
                    }

                    position = new Vector2(
                            rowSize * (this.TileWidth + sideLengthX),
                            columSize * (this.TileHeight + sideLengthY));
                    break;

                default:
                    position = Vector2.Zero;
                    break;
            }

            if (tileset != null)
            {
                position.X += tileset.XDrawingOffset;
                position.Y += tileset.YDrawingOffset + this.TileHeight - tileset.TileHeight;
            }
        }

        /// <summary>
        /// Calculate rectangle
        /// </summary>
        /// <returns>RectangleF</returns>
        internal RectangleF CalcRectangle()
        {
            RectangleF rectangle = new RectangleF();

            Vector2 position;
            this.GetTilePosition(this.TmxMap.Width, this.TmxMap.Height, null, out position);

            rectangle = new RectangleF(0, 0, position.X, position.Y);

            return rectangle;
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
                        position.X - (referencePosition.X * (this.TileWidth + sideLengthX)),
                        position.Y - (referencePosition.Y * (this.TileHeight + sideLengthY)));

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

                    var offsetsStaggerX = new Vector2[]
                    {
                        new Vector2(0,  0),
                        new Vector2(+1, -1),
                        new Vector2(+1,  0),
                        new Vector2(+2,  0),
                    };

                    var offsetsStaggerY = new Vector2[]
                    {
                        new Vector2(0,  0),
                        new Vector2(-1, +1),
                        new Vector2(0, +1),
                        new Vector2(0, +2),
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
        /// <returns>bool</returns>
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
        /// <returns>bool</returns>
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

            this.LoadTmxFile();
        }

        /// <summary>
        /// Reload TMX file
        /// </summary>
        private void ReloadTmxFile()
        {
            this.UnloadTmxFile();
            this.LoadTmxFile();
        }

        /// <summary>
        /// Unload TMX file
        /// </summary>
        private void UnloadTmxFile()
        {
            this.TmxMap = null;

            foreach (var tileset in this.tilesets)
            {
                tileset.Dispose();
            }

            this.tilesets.Clear();
            this.tileLayers.Clear();
            this.objectLayers.Clear();
            this.imageLayers.Clear();
        }

        /// <summary>
        /// Load TMX file
        /// </summary>
        private void LoadTmxFile()
        {
            if (string.IsNullOrEmpty(this.tmxPath) && this.tmxStream == null)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.tmxPath))
                {
                    this.TmxMap = new TmxMap(new WaveDocumentLoader(), this.tmxPath);
                }
                else
                {
                    this.TmxMap = new TmxMap(new WaveDocumentLoader(), this.tmxStream, this.tmxStreamAssetsPath);
                }

                this.Version = this.TmxMap.Version;

                this.Orientation = (TiledMapOrientationType)((int)this.TmxMap.Orientation);
                this.RenderOrder = (TiledMapRenderOrderType)((int)this.TmxMap.RenderOrder);
                this.StaggerAxis = (TiledMapStaggerAxisType)((int)this.TmxMap.StaggerAxis);
                this.StaggerIndex = (TiledMapStaggerIndexType)((int)this.TmxMap.StaggerIndex);
                this.Width = this.TmxMap.Width;
                this.Height = this.TmxMap.Height;
                this.TileWidth = this.TmxMap.TileWidth;
                this.TileHeight = this.TmxMap.TileHeight;
                this.BackgroundColor = new Color(this.TmxMap.BackgroundColor.R, this.TmxMap.BackgroundColor.G, this.TmxMap.BackgroundColor.B, this.TmxMap.BackgroundColor.A);

                if (this.TmxMap.HexSideLength.HasValue)
                {
                    this.HexSideLength = this.TmxMap.HexSideLength.Value;
                }

                this.CreateTilesets();
                this.CreateObjectLayers();

                this.transform.Rectangle = this.CalcRectangle();
                this.CreateImageLayers();
                this.CreateTileLayers();

                this.UpdateLayerDrawOrders();
            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid TiledMap format: A problem occurred during parsing the TMX file.", ex);
            }
        }

        /// <summary>
        /// Create tilesets associated to this map.
        /// </summary>
        private void CreateTilesets()
        {
            // Load tilesets
            foreach (var tmxTileset in this.TmxMap.Tilesets)
            {
                var tileset = new Tileset(tmxTileset, this);
                this.tilesets.Add(tileset);
            }
        }

        /// <summary>
        /// Load object layers
        /// </summary>
        private void CreateObjectLayers()
        {
            foreach (var tmxObjectLayer in this.TmxMap.ObjectGroups)
            {
                var tileMapObjectLayer = new TiledMapObjectLayer(tmxObjectLayer);
                this.objectLayers.Add(tmxObjectLayer.Name, tileMapObjectLayer);
            }
        }

        /// <summary>
        /// Create tile layers
        /// </summary>
        private void CreateImageLayers()
        {
            var oldImageEntities = this.Owner.ChildEntities.Where(c => c.Tag.StartsWith(TileImageTag)).ToList();
            foreach (var entity in oldImageEntities)
            {
                this.Owner.DetachChild(entity.Name);
            }

            // Create layers
            for (int i = 0; i < this.TmxMap.ImageLayers.Count; i++)
            {
                var tmxImageLayer = this.TmxMap.ImageLayers[i];
                this.CreateChildTileImageLayer(tmxImageLayer, i, oldImageEntities);
            }

            foreach (var entity in oldImageEntities)
            {
                entity.Dispose();
            }
        }

        /// <summary>
        /// Create tile layers
        /// </summary>
        private void CreateTileLayers()
        {
            var oldTileEntities = this.Owner.ChildEntities.Where(c => c.Tag.StartsWith(TileLayerTag)).ToList();
            foreach (var entity in oldTileEntities)
            {
                this.Owner.DetachChild(entity.Name);
            }

            // Create layers
            for (int i = 0; i < this.TmxMap.Layers.Count; i++)
            {
                var tmxLayer = this.TmxMap.Layers[i];
                this.CreateChildTileLayer(tmxLayer, i, oldTileEntities);
            }

            foreach (var entity in oldTileEntities)
            {
                entity.Dispose();
            }
        }

        /// <summary>
        /// Create the tile image layer as a child entity
        /// </summary>
        /// <param name="tmxImageLayer">The tmx image layer.</param>
        /// <param name="layerIndex">The layer index</param>
        /// <param name="previousEntities">previousEntities</param>
        private void CreateChildTileImageLayer(TmxImageLayer tmxImageLayer, int layerIndex, IList<Entity> previousEntities)
        {
            var tmxLayerName = tmxImageLayer.Name;

            var tiledMapImageLayer = new TiledMapImageLayer(tmxImageLayer, this);

            Entity layerEntity = null;
            if (previousEntities != null)
            {
                layerEntity = previousEntities.FirstOrDefault(e => e.Tag.StartsWith(TileImageTag) && e.Name == tmxLayerName);
                previousEntities.Remove(layerEntity);
            }

            var tileLayerOffset = new Vector2((float)tmxImageLayer.OffsetX, (float)tmxImageLayer.OffsetY);

            if (layerEntity != null)
            {
                var tileMapTransform = layerEntity.FindComponent<Transform2D>();
                var sprite = layerEntity.FindComponent<Sprite>();

                if (tileMapTransform != null
                 && sprite != null)
                {
                    tileMapTransform.LocalPosition = tileLayerOffset;
                    sprite.TexturePath = tiledMapImageLayer.ImagePath;
                    layerEntity.Name = tmxLayerName;
                }
                else
                {
                    this.Owner.RemoveChild(layerEntity.Name);
                    layerEntity = null;
                }
            }

            if (layerEntity == null)
            {
                layerEntity = new Entity(tmxLayerName)
                    {
                        Tag = TileImageTag
                    }
                    .AddComponent(new Sprite(tiledMapImageLayer.ImagePath))
                    .AddComponent(new Transform2D()
                    {
                        LocalPosition = tileLayerOffset,
                        Origin = this.transform.Origin,
                        Opacity = (float)tmxImageLayer.Opacity
                    })
                    .AddComponent(new SpriteRenderer());
            }

            this.Owner.AddChild(layerEntity);
            this.imageLayers.Add(tmxLayerName, tiledMapImageLayer);
        }

        /// <summary>
        /// Create the tile layer as a child entity
        /// </summary>
        /// <param name="tmxLayer">The tmx layer.</param>
        /// <param name="layerIndex">The layer index</param>
        /// <param name="previousEntities">previousEntities</param>
        private void CreateChildTileLayer(TmxLayer tmxLayer, int layerIndex, IList<Entity> previousEntities)
        {
            var tmxLayerName = tmxLayer.Name;

            Entity layerEntity = null;
            TiledMapLayer tileMapLayer = null;
            if (previousEntities != null)
            {
                layerEntity = previousEntities.FirstOrDefault(e => e.Tag.StartsWith(TileLayerTag) && e.Name == tmxLayerName);
                previousEntities.Remove(layerEntity);
            }

            var tileLayerOffset = new Vector2((float)tmxLayer.OffsetX, (float)tmxLayer.OffsetY);

            if (layerEntity != null)
            {
                var tileMapTransform = layerEntity.FindComponent<Transform2D>();
                tileMapLayer = layerEntity.FindComponent<TiledMapLayer>();

                if (tileMapTransform != null
                 && tileMapLayer != null)
                {
                    tileMapTransform.LocalPosition = tileLayerOffset;
                    tileMapLayer.TmxLayerName = tmxLayerName;
                    layerEntity.Name = tmxLayerName;
                }
                else
                {
                    this.Owner.RemoveChild(layerEntity.Name);
                    layerEntity = null;
                }
            }

            if (layerEntity == null)
            {
                tileMapLayer = new TiledMapLayer()
                {
                    TmxLayerName = tmxLayerName
                };

                layerEntity = new Entity(tmxLayerName)
                {
                    Tag = TileLayerTag
                }
                    .AddComponent(tileMapLayer)
                    .AddComponent(new Transform2D()
                    {
                        LocalPosition = tileLayerOffset,
                        Origin = this.transform.Origin,
                        Opacity = (float)tmxLayer.Opacity
                    })
                    .AddComponent(new TiledMapLayerRenderer());
            }

            this.Owner.AddChild(layerEntity);
            this.tileLayers.Add(tmxLayerName, tileMapLayer);
        }

        /// <summary>
        /// Update draw order of the layers
        /// </summary>
        private void UpdateLayerDrawOrders()
        {
            float drawOrderStep;

            if (this.TmxMap == null)
            {
                return;
            }

            var tmxLayers = this.TmxMap.Layers
                                .Cast<ITmxLayer>()
                                .Concat(this.TmxMap.ImageLayers)
                                .OrderBy(l => l.OrderIndex)
                                .Cast<ITmxElement>();

            if (tmxLayers.Count() > 1)
            {
                drawOrderStep = (this.minLayerDrawOrder - this.maxLayerDrawOrder) / (tmxLayers.Count() - 1);
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
            foreach (var tmxLayer in tmxLayers)
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
