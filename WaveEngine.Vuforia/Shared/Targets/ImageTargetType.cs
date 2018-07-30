// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// The type of an ImageTarget. An ImageTarget can be predefined in a dataset,
    /// created at runtime as a user defined target, or fetched at runtime via
    /// cloud recognition
    /// </summary>
    public enum ImageTargetType
    {
        /// <summary>
        /// The ImageTarget is predefined in a dataset.
        /// </summary>
        PREDEFINED,

        /// <summary>
        /// The ImageTarget is defined by user at runtime.
        /// </summary>
        USER_DEFINED,

        /// <summary>
        /// The ImageTarget is fetched at runtime via cloud recognition.
        /// </summary>
        CLOUD_RECO
    }
}
