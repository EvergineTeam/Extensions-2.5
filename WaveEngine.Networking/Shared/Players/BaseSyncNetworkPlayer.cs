// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking.Players
{
    /// <summary>
    /// This class represents a player, whose properties can be modified and synchronized.
    /// </summary>
    public abstract class BaseSyncNetworkPlayer : BaseNetworkPlayer
    {
        /// <summary>
        /// Indicates what fields need to be synchronized.
        /// </summary>
        private PlayerFliedsFlags pendingSyncFields;

        #region Properties

        /// <summary>
        /// Gets or sets a non-unique nickname of the player. Synced automatically in a room.
        /// </summary>
        /// <remarks>
        /// A player might change his own nickname in a room.
        /// Setting this value updates the server and other players.
        /// </remarks>
        public new string Nickname
        {
            get
            {
                return base.Nickname;
            }

            set
            {
                if (base.Nickname != value)
                {
                    base.Nickname = value;

                    this.pendingSyncFields |= PlayerFliedsFlags.Nickname;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the player needs to be sync or not.
        /// </summary>
        internal bool NeedSync
        {
            get
            {
                return this.pendingSyncFields != PlayerFliedsFlags.None ||
                    this.CustomProperties.NeedSync;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSyncNetworkPlayer" /> class.
        /// </summary>
        public BaseSyncNetworkPlayer()
            : base(hasReadOnlyProperties: false)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the fields to be sync on an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="forceAllFields">Indicates if all fields must be forced to sync</param>
        internal void WriteSyncMessage(OutgoingMessage message, bool forceAllFields = false)
        {
            if (forceAllFields)
            {
                this.pendingSyncFields = PlayerFliedsFlags.All;
                this.CustomProperties.ForceFullSync();
            }
            else if (this.CustomProperties.NeedSync)
            {
                this.pendingSyncFields |= PlayerFliedsFlags.CustomProperties;
            }

            this.WriteToMessage(message, this.pendingSyncFields);

            this.pendingSyncFields = PlayerFliedsFlags.None;
        }

        #endregion
    }
}
