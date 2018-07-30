// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Networking.Client.Players;
using WaveEngine.Networking.Connection;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
using WaveEngine.Networking.Players;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Client
{
    /// <summary>
    /// The client service for matchmaking
    /// </summary>
    [DataContract]
    public class MatchmakingClientService : UpdatableService
    {
        /// <summary>
        /// The network factory used to create the internal network client peer
        /// </summary>
        private INetworkFactory networkFactory;

        /// <summary>
        /// The network client peer
        /// </summary>
        private INetworkClient networkClient;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        private ClientStates state;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        [DataMember]
        private string applicationIdentifier;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        [DataMember]
        private string clientApplicationVersion;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        [DataMember]
        private TimeSpan pingInterval;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        [DataMember]
        private TimeSpan connectionTimeout;

        private TaskCompletionSource<EnterRoomResultCodes> enterRoomTCS;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        private Dictionary<string, RoomInfo> roomsInLobby;

        /// <summary>
        /// Internally used to create the room when matchmaking server respond.
        /// </summary>
        private RoomOptions lastRoomOptionsRequest;

        #region Properties

        /// <summary>
        /// Gets or sets the factory used to create the <see cref="INetworkClient"/> peer.
        /// </summary>
        public INetworkFactory NetworkFactory
        {
            get
            {
                return this.networkFactory;
            }

            set
            {
                if (this.networkClient != null)
                {
                    throw new InvalidOperationException($"{nameof(this.NetworkFactory)} cannot be set once the client has been created");
                }

                this.networkFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the application Identifier. Used by the matchmaking server to separate players by different game.
        /// </summary>
        public string ApplicationIdentifier
        {
            get
            {
                return this.applicationIdentifier;
            }

            set
            {
                this.CheckClientInitializationForProperty(nameof(this.ApplicationIdentifier));
                this.applicationIdentifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of your client. A new version also creates a new "virtual app" to separate players from older
        /// client versions.
        /// </summary>
        public string ClientApplicationVersion
        {
            get
            {
                return this.clientApplicationVersion;
            }

            set
            {
                this.CheckClientInitializationForProperty(nameof(this.ClientApplicationVersion));
                this.clientApplicationVersion = value;
            }
        }

        /// <summary>
        /// Gets the current state this client is in. Careful: several states are "transitions" that lead to other states.
        /// </summary>
        [DontRenderProperty]
        public ClientStates State
        {
            get
            {
                return this.state;
            }

            private set
            {
                ClientStates newState;

                if (value != ClientStates.InLobby &&
                    value != ClientStates.Disconnected &&
                    !this.IsConnected)
                {
                    newState = ClientStates.Disconnected;
                }
                else
                {
                    newState = value;
                }

                if (this.state != newState)
                {
                    this.state = newState;
                    this.StateChanged?.Invoke(this, this.State);
                }
            }
        }

        /// <summary>
        /// Gets or sets the ping interval. Default value is 4 seconds.
        /// </summary>
        public TimeSpan PingInterval
        {
            get
            {
                return this.pingInterval;
            }

            set
            {
                this.CheckClientInitializationForProperty(nameof(this.PingInterval));
                this.pingInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection timeout. Default value is 25 seconds.
        /// </summary>
        public TimeSpan ConnectionTimeout
        {
            get
            {
                return this.connectionTimeout;
            }

            set
            {
                this.CheckClientInitializationForProperty(nameof(this.ConnectionTimeout));
                this.connectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this client is currently connected or connecting to the matchmaking server.
        /// </summary>
        /// <remarks>
        /// This is even true while switching servers. Use <see cref="IsConnectedAndReady"/> to check only for those states
        /// that enable you to send Operations.
        /// </remarks>
        [DontRenderProperty]
        public bool IsConnected
        {
            get
            {
                return this.networkClient != null &&
                       this.networkClient.IsConnected &&
                       this.State != ClientStates.Disconnected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether your connection to the matchmaking server is ready to accept operations.
        /// </summary>
        [DontRenderProperty]
        public bool IsConnectedAndReady
        {
            get
            {
                if (this.networkClient == null ||
                    !this.networkClient.IsConnected)
                {
                    return false;
                }

                switch (this.State)
                {
                    case ClientStates.InLobby:
                    case ClientStates.Joined:
                        return true;   // we are ready to execute any operation
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the local player is never null but not valid unless the client is in a room, too.
        /// The ID will be -1 outside of rooms.
        /// </summary>
        [DontRenderProperty]
        public LocalNetworkPlayer LocalPlayer { get; private set; }

        /// <summary>
        /// Gets a "list" that is populated while being in the lobby.
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<RoomInfo> RoomsInLobby
        {
            get
            {
                return this.roomsInLobby.Values;
            }
        }

        /// <summary>
        /// Gets the current room this client is connected to (null if none available).
        /// </summary>
        [DontRenderProperty]
        public LocalNetworkRoom CurrentRoom { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when this client's State is changed.
        /// </summary>
        /// <remarks>
        /// This can be useful to react to being connected, joined into a room, etc.
        /// </remarks>
        public event EventHandler<ClientStates> StateChanged;

        /// <summary>
        /// Occurs when a the Room Info list is refreshed.
        /// </summary>
        /// <remarks>
        /// This occurs only when the client is in the lobby. This can be useful to
        /// refresh a list of available rooms.
        /// </remarks>
        public event EventHandler RoomsInLobbySynchronized;

        /// <summary>
        /// Occurs when a the current room is refreshed.
        /// </summary>
        /// <remarks>
        /// This occurs only when the client is in the room.
        /// </remarks>
        public event EventHandler CurrentRoomSynchronized;

        /// <summary>
        /// Occurs when a new matchmaking server with the same <see cref="ApplicationIdentifier"/> and <see cref="ClientApplicationVersion"/> is discovered.
        /// Occurs when a new matchmaking server is discovered
        /// </summary>
        public event EventHandler<HostDiscoveredEventArgs> ServerDiscovered;

        /// <summary>
        /// Occurs when a user data message from the matchmaking server is received by the client.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedFromServer;

        /// <summary>
        /// Occurs when a user data message from the current room is received by the client.
        /// </summary>
        /// <remarks>
        /// This occurs only when the client is in a room.
        /// </remarks>
        public event EventHandler<MessageFromPlayerEventArgs> MessageReceivedFromCurrentRoom;

        /// <summary>
        /// Occurs when a user data message from a player in the same room is received by the client.
        /// </summary>
        /// <remarks>
        /// This occurs only when the client is in a room.
        /// </remarks>
        public event EventHandler<MessageFromPlayerEventArgs> MessageReceivedFromPlayer;

        #endregion

        #region Initialize

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            this.networkFactory = new NetworkFactory();
            this.LocalPlayer = new LocalNetworkPlayer();
            this.State = ClientStates.Disconnected;
            this.pingInterval = TimeSpan.FromSeconds(4);
            this.connectionTimeout = TimeSpan.FromSeconds(25);
            this.roomsInLobby = new Dictionary<string, RoomInfo>();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void Update(TimeSpan gameTime)
        {
            if (this.State != ClientStates.Disconnected)
            {
                var playerSyncMessage = this.CreateSyncLocalPlayerMessage(false);
                if (playerSyncMessage != null)
                {
                    this.networkClient.Send(playerSyncMessage, DeliveryMethod.ReliableOrdered);
                }

                var roomSyncMessage = this.CreateSyncLocalRoomMessage(false);
                if (roomSyncMessage != null)
                {
                    this.networkClient.Send(roomSyncMessage, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Starts the process to discover servers in the local network with the same <see cref="ApplicationIdentifier"/>, <see cref="ClientApplicationVersion"/> and <paramref name="port"/>.
        /// Register to <see cref="ServerDiscovered"/> event to be know about discovered servers.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="ApplicationIdentifier"/> must have a valid value before trying to connect.
        /// or
        /// You can't call discovery method while is connected to a matchmaking server. Call before to <see cref="Disconnect"/> method.
        /// </exception>
        public void DiscoverServers(int port)
        {
            if (string.IsNullOrEmpty(this.ApplicationIdentifier))
            {
                throw new InvalidOperationException($"{nameof(this.ApplicationIdentifier)} must have a valid value before trying to connect.");
            }

            var fullIdentifier = $"{this.ApplicationIdentifier}.{this.ClientApplicationVersion}";
            this.EnsureClient(fullIdentifier);

            if (this.networkClient.IsConnected)
            {
                throw new InvalidOperationException("You can't call discovery method while is connected to a matchmaking server. Call before to disconnect method.");
            }

            this.networkClient.DiscoverHosts(port);
        }

        /// <summary>
        /// Connects the client to the specified matchmaking server asynchronously.
        /// </summary>
        /// <param name="serverEndpoint">The matchmaking server endpoint.</param>
        /// <exception cref="InvalidOperationException">
        /// You can't call Connect method while is connected to a matchmaking server. Call before to <see cref="Disconnect"/> method.
        /// or
        /// <see cref="ApplicationIdentifier"/> must have a valid value before trying to connect.
        /// </exception>
        /// <returns>An a value indicating whether the connection was succeed or not</returns>
        public async Task<bool> ConnectAsync(NetworkEndpoint serverEndpoint)
        {
            if (this.State != ClientStates.Disconnected)
            {
                throw new InvalidOperationException("You can't call connect method while is connected to a matchmaking server. Call before to disconnect method.");
            }

            if (string.IsNullOrEmpty(this.ApplicationIdentifier))
            {
                throw new InvalidOperationException($"{nameof(this.ApplicationIdentifier)} must have a valid value before trying to connect.");
            }

            var fullIdentifier = $"{this.ApplicationIdentifier}.{this.ClientApplicationVersion}";

            this.EnsureClient(fullIdentifier);
            var isConnected = await this.InternalConnectAsync(serverEndpoint);

            this.State = isConnected ? ClientStates.InLobby : ClientStates.Disconnected;

            return isConnected;
        }

        /// <summary>
        /// Disconnects this client from any matchmaking server and sets the <see cref="State"/> if the connection
        /// is successfully closed.
        /// </summary>
        public void Disconnect()
        {
            if (this.State != ClientStates.Disconnected &&
                this.networkClient != null)
            {
                if (this.IsConnectedAndReady)
                {
                    this.LeaveRoom();
                }

                this.networkClient.Disconnect();
                this.networkClient.HostDisconnected -= this.NetworkClient_HostDisconnected;
                this.networkClient.HostDiscovered -= this.NetworkClient_HostDiscovered;
                this.networkClient.MessageReceived -= this.NetworkClient_MessageReceived;
                this.networkClient = null;

                this.enterRoomTCS?.TrySetResult(EnterRoomResultCodes.Aborted);

                this.LocalPlayer.Id = -1;
                this.CurrentRoom = null;
                this.roomsInLobby.Clear();

                this.State = ClientStates.Disconnected;
            }
        }

        /// <summary>
        /// Creates a new room on the server (or fails if the name is already in use).
        /// </summary>
        /// <remarks>
        /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a
        /// roomName (a GUID as string). Room names are unique.
        ///
        /// This method can only be called while the client is connected to a matchmaking server.
        /// You should check <see cref="IsConnectedAndReady"/> before calling this method.
        /// Alternatively, check the returned boolean value.
        ///
        /// Even when sent, the Operation will fail (on the server) if the roomName is in use.
        ///
        ///
        /// While the matchmaking server is creating the game, the State will be <see cref="ClientStates.Joining"/>.
        /// The state Joining is used because the client is on the way to enter a room (no matter if joining or creating).
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the client will enter the room. When you're in the room, this client's State will
        /// become <see cref="ClientStates.Joined"/> (both, for joining or creating it).
        /// Subscribe to <see cref="StateChanged"/> event to check for states.
        ///
        /// When entering the room, this client's Player Custom Properties will be sent to the room.
        /// Use <see cref="INetworkPlayer.CustomProperties"/> to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        /// </remarks>
        /// <param name="options">Contains the parameters and properties of the new room. See <see cref="RoomOptions"/> class for a description of each.</param>
        /// <returns>A value indicating if the operation was successful (requires connection to matchmaking server).</returns>
        public Task<EnterRoomResultCodes> CreateRoomAsync(RoomOptions options)
        {
            this.CheckClientIsReady();
            this.CheckClientInLobby();

            this.State = ClientStates.Joining;
            this.lastRoomOptionsRequest = options;
            this.enterRoomTCS = new TaskCompletionSource<EnterRoomResultCodes>();

            this.SendRoomRequest(ServerIncomingMessageTypes.CreateRoomRequest, options);

            return this.enterRoomTCS.Task;
        }

        /// <summary>
        /// Joins a room by roomName. Useful when using room lists in lobby or when you know the name otherwise.
        /// </summary>
        /// <remarks>
        /// This method is useful when you are using a lobby to list rooms and know their names.
        /// A room's name has to be unique (per game version).
        ///
        /// If the room is full, closed or not existing, this will fail.
        ///
        /// This method can only be called while the client is connected to a matchmaking server.
        /// You should check <see cref="IsConnectedAndReady"/> before calling this method.
        /// Alternatively, check the returned boolean value.
        ///
        /// While the matchmaking server is creating the game, the State will be <see cref="ClientStates.Joining"/>.
        /// The state <see cref="ClientStates.Joining"/> is used because the client is on the way to enter a room (no matter if joining or creating).
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the client will enter the room. When you're in the room, this client's State will
        /// become <see cref="ClientStates.Joined"/> (both, for joining or creating it).
        /// Subscribe to <see cref="StateChanged"/> event to check for states.
        ///
        /// When joining a room, this client's Player Custom Properties will be sent to the room.
        /// Use <see cref="INetworkPlayer.CustomProperties"/> to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// It's usually better to use <see cref="JoinOrCreateRoomAsync"/> for invitations.
        /// Then it does not matter if the room is already setup.
        /// </remarks>
        /// <param name="name">The name of the room to join. Must be existing already, open and non-full or can't be joined.</param>
        /// <returns>A value indicating if the operation was successful (requires connection to matchmaking server).</returns>
        public Task<EnterRoomResultCodes> JoinRoomAsync(string name)
        {
            this.CheckClientIsReady();
            this.CheckClientInLobby();

            this.State = ClientStates.Joining;
            this.enterRoomTCS = new TaskCompletionSource<EnterRoomResultCodes>();
            var options = new RoomOptions() { RoomName = name };
            this.lastRoomOptionsRequest = options;

            this.SendRoomRequest(ServerIncomingMessageTypes.JoinRoomRequest, options);

            return this.enterRoomTCS.Task;
        }

        /// <summary>
        /// Joins a specific room by name. If the room does not exist (yet), it will be created implicitly.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="JoinRoomAsync"/>, this operation does not fail if the room does not exist.
        /// This can be useful when you send invitations to a room before actually creating it:
        /// Any invited player (whoever is first) can call this and on demand, the room gets created implicitly.
        ///
        /// If you set room properties in RoomOptions, they get ignored when the room is existing already.
        /// This avoids changing the room properties by late joining players. Only when the room gets created,
        /// the RoomOptions are set in this case.
        ///
        /// If the room is full or closed, this will fail.
        ///
        /// This method can only be called while the client is connected to a server.
        /// You should check <see cref="IsConnectedAndReady"/> before
        /// calling this method. Alternatively, check the returned boolean value.
        ///
        /// While the server is joining the game, the State will be <see cref="ClientStates.Joining"/>.
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the client will enter the room. When you're in the room, this client's State will
        /// become <see cref="ClientStates.Joined"/> (both, for joining or creating it).
        /// Subscribe to <see cref="StateChanged"/> event to check for states.
        ///
        /// When entering the room, this client's Player Custom Properties will be sent to the room.
        /// Use <see cref="INetworkPlayer.CustomProperties"/> to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        /// </remarks>
        /// <param name="options">Contains the parameters and properties of the new room. See <see cref="RoomOptions"/> class for a description of each.</param>
        /// <returns>A value indicating if the operation was successful (requires connection to matchmaking server).</returns>
        public Task<EnterRoomResultCodes> JoinOrCreateRoomAsync(RoomOptions options)
        {
            this.CheckClientIsReady();
            this.CheckClientInLobby();

            this.State = ClientStates.Joining;
            this.lastRoomOptionsRequest = options;
            this.enterRoomTCS = new TaskCompletionSource<EnterRoomResultCodes>();

            this.SendRoomRequest(ServerIncomingMessageTypes.JoinOrCreateRoomRequest, options);

            return this.enterRoomTCS.Task;
        }

        /// <summary>
        /// Leaves the <see cref="CurrentRoom"/> and returns to the lobby.
        /// </summary>
        /// <returns>If the current room could be left (impossible while not in a room).</returns>
        public bool LeaveRoom()
        {
            this.CheckClientIsReady();

            if (this.State != ClientStates.Joined)
            {
                return false;
            }

            this.State = ClientStates.Leaving;
            var leaveRoomMsg = this.CreateClientMessage(ServerIncomingMessageTypes.LeaveRoomRequest);
            this.networkClient.Send(leaveRoomMsg, DeliveryMethod.ReliableOrdered);

            this.LocalPlayer.Id = -1;
            this.CurrentRoom = null;

            return true;
        }

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <returns>The create outgoing message.</returns>
        public OutgoingMessage CreateMessage()
        {
            this.CheckClientIsReady();

            return this.networkClient.CreateMessage();
        }

        /// <summary>
        /// Sends a user data message to the matchmaking server.
        /// </summary>
        /// <param name="messageToSend">The message to be sent.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void SendToServer(OutgoingMessage messageToSend, DeliveryMethod deliveryMethod)
        {
            this.CheckClientIsReady();

            var userDataMsg = this.CreateClientMessage(ServerIncomingMessageTypes.UserDataToHost);
            userDataMsg.Write(messageToSend);
            this.networkClient.Send(userDataMsg, deliveryMethod);
        }

        /// <summary>
        /// Sends a user data message to the all remote players in the current room.
        /// </summary>
        /// <param name="messageToSend">The message to be sent.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <returns>If the message can be sent to the current room (impossible while not in a room).</returns>
        public bool SendToCurrentRoom(OutgoingMessage messageToSend, DeliveryMethod deliveryMethod)
        {
            this.CheckClientIsReady();

            if (this.State != ClientStates.Joined)
            {
                return false;
            }

            var userDataMsg = this.CreateClientMessage(ServerIncomingMessageTypes.UserDataToRoom);
            userDataMsg.Write(messageToSend);
            this.networkClient.Send(userDataMsg, deliveryMethod);

            return true;
        }

        /// <summary>
        /// Sends a user data message to the a remote player in the current room.
        /// </summary>
        /// <param name="messageToSend">The message to be sent.</param>
        /// <param name="remotePlayer">The remote player that should receive the message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <returns>If the message can be sent to the current room (impossible while not in a room).</returns>
        public bool SendToPlayer(OutgoingMessage messageToSend, RemoteNetworkPlayer remotePlayer, DeliveryMethod deliveryMethod)
        {
            this.CheckClientIsReady();

            if (this.State != ClientStates.Joined)
            {
                return false;
            }

            var userDataMsg = this.CreateClientMessage(ServerIncomingMessageTypes.UserDataToOtherClient);
            userDataMsg.Write(remotePlayer.Id);
            userDataMsg.Write(messageToSend);
            this.networkClient.Send(userDataMsg, deliveryMethod);

            return true;
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override void Terminate()
        {
            base.Terminate();
            this.Disconnect();
        }

        private void CheckClientInitializationForProperty(string propertyName)
        {
            if (this.networkClient != null)
            {
                throw new InvalidOperationException($"{propertyName} cannot be changed once a connection is established");
            }
        }

        private void CheckClientIsReady()
        {
            if (!this.IsConnectedAndReady)
            {
                throw new InvalidOperationException($"Client is not connected and ready. Check property {nameof(this.IsConnectedAndReady)}");
            }
        }

        private void CheckClientInLobby()
        {
            if (this.State != ClientStates.InLobby)
            {
                throw new InvalidOperationException($"Client player must be in lobby to join or create a room. Check property {nameof(this.State)}");
            }
        }

        private OutgoingMessage CreateSyncLocalPlayerMessage(bool forceAllFields)
        {
            OutgoingMessage playerSyncMsg = null;

            var somethingToSync = this.LocalPlayer != null &&
                                  (this.LocalPlayer.NeedSync || forceAllFields);

            if (somethingToSync)
            {
                playerSyncMsg = this.CreateClientMessage(ServerIncomingMessageTypes.SetPlayerProperties);
                this.LocalPlayer.WriteSyncMessage(playerSyncMsg, forceAllFields);
            }

            return playerSyncMsg;
        }

        private OutgoingMessage CreateSyncLocalRoomMessage(bool forceAllFields)
        {
            OutgoingMessage roomSyncMsg = null;

            var somethingToSync = this.CurrentRoom != null &&
                                  (this.CurrentRoom.NeedSync || forceAllFields);

            if (somethingToSync)
            {
                roomSyncMsg = this.CreateClientMessage(ServerIncomingMessageTypes.SetRoomProperties);
                this.CurrentRoom.WriteSyncMessage(roomSyncMsg, forceAllFields);
            }

            return roomSyncMsg;
        }

        /// <summary>
        /// Connects the client to the specified server endpoint asynchronously.
        /// </summary>
        /// <param name="server">The server endpoint.</param>
        /// <returns>An a value indicating if the connection was succeed or not</returns>
        private async Task<bool> InternalConnectAsync(NetworkEndpoint server)
        {
            bool result = false;

            var tcs = new TaskCompletionSource<bool>();
            EventHandler<HostConnectedEventArgs> connectHandler = (sender, e) =>
            {
                var connectedHost = e.HostEndpoint;
                if (connectedHost.Address == server.Address && connectedHost.Port == server.Port)
                {
                    tcs.SetResult(true);
                }
            };
            this.networkClient.HostConnected += connectHandler;
            try
            {
                var playerSyncMessage = this.CreateSyncLocalPlayerMessage(true);
                this.networkClient.Connect(server, playerSyncMessage);
                result = await tcs.Task;
            }
            finally
            {
                this.networkClient.HostConnected -= connectHandler;
            }

            return result;
        }

        /// <summary>
        /// Ensures that the network client is created.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        private void EnsureClient(string applicationIdentifier)
        {
            if (this.networkClient == null)
            {
                this.networkClient = this.networkFactory.CreateNetworkClient(
                    applicationIdentifier,
                    (float)this.pingInterval.TotalSeconds,
                    (float)this.connectionTimeout.TotalSeconds);

                this.networkClient.HostDisconnected += this.NetworkClient_HostDisconnected;
                this.networkClient.HostDiscovered += this.NetworkClient_HostDiscovered;
                this.networkClient.MessageReceived += this.NetworkClient_MessageReceived;
            }
        }

        private OutgoingMessage CreateClientMessage(ServerIncomingMessageTypes type)
        {
            var leaveRoomMsg = this.networkClient.CreateMessage();
            leaveRoomMsg.Write(type);
            return leaveRoomMsg;
        }

        private void SendRoomRequest(ServerIncomingMessageTypes roomRequest, RoomOptions options)
        {
            var requestMsg = this.CreateClientMessage(roomRequest);
            options.Write(requestMsg);
            this.networkClient.Send(requestMsg, DeliveryMethod.ReliableOrdered);
        }

        private void NetworkClient_HostDisconnected(object sender, HostDisconnectedEventArgs e)
        {
            this.Disconnect();
            this.State = ClientStates.Disconnected;
        }

        private void NetworkClient_HostDiscovered(object sender, HostDiscoveredEventArgs e)
        {
            this.ServerDiscovered?.Invoke(sender, e);
        }

        private void NetworkClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var messageType = e.ReceivedMessage.ReadClientIncomingMessageType();

            switch (messageType)
            {
                case ClientIncomingMessageTypes.RefreshRoomsInLobby:
                    this.HandleRefreshRoomsInLobby(e.ReceivedMessage);
                    break;
                case ClientIncomingMessageTypes.RefreshRoomInLobbyProperties:
                    var roomName = e.ReceivedMessage.ReadString();
                    this.roomsInLobby[roomName].ReadFromMessage(e.ReceivedMessage);
                    this.RoomsInLobbySynchronized?.Invoke(this, EventArgs.Empty);
                    break;
                case ClientIncomingMessageTypes.RefreshCurrentRoomProperties:
                    this.CurrentRoom.ReadFromMessage(e.ReceivedMessage);
                    this.CurrentRoomSynchronized?.Invoke(this, EventArgs.Empty);
                    break;
                case ClientIncomingMessageTypes.RefreshPlayersInRoom:
                    this.CurrentRoom.ReadSyncPlayersListFromMessage(e.ReceivedMessage);
                    this.CurrentRoomSynchronized?.Invoke(this, EventArgs.Empty);
                    break;
                case ClientIncomingMessageTypes.RefreshLocalPlayerProperties:
                    this.LocalPlayer.ReadFromMessage(e.ReceivedMessage);
                    break;
                case ClientIncomingMessageTypes.RefreshOtherPlayerProperties:
                    this.CurrentRoom.ReadSyncPlayerPropertiesFromMessage(e.ReceivedMessage);
                    this.CurrentRoomSynchronized?.Invoke(this, EventArgs.Empty);
                    break;
                case ClientIncomingMessageTypes.JoinResponse:
                case ClientIncomingMessageTypes.CreateResponse:
                    this.HandleCreateOrJoinResponse(e.ReceivedMessage, messageType);
                    break;
                case ClientIncomingMessageTypes.LeaveResponse:
                    this.State = ClientStates.InLobby;
                    this.HandleRefreshRoomsInLobby(e.ReceivedMessage);
                    break;
                case ClientIncomingMessageTypes.UserDataFromHost:
                    this.MessageReceivedFromServer?.Invoke(this, e);
                    break;
                case ClientIncomingMessageTypes.UserDataFromRoom:
                    var roomclientArgs = this.GetMessageReceivedArgs(e.ReceivedMessage);
                    this.MessageReceivedFromCurrentRoom?.Invoke(this, roomclientArgs);
                    break;
                case ClientIncomingMessageTypes.UserDataFromOtherClient:
                    var clientArgs = this.GetMessageReceivedArgs(e.ReceivedMessage);
                    this.MessageReceivedFromPlayer(this, clientArgs);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid client message type received: {messageType}");
            }
        }

        private MessageFromPlayerEventArgs GetMessageReceivedArgs(IncomingMessage receivedMessage)
        {
            var senderId = receivedMessage.ReadPlayerId();
            var senderPlayer = this.CurrentRoom.GetPlayer<RemoteNetworkPlayer>(senderId);
            return new MessageFromPlayerEventArgs(senderPlayer, receivedMessage);
        }

        private void HandleCreateOrJoinResponse(IncomingMessage receivedMessage, ClientIncomingMessageTypes messageType)
        {
            var resultCode = receivedMessage.ReadEnterRoomResultCode();

            if (resultCode == EnterRoomResultCodes.Succeed)
            {
                if (messageType == ClientIncomingMessageTypes.CreateResponse)
                {
                    this.CurrentRoom = new LocalNetworkRoom(this.lastRoomOptionsRequest, this.LocalPlayer);
                }
                else
                {
                    this.CurrentRoom = LocalNetworkRoom.FromJoinMessage(receivedMessage, this.LocalPlayer, this.lastRoomOptionsRequest);
                }

                this.State = ClientStates.Joined;
            }
            else
            {
                Debug.WriteLine($"[MatchmakingClient] EnterRoom response error: {resultCode}");

                this.State = ClientStates.InLobby;
            }

            this.enterRoomTCS?.TrySetResult(resultCode);
            this.enterRoomTCS = null;
            this.lastRoomOptionsRequest = null;
        }

        private void HandleRefreshRoomsInLobby(IncomingMessage receivedMessage)
        {
            var refreshMessage = RefreshLobbyRoomsMessage.FromMessage(receivedMessage);

            if (refreshMessage.IsAbsolute)
            {
                this.roomsInLobby.Clear();
            }

            foreach (var roomInfo in refreshMessage.IncludedRooms)
            {
                this.roomsInLobby[roomInfo.Name] = roomInfo;
            }

            foreach (var roomName in refreshMessage.RemovedRooms)
            {
                this.roomsInLobby.Remove(roomName);
            }

            this.RoomsInLobbySynchronized?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
