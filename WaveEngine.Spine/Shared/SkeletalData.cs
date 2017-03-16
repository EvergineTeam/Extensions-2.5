#region File Description
//-----------------------------------------------------------------------------
// SkeletalData
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Spine;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Spine
{
    /// <summary>
    /// Hold all skeletal 2D info
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Spine")]
    public class SkeletalData : Component
    {
        /// <summary>
        ///     Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The texture loader
        /// </summary>
        private WaveTextureLoader textureLoader;

        /// <summary>
        /// The atlas path
        /// </summary>
        [DataMember]
        private string atlasPath;

        /// <summary>
        /// The atlas
        /// </summary>
        private Atlas atlas;

        /// <summary>
        /// The transform2D
        /// </summary>
        [RequiredComponent]
        public Transform2D Transform2D;

        /// <summary>
        /// A new atlas has been loaded
        /// </summary>
        internal event EventHandler OnAtlasRefresh;

        #region Properties
        /// <summary>
        /// Gets or sets the Path of Spine atlas file (.xml).
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Unknown, ".atlas")]
        public string AtlasPath
        {
            get
            {
                return this.atlasPath;
            }

            set
            {
                this.atlasPath = value;
                if (this.isInitialized)
                {
                    this.RefreshAtlas();
                }
            }
        }

        /// <summary>
        /// Gets the atlas.
        /// </summary>
        /// <value>
        /// The atlas.
        /// </value>
        public Atlas Atlas
        {
            get { return this.atlas; }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalData" /> class.
        /// </summary>
        public SkeletalData()
            : base("skeletalData" + instances++)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalData" /> class.
        /// </summary>
        /// <param name="atlasPath">The atlas path.</param>
        public SkeletalData(string atlasPath)
            : base("skeletalData" + instances++)
        {
            this.atlasPath = atlasPath;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            this.textureLoader = new WaveTextureLoader(this.Assets);
            this.RefreshAtlas();
        }

        /// <summary>
        /// Refresh the atlas
        /// </summary>
        private void RefreshAtlas()
        {
            if (this.atlas != null)
            {
                this.atlas.Dispose();
                this.atlas = null;
            }

            if (!string.IsNullOrEmpty(this.atlasPath))
            {
                try
                {
                    using (var fileStream = WaveServices.Storage.OpenContentFile(this.atlasPath))
                    {
                        using (var streamReader = new StreamReader(fileStream))
                        {
                            this.atlas = new Atlas(streamReader, Path.GetDirectoryName(this.atlasPath), new WaveTextureLoader(this.Assets));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("The atlas file is not valid: " + e.Message);
                    this.atlas = null;
                }
            }

            if (this.OnAtlasRefresh != null)
            {
                this.OnAtlasRefresh(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
