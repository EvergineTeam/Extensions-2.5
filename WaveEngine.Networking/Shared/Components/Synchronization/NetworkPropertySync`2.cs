// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a property contained on a <see cref="NetworkPropertiesTable"/>.
    /// A <see cref="NetworkPropertiesTable"/> component is needed in the same entity or any of its parents.
    /// This class simplifies the access to the property value with the methods
    /// <see cref="ReadValue(NetworkPropertiesTable)"/> and <see cref="WriteValue(NetworkPropertiesTable, V)"/>
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    /// <typeparam name="V">The type of the property value</typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkPropertySync<K, V> : Component, IDisposable
        where K : struct, IConvertible
    {
        /// <summary>
        /// The internal properties table
        /// </summary>
        private NetworkPropertiesTable propertiesTable;

        /// <summary>
        /// The <see cref="NetworkCustomPropertiesProvider"/> provider
        /// </summary>
        protected NetworkCustomPropertiesProvider propertiesTableProvider;

        /// <summary>
        /// The internal property key stored as a byte
        /// </summary>
        [DataMember]
        protected byte propertyKey;

        /// <summary>
        /// The internal property provider filter
        /// </summary>
        [DataMember]
        private NetworkPropertyProviderFilter providerFilter;

        #region Properties

        /// <summary>
        /// Gets or sets the key of the custom property.
        /// </summary>
        [RenderProperty(Tooltip = "The key of the custom property")]
        public K PropertyKey
        {
            get
            {
                return Unsafe.As<byte, K>(ref this.propertyKey);
            }

            set
            {
                var newValue = Convert.ToByte(value);
                if (this.propertyKey != newValue)
                {
                    this.propertyKey = value.ToByte(null);

                    if (this.isInitialized)
                    {
                        this.ForcePropertyCheck();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that restricts which <see cref="NetworkCustomPropertiesProvider"/> is valid during
        /// the resolution of dependencies
        /// </summary>
        [RenderProperty(Tooltip = "Restricts which provider is valid during the resolution of dependencies")]
        public NetworkPropertyProviderFilter ProviderFilter
        {
            get
            {
                return this.providerFilter;
            }

            set
            {
                if (this.propertiesTableProvider != null)
                {
                    throw new InvalidOperationException($"The {nameof(this.ProviderFilter)} property cannot be changed once the dependency has been resolved");
                }

                this.providerFilter = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the custom property table has a value defined for this <see cref="PropertyKey"/>.
        /// </summary>
        [DontRenderProperty]
        public bool HasValue
        {
            get { return this.propertiesTable.ContainsKey(this.propertyKey); }
        }

        /// <summary>
        /// Gets or sets the value of the custom property.
        /// </summary>
        [DontRenderProperty]
        public V PropertyValue
        {
            get
            {
                if (!this.propertiesTable.ContainsKey(this.propertyKey))
                {
                    return default(V);
                }

                return this.ReadValue(this.propertiesTable);
            }

            set
            {
                this.WriteValue(this.propertiesTable, value);
            }
        }

        #endregion

        #region Initialize

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.providerFilter = NetworkPropertyProviderFilter.Any;

            Type keyType = typeof(K);
            if (!keyType.IsEnum() &&
                 keyType != typeof(byte))
            {
                throw new InvalidOperationException($"{typeof(K)} must be byte or {nameof(System.Enum)}");
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public virtual void Dispose()
        {
            this.UnsubscribeNetworkEvents();
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            if (this.propertiesTableProvider == null &&
                !WaveServices.Platform.IsEditor)
            {
                this.propertiesTableProvider = this.FindNetworkCustomPropertiesProvider();
                this.propertiesTableProvider.CustomPropertiesRefreshed += this.PropertiesTableProvider_CustomPropertiesRefreshed;
                this.PropertiesTableProvider_CustomPropertiesRefreshed(this, null);
            }
        }

        /// <summary>
        /// Called every time a value with the target property key is added or modified
        /// </summary>
        protected abstract void OnPropertyAddedOrChanged();

        /// <summary>
        /// Called every time a value with the target property key is removed
        /// </summary>
        protected abstract void OnPropertyRemoved();

        /// <summary>
        /// Defines a delegate to read the value from the player <see cref="NetworkPropertiesTable"/>
        /// </summary>
        /// <param name="propertiesTable">The properties table that contains the player custom properties</param>
        /// <returns>The property value</returns>
        protected abstract V ReadValue(NetworkPropertiesTable propertiesTable);

        /// <summary>
        /// Defines a delegate to write the value in the player <see cref="NetworkPropertiesTable"/>
        /// </summary>
        /// <param name="propertiesTable">The properties table that contains the player custom properties</param>
        /// <param name="value">The value to write</param>
        protected abstract void WriteValue(NetworkPropertiesTable propertiesTable, V value);

        /// <inheritdoc />
        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.UnsubscribeNetworkEvents();
        }

        private NetworkCustomPropertiesProvider FindNetworkCustomPropertiesProvider()
        {
            Type providerType;
            switch (this.providerFilter)
            {
                default:
                case NetworkPropertyProviderFilter.Any:
                    providerType = typeof(NetworkCustomPropertiesProvider);
                    break;
                case NetworkPropertyProviderFilter.Player:
                    providerType = typeof(NetworkPlayerProvider);
                    break;
                case NetworkPropertyProviderFilter.Room:
                    providerType = typeof(NetworkRoomProvider);
                    break;
            }

            var provider = this.Owner?.FindComponentInParents(providerType, false);

            var otherProviders = provider?.Owner?.FindComponents(providerType, false);
            if (otherProviders != null &&
                otherProviders.Skip(1).Any())
            {
                throw new InvalidOperationException($"More than one {providerType.Name} has been found within the same entity");
            }

            return (NetworkCustomPropertiesProvider)provider;
        }

        private void UnsubscribeNetworkEvents()
        {
            if (this.propertiesTableProvider != null)
            {
                this.propertiesTableProvider.CustomPropertiesRefreshed -= this.PropertiesTableProvider_CustomPropertiesRefreshed;
            }

            if (this.propertiesTable != null)
            {
                this.UnsubsribeFromPropertiesTable(this.propertiesTable);
            }
        }

        private void UnsubsribeFromPropertiesTable(NetworkPropertiesTable propertiesTable)
        {
            propertiesTable.PropertyAdded -= this.CustomProperties_PropertyAddedOrChanged;
            propertiesTable.PropertyChanged -= this.CustomProperties_PropertyAddedOrChanged;
            propertiesTable.PropertyRemoved -= this.CustomProperties_PropertyRemoved;
        }

        private void ForcePropertyCheck()
        {
            if (this.propertiesTable != null &&
                this.propertiesTable.ContainsKey(this.propertyKey))
            {
                this.CustomProperties_PropertyAddedOrChanged(this, this.propertyKey);
            }
        }

        private void PropertiesTableProvider_CustomPropertiesRefreshed(object sender, NetworkPropertiesTable previousPropertyTable)
        {
            if (previousPropertyTable != null)
            {
                this.UnsubsribeFromPropertiesTable(previousPropertyTable);
            }

            this.propertiesTable = this.propertiesTableProvider?.CustomProperties;
            if (this.propertiesTable != null)
            {
                this.propertiesTable.PropertyAdded += this.CustomProperties_PropertyAddedOrChanged;
                this.propertiesTable.PropertyChanged += this.CustomProperties_PropertyAddedOrChanged;
                this.propertiesTable.PropertyRemoved += this.CustomProperties_PropertyRemoved;
            }

            this.ForcePropertyCheck();
        }

        private void CustomProperties_PropertyAddedOrChanged(object sender, byte key)
        {
            if (this.propertyKey == key)
            {
                this.OnPropertyAddedOrChanged();
            }
        }

        private void CustomProperties_PropertyRemoved(object sender, byte key)
        {
            if (this.propertyKey == key)
            {
                this.OnPropertyRemoved();
            }
        }

        #endregion
    }
}
