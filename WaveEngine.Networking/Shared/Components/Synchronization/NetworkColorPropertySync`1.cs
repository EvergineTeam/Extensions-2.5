// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Provides an abstraction to track changes on a <see cref="Color"/> property contained on a
    /// <see cref="NetworkPropertiesTable"/>. A <see cref="NetworkPropertiesTable"/> component is needed in the same
    /// entity or any of its parents
    /// </summary>
    /// <typeparam name="K">The type of the property key. Must be <see cref="byte"/> or <see cref="Enum"/></typeparam>
    [DataContract]
    [AllowMultipleInstances]
    public abstract class NetworkColorPropertySync<K> : NetworkPropertySync<K, Color>
        where K : struct, IConvertible
    {
        #region Private Methods

        /// <inheritdoc />
        protected override Color ReadValue(NetworkPropertiesTable propertiesTable)
        {
            return propertiesTable.GetColor(this.propertyKey);
        }

        /// <inheritdoc />
        protected override void WriteValue(NetworkPropertiesTable propertiesTable, Color value)
        {
            propertiesTable.Set(this.propertyKey, value);
        }

        #endregion
    }
}
