// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace WaveEngine.Vuforia.QCAR
{
    /// <summary>
    /// Represents a Vuforia trackable result
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QCAR_TrackableResult
    {
        /// <summary>
        /// Maximum number of bytes for a trackable data
        /// </summary>
        public const int TRACK_DATA_SIZE = 100;

        /// <summary>
        /// Identifier of the trackable
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int Id;

        /// <summary>
        /// The status of the trackable result
        /// </summary>
        public TrackableStatus Status;

        /// <summary>
        /// The trackable pose matrix
        /// </summary>
        public QCAR_Matrix4x4 TrackPose;

        #region VuMark fields

        /// <summary>
        /// Template identifier of the Vumark target
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int TemplateId;

        /// <summary>
        /// Byte buffer filled with a number of bytes containing the InstanceId.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TRACK_DATA_SIZE)]
        public byte[] Data;

        /// <summary>
        /// The ID as unsigned long long if ID DataType is <see cref="VuMarkDataTypes.Numeric"/>, otherwise is 0.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint NumericValue;

        /// <summary>
        /// The length of the data.
        ///  If the instance ID data type is
        ///   - Bytes: The number of bytes in the ID.
        ///   - String: The maximum number of characters that could be returned, not
        ///             counting the ending null.  If copying the string data into
        ///             your own buffer, allocate this length + 1.
        ///   - Numeric: The number of bytes needed to store the ID's numeric value
        ///              (e.g. 8 bytes for 64 bits).
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint DataSize;

        /// <summary>
        /// The type of data this instance ID stores.
        /// </summary>
        public VuMarkDataTypes DataType;

        #endregion
    }
}
