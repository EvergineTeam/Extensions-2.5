// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a <see cref="int"/> property contained on a
    /// <see cref="NetworkPropertiesTable"/>. A <see cref="NetworkPropertiesTable"/> component is needed in the same
    /// entity or any of its parents
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkIntegerPropertySync<K> : NetworkPropertySync<K, int>
        where K : struct, IConvertible
    {
        #region Private Methods

        /// <inheritdoc />
        protected override int ReadValue(NetworkPropertiesTable propertiesTable)
        {
            return propertiesTable.GetInt32(this.propertyKey);
        }

        /// <inheritdoc />
        protected override void WriteValue(NetworkPropertiesTable propertiesTable, int value)
        {
            propertiesTable.Set(this.propertyKey, value);
        }

        #endregion
    }
}
