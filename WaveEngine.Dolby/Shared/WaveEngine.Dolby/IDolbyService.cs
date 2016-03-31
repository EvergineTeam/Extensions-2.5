#region File Description
//-----------------------------------------------------------------------------
// IDolbyService
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
#endregion

namespace WaveEngine.Dolby
{
    /// <summary>
    /// Dolby Service interface
    /// </summary>
    internal interface IDolbyService
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the dolby profile.
        /// </summary>
        /// <value>
        /// The dolby profile.
        /// </value>
        DolbyProfile DolbyProfile { get; set; }

        /// <summary>
        /// Creates the dolby.
        /// </summary>
        void StartDolbySession();

        /// <summary>
        /// Releases the dolby.
        /// </summary>
        void ReleaseDolbySession();

        /// <summary>
        /// Restarts the session.
        /// </summary>
        void RestartDolbySession();

        /// <summary>
        /// Suspends the dolby session.
        /// </summary>
        void SuspendDolbySession();
    }
}
