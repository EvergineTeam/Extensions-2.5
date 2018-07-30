// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// A component that binds an Vuforia VuMark target with an entity
    /// </summary>
    [DataContract]
    public class ARVuMarkTrackableBehavior : ARTrackableBehavior
    {
        /// <summary>
        /// Gets enumerable that contains the names of the VuMark trackables described by the dataset.
        /// </summary>
        [DontRenderProperty]
        public new IEnumerable<string> AvailableTrackables
        {
            get
            {
                var vuforiaService = WaveServices.GetService<VuforiaService>();

                if (vuforiaService != null)
                {
                    return vuforiaService.ParseTrackableNames()
                                         .Where(t => t.Value == TargetTypes.VuMark)
                                         .Select(t => t.Key);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the ID data type that this entity should match.
        /// </summary>
        [DataMember]
        [RenderProperty(
            Tag = 1,
            Tooltip = "The ID data type that this entity should match")]
        public VuMarkDataTypes IDType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the numeric ID value that match with this entity if the datatype
        /// is marked as a <see cref="VuMarkDataTypes.Numeric"/>.
        /// </summary>
        [DataMember]
        [RenderProperty(
            AttatchToTag = 1,
            AttachToValue = VuMarkDataTypes.Numeric,
            CustomPropertyName = "Numeric ID",
            Tooltip = "The numeric ID value that match with this entity if the datatype is marked as a 'Numeric'")]
        public ulong NumericValue
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the string ID value that match with this entity if the datatype
        /// is marked as a <see cref="VuMarkDataTypes.String"/>.
        /// </summary>
        [DataMember]
        [RenderProperty(
            AttatchToTag = 1,
            AttachToValue = VuMarkDataTypes.String,
            CustomPropertyName = "String ID",
            Tooltip = "The string ID value that match with this entity if the datatype is marked as a 'String'")]
        public string StringValue
        {
            get; set;
        }

        /// <summary>
        /// Returns the TrackableResult that match this trackable.
        /// </summary>
        /// <param name="detectedTrackables">The detected trackables.</param>
        /// <returns>The TrackableResult that match this trackable.</returns>
        internal override TrackableResult FindMatchedTrackable(IEnumerable<TrackableResult> detectedTrackables)
        {
            var detectedVuMarks = detectedTrackables
                        .Where(tr => tr is VuMarkTargetResult &&
                                     tr.Trackable.Name == this.TrackableName)
                        .Cast<VuMarkTargetResult>();

            TrackableResult machedTrackable = null;
            foreach (var vtr in detectedVuMarks)
            {
                if (this.IDType == vtr.DataType)
                {
                    if (this.IDType == VuMarkDataTypes.Bytes ||
                        (this.IDType == VuMarkDataTypes.String && vtr.StringValue == this.StringValue) ||
                        (this.IDType == VuMarkDataTypes.Numeric && vtr.NumericValue == this.NumericValue))
                    {
                        machedTrackable = vtr;
                        break;
                    }
                }
            }

            return machedTrackable;
        }
    }
}
