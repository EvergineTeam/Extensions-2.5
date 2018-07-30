// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Threading;
using WaveEngine.MixedReality.Utilities;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Perception.Spatial;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endregion

namespace WaveEngine.MixedReality.Interaction
{
    /// <summary>
    /// Handler all the input event.
    /// </summary>
    internal class SpatialInputManager : ISpatialInputManager
    {

        /// <summary>
        /// The MixedReality application
        /// </summary>
        private BaseMixedRealityApplication mixedRealityApplication;

        /// <summary>
        /// The spatial input service
        /// </summary>
        private SpatialInputService service;

        /// <summary>
        /// Instance of SpatialInteractionManager.
        /// </summary>
        private SpatialInteractionManager interactionManager;

        /// <summary>
        /// The gesture recognizer
        /// </summary>
        private SpatialGestureRecognizer gestureRecognizer;

        /// <summary>
        /// Intance of currenct SpatialState.
        /// </summary>
        private SpatialState currentSpatialState;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The generic controller array
        /// </summary>
        private VRGenericControllerState[] genericControllersArray = new VRGenericControllerState[2];

        /// <summary>
        /// Occurs when a new hand, controller, or source of voice commands has been detected.
        /// </summary>
        public event SpatialInputService.InputSourceDelegate SourceDetected;

        /// <summary>
        /// Occurs when a hand, controller, or source of voice commands is no longer available.
        /// </summary>
        public event SpatialInputService.InputSourceDelegate SourceLost;

        /// <summary>
        /// Occurs when a hand or controller has entered the pressed state.
        /// </summary>
        public event SpatialInputService.InputSourceDelegate SourcePressed;

        /// <summary>
        /// Occurs when a hand or controller has exited the pressed state.
        /// </summary>
        public event SpatialInputService.InputSourceDelegate SourceReleased;

        /// <summary>
        /// Occurs when a hand or controller has experienced a change to its SpatialInteractionSourceState.
        /// </summary>
        public event SpatialInputService.InputSourceDelegate SourceUpdated;

        /// <summary>
        /// Gesture error event
        /// </summary>
        public event SpatialInputService.GestureErrorDelegate GestureErrorEvent;

        /// <summary>
        /// The hold gesture is canceled
        /// </summary>
        public event SpatialInputService.HoldCanceledEventDelegate HoldCanceledEvent;

        /// <summary>
        /// The hold gesture is completed
        /// </summary>
        public event SpatialInputService.HoldCompletedEventDelegate HoldCompletedEvent;

        /// <summary>
        /// The hold gesture is started
        /// </summary>
        public event SpatialInputService.HoldStartedEventDelegate HoldStartedEvent;

        /// <summary>
        /// The manipulation gesture is canceled
        /// </summary>
        public event SpatialInputService.ManipulationCanceledEventDelegate ManipulationCanceledEvent;

        /// <summary>
        /// The manipulation gesture is completed
        /// </summary>
        public event SpatialInputService.ManipulationCompletedEventDelegate ManipulationCompletedEvent;

        /// <summary>
        /// The manipulation gesture is started
        /// </summary>
        public event SpatialInputService.ManipulationStartedEventDelegate ManipulationStartedEvent;

        /// <summary>
        /// The manipulation gesture is updated
        /// </summary>
        public event SpatialInputService.ManipulationUpdatedEventDelegate ManipulationUpdatedEvent;

        /// <summary>
        /// The navigation gesture is canceled
        /// </summary>
        public event SpatialInputService.NavigationCanceledEventDelegate NavigationCanceledEvent;

        /// <summary>
        /// The navigation gesture is completed
        /// </summary>
        public event SpatialInputService.NavigationCompletedEventDelegate NavigationCompletedEvent;

        /// <summary>
        /// The navigation gesture is started
        /// </summary>
        public event SpatialInputService.NavigationStartedEventDelegate NavigationStartedEvent;

        /// <summary>
        /// The navigation gesture is updated
        /// </summary>
        public event SpatialInputService.NavigationUpdatedEventDelegate NavigationUpdatedEvent;

        /// <summary>
        /// The recognition gesture is ended
        /// </summary>
        public event SpatialInputService.RecognitionEndedEventDelegate RecognitionEndedEvent;

        /// <summary>
        /// The recognition gesture is started
        /// </summary>
        public event SpatialInputService.RecognitionStartedEventDelegate RecognitionStartedEvent;

        /// <summary>
        /// Occurs when there is a tap gesture
        /// </summary>
        public event SpatialInputService.TappedEventDelegate TappedEvent;

        /// <summary>
        /// If the mixed reality controllers could be available
        /// </summary>
        private bool isMixedRealityControllerAvailable;

        #region Properties

        /// <summary>
        /// Gets or sets the enabled gestures
        /// </summary>
        public SpatialGestures EnabledGestures
        {
            get
            {
                return this.mixedRealityApplication != null ? (SpatialGestures)this.gestureRecognizer.GestureSettings : SpatialGestures.None;
            }

            set
            {
                if (this.mixedRealityApplication != null && value != this.EnabledGestures)
                {
                    this.RefreshGestureRecognizer(value);
                }
            }
        }

        /// <summary>
        /// Gets the coordinat system
        /// </summary>
        private SpatialCoordinateSystem CoordinateSystem
        {
            get
            {
                if (this.mixedRealityApplication?.ReferenceFrame != null)
                {
                    return this.mixedRealityApplication.ReferenceFrame.CoordinateSystem;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current Spatial State.
        /// </summary>
        public SpatialState SpatialState
        {
            get
            {
                return this.currentSpatialState;
            }
        }

        /// <summary>
        /// Gets the controller array
        /// </summary>
        public VRGenericControllerState[] GenericControllersArray
        {
            get
            {
                return this.genericControllersArray;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialInputManager"/> class.
        /// </summary>
        /// <param name="service">The service</param>
        public SpatialInputManager(SpatialInputService service)
        {
            this.currentSpatialState = new SpatialState();
            this.service = service;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpatialInputManager"/> class.
        /// </summary>
        ~SpatialInputManager()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        public void Initialize()
        {
            var mixedRealityService = WaveServices.GetService<MixedRealityService>();
            this.mixedRealityApplication = mixedRealityService?.MixedRealityApplication as BaseMixedRealityApplication;

            if (this.mixedRealityApplication != null)
            {
                this.isMixedRealityControllerAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);

                // The interaction manager provides an event that informs when spacial interactions are detected.
                this.interactionManager = SpatialInteractionManager.GetForCurrentView();

                this.interactionManager.InteractionDetected += this.OnInteractionDetected;
                this.interactionManager.SourceDetected += this.OnSourceDetected;
                this.interactionManager.SourceLost += this.OnSourceLost;
                this.interactionManager.SourcePressed += this.OnSourcePressed;
                this.interactionManager.SourceReleased += this.OnSourceReleased;
                this.interactionManager.SourceUpdated += this.OnSourceUpdated;

                this.gestureRecognizer = new SpatialGestureRecognizer(SpatialGestureSettings.None);
            }
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        public void Terminate()
        {
            this.Dispose();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Refresh the gesture recognizer
        /// </summary>
        /// <param name="enabledGestures">The enabled gestures</param>
        private void RefreshGestureRecognizer(SpatialGestures enabledGestures)
        {
            this.UnsubscribeGestureEvents();

            bool ok = this.gestureRecognizer.TrySetGestureSettings((SpatialGestureSettings)enabledGestures);
            if (!ok)
            {
                this.gestureRecognizer.CancelPendingGestures();
                this.gestureRecognizer.TrySetGestureSettings((SpatialGestureSettings)enabledGestures);
            }

            // Tap events
            if ((enabledGestures & (SpatialGestures.Tap | SpatialGestures.DoubleTap)) != 0)
            {
                this.gestureRecognizer.Tapped += this.OnGestureTapped;
            }

            // Hold events
            if (enabledGestures.HasFlag(SpatialGestures.Hold))
            {
                this.gestureRecognizer.HoldCanceled += this.OnGestureHoldCanceled;
                this.gestureRecognizer.HoldCompleted += this.OnGestureHoldCompleted;
                this.gestureRecognizer.HoldStarted += this.OnGestureHoldStarted;
            }

            // manipulation events
            if (enabledGestures.HasFlag(SpatialGestures.ManipulationTranslate))
            {
                this.gestureRecognizer.ManipulationCanceled += this.OnGestureManipulationCanceled;
                this.gestureRecognizer.ManipulationCompleted += this.OnGestureManipulationCompleted;
                this.gestureRecognizer.ManipulationStarted += this.OnGestureManipulationStarted;
                this.gestureRecognizer.ManipulationUpdated += this.OnGestureManipulationUpdated;
            }

            // navigation events
            if ((enabledGestures & (SpatialGestures.NavigationX
                | SpatialGestures.NavigationY
                | SpatialGestures.NavigationZ
                | SpatialGestures.NavigationRailsX
                | SpatialGestures.NavigationRailsY
                | SpatialGestures.NavigationRailsZ)) != 0)
            {
                this.gestureRecognizer.NavigationCanceled += this.OnGestureNavigationCanceled;
                this.gestureRecognizer.NavigationCompleted += this.OnGestureNavigationCompleted;
                this.gestureRecognizer.NavigationStarted += this.OnGestureNavigationStarted;
                this.gestureRecognizer.NavigationUpdated += this.OnGestureNavigationUpdated;
            }

            // Gesture recognition events
            if (enabledGestures != SpatialGestures.None)
            {
                this.gestureRecognizer.RecognitionEnded += this.OnGestureRecognitionEnded;
                this.gestureRecognizer.RecognitionStarted += this.OnGestureRecognitionStarted;
            }
        }

        private void OnGestureHoldCanceled(SpatialGestureRecognizer sender, SpatialHoldCanceledEventArgs args)
        {
            if (this.HoldCanceledEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.HoldCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureHoldCompleted(SpatialGestureRecognizer sender, SpatialHoldCompletedEventArgs args)
        {
            if (this.HoldCompletedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.HoldCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureHoldStarted(SpatialGestureRecognizer sender, SpatialHoldStartedEventArgs args)
        {
            if (this.HoldStartedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.HoldStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationCanceled(SpatialGestureRecognizer sender, SpatialManipulationCanceledEventArgs args)
        {
            if (this.ManipulationCanceledEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.ManipulationCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationCompleted(SpatialGestureRecognizer sender, SpatialManipulationCompletedEventArgs args)
        {
            if (this.ManipulationCompletedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.ManipulationCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.TryGetCumulativeDelta(this.CoordinateSystem).Translation.ToWave(),
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationStarted(SpatialGestureRecognizer sender, SpatialManipulationStartedEventArgs args)
        {
            if (this.ManipulationStartedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.ManipulationStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationUpdated(SpatialGestureRecognizer sender, SpatialManipulationUpdatedEventArgs args)
        {
            if (this.ManipulationUpdatedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.ManipulationUpdatedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.TryGetCumulativeDelta(this.CoordinateSystem).Translation.ToWave(),
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationCanceled(SpatialGestureRecognizer sender, SpatialNavigationCanceledEventArgs args)
        {
            if (this.NavigationCanceledEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.NavigationCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationCompleted(SpatialGestureRecognizer sender, SpatialNavigationCompletedEventArgs args)
        {
            if (this.NavigationCompletedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.NavigationCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.NormalizedOffset.ToWave(),
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationStarted(SpatialGestureRecognizer sender, SpatialNavigationStartedEventArgs args)
        {
            if (this.NavigationStartedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.NavigationStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.IsNavigatingX,
                        args.IsNavigatingY,
                        args.IsNavigatingZ,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationUpdated(SpatialGestureRecognizer sender, SpatialNavigationUpdatedEventArgs args)
        {
            if (this.NavigationUpdatedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.NavigationUpdatedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.NormalizedOffset.ToWave(),
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        private void OnGestureRecognitionEnded(SpatialGestureRecognizer sender, SpatialRecognitionEndedEventArgs args)
        {
            if (this.RecognitionEndedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.RecognitionEndedEvent(this.service, (SpatialSource)args.InteractionSourceKind);
                });
            }
        }

        private void OnGestureRecognitionStarted(SpatialGestureRecognizer sender, SpatialRecognitionStartedEventArgs args)
        {
            if (this.RecognitionStartedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.RecognitionStartedEvent(this.service, (SpatialSource)args.InteractionSourceKind);
                });
            }
        }

        private void OnGestureTapped(SpatialGestureRecognizer sender, SpatialTappedEventArgs args)
        {
            if (this.TappedEvent != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.TappedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        (int)args.TapCount,
                        this.mixedRealityApplication.HeadRay);
                });
            }
        }

        /// <summary>
        /// Interaction detected
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The args</param>
        private void OnInteractionDetected(SpatialInteractionManager sender, SpatialInteractionDetectedEventArgs args)
        {
            if (this.gestureRecognizer.GestureSettings != SpatialGestureSettings.None)
            {
                this.gestureRecognizer.CaptureInteraction(args.Interaction);
            }
        }

        /// <summary>
        /// Occurs when a new hand, controller, or source of voice commands has been detected.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourceDetected(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedState(args);

            ////this.LoadModel(args);

            if (this.SourceDetected != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.SourceDetected(this.service, this.currentSpatialState);
                });
            }
        }

        /// <summary>
        /// Load controller models
        /// </summary>
        /// <param name="args">Spatial Interaction Source</param>
        private async void LoadModel(SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = source.Controller.TryGetRenderableModelAsync();

            if (modelTask == null)
            {
                Debug.WriteLine("Model task is null.");
                return;
            }

            while (modelTask.Status == AsyncStatus.Started)
            {
                return;
            }

            IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

            if (modelStream == null)
            {
                Debug.WriteLine("Model stream is null.");
                return;
            }

            if (modelStream.Size == 0)
            {
                Debug.WriteLine("Model stream is empty.");
                return;
            }

            byte[] fileBytes = new byte[modelStream.Size];

            using (DataReader reader = new DataReader(modelStream))
            {
                DataReaderLoadOperation loadModelOp = reader.LoadAsync((uint)modelStream.Size);

                while (loadModelOp.Status == AsyncStatus.Started)
                {
                    return;
                }

                reader.ReadBytes(fileBytes);
            }

            await this.AsStorageFile(fileBytes, "MotionController.gltf");
        }

        /// <summary>
        /// Storage controller model
        /// </summary>
        /// <param name="byteArray">gltf byte array</param>
        /// <param name="fileName">File name</param>
        /// <returns>Task instance</returns>
        private async Task<StorageFile> AsStorageFile(byte[] byteArray, string fileName)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBytesAsync(sampleFile, byteArray);
            return sampleFile;
        }

        /// <summary>
        /// Occurs when a hand, controller, or source of voice commands is no longer available.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedState(args);

            if (this.SourceLost != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.SourceLost(this.service, this.currentSpatialState);
                });
            }

            this.ResetState();
        }

        /// <summary>
        /// Reset input state
        /// </summary>
        private void ResetState()
        {
            this.currentSpatialState.IsDetected = false;
            this.currentSpatialState.IsSelected = false;
            this.currentSpatialState.Kind = SpatialSource.Other;
            this.currentSpatialState.SourceLossRisk = 0;
            this.currentSpatialState.Velocity = Vector3.Zero;
            this.currentSpatialState.Position = Vector3.Zero;
        }

        /// <summary>
        /// Occurs when a hand or controller has entered the pressed state.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourcePressed(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedState(args);

            if (this.SourcePressed != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.SourcePressed(this.service, this.currentSpatialState);
                });
            }
        }

        /// <summary>
        /// Occurs when a hand or controller has exited the pressed state.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourceReleased(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedState(args);

            if (this.SourceReleased != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.SourceReleased(this.service, this.currentSpatialState);
                });
            }
        }

        /// <summary>
        /// Occurs when a hand or controller has experienced a change to its SpatialInteractionSourceState.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourceUpdated(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedState(args);

            if (this.SourceUpdated != null)
            {
                WaveForegroundTask.Run(() =>
                {
                    this.SourceUpdated(this.service, this.currentSpatialState);
                });
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.interactionManager.InteractionDetected -= this.OnInteractionDetected;
                    this.interactionManager.SourceDetected -= this.OnSourceDetected;
                    this.interactionManager.SourceLost -= this.OnSourceLost;
                    this.interactionManager.SourcePressed -= this.OnSourcePressed;
                    this.interactionManager.SourceReleased -= this.OnSourceReleased;
                    this.interactionManager.SourceUpdated -= this.OnSourceUpdated;
                    this.interactionManager = null;

                    this.UnsubscribeGestureEvents();
                    this.gestureRecognizer = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// unsubscribe gesture events
        /// </summary>
        private void UnsubscribeGestureEvents()
        {
            this.gestureRecognizer.HoldCanceled -= this.OnGestureHoldCanceled;
            this.gestureRecognizer.HoldCompleted -= this.OnGestureHoldCompleted;
            this.gestureRecognizer.HoldStarted -= this.OnGestureHoldStarted;
            this.gestureRecognizer.ManipulationCanceled -= this.OnGestureManipulationCanceled;
            this.gestureRecognizer.ManipulationCompleted -= this.OnGestureManipulationCompleted;
            this.gestureRecognizer.ManipulationStarted -= this.OnGestureManipulationStarted;
            this.gestureRecognizer.ManipulationUpdated -= this.OnGestureManipulationUpdated;
            this.gestureRecognizer.NavigationCanceled -= this.OnGestureNavigationCanceled;
            this.gestureRecognizer.NavigationCompleted -= this.OnGestureNavigationCompleted;
            this.gestureRecognizer.NavigationStarted -= this.OnGestureNavigationStarted;
            this.gestureRecognizer.NavigationUpdated -= this.OnGestureNavigationUpdated;
            this.gestureRecognizer.RecognitionEnded -= this.OnGestureRecognitionEnded;
            this.gestureRecognizer.RecognitionStarted -= this.OnGestureRecognitionStarted;
            this.gestureRecognizer.Tapped -= this.OnGestureTapped;
        }

        /// <summary>
        /// Update the spatial input state
        /// </summary>
        /// <param name="args">The argument</param>
        private void UpdatedState(SpatialInteractionSourceEventArgs args)
        {
            var coordinateSystem = this.CoordinateSystem;

            this.currentSpatialState.IsDetected = true;
            this.currentSpatialState.IsSelected = args.State.IsPressed;
            SpatialInteractionSource source = args.State.Source;
            this.currentSpatialState.Kind = (SpatialSource)source.Kind;
            this.currentSpatialState.SourceLossRisk = (float)args.State.Properties.SourceLossRisk;

            if (this.isMixedRealityControllerAvailable && source.Kind == SpatialInteractionSourceKind.Controller)
            {
                SpatialInteractionSourceState sourceState = args.State;
                SpatialInteractionControllerProperties controllerProperties = sourceState.ControllerProperties;

                this.currentSpatialState.Handedness = (SpatialInteractionHandedness)source.Handedness;

                // Buttons
                this.currentSpatialState.IsThumbstickPressed = controllerProperties.IsThumbstickPressed;
                this.currentSpatialState.IsTouchpadPressed = controllerProperties.IsTouchpadPressed;
                this.currentSpatialState.IsTouchpadTouched = controllerProperties.IsTouchpadTouched;
                this.currentSpatialState.IsGraspPressed = sourceState.IsGrasped;
                this.currentSpatialState.IsMenuPressed = sourceState.IsMenuPressed;

                // Trigger
                this.currentSpatialState.IsSelectTriggerPressed = sourceState.IsSelectPressed;
                this.currentSpatialState.SelectTriggerValue = (float)sourceState.SelectPressedValue;

                // Thumbstick
                Vector2 thumbstickValues = this.currentSpatialState.Thumbstick;
                thumbstickValues.X = (float)controllerProperties.ThumbstickX;
                thumbstickValues.Y = (float)controllerProperties.ThumbstickY;
                this.currentSpatialState.Thumbstick = thumbstickValues;

                // Touchpad
                Vector2 touchpadValues = this.currentSpatialState.Touchpad;
                touchpadValues.X = (float)controllerProperties.TouchpadX;
                touchpadValues.Y = (float)controllerProperties.TouchpadY;
                this.currentSpatialState.Touchpad = touchpadValues;

                // Tip of the controller
                SpatialPointerInteractionSourcePose pointer = sourceState.TryGetPointerPose(this.CoordinateSystem)?.TryGetInteractionSourcePose(source);
                if (pointer != null)
                {
                    pointer.Position.ToWave(out this.currentSpatialState.TipControllerPosition);
                    pointer.ForwardDirection.ToWave(out this.currentSpatialState.TipControllerForward);
                }
            }

            var location = args.State.Properties.TryGetLocation(coordinateSystem);

            if (location != null)
            {
                if (location.Position.HasValue)
                {
                    location.Position.Value.ToWave(out this.currentSpatialState.Position);
                }

                if (location.Orientation.HasValue)
                {
                    location.Orientation.Value.ToWave(out this.currentSpatialState.Orientation);
                }

                if (location.Velocity.HasValue)
                {
                    location.Velocity.Value.ToWave(out this.currentSpatialState.Velocity);
                }
            }

            this.UpdateGenericController();
        }

        /// <summary>
        /// Update Generic Controller
        /// </summary>
        private void UpdateGenericController()
        {
            VRGenericControllerState genericController = new VRGenericControllerState()
            {
                IsConnected = true,
                Role = this.ConvertHandednessToRole(this.currentSpatialState.Handedness),
                ThumbStick = this.currentSpatialState.Thumbstick,
                Trigger = this.currentSpatialState.SelectTriggerValue,
                TriggerButton = this.currentSpatialState.IsSelectTriggerPressed ? Common.Input.ButtonState.Pressed : Common.Input.ButtonState.Released,
                Grip = this.currentSpatialState.IsGraspPressed ? Common.Input.ButtonState.Pressed : Common.Input.ButtonState.Released,
                Menu = this.currentSpatialState.IsMenuPressed ? Common.Input.ButtonState.Pressed : Common.Input.ButtonState.Released,
                ThumbStickButton = this.currentSpatialState.IsThumbstickPressed ? Common.Input.ButtonState.Pressed : Common.Input.ButtonState.Released,
                Touchpad = this.currentSpatialState.Touchpad,
                //////Button1 = this.A,
                ////Button2 = ButtonState.Released,
                Pose = this.ConvertToVRPose(this.currentSpatialState)
            };

            switch (genericController.Role)
            {
                case VRControllerRole.LeftHand:
                    this.genericControllersArray[0] = genericController;
                    break;
                case VRControllerRole.RightHand:
                    this.genericControllersArray[1] = genericController;
                    break;
                case VRControllerRole.Undefined:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Convert to VRPose struct
        /// </summary>
        /// <param name="spatialState">Spatial state</param>
        /// <returns>VRPose instance</returns>
        private VRPose ConvertToVRPose(SpatialState spatialState)
        {
            VRPose pose;
            pose.Position = spatialState.Position;
            pose.Orientation = spatialState.Orientation;
            return pose;
        }

        /// <summary>
        /// Convert SpatialInteractionHandedness to VRControllerRole
        /// </summary>
        /// <param name="handedness">Spatial Interaction Handedness</param>
        /// <returns>VRController role</returns>
        private VRControllerRole ConvertHandednessToRole(SpatialInteractionHandedness handedness)
        {
            VRControllerRole role;
            switch (handedness)
            {
                case SpatialInteractionHandedness.Left:
                    role = VRControllerRole.LeftHand;
                    break;
                case SpatialInteractionHandedness.Right:
                    role = VRControllerRole.RightHand;
                    break;
                case SpatialInteractionHandedness.Unspecified:
                    role = VRControllerRole.Undefined;
                    break;
                default:
                    role = VRControllerRole.Undefined;
                    break;
            }

            return role;
        }

        #endregion
    }
}
