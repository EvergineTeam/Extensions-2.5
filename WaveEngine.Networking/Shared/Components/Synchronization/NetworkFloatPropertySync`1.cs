// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a <see cref="float"/> property contained on a
    /// <see cref="NetworkPropertiesTable"/>. A <see cref="NetworkPropertiesTable"/> component is needed in the same
    /// entity or any of its parents
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkFloatPropertySync<K> : NetworkPropertySync<K, float>
        where K : struct, IConvertible
    {
        #region Private Methods

        /// <inheritdoc />
        protected override float ReadValue(NetworkPropertiesTable propertiesTable)
        {
            return propertiesTable.GetFloat(this.propertyKey);
        }

        /// <inheritdoc />
        protected override void WriteValue(NetworkPropertiesTable propertiesTable, float value)
        {
            propertiesTable.Set(this.propertyKey, value);
        }

        #endregion
    }
}
