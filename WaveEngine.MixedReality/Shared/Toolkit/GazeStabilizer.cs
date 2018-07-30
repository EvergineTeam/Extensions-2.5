// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.MixedReality.Toolkit
{
    /// <summary>
    /// GazeStabilizer
    /// </summary>
    [DataContract]
    public class GazeStabilizer : Component
    {
        /// <summary>
        /// gazeBehavior
        /// </summary>
        [RequiredComponent]
        private GazeBehavior gazeBehavior = null;

        /// <summary>
        /// Positions samples queue.
        /// </summary>
        private Queue<Vector3> stabilitySamples;

        #region Properties

        /// <summary>
        /// Gets or sets storedStabilitySamples
        /// </summary>
        [DataMember]
        [RenderPropertyAsInput(1, 120, Tooltip = "Number of samples that you want to iterate on.")]
        public int StoredStabilitySamples { get; set; }

        #endregion

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.StoredStabilitySamples = 10;
            this.stabilitySamples = new Queue<Vector3>();
        }

        /// <summary>
        /// Update Cu
        /// </summary>
        /// <param name="cursorPosition">Cursor position</param>
        /// <returns>Smooth cursor position</returns>
        public Vector3 UpdateSmoothCursorPosition(Vector3 cursorPosition)
        {
            if (this.stabilitySamples.Count >= this.StoredStabilitySamples)
            {
                this.stabilitySamples.Dequeue();
            }

            this.stabilitySamples.Enqueue(cursorPosition);
            Vector3 smoothCursorPosition = Vector3.Zero;
            foreach (Vector3 v in this.stabilitySamples)
            {
                smoothCursorPosition += v;
            }

            smoothCursorPosition /= this.stabilitySamples.Count;

            return smoothCursorPosition;
        }
    }
}
