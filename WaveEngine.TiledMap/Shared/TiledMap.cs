#region File Description
//-----------------------------------------------------------------------------
// TiledMap
//
// Copyright © 2014 Wave Corporation
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
        /// Map orientation. Tiled supports "orthogonal", "isometric" and "staggered" (since 0.9.0) at the moment.
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
        /// The map width in tiles.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The map height in tiles.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The width of a tile.s
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

            this.tmxMap = new TmxMap(this.tmxPath);
            this.Version = this.tmxMap.Version;
            this.Orientation = (TiledMapOrientationType)((int)this.tmxMap.Orientation);
            this.RenderOrder = (TiledMapRenderOrderType)((int)this.tmxMap.RenderOrder);
            this.Width = this.tmxMap.Width;
            this.Height = this.tmxMap.Height;
            this.TileWidth = this.tmxMap.TileWidth;
            this.TileHeight = this.tmxMap.TileHeight;
            this.BackgroundColor = new Color(this.tmxMap.BackgroundColor.R, this.tmxMap.BackgroundColor.G, this.tmxMap.BackgroundColor.B);

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
        public void GetTileCoordinatesByPosition(Vector2 position, out int tileX, out int tileY)
        {
            position = Vector3.Transform(position.ToVector3(this.transform.DrawOrder), this.transform.WorldInverseTransform).ToVector2();
            position.X /= this.TileWidth;
            position.Y /= this.TileHeight;

            switch (this.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    tileX = (int)position.X;
                    tileY = (int)position.Y;
                    break;

                case TiledMapOrientationType.Isometric:
                    float halfHeight = this.Height * 0.5f;
                    tileX = (int)(-halfHeight + (position.X + position.Y));

                    float y = halfHeight + (-position.X + position.Y);
                    y = (y >= 0) ? y : y - 1;
                    tileY = (int)y;
                    break;

                case TiledMapOrientationType.Staggered:
                    int coordX = (int)(-0.5f + (position.X + position.Y));

                    y = 0.5f + (-position.X + position.Y);
                    y = (y >= 0) ? y : y - 1;
                    int coordY = (int)y;

                    tileX = (coordX - coordY) / 2;
                    tileY = coordX + coordY;
                    break;

                default:
                    tileX = 0;
                    tileY = 0;
                    break;
            }
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
                Opacity = (float)tmxLayer.Opacity
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
