// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Options affecting how to transition an AR session's current state when the tracking is started.
    /// </summary>
    [Flags]
    public enum ARMobileStartOptions
    {
        /// <summary>
        /// The session is resumed if it is not running for the first time.
        /// </summary>
        None = 0,

        /// <summary>
        /// The session does not continue device position/motion tracking from the previous world origin.
        /// </summary>
        ResetTracking = 1,

        /// <summary>
        /// Any anchor objects associated with the session in its previous configuration are removed.
        /// </summary>
        RemoveExistingAnchors = 2,
    }
}
