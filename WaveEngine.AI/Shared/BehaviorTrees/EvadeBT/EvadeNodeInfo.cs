

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.AI.BehaviorTrees;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Evade node info class
    /// </summary>
    public class EvadeNodeInfo : NodeInfo
    {
        /// <summary>
        /// The time in movement
        /// </summary>
        public TimeSpan TimeInMovement;

        /// <summary>
        /// The target position
        /// </summary>
        private Vector3 targetPosition;

        /// <summary>
        /// The source position
        /// </summary>
        private Vector3 sourcePosition;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is moving.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is moving; otherwise, <c>false</c>.
        /// </value>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Gets or sets the target position.
        /// </summary>
        /// <value>
        /// The target position.
        /// </value>
        public Vector3 TargetPosition
        {
            get
            {
                return this.targetPosition;
            }

            set
            {
                if (value != this.targetPosition)
                {
                    this.targetPosition = value;
                    this.EvaluateTree = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the source position.
        /// </summary>
        /// <value>
        /// The source position.
        /// </value>
        public Vector3 SourcePosition
        {
            get
            {
                return this.sourcePosition;
            }

            set
            {
                if (value != this.sourcePosition)
                {
                    this.sourcePosition = value;
                    this.EvaluateTree = true;
                }
            }
        }
        #endregion
    }
}