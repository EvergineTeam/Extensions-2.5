// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Framework;

namespace WaveEngine.AI.BehaviorTrees
{
    /// <summary>
    /// Represent a behavior an Entity need to contain and execute a behavior tree
    /// </summary>
    /// <typeparam name="T">The type of elements a behavior tree will contain</typeparam>
    public class TreeExecutorBehavior<T> : Behavior
        where T : NodeInfo
    {
        /// <summary>
        /// The tree evaluator
        /// </summary>
        private TreeEvaluator<T> evaluator;

        /// <summary>
        /// The validator
        /// </summary>
        private TreeValidator validator;

        /// <summary>
        /// The tree
        /// </summary>
        private Node<T> tree;

        /// <summary>
        /// The node information
        /// </summary>
        public T NodeInfo;

        /// <summary>
        /// Gets or sets the behavior tree.
        /// </summary>
        /// <value>
        /// The tree.
        /// </value>
        public Node<T> Tree
        {
            get
            {
                return this.tree;
            }

            set
            {
                this.validator.Validate(value);
                this.tree = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeExecutorBehavior{T}"/> class.
        /// </summary>
        public TreeExecutorBehavior()
            : this(new TreeValidator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeExecutorBehavior{T}"/> class.
        /// </summary>
        /// <param name="treeValidator">The behavior tree validator.</param>
        internal TreeExecutorBehavior(TreeValidator treeValidator)
            : this(treeValidator, new TreeEvaluator<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeExecutorBehavior{T}"/> class.
        /// </summary>
        /// <param name="treeValidator">The behavior tree validator.</param>
        /// <param name="treeEvaluator">The behavior tree evaluator.</param>
        internal TreeExecutorBehavior(TreeValidator treeValidator, TreeEvaluator<T> treeEvaluator)
        {
            this.validator = treeValidator;
            this.evaluator = treeEvaluator;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// The update
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.Execute(gameTime);
        }

        /// <summary>
        /// Executes or evaluates the tree of the entity.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Execute(TimeSpan gameTime)
        {
            if (this.NodeInfo.EvaluateTree)
            {
                this.NodeInfo.GameTime = gameTime;
                this.evaluator.Evaluate(this.NodeInfo, this.Tree);
                this.NodeInfo.EvaluateTree = false;
            }

            if (this.evaluator.CurrentNode != null)
            {
                this.evaluator.CurrentNode.Execute(this.NodeInfo);
            }
        }
    }
}
