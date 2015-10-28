#region File Description
//-----------------------------------------------------------------------------
// MoveNode
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.AI.ChaseAndEvade;
#endregion

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Move node
    /// </summary>
    public class MoveNode : Node<EvadeNodeInfo>
    {
        /// <summary>
        /// The strategy
        /// </summary>
        private MovementBase strategy;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveNode"/> class.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        public MoveNode(MovementBase strategy)
        {
            this.strategy = strategy;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executes the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        public override void Execute(EvadeNodeInfo info)
        {
            if (!info.IsMoving)
            {
                info.TimeInMovement = TimeSpan.Zero;
                info.IsMoving = true;
            }

            info.TimeInMovement += info.GameTime;
            this.strategy.Move(info.GameTime);
        }

        /// <summary>
        /// Evaluates the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>True if must be evaluated, false in other case</returns>
        public override bool Evaluate(EvadeNodeInfo info)
        {
            return true;
        }
        #endregion
    }
}