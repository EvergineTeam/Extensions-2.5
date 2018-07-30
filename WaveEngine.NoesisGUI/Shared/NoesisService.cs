// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using Noesis;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Threading;
using WaveEngine.NoesisGUI.Providers;
#endregion

namespace WaveEngine.NoesisGUI
{
    /// <summary>
    /// Noesis Service
    /// </summary>
    [DataContract]
    public partial class NoesisService : Service
    {
        #region Attributes
        private string style;
        private bool noesisInitialized;
        private ContentXamlProvider xamlProvider;
        private ContentTextureProvider textureProvider;
        private ContentFontProvider fontProvider;
        private RenderDevice renderDevice;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path to a resources XAML that defines the style of all Noesis panels.
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(
            AssetType.Unknown,
            ".xaml",
            CustomPropertyName = "Style",
            Tooltip = "The style XAML containing the resources that will be used with all Noesis panels.")]
        public string Style
        {
            get
            {
                return this.style;
            }

            set
            {
                this.style = value;

                // Prevent setting the style before the content providers are set
                if (this.IsInitialized)
                {
                    WaveForegroundTask.Run(() =>
                    {
                        this.ChangeStyle(value);
                    });
                }
            }
        }

        /// <summary>
        /// Gets or sets the current style
        /// </summary>
        internal string CurrentStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current style is loaded and valid
        /// </summary>
        internal bool StyleValid { get; set; }

        /// <summary>
        /// Gets the render device used to render Noesis
        /// </summary>
        internal RenderDevice RenderDevice
        {
            get
            {
                if (this.renderDevice == null)
                {
                    if (WaveServices.Platform.AdapterType == Common.AdapterType.DirectX)
                    {
                        if (WaveServices.GraphicsDevice.Graphics is IGraphicsPtr)
                        {
                            IntPtr devicePtr = ((IGraphicsPtr)WaveServices.GraphicsDevice.Graphics).NativePointer;
                            this.renderDevice = new RenderDeviceD3D11(devicePtr);
                        }
                        else
                        {
                            throw new InvalidOperationException("DirectX graphics needs to implement IGraphicsPtr interface.");
                        }
                    }
                    else
                    {
                        this.renderDevice = new RenderDeviceGL();
                    }
                }

                return this.renderDevice;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize NoesisService
        /// </summary>
        protected override void Initialize()
        {
            this.xamlProvider = new ContentXamlProvider();
            this.textureProvider = new ContentTextureProvider();
            this.fontProvider = new ContentFontProvider();

            this.ChangeStyle(this.style);

            base.Initialize();
        }

        /// <summary>
        /// Preload a texture so the provider can find them faster when needed
        /// </summary>
        /// <param name="filename">The file name of the texture to preload</param>
        /// <returns>Whether the texture was loaded (because it was not previously loaded)</returns>
        public bool PreloadTexture(string filename)
        {
            return this.textureProvider.PreloadTexture(filename);
        }

        /// <summary>
        /// Register a texture with a given identifier
        /// </summary>
        /// <param name="identifier">The texture identifier</param>
        /// <param name="texture">The texture</param>
        /// <returns>Whether the texture was registered (because it was not previously registered)</returns>
        public bool RegisterTexture(string identifier, WaveEngine.Common.Graphics.Texture texture)
        {
            return this.textureProvider.RegisterTexture(identifier, texture);
        }

        /// <inheritdoc/>
        protected override void Terminate()
        {
            base.Terminate();

            if (this.noesisInitialized)
            {
                this.noesisInitialized = false;

                WaveForegroundTask.Run(() =>
                {
                    Noesis.GUI.UnregisterNativeTypes();
                    this.renderDevice = null;
                });
            }
        }
        #endregion

        #region Private Methods
        private void ChangeStyle(string style)
        {
            try
            {
                if (!this.noesisInitialized)
                {
                    this.noesisInitialized = true;

                    Noesis.GUI.Init();

                    Noesis.GUI.SetXamlProvider(this.xamlProvider);
                    Noesis.GUI.SetTextureProvider(this.textureProvider);
                    Noesis.GUI.SetFontProvider(this.fontProvider);
                }

                object theme = null;

                if (!string.IsNullOrEmpty(style))
                {
                    theme = Noesis.GUI.LoadXaml(style) ?? throw new Exception($"Unable to load style XAML {style}");
                    theme = theme as ResourceDictionary ?? throw new Exception($"{style} is not a ResourceDictionary");
                }
                else
                {
                    this.style = null;
                }

                Noesis.GUI.SetApplicationResources((ResourceDictionary)theme);

                this.CurrentStyle = style;

                this.StyleValid = true;
            }
            catch (Exception e)
            {
                NoesisErrorConsole.PrintServiceError(e);
            }
        }
        #endregion
    }
}
