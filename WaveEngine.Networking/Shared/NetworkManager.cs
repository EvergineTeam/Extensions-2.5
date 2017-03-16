#region File Description
//-----------------------------------------------------------------------------
// NetworkManager
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaveEngine.Common.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Models.Entities;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network manager
    /// </summary>
    public class NetworkManager : IDisposable
    {
        /// <summary>
        /// The serialization factory identifier
        /// </summary>
        private const string SerializationFactoryId = "Wave.Serialization.Factory";

        /// <summary>
        /// The registered scenes
        /// </summary>
        private static Dictionary<string, WeakReference<NetworkManager>> registeredScenes = new Dictionary<string, WeakReference<NetworkManager>>();

        /// <summary>
        /// The scene
        /// </summary>
        private Scene scene;

        /// <summary>
        /// The network service
        /// </summary>
        private NetworkService networkService;

        /// <summary>
        /// The factories
        /// </summary>
        private Dictionary<string, Func<string, string, Entity>> factories;

        /// <summary>
        /// The entity behaviors
        /// </summary>
        private Dictionary<string, NetworkBehavior> entityBehaviors;

        /// <summary>
        /// The entity serializer
        /// </summary>
        private ISerializer serializer;

        /// <summary>
        /// Gets the scene identifier.
        /// </summary>
        /// <value>
        /// The scene identifier.
        /// </value>
        public string SceneId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager" /> class.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="networkService">The network service.</param>
        /// <exception cref="System.ArgumentNullException">The 'scene' or 'networkService' argument can't be null.</exception>
        internal NetworkManager(Scene scene, NetworkService networkService)
        {
            if (scene == null)
            {
                throw new ArgumentNullException("The 'scene' argument can't be null.");
            }

            if (networkService == null)
            {
                throw new ArgumentNullException("The 'networkService' argument can't be null.");
            }

            this.scene = scene;
            this.networkService = networkService;
            this.factories = new Dictionary<string, Func<string, string, Entity>>();
            this.entityBehaviors = new Dictionary<string, NetworkBehavior>();
            this.serializer = SerializerFactory.GetSerializer<Entity>();
        }

        /// <summary>
        /// Registers the scene.
        /// </summary>
        /// <param name="sceneId">The scene identifier.</param>
        /// <exception cref="System.ArgumentException">The 'sceneId' argument can't be null or empty</exception>
        internal void RegisterScene(string sceneId)
        {
            if (string.IsNullOrEmpty(sceneId))
            {
                throw new ArgumentException("The 'sceneId' argument can't be null or empty");
            }

            this.SceneId = sceneId;
            NetworkManager.RegisterScene(sceneId, this);

            var message = this.CreateNetworkBehaviorMessage(null, null, NetworkSyncType.Start);
            this.networkService.SendToServer(message, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Adds the factory.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="factory">The factory.</param>
        public void AddFactory(string factoryId, Func<string, string, Entity> factory)
        {
            this.factories.Add(factoryId, factory);
        }

        /// <summary>
        /// Removes the factory.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        public void RemoveFactory(string factoryId)
        {
            this.factories.Remove(factoryId);
        }

        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="offlineEntity">The offline entity.</param>
        /// <returns>The added entity.</returns>
        public Entity AddEntity(Entity offlineEntity)
        {
            var networkBehaviorId = Guid.NewGuid().ToString();
            var behavior = this.RegisterEntityBehavior(offlineEntity, this.networkService.ClientIdentifier, networkBehaviorId, this.SceneId, SerializationFactoryId);

            var message = this.CreateNetworkBehaviorMessage(behavior.NetworkBehaviorId, SerializationFactoryId, NetworkSyncType.Create);
            this.WriteEntity(message, offlineEntity);
            behavior.WriteSyncData(message, behavior.NetworkSyncComponents);
            this.networkService.SendToServer(message, DeliveryMethod.ReliableOrdered);
            return offlineEntity;
        }

        /// <summary>
        /// Writes the serialized entity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="offlineEntity">The offline entity.</param>
        private void WriteEntity(OutgoingMessage message, Entity offlineEntity)
        {
            using (var stream = new MemoryStream())
            {
                var offlineEntityModel = new EntityModel(offlineEntity);
                this.serializer.Serialize(stream, offlineEntityModel);
                var data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                message.Write(data);
            }
        }

        /// <summary>
        /// Reads the entity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Return the serialized entity from message</returns>
        private Entity ReadEntity(IncomingMessage message)
        {
            var data = message.ReadBytes();
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);

                var entityModel = this.serializer.Deserialize(stream);
                return ((EntityModel)entityModel).Entity;
            }
        }

        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The added entity</returns>
        public Entity AddEntity(string factoryId)
        {
            var networkBehaviorId = Guid.NewGuid().ToString();
            var entity = this.CreateEntity(factoryId, this.networkService.ClientIdentifier, networkBehaviorId);
            var behavior = this.RegisterEntityBehavior(entity, this.networkService.ClientIdentifier, networkBehaviorId, this.SceneId, factoryId);

            this.scene.EntityManager.Add(entity);
            var message = this.CreateNetworkBehaviorMessage(behavior.NetworkBehaviorId, factoryId, NetworkSyncType.Create);
            behavior.WriteSyncData(message, behavior.NetworkSyncComponents);
            this.networkService.SendToServer(message, DeliveryMethod.ReliableOrdered);
            return entity;
        }

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="fromNetworkId">From network identifier.</param>
        /// <param name="fromNetworkBehaviorId">From network behavior identifier.</param>
        /// <returns>The created entity</returns>
        /// <exception cref="System.InvalidOperationException">No factory with especified id has been found.</exception>
        private Entity CreateEntity(string factoryId, string fromNetworkId, string fromNetworkBehaviorId)
        {
            Func<string, string, Entity> factory;
            if (!this.factories.TryGetValue(factoryId, out factory))
            {
                throw new InvalidOperationException(string.Format("No factory with id '{0}' has been found", factoryId));
            }

            var entity = factory(fromNetworkId, fromNetworkBehaviorId);

            return entity;
        }

        /// <summary>
        /// Registers the entity behavior.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="networkId">The network identifier.</param>
        /// <param name="behaviorId">The behavior identifier.</param>
        /// <param name="sceneId">The scene identifier.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="createdByBehavior">if set to <c>true</c> [created by behavior].</param>
        /// <returns>The network behavior registered</returns>
        private NetworkBehavior RegisterEntityBehavior(Entity entity, string networkId, string behaviorId, string sceneId, string factoryId, bool createdByBehavior = false)
        {
            var behavior = (NetworkBehavior)entity.Components.Single(x => x.GetType() == typeof(NetworkBehavior));
            behavior.NetworkManager = this;
            behavior.NetworkOwnerId = networkId;
            behavior.NetworkBehaviorId = behaviorId;
            behavior.CreatedByBehavior = createdByBehavior;
            behavior.FactoryId = factoryId;
            this.entityBehaviors.Add(behavior.NetworkBehaviorId, behavior);
            return behavior;
        }

        /// <summary>
        /// Removes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void RemoveEntity(Entity entity)
        {
            var behavior = (NetworkBehavior)entity.Components.Single(x => x.GetType() == this.GetType());
            this.RemoveNetworkEntity(behavior);

            var message = this.CreateNetworkBehaviorMessage(behavior.NetworkBehaviorId, null, NetworkSyncType.Remove);
            behavior.WriteSyncData(message, behavior.NetworkSyncComponents);
            this.networkService.SendToServer(message, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="receivedmessage">The receivedmessage.</param>
        private void HandleMessage(IncomingMessage receivedmessage)
        {
            var fromNetworkId = receivedmessage.ReadString();
            var type = (NetworkSyncType)receivedmessage.ReadInt32();
            var fromNetworkBehaviorId = receivedmessage.ReadString();
            var factoryId = receivedmessage.ReadString();

            if (type == NetworkSyncType.Update)
            {
                NetworkBehavior behavior;
                if (this.entityBehaviors.TryGetValue(fromNetworkBehaviorId, out behavior))
                {
                    if (behavior.CreatedByBehavior)
                    {
                        behavior.ReadSyncData(receivedmessage);
                    }
                }
            }
            else if (type == NetworkSyncType.Start)
            {
                foreach (var item in this.entityBehaviors.Where(x => !x.Value.CreatedByBehavior).ToList())
                {
                    this.SendResponseToRecreateThisEntity(item.Value);
                }
            }
            else if (type == NetworkSyncType.Create && !this.entityBehaviors.ContainsKey(fromNetworkBehaviorId))
            {
                var behavior = this.CreateNewEntity(fromNetworkId, fromNetworkBehaviorId, receivedmessage, factoryId);
            }
            else if (type == NetworkSyncType.Remove)
            {
                NetworkBehavior behavior;
                if (this.entityBehaviors.TryGetValue(fromNetworkBehaviorId, out behavior))
                {
                    if (behavior.CreatedByBehavior)
                    {
                        this.RemoveNetworkEntity(behavior);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the response to recreate this entity.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        private void SendResponseToRecreateThisEntity(NetworkBehavior behavior)
        {
            var responseMessage = this.CreateNetworkBehaviorMessage(behavior.NetworkBehaviorId, behavior.FactoryId, NetworkSyncType.Create);
            if (behavior.FactoryId == SerializationFactoryId)
            {
                this.WriteEntity(responseMessage, behavior.Owner);
            }

            behavior.WriteSyncData(responseMessage, behavior.NetworkSyncComponents);
            this.networkService.SendToServer(responseMessage, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Creates the new entity.
        /// </summary>
        /// <param name="fromNetworkId">From network identifier.</param>
        /// <param name="fromNetworkBehaviorId">From network behavior identifier.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <returns>The network behavior os created entity</returns>
        private NetworkBehavior CreateNewEntity(string fromNetworkId, string fromNetworkBehaviorId, IncomingMessage reader, string factoryId)
        {
            Entity entity;
            if (factoryId == SerializationFactoryId)
            {
                entity = this.ReadEntity(reader);
            }
            else
            {
                entity = this.CreateEntity(factoryId, fromNetworkId, fromNetworkBehaviorId);
            }

            var behavior = this.RegisterEntityBehavior(entity, fromNetworkId, fromNetworkBehaviorId, this.SceneId, factoryId, true);
            this.scene.EntityManager.Add(entity);
            behavior.ReadSyncData(reader);

            return behavior;
        }

        /// <summary>
        /// Removes the network entity.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        private void RemoveNetworkEntity(NetworkBehavior behavior)
        {
            this.entityBehaviors.Remove(behavior.NetworkBehaviorId);
            this.scene.EntityManager.Remove(behavior.Owner);
        }

        /// <summary>
        /// Updates the behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        internal void UpdateBehavior(NetworkBehavior behavior)
        {
            if (!behavior.CreatedByBehavior && behavior.NeedSyncComponentsUpdatingComponentsToSync())
            {
                var message = this.CreateNetworkBehaviorMessage(behavior.NetworkBehaviorId, behavior.FactoryId, NetworkSyncType.Update);
                behavior.WriteSyncData(message, behavior.ComponentsToSync);
                this.networkService.SendToServer(message, DeliveryMethod.ReliableSequenced);
            }
        }

        /// <summary>
        /// Creates the network behavior message.
        /// </summary>
        /// <param name="fromNetworkId">From network identifier.</param>
        /// <param name="factoryId">The factory identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message</returns>
        private OutgoingMessage CreateNetworkBehaviorMessage(string fromNetworkId, string factoryId, NetworkSyncType type)
        {
            var message = this.networkService.CreateClientMessage(MessageType.Synchronization);
            message.Write(this.SceneId);
            message.Write(this.networkService.ClientIdentifier);
            message.Write((int)type);
            message.Write(fromNetworkId);
            message.Write(factoryId);
            return message;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.SceneId != null)
            {
                NetworkManager.UnregisterScene(this.SceneId);
            }
        }

        /// <summary>
        /// Registers the scene.
        /// </summary>
        /// <param name="sceneId">The scene identifier.</param>
        /// <param name="manager">The manager.</param>
        /// <exception cref="System.InvalidOperationException">The specified sceneId has been already registered.</exception>
        private static void RegisterScene(string sceneId, NetworkManager manager)
        {
            WeakReference<NetworkManager> managerReference;
            if (registeredScenes.TryGetValue(sceneId, out managerReference))
            {
                NetworkManager registeredManager;
                if (managerReference.TryGetTarget(out registeredManager))
                {
                    throw new InvalidOperationException(string.Format("The specified sceneId '{0}' has been already registered.", sceneId));
                }
                else
                {
                    managerReference.SetTarget(manager);
                }
            }
            else
            {
                registeredScenes.Add(sceneId, new WeakReference<NetworkManager>(manager));
            }
        }

        /// <summary>
        /// Unregisters the scene.
        /// </summary>
        /// <param name="sceneId">The scene identifier.</param>
        private static void UnregisterScene(string sceneId)
        {
            registeredScenes.Remove(sceneId);
        }

        /// <summary>
        /// Handles the menssage.
        /// </summary>
        /// <param name="receivedmessage">The receivedmessage.</param>
        internal static void HandleMenssage(IncomingMessage receivedmessage)
        {
            var sceneId = receivedmessage.ReadString();
            WeakReference<NetworkManager> managerReference;
            if (sceneId != null && registeredScenes.TryGetValue(sceneId, out managerReference))
            {
                NetworkManager registeredManager;
                if (managerReference.TryGetTarget(out registeredManager))
                {
                    registeredManager.HandleMessage(receivedmessage);
                }
            }
        }
    }
}
