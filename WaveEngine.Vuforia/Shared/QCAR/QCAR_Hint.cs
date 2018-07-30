// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Use when calling setHint()
    /// </summary>
    internal enum QCAR_Hint
    {
        // How many image targets to detect and track at the same time
        /**
         * This hint tells the tracker how many image shall be processed
         * at most at the same time. E.g. if an app will never require
         * tracking more than two targets, this value should be set to 2.
         * Default is: 1.
         */
        HINT_MAX_SIMULTANEOUS_IMAGE_TARGETS = 0,

        // How many object targets to detect and track at the same time
        /**
        * This hint tells the tracker how many 3D objects shall be processed
        * at most at the same time. E.g. if an app will never require
        * tracking more than 1 target, this value should be set to 1.
        * Default is: 1.
        */
        HINT_MAX_SIMULTANEOUS_OBJECT_TARGETS = 1,

        // Force delayed loading for object target Dataset
        /**
        * This hint tells the tracker to enable/disable delayed loading
        * of object target datasets upon first detection.
        * Loading time of large object dataset will be reduced
        * but the initial detection time of targets will increase.
        * Please note that the hint should be set before loading
        * any object target dataset to be effective.
        * To enable delayed loading set the hint value to 1.
        * To disable delayed loading set the hint value to 0.
        * Default is: 0.
        */
        HINT_DELAYED_LOADING_OBJECT_DATASETS = 2,
    }
}
