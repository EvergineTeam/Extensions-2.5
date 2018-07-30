// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Target factory
    /// </summary>
    internal static class TargetFactory
    {
        /// <summary>
        /// Create a trackable based on a <see cref="QCAR_Trackable"/>.
        /// </summary>
        /// <param name="trackable">The Vuforia trackable</param>
        /// <returns>Returns a <see cref="Trackable"/>.</returns>
        internal static Trackable CreateTarget(QCAR_Trackable trackable)
        {
            Trackable result;

            switch (trackable.TargetType)
            {
                case TargetTypes.ImageTarget:
                    result = new ImageTarget(trackable);
                    break;
                case TargetTypes.VuMark:
                    result = new VuMarkTarget(trackable);
                    break;
                default:
                    throw new NotImplementedException("Invalid target type");
            }

            return result;
        }

        /// <summary>
        /// Create a trackable result based on a <see cref="QCAR_TrackableResult"/>.
        /// </summary>
        /// <param name="trackableResult">The Vuforia trackable result</param>
        /// <param name="dataset">The dataset that contains the definition of the targets</param>
        /// <returns>Returns a <see cref="TrackableResult"/>.</returns>
        internal static TrackableResult CreateTrackableResult(QCAR_TrackableResult trackableResult, DataSet dataset)
        {
            TrackableResult result;

            var trackable = dataset.Trackables.First(t => t.Id == trackableResult.TemplateId);

            if (trackable is ImageTarget)
            {
                result = new TrackableResult(trackableResult, trackable);
            }
            else if (trackable is VuMarkTarget)
            {
                result = new VuMarkTargetResult(trackableResult, (VuMarkTarget)trackable);
            }
            else
            {
                throw new NotImplementedException("Invalid target type");
            }

            return result;
        }
    }
}
