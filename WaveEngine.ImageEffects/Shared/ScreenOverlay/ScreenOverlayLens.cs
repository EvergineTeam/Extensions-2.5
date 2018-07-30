// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a ScreenOverlayLens as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class ScreenOverlayLens : Lens
    {
        private string overlayTexturePath;

        #region Properties

        /// <summary>
        /// Gets or sets the ColorSpace, default value is ColorCorrectionMaterial.ColorSpaceType.Default;
        /// </summary>
        [DataMember]
        public ScreenOverlayMaterial.BlendMode Mode
        {
            get
            {
                return (this.material as ScreenOverlayMaterial).OverlayMode;
            }

            set
            {
                (this.material as ScreenOverlayMaterial).OverlayMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>
        /// The texture path.
        /// </value>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string TexturePath
        {
            get
            {
                return (this.material as ScreenOverlayMaterial).OverlayTexturePath;
            }

            set
            {
                (this.material as ScreenOverlayMaterial).OverlayTexturePath = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenOverlayLens"/> class.
        /// </summary>
        public ScreenOverlayLens()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenOverlayLens"/> class.
        /// </summary>
        /// <param name="overlayTexturePath">The overlay texture path.</param>
        public ScreenOverlayLens(string overlayTexturePath)
            : base()
        {
            this.overlayTexturePath = overlayTexturePath;
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new ScreenOverlayMaterial(this.overlayTexturePath);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as ScreenOverlayMaterial;
            mat.Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
        }
        #endregion

        #region Private Methods

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
