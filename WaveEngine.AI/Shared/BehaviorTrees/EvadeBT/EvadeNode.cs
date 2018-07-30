

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
    /// Evade Node class
    /// </summary>
    public class EvadeNode : Node<EvadeNodeInfo>
    {
        /// <summary>
        /// The strategy
        /// </summary>
        private EvadeStrategy strategy;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="EvadeNode"/> class.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        public EvadeNode(EvadeStrategy strategy)
        {
            this.strategy = strategy;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executes the specified node information.
        /// </summary>
        /// <param name="nodeInfo">The node information.</param>
        public override void Execute(EvadeNodeInfo nodeInfo)
        {
            nodeInfo.TimeInMovement = TimeSpan.Zero;
            this.strategy.Evade(nodeInfo.GameTime);
        }

        /// <summary>
        /// Evaluates the specified node information.
        /// </summary>
        /// <param name="nodeInfo">The node information.</param>
        /// <returns>True if need to evade, false in other case</returns>
        public override bool Evaluate(EvadeNodeInfo nodeInfo)
        {
            return this.strategy.NeedToEvade(nodeInfo.GameTime);
        }
        #endregion
    }
}
