

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Updates Evade node info 
    /// </summary>
    internal class UpdateEvadeNodeInfoBehavior : Behavior
    {
        /// <summary>
        /// The instances
        /// </summary>
        private static int instances = 0;

        /// <summary>
        /// The mouse position
        /// </summary>
        [RequiredComponent(false)]
        private Transform3D position;

        /// <summary>
        /// The cat position
        /// </summary>
        private Transform3D enemyPosition;

        /// <summary>
        /// The evade node information
        /// </summary>
        public EvadeNodeInfo EvadeNodeInfo;

        /// <summary>
        /// The enemy
        /// </summary>
        public WaveEngine.Framework.Entity Enemy;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEvadeNodeInfoBehavior"/> class.
        /// </summary>
        public UpdateEvadeNodeInfoBehavior()
            : base("UpdateEvadeNodeInfoBehavior" + instances++, FamilyType.PriorityBehavior)
        {
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.enemyPosition = this.Enemy.FindComponent<Transform3D>(isExactType: false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.EvadeNodeInfo.SourcePosition = this.position.Position;
            this.EvadeNodeInfo.TargetPosition = this.enemyPosition.Position;
        }
        #endregion
    }
}
