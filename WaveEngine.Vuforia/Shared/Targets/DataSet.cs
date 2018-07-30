// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// This class represents a dataset that can be loaded and holds a collection of trackables.
    /// Trackables can also be created and destroyed at runtime.
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// Gets returns the path to the data set.
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trackables that are defined in the data set.
        /// </summary>
        public IEnumerable<Trackable> Trackables
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public DataSet(string path)
        {
            this.Path = path;
        }
    }
}
