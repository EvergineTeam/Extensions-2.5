// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// The type of VuMark data encoded in this ID.
    /// </summary>
    public enum VuMarkDataTypes
    {
        /// <summary>
        /// Generic byte data, stored in little-endian order in the buffer.
        /// For example, and ID of 0x123456 would appear as { 0x56, 0x34, 0x12 }
        /// </summary>
        Bytes = 0,

        /// <summary>
        /// Printable string data in ASCII.
        /// </summary>
        String,

        /// <summary>
        /// Numeric data, not larger than a 64 bit unsigned long long.
        /// </summary>
        Numeric
    }
}
