#region File Description
//-----------------------------------------------------------------------------
// NetworkBehavior
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network behavior
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Networking")]
    public class NetworkBehavior : Behavior
    {
        /// <summary>
        /// The network manager
        /// </summary>
        internal NetworkManager NetworkManager;

        /// <summary>
        /// The factory identifier
        /// </summary>
        internal string FactoryId;

        /// <summary>
        /// The created by behavior
        /// </summary>
        internal bool CreatedByBehavior;

        /// <summary>
        /// The network synchronize components
        /// </summary>
        internal NetworkSyncComponent[] NetworkSyncComponents;

        /// <summary>
        /// The components to synchronize
        /// </summary>
        internal NetworkSyncComponent[] ComponentsToSync;

        /// <summary>
        /// Gets the network owner identifier.
        /// </summary>
        /// <value>
        /// The network owner identifier.
        /// </value>
        public string NetworkOwnerId { get; internal set; }

        /// <summary>
        /// Gets the network behavior identifier.
        /// </summary>
        /// <value>
        /// The network behavior identifier.
        /// </value>
        public string NetworkBehaviorId { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkBehavior"/> class.
        /// </summary>
        public NetworkBehavior()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (this.CreatedByBehavior)
            {
                this.IsActive = false;
            }
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.NetworkSyncComponents = this.Owner.Components.OfType<NetworkSyncComponent>().OrderBy(x => x.GetType().Name).ToArray();
            this.ComponentsToSync = new NetworkSyncComponent[this.NetworkSyncComponents.Length];
        }

        /// <summary>
        /// Actives the notification.
        /// </summary>
        /// <param name="active">if set to <c>true</c> [active].</param>
        public sealed override void ActiveNotification(bool active)
        {
            if (active && this.CreatedByBehavior)
            {
                this.IsActive = false;
            }
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.NetworkManager != null)
            {
                this.NetworkManager.UpdateBehavior(this);
            }
        }

        /// <summary>
        /// Needs the synchronize components updating components to synchronize.
        /// </summary>
        /// <returns>If there are any component that need to sync</returns>
        internal bool NeedSyncComponentsUpdatingComponentsToSync()
        {
            var needSync = false;
            for (int componentIndex = 0; componentIndex < this.NetworkSyncComponents.Length; componentIndex++)
            {
                var component = this.NetworkSyncComponents[componentIndex];
                if (component.NeedSendSyncData())
                {
                    this.ComponentsToSync[componentIndex] = component;
                    needSync = true;
                }
                else
                {
                    this.ComponentsToSync[componentIndex] = null;
                }
            }

            return needSync;
        }

        /// <summary>
        /// Reads the synchronize data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void ReadSyncData(IncomingMessage reader)
        {
            for (int componentIndex = 0; componentIndex < this.NetworkSyncComponents.Length; componentIndex++)
            {
                if (reader.ReadBoolean())
                {
                    var component = this.NetworkSyncComponents[componentIndex];
                    component.ReadSyncData(reader);
                }
            }
        }

        /// <summary>
        /// Writes the synchronize data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="components">The components.</param>
        internal void WriteSyncData(OutgoingMessage writer, NetworkSyncComponent[] components)
        {
            for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
            {
                var component = components[componentIndex];
                if (component != null)
                {
                    writer.Write(true);
                    component.WriteSyncData(writer);
                }
                else
                {
                    writer.Write(false);
                }
            }
        }
    }
}
