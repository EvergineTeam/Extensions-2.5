// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a <see cref="TimeSpan"/> property contained on a
    /// <see cref="NetworkPropertiesTable"/>. A <see cref="NetworkPropertiesTable"/> component is needed in the same
    /// entity or any of its parents
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkTimeSpanPropertySync<K> : NetworkPropertySync<K, TimeSpan>
        where K : struct, IConvertible
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TimeSpan"/> will be stored using high precision
        /// (ticks) or not (milliseconds)
        /// </summary>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to
        /// "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds)
        /// </remarks>
        [DataMember]
        [RenderProperty(
            Tooltip = "Indicates whether the TimeSpan will be stored using high precision (ticks) or not (milliseconds)")]
        public bool HighPrecision { get; set; }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override TimeSpan ReadValue(NetworkPropertiesTable propertiesTable)
        {
            return propertiesTable.GetTimeSpan(this.propertyKey, this.HighPrecision);
        }

        /// <inheritdoc />
        protected override void WriteValue(NetworkPropertiesTable propertiesTable, TimeSpan value)
        {
            propertiesTable.Set(this.propertyKey, value, this.HighPrecision);
        }

        #endregion
    }
}
