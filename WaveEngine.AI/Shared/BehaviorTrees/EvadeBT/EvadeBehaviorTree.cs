

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.AI.ChaseAndEvade;
using WaveEngine.Framework;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework.Graphics;
using WaveEngine.AI.BehaviorTrees;
#endregion

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Behavior tree for evade behaviors
    /// </summary>
    [DataContract(Namespace = "WaveEngine.AI.BehaviorTrees")]
    public class EvadeBehaviorTree : Component
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// The tree executor
        /// </summary>
        private TreeExecutorBehavior<EvadeNodeInfo> treeExecutor;

        /// <summary>
        /// The nodeinfo
        /// </summary>
        private EvadeNodeInfo nodeinfo;

        /// <summary>
        /// The free movement strategy
        /// </summary>
        [RequiredComponent(false)]
        public MovementBase FreeMovementStrategy;

        /// <summary>
        /// The evade strategy
        /// </summary>
        [RequiredComponent(false)]
        public EvadeStrategy EvadeStrategy;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="EvadeBehaviorTree" /> class.
        /// </summary>
        public EvadeBehaviorTree()
            : base("EvadeBehaviorTree" + instances++)
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.treeExecutor = new TreeExecutorBehavior<EvadeNodeInfo>();
            this.treeExecutor.NodeInfo = new EvadeNodeInfo();
            this.treeExecutor.Tree = this.GenerateTreeWithClasses();
            this.Owner.AddComponent(this.treeExecutor);

            if (this.EvadeStrategy.Enemy != null)
            {
                this.Owner.AddComponent(new UpdateEvadeNodeInfoBehavior() { EvadeNodeInfo = this.treeExecutor.NodeInfo, Enemy = this.EvadeStrategy.Enemy });
            }

            this.Owner.RefreshDependencies();
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();           
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates the tree with classes.
        /// </summary>
        /// <returns>A basicall tree for evade behavior</returns>
        private Node<EvadeNodeInfo> GenerateTreeWithClasses()
        {
            Node<EvadeNodeInfo> tree = new InitNode<EvadeNodeInfo>();

            tree.AddChild(new EvadeNode(this.EvadeStrategy));
            
            tree.AddChild(new MoveNode(this.FreeMovementStrategy));

            return tree;
        }
        #endregion
    }
}
