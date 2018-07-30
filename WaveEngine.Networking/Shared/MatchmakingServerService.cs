// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Networking.Connection;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
using WaveEngine.Networking.Server.Players;
using WaveEngine.Networking.Server.Rooms;
#endregion

namespace WaveEngine.Networking.Server
{
    /// <summary>
    /// The server service for matchmaking
    /// </summary>
    [DataContract]
    public class MatchmakingServerService : UpdatableService
    {
        /// <summary>
        /// The network factory used to create the internal network peer
        /// </summary>
        private INetworkFactory networkFactory;

        /// <summary>
        /// The network server peer
        /// </summary>
        private INetworkServer networkServer;

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

        /// <summary>
        /// All the players connected to the matchmaking server.
        /// </summary>
        private Dictionary<int, ServerPlayer> connectedPlayers;

        /// <summary>
        /// Active rooms in the lobby. It contains ServerRoom per roomName (keys).
        /// </summary>
        private Dictionary<string, ServerRoom> lobbyRooms;

        #region Properties

        /// <summary>
        /// Gets or sets the factory used to create the <see cref="INetworkServer"/> peer.
        /// </summary>
        public INetworkFactory NetworkFactory
        {
            get
            {
                return this.networkFactory;
            }

            set
            {
                if (this.networkServer != null)
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
                this.CheckServerInitializationForProperty(nameof(this.ApplicationIdentifier));
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
                this.CheckServerInitializationForProperty(nameof(this.ClientApplicationVersion));
                this.clientApplicationVersion = value;
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
                this.CheckServerInitializationForProperty(nameof(this.PingInterval));
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
                this.CheckServerInitializationForProperty(nameof(this.ConnectionTimeout));
                this.connectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets all the players connected to the matchmaking service
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<ServerPlayer> AllConnectedPlayers
        {
            get
            {
                return this.connectedPlayers.Values;
            }
        }

        /// <summary>
        /// Gets all the players that are in the lobby
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<ServerPlayer> PlayersInLobby
        {
            get
            {
                return this.connectedPlayers.Values.Where(p => p.IsInLobby);
            }
        }

        /// <summary>
        /// Gets all the players that are in the lobby
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<ServerRoom> AllRooms
        {
            get
            {
                return this.lobbyRooms.Values;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a new player connects with the server
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerConnected;

        /// <summary>
        /// Occurs when a player disconnects from the server
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerDisconnected;

        /// <summary>
        /// Occurs when a connected player is joining to an existing room
        /// </summary>
        public event EventHandler<PlayerJoiningEventArgs> PlayerJoining;

        /// <summary>
        /// Occurs when a connected player joins in an existing room
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerJoined;

        /// <summary>
        /// Occurs when a connected player is leaving an existing room
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerLeaving;

        /// <summary>
        /// Occurs when a connected player left an existing room
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerLeft;

        /// <summary>
        /// Occurs when the server receives changes of the properties of a player
        /// </summary>
        public event EventHandler<ServerPlayer> PlayerSynchronized;

        /// <summary>
        /// Occurs when the server creates a new room in the lobby
        /// </summary>
        public event EventHandler<ServerRoom> RoomCreated;

        /// <summary>
        /// Occurs when the server destroys an existing room in the lobby
        /// </summary>
        public event EventHandler<ServerRoom> RoomDestroyed;

        /// <summary>
        /// Occurs when the server receives changes of the properties of a room
        /// </summary>
        public event EventHandler<ServerRoom> RoomSynchronized;

        /// <summary>
        /// Occurs when a user data message from a client is received by the host
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedFromClient;

        #endregion

        #region Initialize

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            this.connectedPlayers = new Dictionary<int, ServerPlayer>();
            this.lobbyRooms = new Dictionary<string, ServerRoom>();
            this.pingInterval = TimeSpan.FromSeconds(4);
            this.connectionTimeout = TimeSpan.FromSeconds(25);
            this.networkFactory = new NetworkFactory();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the matchmaking server on the specified port.
        /// </summary>
        /// <param name="port">The port to bind to.</param>
        /// <exception cref="System.ArgumentException">Application identifier parameter can't be null or empty.</exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="ApplicationIdentifier"/> must have a valid value before initialize.
        /// or
        /// You can't call multiple times to Start method.
        /// </exception>
        public void Start(int port)
        {
            if (string.IsNullOrEmpty(this.ApplicationIdentifier))
            {
                throw new InvalidOperationException($"{nameof(this.ApplicationIdentifier)} must have a valid value before initialize.");
            }

            if (this.networkServer != null)
            {
                throw new InvalidOperationException("You can't call multiple times to InitializeServer method.");
            }

            try
            {
                var fullIdentifier = $"{this.ApplicationIdentifier}.{this.ClientApplicationVersion}";

                this.networkServer = this.networkFactory.CreateNetworkServer(
                    fullIdentifier,
                    port,
                    (float)this.pingInterval.TotalSeconds,
                    (float)this.connectionTimeout.TotalSeconds);

                this.networkServer.Start();
                this.networkServer.ClientConnecting += this.NetworkServer_ClientConnectionRequested;
                this.networkServer.ClientConnected += this.NetworkServer_ClientConnected;
                this.networkServer.ClientDisconnected += this.NetworkServer_ClientDisconnected;
                this.networkServer.MessageReceived += this.NetworkServer_MessageReceived;
            }
            catch
            {
                this.networkServer = null;
                throw;
            }
        }

        /// <summary>
        /// Shutdowns the matchmaking server on the specified port.
        /// </summary>
        public void Shutdown()
        {
            if (this.networkServer != null)
            {
                this.networkServer.Shutdown();
                this.networkServer.ClientConnecting -= this.NetworkServer_ClientConnectionRequested;
                this.networkServer.ClientConnected -= this.NetworkServer_ClientConnected;
                this.networkServer.ClientDisconnected -= this.NetworkServer_ClientDisconnected;
                this.networkServer.MessageReceived -= this.NetworkServer_MessageReceived;
                this.networkServer = null;

                this.connectedPlayers.Clear();
                this.lobbyRooms.Clear();
            }
        }

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <returns>The create outgoing message.</returns>
        public OutgoingMessage CreateMessage()
        {
            this.CheckServerIsStarted();

            return this.networkServer.CreateMessage();
        }

        /// <summary>
        /// Sends a user data message to the matchmaking server.
        /// </summary>
        /// <param name="messageToSend">The message to be sent.</param>
        /// <param name="destinationClient">The destination client endpoint.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void SendToClient(OutgoingMessage messageToSend, NetworkEndpoint destinationClient, DeliveryMethod deliveryMethod)
        {
            this.CheckServerIsStarted();

            var userDataMsg = this.CreateServerMessage(ClientIncomingMessageTypes.UserDataFromHost);
            userDataMsg.Write(messageToSend);
            this.networkServer.Send(userDataMsg, deliveryMethod, destinationClient);
        }

        /// <summary>
        /// Looks for a player with the specified endpoint.
        /// </summary>
        /// <param name="playerEndpoint">The player endpoint.</param>
        /// <returns>The <see cref="ServerPlayer"/> if the player can be found; otherwise, null.</returns>
        public ServerPlayer FindPlayer(NetworkEndpoint playerEndpoint)
        {
            ServerPlayer player;
            this.connectedPlayers.TryGetValue(playerEndpoint.GetHashCode(), out player);

            return player;
        }

        /// <inheritdoc />
        public override void Update(TimeSpan gameTime)
        {
            foreach (var player in this.connectedPlayers.Values)
            {
                var playerSyncMessage = this.CreateSyncPlayerMessage(player);
                if (playerSyncMessage != null)
                {
                    var msgToPlayer = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshLocalPlayerProperties);
                    msgToPlayer.Write(playerSyncMessage);

                    this.SendToPlayer(msgToPlayer, player);

                    if (player.Room != null)
                    {
                        var msgToPlayers = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshOtherPlayerProperties);
                        msgToPlayers.Write(player.Id);
                        msgToPlayers.Write(playerSyncMessage);

                        var otherPlayers = this.GetOtherPlayersInRoom(player);
                        this.SendToPlayers(msgToPlayers, otherPlayers);
                    }
                }
            }

            foreach (var room in this.lobbyRooms.Values)
            {
                var roomSyncMessage = this.CreateSyncRoomMessage(room);
                if (roomSyncMessage != null)
                {
                    this.SendToPlayers(roomSyncMessage, room.AllPlayers);
                }
            }
        }

        #endregion

        #region Private Methods

        private void CheckServerIsStarted()
        {
            if (this.networkServer == null)
            {
                throw new InvalidOperationException($"Server is not started. Call Start method before any operation");
            }
        }

        private void CheckServerInitializationForProperty(string propertyName)
        {
            if (this.networkServer != null)
            {
                throw new InvalidOperationException($"{propertyName} cannot be changed once the server has been started");
            }
        }

        private void NetworkServer_ClientConnectionRequested(object sender, ClientConnectingEventArgs e)
        {
            var playerKey = e.ClientEndpoint.GetHashCode();

            if (this.connectedPlayers.ContainsKey(playerKey))
            {
                Debug.WriteLine("Client connection request rejected: The user is already registered");
                e.Reject();
                return;
            }

            if (e.HailMessage == null ||
                e.HailMessage.ReadServerIncomingMessageType() != ServerIncomingMessageTypes.SetPlayerProperties)
            {
                Debug.WriteLine("Client connection request rejected: SetPlayerProperties message not received");
                e.Reject();
                return;
            }

            var newPlayer = new ServerPlayer(playerKey, e.ClientEndpoint);
            newPlayer.ReadFromMessage(e.HailMessage);
            this.connectedPlayers.Add(playerKey, newPlayer);
        }

        private void NetworkServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            var newPlayer = this.FindPlayer(e.ClientEndpoint);
            var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomsInLobby);
            var refreshLobbyMessage = new RefreshLobbyRoomsMessage();
            var lobbyVisibleRoomInfos = this.lobbyRooms.Values.Where(r => r.IsVisible).Select(r => r.RoomInfo);
            refreshLobbyMessage.IsAbsolute = true;
            refreshLobbyMessage.IncludedRooms.AddRange(lobbyVisibleRoomInfos);
            refreshLobbyMessage.Write(othersRoomMsg);
            this.SendToPlayer(othersRoomMsg, newPlayer);

            this.PlayerConnected?.Invoke(this, newPlayer);
        }

        private void NetworkServer_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            var player = this.FindPlayer(e.ClientEndpoint);
            if (player != null)
            {
                this.HandlePlayerDisconection(player);
            }
        }

        private void NetworkServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var messageType = e.ReceivedMessage.ReadServerIncomingMessageType();

            switch (messageType)
            {
                case ServerIncomingMessageTypes.SetPlayerProperties:
                    this.HandleSetPlayerProperties(e.FromEndpoint, e.ReceivedMessage);
                    break;
                case ServerIncomingMessageTypes.SetRoomProperties:
                    this.HandleSetRoomProperties(e.FromEndpoint, e.ReceivedMessage);
                    break;
                case ServerIncomingMessageTypes.CreateRoomRequest:
                    this.HandleCreateRoomRequest(e.FromEndpoint, e.ReceivedMessage);
                    break;
                case ServerIncomingMessageTypes.JoinOrCreateRoomRequest:
                    this.HandleJoinRoomRequest(e.FromEndpoint, e.ReceivedMessage, true);
                    break;
                case ServerIncomingMessageTypes.JoinRoomRequest:
                    this.HandleJoinRoomRequest(e.FromEndpoint, e.ReceivedMessage, false);
                    break;
                case ServerIncomingMessageTypes.LeaveRoomRequest:
                    this.HandleLeaveRoomRequest(e.FromEndpoint);
                    break;
                case ServerIncomingMessageTypes.UserDataToHost:
                    this.MessageReceivedFromClient?.Invoke(this, e);
                    break;
                case ServerIncomingMessageTypes.UserDataToRoom:
                    this.HandleUserDataToRoom(e.FromEndpoint, e.ReceivedMessage);
                    break;
                case ServerIncomingMessageTypes.UserDataToOtherClient:
                    this.HandleUserDataToOtherClient(e.FromEndpoint, e.ReceivedMessage);
                    break;
                default:
                    // TODO: Instead of exception, answer with error
                    throw new InvalidOperationException($"Invalid server message type received: {messageType}");
            }
        }

        private void HandlePlayerDisconection(ServerPlayer player)
        {
            this.RemovePlayerFromRoom(player);
            this.PlayerDisconnected?.Invoke(this, player);
            this.connectedPlayers.Remove(player.ServerKey);
        }

        private void HandleSetPlayerProperties(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);

            if (fromPlayer != null)
            {
                OutgoingMessage refreshPlayerPropsMsg = null;

                if (fromPlayer.Room != null)
                {
                    // Send to players in the same room
                    refreshPlayerPropsMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshOtherPlayerProperties);
                    refreshPlayerPropsMsg.Write(fromPlayer.Id);
                    refreshPlayerPropsMsg.Write(receivedMessage);
                }

                fromPlayer.ReadFromMessage(receivedMessage);

                if (refreshPlayerPropsMsg != null)
                {
                    var otherPlayersInRoom = this.GetOtherPlayersInRoom(fromPlayer);
                    this.SendToPlayers(refreshPlayerPropsMsg, otherPlayersInRoom);
                }

                this.PlayerSynchronized?.Invoke(this, fromPlayer);
            }
        }

        private void HandleSetRoomProperties(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);

            if (fromPlayer != null &&
                fromPlayer.Room != null)
            {
                var playerRoom = fromPlayer.Room;
                var previousVisibility = playerRoom.IsVisible;

                var refreshRoomPropsMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshCurrentRoomProperties);
                refreshRoomPropsMsg.Write(receivedMessage);

                playerRoom.ReadFromMessage(receivedMessage);

                if (previousVisibility != playerRoom.IsVisible)
                {
                    // Update rooms in lobby to other users
                    var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomsInLobby);
                    var refreshLobbyMessage = new RefreshLobbyRoomsMessage();
                    if (playerRoom.IsVisible)
                    {
                        refreshLobbyMessage.IncludedRooms.Add(playerRoom.RoomInfo);
                    }
                    else
                    {
                        refreshLobbyMessage.RemovedRooms.Add(playerRoom.Name);
                    }

                    refreshLobbyMessage.Write(othersRoomMsg);

                    var lobbyPlayers = this.GetPlayersInLobby();
                    this.SendToPlayers(othersRoomMsg, lobbyPlayers);
                }

                if (playerRoom.IsVisible &&
                    playerRoom.RoomInfo.NeedSync)
                {
                    var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomInLobbyProperties);
                    othersRoomMsg.Write(playerRoom.RoomInfo.Name);
                    playerRoom.RoomInfo.WriteSyncMessage(othersRoomMsg);

                    var lobbyPlayers = this.GetPlayersInLobby();
                    this.SendToPlayers(othersRoomMsg, lobbyPlayers);
                }

                // Send room changes to other players
                var otherPlayersInRoom = this.GetOtherPlayersInRoom(fromPlayer);
                this.SendToPlayers(refreshRoomPropsMsg, otherPlayersInRoom);

                this.RoomSynchronized?.Invoke(this, playerRoom);
            }
        }

        private void HandleCreateRoomRequest(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);

            if (!fromPlayer.IsInLobby)
            {
                return;
            }

            var roomOptions = new RoomOptions();
            roomOptions.Read(receivedMessage);

            var createResponse = this.CreateServerMessage(ClientIncomingMessageTypes.CreateResponse);

            if (this.lobbyRooms.ContainsKey(roomOptions.RoomName))
            {
                createResponse.Write(EnterRoomResultCodes.RoomAlreadyExists);
            }
            else
            {
                this.CreateRoom(roomOptions, fromPlayer);
                createResponse.Write(EnterRoomResultCodes.Succeed);
            }

            this.SendToPlayer(createResponse, fromPlayer);
        }

        private void HandleJoinRoomRequest(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage, bool createIfNotExists)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);

            if (!fromPlayer.IsInLobby)
            {
                return;
            }

            var roomOptions = new RoomOptions();
            roomOptions.Read(receivedMessage);

            OutgoingMessage joinResponse;

            ServerRoom existingRoom;
            if (this.lobbyRooms.TryGetValue(roomOptions.RoomName, out existingRoom))
            {
                joinResponse = this.CreateServerMessage(ClientIncomingMessageTypes.JoinResponse);

                if (existingRoom.IsFull)
                {
                    joinResponse.Write(EnterRoomResultCodes.RoomIsFull);
                }
                else
                {
                    var joinRejected = false;
                    var joiningHandler = this.PlayerJoining;
                    if (joiningHandler != null)
                    {
                        var eventArgs = new PlayerJoiningEventArgs(existingRoom, fromPlayer);
                        joiningHandler(this, eventArgs);
                        joinRejected = eventArgs.IsRejected;
                    }

                    if (joinRejected)
                    {
                        joinResponse.Write(EnterRoomResultCodes.Rejected);
                    }
                    else
                    {
                        existingRoom.AddPlayer(fromPlayer);

                        joinResponse.Write(EnterRoomResultCodes.Succeed);
                        existingRoom.WriteJoinToMessage(joinResponse, fromPlayer);

                        // Send RefreshRoomInLobbyProperties to players in lobby
                        if (existingRoom.IsVisible &&
                            existingRoom.RoomInfo.NeedSync)
                        {
                            var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomInLobbyProperties);
                            othersRoomMsg.Write(existingRoom.RoomInfo.Name);
                            existingRoom.RoomInfo.WriteSyncMessage(othersRoomMsg);

                            var lobbyPlayers = this.GetPlayersInLobby();
                            this.SendToPlayers(othersRoomMsg, lobbyPlayers);
                        }

                        // Send RefreshPlayersInRoom to players in the same room
                        var refreshPlayersInRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshPlayersInRoom);
                        existingRoom.WriteSyncPlayersListToMessage(refreshPlayersInRoomMsg);
                        var otherPlayersInRoom = this.GetOtherPlayersInRoom(fromPlayer);
                        this.SendToPlayers(refreshPlayersInRoomMsg, otherPlayersInRoom);

                        this.PlayerJoined?.Invoke(this, fromPlayer);
                    }
                }
            }
            else if (createIfNotExists)
            {
                joinResponse = this.CreateServerMessage(ClientIncomingMessageTypes.CreateResponse);
                this.CreateRoom(roomOptions, fromPlayer);
                joinResponse.Write(EnterRoomResultCodes.Succeed);
            }
            else
            {
                joinResponse = this.CreateServerMessage(ClientIncomingMessageTypes.JoinResponse);
                joinResponse.Write(EnterRoomResultCodes.RoomNotExists);
            }

            this.SendToPlayer(joinResponse, fromPlayer);
        }

        private void HandleLeaveRoomRequest(NetworkEndpoint fromEndpoint)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);
            var playerRoom = this.RemovePlayerFromRoom(fromPlayer);

            var responseMsg = this.CreateServerMessage(ClientIncomingMessageTypes.LeaveResponse);
            var refreshLobbyMessage = new RefreshLobbyRoomsMessage();
            var lobbyVisibleRoomInfos = this.lobbyRooms.Values.Where(r => r.IsVisible).Select(r => r.RoomInfo);
            refreshLobbyMessage.IsAbsolute = true;
            refreshLobbyMessage.IncludedRooms.AddRange(lobbyVisibleRoomInfos);
            refreshLobbyMessage.Write(responseMsg);
            this.SendToPlayer(responseMsg, fromPlayer);
        }

        private void HandleUserDataToRoom(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);
            if (fromPlayer.Room != null)
            {
                var userDataMsg = this.CreateServerMessage(ClientIncomingMessageTypes.UserDataFromRoom);
                userDataMsg.Write(fromPlayer.Id);
                userDataMsg.Write(receivedMessage);
                var otherPlayersInRoom = this.GetOtherPlayersInRoom(fromPlayer);
                this.SendToPlayers(userDataMsg, otherPlayersInRoom);
            }
        }

        private void HandleUserDataToOtherClient(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            var fromPlayer = this.FindPlayer(fromEndpoint);
            if (fromPlayer.Room != null)
            {
                var destinationPlayerId = receivedMessage.ReadPlayerId();

                var userDataMsg = this.CreateServerMessage(ClientIncomingMessageTypes.UserDataFromOtherClient);
                userDataMsg.Write(fromPlayer.Id);
                userDataMsg.Write(receivedMessage);
                var otherPlayer = fromPlayer.Room.GetPlayer<ServerPlayer>(destinationPlayerId);
                this.SendToPlayer(userDataMsg, otherPlayer);
            }
        }

        private void CreateRoom(RoomOptions options, ServerPlayer player)
        {
            var newRoom = new ServerRoom(options);
            newRoom.AddPlayer(player);

            this.lobbyRooms.Add(options.RoomName, newRoom);

            if (newRoom.IsVisible)
            {
                // Send RefreshRoomsInLobby to clients in lobby
                var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomsInLobby);
                var refreshLobbyMessage = new RefreshLobbyRoomsMessage();
                refreshLobbyMessage.IncludedRooms.Add(newRoom.RoomInfo);
                refreshLobbyMessage.Write(othersRoomMsg);

                var lobbyPlayers = this.GetPlayersInLobby();
                this.SendToPlayers(othersRoomMsg, lobbyPlayers);
            }

            this.RoomCreated?.Invoke(this, newRoom);
            this.PlayerJoined?.Invoke(this, player);
        }

        private ServerRoom RemovePlayerFromRoom(ServerPlayer player)
        {
            var room = player.Room;
            if (room != null)
            {
                this.PlayerLeaving?.Invoke(this, player);
                room.RemovePlayer(player);
                this.PlayerLeft?.Invoke(this, player);

                if (room.PlayerCount == 0)
                {
                    this.lobbyRooms.Remove(room.Name);

                    // Send RefreshRoomsInLobby to clients in lobby
                    var othersRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshRoomsInLobby);
                    var refreshLobbyMessage = new RefreshLobbyRoomsMessage();
                    refreshLobbyMessage.RemovedRooms.Add(room.Name);
                    refreshLobbyMessage.Write(othersRoomMsg);

                    var lobbyPlayers = this.GetPlayersInLobby();
                    this.SendToPlayers(othersRoomMsg, lobbyPlayers);

                    this.RoomDestroyed?.Invoke(this, room);
                }
                else
                {
                    // Send RefreshPlayersInRoom to players in the same room
                    var refreshPlayersInRoomMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshPlayersInRoom);
                    room.WriteSyncPlayersListToMessage(refreshPlayersInRoomMsg);
                    this.SendToPlayers(refreshPlayersInRoomMsg, room.AllPlayers);
                }
            }

            return room;
        }

        private IEnumerable<ServerPlayer> GetOtherPlayersInRoom(ServerPlayer fromPlayer)
        {
            var playerRoom = fromPlayer.Room;

            return playerRoom.AllPlayers.Where(p => p != fromPlayer);
        }

        private IEnumerable<ServerPlayer> GetPlayersInLobby()
        {
            return this.PlayersInLobby.Cast<ServerPlayer>();
        }

        private OutgoingMessage CreateSyncPlayerMessage(ServerPlayer player)
        {
            OutgoingMessage playerSyncMsg = null;

            var somethingToSync = player?.NeedSync ?? false;

            if (somethingToSync)
            {
                playerSyncMsg = this.networkServer.CreateMessage();
                player.WriteSyncMessage(playerSyncMsg);
            }

            return playerSyncMsg;
        }

        private OutgoingMessage CreateSyncRoomMessage(ServerRoom room)
        {
            OutgoingMessage roomSyncMsg = null;

            var somethingToSync = room?.NeedSync ?? false;

            if (somethingToSync)
            {
                roomSyncMsg = this.CreateServerMessage(ClientIncomingMessageTypes.RefreshCurrentRoomProperties);
                room.WriteSyncMessage(roomSyncMsg);
            }

            return roomSyncMsg;
        }

        private OutgoingMessage CreateServerMessage(ClientIncomingMessageTypes type)
        {
            var othersRoomMsg = this.networkServer.CreateMessage();
            othersRoomMsg.Write(type);
            return othersRoomMsg;
        }

        private void SendToPlayer(OutgoingMessage message, ServerPlayer player)
        {
            this.networkServer.Send(message, DeliveryMethod.ReliableOrdered, player.Endpoint);
        }

        private void SendToPlayers(OutgoingMessage message, IEnumerable<ServerPlayer> players)
        {
            var playersEPs = players.Select(p => p.Endpoint);
            this.networkServer.Send(message, DeliveryMethod.ReliableOrdered, playersEPs);
        }

        #endregion
    }
}
