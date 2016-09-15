#region File Description
//-----------------------------------------------------------------------------
// ConfidenceLevel
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.Hololens.Speech
{
    /// <summary>
    /// Specifies confidence levels that indicate how accurately a spoken phrase was
    //  matched to a phrase in an active constraint.
    /// </summary>
    public enum ConfidenceLevel
    {
        /// <summary>
        /// The confidence level is high.
        /// </summary>
        High = 0,

        /// <summary>
        /// The confidence level is medium.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// The confidence level is low.
        /// </summary>
        Low = 2,

        /// <summary>
        /// The spoken phrase was not matched to any phrase in any active grammar.
        /// </summary>
        Rejected = 3
    }
}
