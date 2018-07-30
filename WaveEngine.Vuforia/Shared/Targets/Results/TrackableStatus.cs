// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// States of a TrackableResult
    /// </summary>
    public enum TrackableStatus
    {
        /// <summary>
        /// The state of the Trackable is unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The state of the Trackable is not defined (this Trackable does not have a state)
        /// </summary>
        Undefined,

        /// <summary>
        /// The Trackable was detected
        /// </summary>
        Detected,

        /// <summary>
        /// The Trackable was tracked
        /// </summary>
        Tracked,

        /// <summary>
        /// The Trackable was extended tracked
        /// </summary>
        ExtendedTracked
    }
}
