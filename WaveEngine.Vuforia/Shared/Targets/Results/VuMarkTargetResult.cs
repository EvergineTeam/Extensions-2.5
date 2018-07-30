// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Result for a VuMarkTarget.
    /// The same VuMarkTarget can have multiple physical instances on screen
    /// simultaneously. In this case each appearance has its own VuMarkTargetResult,
    ///  pointing to the same VuMarkTarget with the same instance ID.
    /// </summary>
    public class VuMarkTargetResult : TrackableResult
    {
        /// <summary>
        /// Gets a unique id for a particular VuMark result, which is consistent
        /// frame-to-frame, while being tracked. Note that this id is separate
        /// from the trackable id.
        /// </summary>
        public new int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the corresponding Trackable that this result represents
        /// </summary>
        public new VuMarkTarget Trackable
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of data the VuMarkResult contains.
        /// </summary>
        public VuMarkDataTypes DataType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw byte data.
        /// </summary>
        public byte[] RawValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the byte buffer as a 64bit unsigned long if the data type is
        /// marked as a <see cref="VuMarkDataTypes.Numeric"/>. 0 is returned otherwise.
        /// </summary>
        public ulong NumericValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the byte buffer as a 64bit unsigned long if the data type is
        /// marked as a <see cref="VuMarkDataTypes.String"/>. 0 is returned otherwise.
        /// </summary>
        public string StringValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VuMarkTargetResult" /> class.
        /// </summary>
        /// <param name="trackableResult">The trackable result.</param>
        /// <param name="vuMarkTarget">The VuMark target.</param>
        internal VuMarkTargetResult(QCAR_TrackableResult trackableResult, VuMarkTarget vuMarkTarget)
            : base(trackableResult, vuMarkTarget)
        {
            this.Trackable = vuMarkTarget;
            this.Refresh(trackableResult);
        }

        /// <summary>
        /// Refresh the current result with the new <see cref="QCAR_TrackableResult"/>.
        /// </summary>
        /// <param name="trackableResult">The new <see cref="QCAR_TrackableResult"/></param>
        internal override void Refresh(QCAR_TrackableResult trackableResult)
        {
            base.Refresh(trackableResult);

            this.Id = trackableResult.Id;
            this.RawValue = trackableResult.Data;
            this.NumericValue = trackableResult.NumericValue;
            this.DataType = trackableResult.DataType;

            if (this.DataType == VuMarkDataTypes.String)
            {
                this.StringValue = Encoding.ASCII.GetString(this.RawValue).TrimEnd('\0');
            }
            else
            {
                this.StringValue = string.Empty;
            }
        }
    }
}
