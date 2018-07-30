// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.VR;
#endregion

namespace WaveEngine.MixedReality.Interaction
{
    /// <summary>
    /// Interface for the spatial input manager implementation
    /// </summary>
    internal interface ISpatialInputManager : IDisposable
    {
        /// <summary>
        /// Occurs when a new hand, controller, or source of voice commands has been detected.
        /// </summary>
        event SpatialInputService.InputSourceDelegate SourceDetected;

        /// <summary>
        /// Occurs when a hand, controller, or source of voice commands is no longer available.
        /// </summary>
        event SpatialInputService.InputSourceDelegate SourceLost;

        /// <summary>
        /// Occurs when a hand or controller has entered the pressed state.
        /// </summary>
        event SpatialInputService.InputSourceDelegate SourcePressed;

        /// <summary>
        /// Occurs when a hand or controller has exited the pressed state.
        /// </summary>
        event SpatialInputService.InputSourceDelegate SourceReleased;

        /// <summary>
        /// Occurs when a hand or controller has experienced a change to its SpatialInteractionSourceState.
        /// </summary>
        event SpatialInputService.InputSourceDelegate SourceUpdated;

        /// <summary>
        /// Gesture error event
        /// </summary>
        event SpatialInputService.GestureErrorDelegate GestureErrorEvent;

        /// <summary>
        /// The hold gesture is canceled
        /// </summary>
        event SpatialInputService.HoldCanceledEventDelegate HoldCanceledEvent;

        /// <summary>
        /// The hold gesture is completed
        /// </summary>
        event SpatialInputService.HoldCompletedEventDelegate HoldCompletedEvent;

        /// <summary>
        /// The hold gesture is started
        /// </summary>
        event SpatialInputService.HoldStartedEventDelegate HoldStartedEvent;

        /// <summary>
        /// The manipulation gesture is canceled
        /// </summary>
        event SpatialInputService.ManipulationCanceledEventDelegate ManipulationCanceledEvent;

        /// <summary>
        /// The manipulation gesture is completed
        /// </summary>
        event SpatialInputService.ManipulationCompletedEventDelegate ManipulationCompletedEvent;

        /// <summary>
        /// The manipulation gesture is started
        /// </summary>
        event SpatialInputService.ManipulationStartedEventDelegate ManipulationStartedEvent;

        /// <summary>
        /// The manipulation gesture is updated
        /// </summary>
        event SpatialInputService.ManipulationUpdatedEventDelegate ManipulationUpdatedEvent;

        /// <summary>
        /// The navigation gesture is canceled
        /// </summary>
        event SpatialInputService.NavigationCanceledEventDelegate NavigationCanceledEvent;

        /// <summary>
        /// The navigation gesture is completed
        /// </summary>
        event SpatialInputService.NavigationCompletedEventDelegate NavigationCompletedEvent;

        /// <summary>
        /// The navigation gesture is started
        /// </summary>
        event SpatialInputService.NavigationStartedEventDelegate NavigationStartedEvent;

        /// <summary>
        /// The navigation gesture is updated
        /// </summary>
        event SpatialInputService.NavigationUpdatedEventDelegate NavigationUpdatedEvent;

        /// <summary>
        /// The recognition gesture is ended
        /// </summary>
        event SpatialInputService.RecognitionEndedEventDelegate RecognitionEndedEvent;

        /// <summary>
        /// The recognition gesture is started
        /// </summary>
        event SpatialInputService.RecognitionStartedEventDelegate RecognitionStartedEvent;

        /// <summary>
        /// Occurs when there is a tap gesture
        /// </summary>
        event SpatialInputService.TappedEventDelegate TappedEvent;

        /// <summary>
        /// Gets the spatial state
        /// </summary>
        SpatialState SpatialState { get; }

        /// <summary>
        /// Gets the Controller states
        /// </summary>
        VRGenericControllerState[] GenericControllersArray { get; }

        /// <summary>
        /// Gets or sets the enabled gestures
        /// </summary>
        SpatialGestures EnabledGestures { get; set; }

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        void Terminate();
    }
}
