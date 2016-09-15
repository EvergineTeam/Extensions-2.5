#region File Description
//-----------------------------------------------------------------------------
// SpatialInputManager
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Hololens.Utilities;
using Windows.Perception.Spatial;
using Windows.UI.Input;
using Windows.UI.Input.Spatial;
#endregion

namespace WaveEngine.Hololens.Interaction
{
    /// <summary>
    /// Handler all the input event.
    /// </summary>
    internal class SpatialInputManager : ISpatialInputManager
    {
        /// <summary>
        /// The hololens application
        /// </summary>
        private BaseHololensApplication hololensApplication;

        /// <summary>
        /// The spatial input service
        /// </summary>
        private SpatialInputService service;

        /// <summary>
        /// Wave dispatcher
        /// </summary>
        private Dispatcher waveDispatcher;

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

        #region Properties
        /// <summary>
        /// Gets or sets the enabled gestures
        /// </summary>
        public SpatialGestures EnabledGestures
        {
            get
            {
                return (SpatialGestures)this.gestureRecognizer.GestureSettings;
            }

            set
            {
                if (value != this.EnabledGestures)
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
                if (this.hololensApplication.ReferenceFrame != null)
                {
                    return this.hololensApplication.ReferenceFrame.CoordinateSystem;
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
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialInteractionManager"/> class.
        /// </summary>
        /// <param name="service">The service</param>
        public SpatialInputManager(SpatialInputService service)
        {
            this.currentSpatialState = new SpatialState();
            this.service = service;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpatialInteractionManager"/> class.
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
            this.waveDispatcher = WaveServices.Dispatcher;
            var hololensService = WaveServices.GetService<HololensService>();
            this.hololensApplication = hololensService.HololensApplication as BaseHololensApplication;

            if (this.hololensApplication != null)
            {
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
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.HoldCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureHoldCompleted(SpatialGestureRecognizer sender, SpatialHoldCompletedEventArgs args)
        {
            if (this.HoldCompletedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.HoldCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureHoldStarted(SpatialGestureRecognizer sender, SpatialHoldStartedEventArgs args)
        {
            if (this.HoldStartedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.HoldStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationCanceled(SpatialGestureRecognizer sender, SpatialManipulationCanceledEventArgs args)
        {
            if (this.ManipulationCanceledEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.ManipulationCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,                        
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationCompleted(SpatialGestureRecognizer sender, SpatialManipulationCompletedEventArgs args)
        {
            if (this.ManipulationCompletedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.ManipulationCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.TryGetCumulativeDelta(this.CoordinateSystem).Translation.ToWave(),
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationStarted(SpatialGestureRecognizer sender, SpatialManipulationStartedEventArgs args)
        {
            if (this.ManipulationStartedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.ManipulationStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,                        
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureManipulationUpdated(SpatialGestureRecognizer sender, SpatialManipulationUpdatedEventArgs args)
        {
            if (this.ManipulationUpdatedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.ManipulationUpdatedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.TryGetCumulativeDelta(this.CoordinateSystem).Translation.ToWave(),
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationCanceled(SpatialGestureRecognizer sender, SpatialNavigationCanceledEventArgs args)
        {
            if (this.NavigationCanceledEvent != null)
            { 
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.NavigationCanceledEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,                        
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationCompleted(SpatialGestureRecognizer sender, SpatialNavigationCompletedEventArgs args)
        {
            if (this.NavigationCompletedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.NavigationCompletedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.NormalizedOffset.ToWave(),
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationStarted(SpatialGestureRecognizer sender, SpatialNavigationStartedEventArgs args)
        {
            if (this.NavigationStartedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.NavigationStartedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.IsNavigatingX,
                        args.IsNavigatingY,
                        args.IsNavigatingZ,
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureNavigationUpdated(SpatialGestureRecognizer sender, SpatialNavigationUpdatedEventArgs args)
        {
            if (this.NavigationUpdatedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.NavigationUpdatedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        args.NormalizedOffset.ToWave(),
                        this.hololensApplication.HeadRay);
                });
            }
        }

        private void OnGestureRecognitionEnded(SpatialGestureRecognizer sender, SpatialRecognitionEndedEventArgs args)
        {
            if (this.RecognitionEndedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.RecognitionEndedEvent( this.service, (SpatialSource)args.InteractionSourceKind);
                });
            }
        }

        private void OnGestureRecognitionStarted(SpatialGestureRecognizer sender, SpatialRecognitionStartedEventArgs args)
        {
            if (this.RecognitionStartedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.RecognitionStartedEvent( this.service, (SpatialSource)args.InteractionSourceKind);
                });
            }
        }

        private void OnGestureTapped(SpatialGestureRecognizer sender, SpatialTappedEventArgs args)
        {
            if (this.TappedEvent != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.TappedEvent(
                        this.service,
                        (SpatialSource)args.InteractionSourceKind,
                        (int)args.TapCount,
                        this.hololensApplication.HeadRay);
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
            this.UpdatedSate(args);

            if (this.SourceDetected != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
                {
                    this.SourceDetected(this.service, this.currentSpatialState);
                });
            }
        }
        /// <summary>
        /// Occurs when a hand, controller, or source of voice commands is no longer available.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Raw args.</param>
        private void OnSourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            this.UpdatedSate(args);

            if (this.SourceLost != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
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
            this.currentSpatialState.Source = SpatialSource.Other;
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
            this.UpdatedSate(args);

            if (this.SourcePressed != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
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
            this.UpdatedSate(args);

            if (this.SourceReleased != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
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
            this.UpdatedSate(args);

            if (this.SourceUpdated != null)
            {
                this.waveDispatcher.RunOnWaveThread(() =>
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
        private void UpdatedSate(SpatialInteractionSourceEventArgs args)
        {
            var coordinateSystem = this.CoordinateSystem;

            this.currentSpatialState.IsDetected = true;
            this.currentSpatialState.IsSelected = args.State.IsPressed;
            this.currentSpatialState.Source = (SpatialSource)args.State.Source.Kind;
            this.currentSpatialState.SourceLossRisk = (float)args.State.Properties.SourceLossRisk;

            var location = args.State.Properties.TryGetLocation(coordinateSystem);
            if (location != null)
            {
                if (location.Position.HasValue)
                {
                    location.Position.Value.ToWave(out this.currentSpatialState.Position);
                }

                if (location.Velocity.HasValue)
                {
                    location.Velocity.Value.ToWave(out this.currentSpatialState.Velocity);
                }
            }
        }
        #endregion
    }
}
