// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a <see cref="Vector4"/> property contained on a
    /// <see cref="NetworkPropertiesTable"/>. A <see cref="NetworkPropertiesTable"/> component is needed in the same
    /// entity or any of its parents
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkVector4PropertySync<K> : NetworkPropertySync<K, Vector4>
        where K : struct, IConvertible
    {
        #region Private Methods

        /// <inheritdoc />
        protected override Vector4 ReadValue(NetworkPropertiesTable propertiesTable)
        {
            return propertiesTable.GetVector4(this.propertyKey);
        }

        /// <inheritdoc />
        protected override void WriteValue(NetworkPropertiesTable propertiesTable, Vector4 value)
        {
            propertiesTable.Set(this.propertyKey, value);
        }

        #endregion
    }
}
