#region File Description
//-----------------------------------------------------------------------------
// SpatialInputService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Hololens.Interaction
{
    /// <summary>
    /// Handler all the input event.
    /// </summary>
    public class SpatialInputService : Service, IDisposable
    {
        /// <summary>
        /// Instance of SpatialInteractionManager.
        /// </summary>
        private ISpatialInputManager spatialInputManager;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        #region Events
        /// <summary>
        /// Occurs when a new hand, controller, or source of voice commands has been detected.
        /// </summary>
        public event InputSourceDelegate SourceDetected
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceDetected += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceDetected -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a hand, controller, or source of voice commands is no longer available.
        /// </summary>
        public event InputSourceDelegate SourceLost
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceLost += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceLost -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a hand or controller has entered the pressed state.
        /// </summary>
        public event InputSourceDelegate SourcePressed
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourcePressed += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourcePressed -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a hand or controller has exited the pressed state.
        /// </summary>
        public event InputSourceDelegate SourceReleased
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceReleased += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceReleased -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a hand or controller has experienced a change to its SpatialInteractionSourceState.
        /// </summary>
        public event InputSourceDelegate SourceUpdated
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceUpdated += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.SourceUpdated -= value;
                }
            }
        }

        /// <summary>
        /// Gesture error event
        /// </summary>
        public event GestureErrorDelegate GestureError
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.GestureErrorEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.GestureErrorEvent -= value;
                }
            }
        }

        /// <summary>
        /// The hold gesture is canceled
        /// </summary>
        public event HoldCanceledEventDelegate HoldCanceled
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldCanceledEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldCanceledEvent -= value;                    
                }
            }
        }

        /// <summary>
        /// The hold gesture is completed
        /// </summary>
        public event HoldCompletedEventDelegate HoldCompleted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldCompletedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldCompletedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The hold gesture is started
        /// </summary>
        public event HoldStartedEventDelegate HoldStarted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldStartedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.HoldStartedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The manipulation gesture is canceled
        /// </summary>
        public event ManipulationCanceledEventDelegate ManipulationCanceled
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationCanceledEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationCanceledEvent -= value;
                }
            }
        }

        /// <summary>
        /// The manipulation gesture is completed
        /// </summary>
        public event ManipulationCompletedEventDelegate ManipulationCompleted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationCompletedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationCompletedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The manipulation gesture is started
        /// </summary>
        public event ManipulationStartedEventDelegate ManipulationStarted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationStartedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationStartedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The manipulation gesture is updated
        /// </summary>
        public event ManipulationUpdatedEventDelegate ManipulationUpdated
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationUpdatedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.ManipulationUpdatedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The navigation gesture is canceled
        /// </summary>
        public event NavigationCanceledEventDelegate NavigationCanceled
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationCanceledEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationCanceledEvent -= value;
                }
            }
        }

        /// <summary>
        /// The navigation gesture is completed
        /// </summary>
        public event NavigationCompletedEventDelegate NavigationCompleted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationCompletedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationCompletedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The navigation gesture is started
        /// </summary>
        public event NavigationStartedEventDelegate NavigationStarted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationStartedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationStartedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The navigation gesture is updated
        /// </summary>
        public event NavigationUpdatedEventDelegate NavigationUpdated
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationUpdatedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.NavigationUpdatedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The recognition gesture is ended
        /// </summary>
        public event RecognitionEndedEventDelegate RecognitionEnded
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.RecognitionEndedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.RecognitionEndedEvent -= value;
                }
            }
        }

        /// <summary>
        /// The recognition gesture is started
        /// </summary>
        public event RecognitionStartedEventDelegate RecognitionStarted
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.RecognitionStartedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.RecognitionStartedEvent -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when there is a tap gesture
        /// </summary>
        public event TappedEventDelegate TappedEvent
        {
            add
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.TappedEvent += value;
                }
            }

            remove
            {
                if (this.IsConnected)
                {
                    this.spatialInputManager.TappedEvent -= value;
                }
            }
        }
        #endregion

        #region Delegates
        /// <summary>
        /// The input source delegate
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="state">The state</param>
        public delegate void InputSourceDelegate(SpatialInputService sender, SpatialState state);

        /// <summary>
        /// Gesture error delegate
        /// </summary>
        /// <param name="error">The error message</param>
        /// <param name="hresult">The hresult handler</param>
        public delegate void GestureErrorDelegate(string error, int hresult);

        /// <summary>
        /// The hold gesture is canceled
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void HoldCanceledEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The hold gesture is completed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void HoldCompletedEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The hold gesture is started
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void HoldStartedEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The manipulation gesture is canceled
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void ManipulationCanceledEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The manipulation gesture is completed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="cumulativeDelta">The cumulative delta</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void ManipulationCompletedEventDelegate(SpatialInputService sender, SpatialSource source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// The manipulation gesture is started
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void ManipulationStartedEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The manipulation gesture is updated
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="cumulativeDelta">The cumulative delta</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void ManipulationUpdatedEventDelegate(SpatialInputService sender, SpatialSource source, Vector3 cumulativeDelta, Ray headRay);

        /// <summary>
        /// The manipulation gesture is updated
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>        
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void NavigationCanceledEventDelegate(SpatialInputService sender, SpatialSource source, Ray headRay);

        /// <summary>
        /// The navigation gesture is completed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="normalizedOffset">The normalized offset</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void NavigationCompletedEventDelegate(SpatialInputService sender, SpatialSource source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// The navigation gesture is started
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="isNavigatingX">Navigating in X axis</param>
        /// <param name="isNavigatingY">Navigating in Y axis</param>
        /// <param name="isNavigatingZ">Navigating in Z axis</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void NavigationStartedEventDelegate(SpatialInputService sender, SpatialSource source, bool isNavigatingX, bool isNavigatingY, bool isNavigatingZ, Ray headRay);

        /// <summary>
        /// The navigation gesture is updated
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="normalizedOffset">The normalized offset</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void NavigationUpdatedEventDelegate(SpatialInputService sender, SpatialSource source, Vector3 normalizedOffset, Ray headRay);

        /// <summary>
        /// A gesture recognition is ended
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>        
        public delegate void RecognitionEndedEventDelegate(SpatialInputService sender, SpatialSource source);

        /// <summary>
        /// A gesture recognition is started
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>        
        public delegate void RecognitionStartedEventDelegate(SpatialInputService sender, SpatialSource source);

        /// <summary>
        /// A tap gesture is detected
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="source">The source of the event</param>
        /// <param name="tapCount">The number of taps detected</param>
        /// <param name="headRay">The head ray which can be used to determine what the user was looking at when the event occurred since it can be from a previous frame</param>
        public delegate void TappedEventDelegate(SpatialInputService sender, SpatialSource source, int tapCount, Ray headRay); 
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.spatialInputManager != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SpatialInputService" /> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return this.disposed; }
        }

        /// <summary>
        /// Gets the current Spatial State.
        /// </summary>
        public SpatialState SpatialState
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("HoloLens is not available.");
                }

                return this.spatialInputManager.SpatialState;
            }
        }

        /// <summary>
        /// Gets or sets the enabled gestures
        /// </summary>
        public SpatialGestures EnabledGestures
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("HoloLens is not available.");
                }

                return this.spatialInputManager.EnabledGestures;
            }

            set
            {
                if (!this.IsConnected)
                {
                    throw new Exception("HoloLens is not available.");
                }

                this.spatialInputManager.EnabledGestures = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialInputService"/> class.
        /// </summary>
        public SpatialInputService()
        {
#if UWP
            this.spatialInputManager = new SpatialInputManager(this);
#endif
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpatialInputService"/> class.
        /// </summary>
        ~SpatialInputService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (this.IsConnected)
            {
                this.spatialInputManager.Initialize();
            }
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        protected override void Terminate()
        {
            if (this.IsConnected)
            {
                this.spatialInputManager.Terminate();
            }

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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.IsConnected)
                    {
                        this.spatialInputManager.Dispose();
                        this.spatialInputManager = null;
                    }
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
