// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// The AR adapter states
    /// </summary>
    public enum ARState
    {
        /// <summary>
        /// The AR adapter is stopped. Initial state.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// The AR adapter is initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// The AR adapter has started the tracking.
        /// </summary>
        Tracking
    }
}
