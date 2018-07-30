// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Client.Players;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Players
{
    /// <summary>
    /// Base class for network players
    /// </summary>
    public abstract class BaseNetworkPlayer : INetworkPlayer
    {
        /// <summary>
        /// Player field flags used for sync.
        /// </summary>
        [Flags]
        protected enum PlayerFliedsFlags : byte
        {
            /// <summary>
            /// No field flag
            /// </summary>
            None = 0,

            /// <summary>
            /// Nickname field flag
            /// </summary>
            Nickname = 2,

            /// <summary>
            /// Custom properties field flag
            /// </summary>
            CustomProperties = 4,

            /// <summary>
            /// All fields flag
            /// </summary>
            All = byte.MaxValue
        }

        #region Properties

        /// <inheritdoc />
        public int Id { get; internal set; }

        /// <inheritdoc />
        public string Nickname { get; protected set; }

        /// <inheritdoc />
        public NetworkPropertiesTable CustomProperties { get; private set; }

        /// <inheritdoc />
        public object TagObject { get; set; }

        /// <inheritdoc />
        public bool IsMasterClient
        {
            get
            {
                if (this.Room == null)
                {
                    return false;
                }

                return this.Id == this.Room.MasterClientId;
            }
        }

        /// <inheritdoc />
        public bool IsInLobby
        {
            get
            {
                return this.Room == null;
            }
        }

        /// <inheritdoc />
        public bool IsLocalPlayer
        {
            get
            {
                return this is LocalNetworkPlayer;
            }
        }

        /// <inheritdoc />
        public INetworkRoom Room
        {
            get; internal set;
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler OnNicknameChanged;

        /// <inheritdoc />
        public event EventHandler OnCustomPropertiesChanged;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNetworkPlayer" /> class.
        /// </summary>
        /// <param name="hasReadOnlyProperties">Indicates if the custom properties of the player are read only</param>
        public BaseNetworkPlayer(bool hasReadOnlyProperties)
        {
            this.Id = -1;
            this.CustomProperties = new NetworkPropertiesTable(hasReadOnlyProperties);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh the player fields based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadFromMessage(IncomingMessage message)
        {
            var changedFields = (PlayerFliedsFlags)message.ReadByte();

            if (changedFields.HasFlag(PlayerFliedsFlags.Nickname))
            {
                this.Nickname = message.ReadString();

                this.OnNicknameChanged?.Invoke(this, EventArgs.Empty);
            }

            if (changedFields.HasFlag(PlayerFliedsFlags.CustomProperties))
            {
                this.CustomProperties.ReadFromMessage(message);

                this.OnCustomPropertiesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Writes the specified fields to an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="includedFields">Indicates the fields that must synchronized</param>
        protected void WriteToMessage(OutgoingMessage message, PlayerFliedsFlags includedFields)
        {
            message.Write((byte)includedFields);

            if (includedFields.HasFlag(PlayerFliedsFlags.Nickname))
            {
                message.Write(this.Nickname);
            }

            if (includedFields.HasFlag(PlayerFliedsFlags.CustomProperties))
            {
                this.CustomProperties.WriteToMessage(message);
            }
        }

        #endregion
    }
}
