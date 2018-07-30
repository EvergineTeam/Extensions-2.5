// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#if WINDOWS || ANDROID || __IOS__ || MAC || LINUX
#define OPENGL
#endif

#region Using Statements
using Noesis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

#if ANDROID || __IOS__
using OpenTK.Graphics.ES20;
#elif OPENGL
using OpenTK.Graphics.OpenGL;
#endif

#if ANDROID
using Android.App;
#endif
#endregion

namespace WaveEngine.NoesisGUI
{
    /// <summary>
    /// NoesisGUI integration panel
    /// </summary>
    [DataContract]
    public partial class NoesisPanel : Behavior, IDisposable
    {
        [RequiredService]
        private Input WaveInput = null;

        [RequiredService]
        private GraphicsDevice graphicsDevice = null;

        [RequiredService]
        private NoesisService noesisService = null;

        #region Events

        /// <summary>
        /// Event raised to obtain external input
        /// </summary>
        public event EventHandler<Noesis.View> OnExternalInput;

        /// <summary>
        /// Event raised when the Noesis view is created
        /// </summary>
        public event EventHandler<Noesis.View> OnViewCreated;
        #endregion

        #region Attributes
        private TimeSpan totalTime;
        private bool disposed;
        private bool disposing;

        // Property attributes
        private string xaml;
        private Common.Graphics.Color backgroundColor;
        private int width;
        private int height;
        private View.AntialiasingMode antiAliasingMode;
        private View.TessellationQuality tessellationQuality;
        private ClearFlags clearFlags;
        private bool flipY;
        private bool enablePostProcess;
        private View.RenderFlags renderFlags;

        private bool viewSettingsDirty;

        // Wave attributes
        private Camera camera;
        private Transform3D transform;
        private RenderTarget renderTarget;
        private Viewport screenViewport;

        // NoesisGUI attributes
        private View view;
        private Renderer renderer;

        // Cached values
        private int lastMouseX;
        private int lastMouseY;
        private ButtonState mouseLeftButton;
        private ButtonState mouseRightButton;
        private ButtonState mouseMiddleButton;
        private Array definedKeys;
        private KeyboardState lastKeyboardState;
        private string lastStyle;
        private bool usingOpenGL;

#if OPENGL
        private int maxTextureUnits = 0;
        private bool disableSamplers;
#endif

        #endregion

        #region Properties

        /// <summary>
        /// Gets the root of the loaded Xaml.
        /// </summary>
        public FrameworkElement Content
        {
            get
            {
                return this.view?.Content;
            }
        }

        /// <summary>
        /// Gets or sets the path to the XAML that will be loaded when this component is enabled.
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(
            AssetType.Unknown,
            ".xaml",
            CustomPropertyName = "XAML",
            Tooltip = "The XAML file that will be rendered on the panel.")]
        public string Xaml
        {
            get
            {
                return this.xaml;
            }

            set
            {
                this.xaml = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Background Color",
            Tooltip = "The background color for the transparent areas in the XAML design.")]
        public Common.Graphics.Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                this.backgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the render width.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Width",
            Tooltip = "The panel render width.")]
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the render height.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Height",
            Tooltip = "The panel render height.")]
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the antialiasing mode.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Antialiasing Mode",
            Tooltip = "The antialiasing mode that the panel will use when rendering vector graphics.")]
        public View.AntialiasingMode AntiAliasingMode
        {
            get
            {
                return this.antiAliasingMode;
            }

            set
            {
                this.antiAliasingMode = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the tesselation quality, which determines the quantity of triangles generated for vector shapes.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Tessellation Quality",
            Tooltip = "The tessellation quality that the panel will apply when rendering vector graphics.")]
        public View.TessellationQuality TessellationQuality
        {
            get
            {
                return this.tessellationQuality;
            }

            set
            {
                this.tessellationQuality = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the clear flags used for cleaning the frameBuffer in rendertarget mode.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Clear Flags",
            Tooltip = "The clear flags for the panel.")]
        public ClearFlags ClearFlags
        {
            get
            {
                return this.clearFlags;
            }

            set
            {
                this.clearFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to flip the Y axis in the UI.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Flip Y",
            Tooltip = "Indicates whether to flip the Y axis in the UI.")]
        public bool FlipY
        {
            get
            {
                return this.flipY;
            }

            set
            {
                this.flipY = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UI is affected by image post-processing.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Enable Post-Process",
            Tooltip = "Indicates whether post-processing effects affect this panel.")]
        public bool EnablePostProcess
        {
            get
            {
                return this.enablePostProcess;
            }

            set
            {
                this.enablePostProcess = value;

                if (this.camera != null)
                {
                    if (this.enablePostProcess)
                    {
                        this.camera.OnPostRender -= this.Noesis_OnCameraPostRender;
                        this.camera.OnPreImageEffects += this.Noesis_OnCameraPostRender;
                    }
                    else
                    {
                        this.camera.OnPreImageEffects -= this.Noesis_OnCameraPostRender;
                        this.camera.OnPostRender += this.Noesis_OnCameraPostRender;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard input management is enabled.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Enable Keyboard",
            Tooltip = "Indicates whether the panel will receive keyboard events.")]
        public bool EnableKeyboard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mouse input management is enabled.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Enable Mouse",
            Tooltip = "Indicates whether the panel will receive mouse events.")]
        public bool EnableMouse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether touch input management is enabled.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Enable Touch",
            Tooltip = "Indicates whether the panel will receive touch events.")]
        public bool EnableTouch { get; set; }

        /// <summary>
        /// Gets or sets the flags used for rendering debug purposes.
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Render Flags",
            Tooltip = "Flags used for rendering debug purposes.")]
        public View.RenderFlags RenderFlags
        {
            get
            {
                return this.renderFlags;
            }

            set
            {
                this.renderFlags = value;

                this.viewSettingsDirty = true;
            }
        }

        /// <summary>
        /// Gets the texture, if render to texture is enabled, null in other cases.
        /// </summary>
        private RenderTarget Texture
        {
            get
            {
                return this.renderTarget;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this component is rendering UI to a RenderTexture.
        /// </summary>
        private bool IsRenderToTexture
        {
            get
            {
                return this.camera == null;
            }
        }
        #endregion

        #region Component Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="NoesisPanel"/> class.
        /// </summary>
        public NoesisPanel()
            : base("NoesisPanel", FamilyType.PriorityBehavior)
        {
        }

        /// <summary>
        /// Initializes all default values of this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.definedKeys = Enum.GetValues(typeof(Keys));

            this.Width = 256;
            this.Height = 256;

            this.EnableMouse = true;
            this.EnableKeyboard = false;
            this.EnableTouch = false;
            this.enablePostProcess = false;
            this.antiAliasingMode = View.AntialiasingMode.MSAA;
            this.tessellationQuality = View.TessellationQuality.Medium;
            this.backgroundColor = Common.Graphics.Color.Black;
            this.clearFlags = ClearFlags.All;
            this.flipY = false;
            this.disposed = false;

            this.Family = FamilyType.PriorityBehavior;
        }

        /// <summary>
        /// Initializes all parameters.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.usingOpenGL = WaveServices.Platform.AdapterType != Common.AdapterType.DirectX;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            // Register Noesis service
            NoesisService svc = WaveServices.GetService<NoesisService>();
            if (svc == null)
            {
                WaveServices.RegisterService(new NoesisService());
            }

            base.ResolveDependencies();

            this.camera = this.Owner.FindComponent<Camera>(false);

#if OPENGL
            if (this.usingOpenGL)
            {
                GL.GetInteger(GetPName.MaxTextureImageUnits, out this.maxTextureUnits);
            }
#endif

#if ANDROID
            var activityManager = (ActivityManager)(Game.Current.Application as Activity).GetSystemService(Android.Content.Context.ActivityService);
            var version = activityManager.DeviceConfigurationInfo.GlEsVersion;

            this.disableSamplers = !version.StartsWith("2");
#elif OPENGL
            this.disableSamplers = true;
#endif
        }

        /// <summary>
        /// Delete all required dependencies.
        /// </summary>
        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();

            this.camera = null;
        }

        /// <summary>
        /// Update all methods required for this instance.
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.UpdateViewSettings();

            this.totalTime += gameTime;

            if (this.renderer != null)
            {
                this.UpdateInputs();

                try
                {
                    this.view.Update(this.totalTime.TotalSeconds);
                }
                catch (Exception e)
                {
                    // This usually happens when the view datacontext references undefined items or with some Noesis limitations
                    NoesisErrorConsole.PrintPanelError(this.Owner.Name, e);
                }
            }
        }

        /// <inheritdoc/>
        protected override void Removed()
        {
            base.Removed();

            Framework.Threading.WaveForegroundTask.Run(() =>
            {
                // When changing scenes in the editor, the panel doesn't render correctly.
                this.DestroyPanel();

                this.viewSettingsDirty = true;
            });
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the projected position of a mouse or touch input.
        /// </summary>
        /// <param name="screenX">Input x.</param>
        /// <param name="screenY">Input y.</param>
        /// <param name="projectedX">Projected x.</param>
        /// <param name="projectedY">Projected y.</param>
        /// <returns>Whether the pointer intersected the plane.</returns>
        public bool ProjectPointer(int screenX, int screenY, out int projectedX, out int projectedY)
        {
            if ((!this.IsRenderToTexture || this.transform == null) && this.RenderManager.ActiveCamera2D != null)
            {
                var vs = this.RenderManager.ActiveCamera2D.UsedVirtualScreen;
                projectedX = (int)((screenX * this.Width) / vs.ScreenWidth);
                projectedY = (int)((screenY * this.Height) / vs.ScreenHeight);
                return true;
            }
            else
            {
                Vector2 screenPosition = new Vector2(screenX, screenY);
                Ray ray;
                float? distance;
                this.RenderManager.ActiveCamera3D.CalculateRay(ref screenPosition, out ray);
                return this.ProjectRay(ray, out projectedX, out projectedY, out distance);
            }
        }

        /// <summary>
        /// Get the projected position of a 3D ray.
        /// </summary>
        /// <param name="ray">The ray to project.</param>
        /// <param name="projectedX">Projected x.</param>
        /// <param name="projectedY">Projected y.</param>
        /// <param name="distance">Distance of the projected point.</param>
        /// <returns>Whether the ray intersected the plane.</returns>
        public bool ProjectRay(Ray ray, out int projectedX, out int projectedY, out float? distance)
        {
            Matrix worldInverseTransform = this.transform.WorldInverseTransform;
            ray.Position = Common.Math.Vector3.Transform(ray.Position, worldInverseTransform);
            ray.Direction = Common.Math.Vector3.TransformNormal(ray.Direction, worldInverseTransform);

            projectedX = 0;
            projectedY = 0;
            distance = null;

            // Ignored negative rays
            if (ray.Position.Y > 0)
            {
                Plane plane = new Plane(Common.Math.Vector3.Up, 0);
                distance = ray.Intersects(plane);

                // if Intersection exists
                if (distance.HasValue)
                {
                    Common.Math.Vector3 target = ray.GetPoint(distance.Value);

                    if (target.X < 0.5f && target.X > -0.5f
                        && target.Z < 0.5f && target.Z > -0.5f)
                    {
                        Vector2 coord = new Vector2(target.X + 0.5f, target.Z + 0.5f);

                        // Scale coord to the size.
                        projectedX = (int)(coord.X * this.width);
                        projectedY = (int)(coord.Y * this.height);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Rebuild the panel in the next update cycle.
        /// </summary>
        public void RebuildPanel()
        {
            this.viewSettingsDirty = true;
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (!this.disposing)
                {
                    this.disposing = true;

                    Framework.Threading.WaveForegroundTask.Run(() =>
                    {
                        this.DestroyPanel();

                        this.disposed = true;
                        this.disposing = false;
                    });
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Update the view settings.
        /// </summary>
        private void UpdateViewSettings()
        {
            var style = this.noesisService.CurrentStyle;

            if (this.lastStyle != style)
            {
                this.lastStyle = style;
                this.viewSettingsDirty = true;
            }

            if (this.viewSettingsDirty)
            {
                // Recreate panel
                this.DestroyPanel();
                this.CreatePanel();
            }
        }

        /// <summary>
        /// Destroy the panel.
        /// </summary>
        private void DestroyPanel()
        {
            if (this.camera != null)
            {
                this.camera.OnPreRender -= this.Noesis_OnCameraPreRender;
                this.camera.OnPostRender -= this.Noesis_OnCameraPostRender;
                this.camera.OnPreImageEffects -= this.Noesis_OnCameraPreRender;
            }
            else
            {
                this.RenderManager.OnPreRender -= this.Noesis_RenderToTexture;
            }

            if (this.renderTarget != null)
            {
                this.graphicsDevice.RenderTargets.DestroyRenderTarget(this.renderTarget);
                this.renderTarget = null;
            }

            if (this.renderer != null)
            {
                this.renderer.Shutdown();
                this.renderer = null;
            }

            if (this.view != null)
            {
                this.Content.DataContext = null;
                this.view = null;
            }
        }

        /// <summary>
        /// Create the panel.
        /// </summary>
        private void CreatePanel()
        {
            if (!this.noesisService.StyleValid)
            {
                return;
            }

            this.viewSettingsDirty = false;

            this.totalTime = TimeSpan.Zero;

            try
            {
                if (!string.IsNullOrEmpty(this.Xaml))
                {
                    object root = Noesis.GUI.LoadXaml(this.Xaml);

                    if (root == null)
                    {
                        throw new Exception($"Unable to load XAML {this.Xaml}");
                    }

                    if (!(root is FrameworkElement))
                    {
                        throw new Exception($"{this.Xaml} is not a FrameworkElement");
                    }

                    this.view = Noesis.GUI.CreateView((FrameworkElement)root);

                    if (!this.IsRenderToTexture)
                    {
                        var vsm = this.Owner.Scene.VirtualScreenManager.VirtualScreenRectangle;

                        this.width = (int)vsm.Width;
                        this.height = (int)vsm.Height;
                    }

                    if (this.width <= 0)
                    {
                        throw new Exception("Panel width must be positive");
                    }

                    if (this.height <= 0)
                    {
                        throw new Exception("Panel height must be positive");
                    }

                    this.view.SetSize(this.width, this.height);
                    this.view.SetIsPPAAEnabled(this.antiAliasingMode == View.AntialiasingMode.PPAA);
                    this.view.SetTessellationQuality(this.tessellationQuality);

                    // In OpenGL platforms, enable FlipY when the component is rendering to texture
                    View.RenderFlags renderFlags = this.renderFlags;
                    bool doFlip = (WaveServices.Platform.AdapterType != AdapterType.DirectX && this.IsRenderToTexture) ^ this.flipY;
                    renderFlags |= doFlip ? (View.RenderFlags)View.HiddenRenderFlags.FlipY : 0;
                    this.view.SetFlags(renderFlags);

                    this.renderer = this.view.Renderer;

                    this.NoesisBegin(false);
                    this.renderer.Init(this.noesisService.RenderDevice);
                    this.NoesisEnd();

                    this.view.Update(0);

                    if (!this.IsRenderToTexture)
                    {
                        this.camera.OnPreRender += this.Noesis_OnCameraPreRender;
                        this.EnablePostProcess = this.enablePostProcess;
                    }
                    else
                    {
                        this.RenderManager.OnPreRender += this.Noesis_RenderToTexture;
                        this.transform = this.Owner.FindComponent<Transform3D>(false);
                        this.screenViewport = new Viewport(0, 0, this.width, this.height);
                        this.renderTarget = this.graphicsDevice.RenderTargets.CreateRenderTarget(this.width, this.height);

                        // Try to set the texture to the material.
                        this.SetPanelMaterialTexture(this.Texture);
                    }

                    this.OnViewCreated?.Invoke(this, this.view);
                }
            }
            catch (Exception e)
            {
                // This usually happens when loading a XAML with undefined resources or with an incorrect root type
                NoesisErrorConsole.PrintPanelError(this.Owner.Name, e);

                // Set default texture
                this.SetPanelMaterialTexture(StaticResources.DefaultTexture);
            }
        }

        private void SetPanelMaterialTexture(WaveEngine.Common.Graphics.Texture texture)
        {
            Material panelMaterial = null;

            var materialComponent = this.Owner.FindComponent<MaterialComponent>();
            if (materialComponent != null)
            {
                panelMaterial = materialComponent.Material;
            }
            else
            {
                // TODO: Remove MaterialsMap support, as it's deprecated.
                var materialsMap = this.Owner.FindComponent<MaterialsMap>();
                if (materialsMap != null)
                {
                    panelMaterial = materialsMap.DefaultMaterial;
                }
            }

            if (panelMaterial is Materials.StandardMaterial)
            {
                (panelMaterial as Materials.StandardMaterial).Diffuse1 = texture;
            }
            else if (panelMaterial is Materials.ForwardMaterial)
            {
                (panelMaterial as Materials.ForwardMaterial).Diffuse = texture;
            }
        }

        /// <summary>
        /// Update inputs events.
        /// </summary>
        private void UpdateInputs()
        {
            if (this.EnableMouse)
            {
                var mouse = this.WaveInput.MouseState;

                if (mouse.IsConnected)
                {
                    int mouseX = 0, mouseY = 0;
                    var projected = this.ProjectPointer(mouse.X, mouse.Y, out mouseX, out mouseY);

                    if (projected)
                    {
                        if (this.lastMouseX != mouseX || this.lastMouseY != mouseY)
                        {
                            this.view.MouseMove(mouseX, mouseY);
                        }

                        if (this.mouseLeftButton != mouse.LeftButton)
                        {
                            this.mouseLeftButton = mouse.LeftButton;
                            this.UpdateMouseEvent(mouseX, mouseY, this.mouseLeftButton, MouseButton.Left);
                        }

                        if (this.mouseRightButton != mouse.RightButton)
                        {
                            this.mouseRightButton = mouse.RightButton;
                            this.UpdateMouseEvent(mouseX, mouseY, this.mouseRightButton, MouseButton.Right);
                        }

                        if (this.mouseMiddleButton != mouse.MiddleButton)
                        {
                            this.mouseMiddleButton = mouse.MiddleButton;
                            this.UpdateMouseEvent(mouseX, mouseY, this.mouseMiddleButton, MouseButton.Middle);
                        }

                        if (mouse.Wheel != 0)
                        {
                            this.view.MouseWheel(mouseX, mouseY, mouse.Wheel);
                        }
                    }

                    this.lastMouseX = mouseX;
                    this.lastMouseY = mouseY;
                }
            }

            if (this.EnableTouch)
            {
                var touches = this.WaveInput.TouchPanelState;

                foreach (var touch in touches)
                {
                    int touchX = 0, touchY = 0;
                    var projected = this.ProjectPointer((int)touch.Position.X, (int)touch.Position.Y, out touchX, out touchY);

                    if (projected)
                    {
                        switch (touch.State)
                        {
                            case TouchLocationState.Pressed:
                                this.view.TouchDown(touchX, touchY, (uint)touch.Id);
                                break;
                            case TouchLocationState.Moved:
                                this.view.TouchMove(touchX, touchY, (uint)touch.Id);
                                break;
                            case TouchLocationState.Release:
                                this.view.TouchUp(touchX, touchY, (uint)touch.Id);
                                break;
                            case TouchLocationState.Invalid:
                            default:
                                break;
                        }
                    }
                }
            }

            if (this.EnableKeyboard)
            {
                var keyboard = this.WaveInput.KeyboardState;

                if (keyboard.IsConnected)
                {
                    foreach (Keys key in this.definedKeys)
                    {
                        bool current = keyboard.IsKeyPressed(key);
                        bool last = this.lastKeyboardState.IsKeyPressed(key);
                        if (current)
                        {
                            if (!last)
                            {
                                var keyPressed = NoesisKeyCodes.Convert(key);
                                if (keyPressed != Key.None)
                                {
                                    this.view.KeyDown(keyPressed);
                                }

                                uint c = keyboard.GetChar(key);
                                this.view.Char(c);
                            }
                        }
                        else
                        {
                            if (last)
                            {
                                var keyRelease = NoesisKeyCodes.Convert(key);
                                if (keyRelease != Key.None)
                                {
                                    this.view.KeyUp(keyRelease);
                                }
                            }
                        }
                    }
                }

                this.lastKeyboardState = keyboard;
            }

            this.OnExternalInput?.Invoke(this, this.view);
        }

        /// <summary>
        /// Determine if was a MouseDown or Mouseup event.
        /// </summary>
        /// <param name="mouseX">The mouse x position.</param>
        /// <param name="mouseY">The mouse y position.</param>
        /// <param name="state">The current button state.</param>
        /// <param name="noesisButton">The specific button.</param>
        private void UpdateMouseEvent(int mouseX, int mouseY, ButtonState state, MouseButton noesisButton)
        {
            if (state == ButtonState.Pressed)
            {
                this.view.MouseButtonDown(mouseX, mouseY, noesisButton);
            }
            else
            {
                this.view.MouseButtonUp(mouseX, mouseY, noesisButton);
            }
        }

        /// <summary>
        /// Run Noesis offscreen render on the camera pre-render event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The render args.</param>
        private void Noesis_OnCameraPreRender(object sender, RenderEventArgs e)
        {
            this.Noesis_OffscreenRender();
        }

        /// <summary>
        /// Run Noesis main render on the camera post-render event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The render args.</param>
        private void Noesis_OnCameraPostRender(object sender, RenderEventArgs e)
        {
            this.Noesis_Render();
        }

        /// <summary>
        /// Render the Noesis panel to a texture.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The render args.</param>
        private void Noesis_RenderToTexture(object sender, RenderEventArgs e)
        {
            if (this.Noesis_OffscreenRender())
            {
                var renderTarget = this.RenderManager.ActiveRenderTarget;
                var viewport = this.graphicsDevice.Viewport;

                this.graphicsDevice.RenderTargets.SetRenderTarget(this.renderTarget);
                this.graphicsDevice.Viewport = this.screenViewport;
                this.graphicsDevice.ApplyRenderState();
                this.graphicsDevice.Clear(ref this.backgroundColor, this.clearFlags, 1.0f);

                this.Noesis_Render();

                this.graphicsDevice.RenderTargets.SetRenderTarget(renderTarget);
                this.graphicsDevice.Viewport = viewport;
                this.graphicsDevice.ApplyRenderState();
            }
        }

        /// <summary>
        /// Run Noesis offscreen render.
        /// </summary>
        /// <returns>Whether the render tree was updated or the offscreen texture was rendered.</returns>
        private bool Noesis_OffscreenRender()
        {
            if (this.Owner.IsVisible && this.renderer != null)
            {
                bool updated = this.renderer.UpdateRenderTree();
                bool offscreen = this.renderer.NeedsOffscreen();

                if (offscreen)
                {
                    this.NoesisBegin(true);
                    this.renderer.RenderOffscreen();
                    this.NoesisEnd();
                    this.graphicsDevice.InvalidateState();
                }

                return updated || offscreen;
            }

            return false;
        }

        /// <summary>
        /// Run Noesis main render.
        /// </summary>
        private void Noesis_Render()
        {
            if (this.Owner.IsVisible && this.renderer != null)
            {
                this.NoesisBegin(true);
                this.renderer.Render();
                this.NoesisEnd();
                this.graphicsDevice.InvalidateState();
            }
        }
        #endregion

        #region Render state
        private void NoesisBegin(bool restore)
        {
#if OPENGL
            if (this.usingOpenGL)
            {
                this.GLStore(ref this.waveState);
                if (restore)
                {
                    this.GLRestore(ref this.noesisState);
                }
            }
#endif
        }

        private void NoesisEnd()
        {
#if OPENGL
            if (this.usingOpenGL)
            {
                this.GLStore(ref this.noesisState);
                this.GLRestore(ref this.waveState);
            }
#endif
        }

#if OPENGL
        private struct GLRenderState
        {
            public int unpackAlignment;
            public bool[] colorWriteMask;
            public bool blendEnabled;
            public bool stencilTestEnabled;
        }

        private GLRenderState waveState;
        private GLRenderState noesisState;

        private void GLStore(ref GLRenderState glState)
        {
            if (glState.colorWriteMask == null)
            {
                glState.colorWriteMask = new bool[4];
            }

            // Store OpenGL context
            GL.GetInteger(GetPName.UnpackAlignment, out glState.unpackAlignment);
            GL.GetBoolean(GetPName.ColorWritemask, glState.colorWriteMask);
            glState.blendEnabled = GL.IsEnabled(EnableCap.Blend);
            GL.GetBoolean(GetPName.StencilTest, out glState.stencilTestEnabled);
        }

        private void GLRestore(ref GLRenderState glState)
        {
#if ANDROID || __IOS__
            GL.Oes.BindVertexArray(0);
#elif !MAC
            GL.BindVertexArray(0);
#endif
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Unbind stuff
            if (this.disableSamplers)
            {
                for (int i = 0; i < this.maxTextureUnits; i++)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.BindTexture(TextureTarget.Texture2D, 0);

#if ANDROID || __IOS__
                    OpenTK.Graphics.ES30.GL.BindSampler(i, 0);
#elif !MAC
                    GL.BindSampler(i, 0);
#endif
                }
            }

            GL.UseProgram(0);

            // Restore OpenGL context
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, glState.unpackAlignment);
            GL.ColorMask(glState.colorWriteMask[0], glState.colorWriteMask[1], glState.colorWriteMask[2], glState.colorWriteMask[3]);

            if (glState.blendEnabled)
            {
                GL.Enable(EnableCap.Blend);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
            }

            if (glState.stencilTestEnabled)
            {
                GL.Enable(EnableCap.StencilTest);
            }
            else
            {
                GL.Disable(EnableCap.StencilTest);
            }
        }
#endif
        #endregion
    }
}
