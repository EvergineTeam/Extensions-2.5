#region File Description
//-----------------------------------------------------------------------------
// LeapGestureEventArgs
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using Leap;
using System;
#endregion

namespace WaveEngine.LeapMotion
{
    /// <summary>
    /// This class represent the args in a gesture event.
    /// </summary>
    public class LeapGestureEventArgs : EventArgs
    {
        /// <summary>
        /// Gesture object.
        /// </summary>
        private Gesture gesture;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeapGestureEventArgs"/> class.
        /// </summary>
        /// <param name="gesture">The leap motion gesture object.</param>
        public LeapGestureEventArgs(Gesture gesture)
        {
            this.gesture = gesture;
        }
    }
}
