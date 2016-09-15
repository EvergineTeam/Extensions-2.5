#region File Description
//-----------------------------------------------------------------------------
// StreamResolution
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Windows.Media.MediaProperties;
#endregion

namespace WaveEngine.Hololens.Media
{
    /// <summary>
    /// This class represent a supported resolution.
    /// </summary>
    public class StreamResolution : ICameraResolution
    {
        private IMediaEncodingProperties properties;

        #region Properties
        /// <summary>
        /// Gets the encoding properties.
        /// </summary>
        public IMediaEncodingProperties EncodingProperties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// The width resolution.
        /// </summary>
        public uint Width
        {
            get
            {
                if (this.properties is ImageEncodingProperties)
                {
                    return (this.properties as ImageEncodingProperties).Width;
                }
                else if (this.properties is VideoEncodingProperties)
                {
                    return (this.properties as VideoEncodingProperties).Width;
                }

                return 0;
            }
        }

        /// <summary>
        /// The height resolution.
        /// </summary>
        public uint Height
        {
            get
            {
                if (this.properties is ImageEncodingProperties)
                {
                    return (this.properties as ImageEncodingProperties).Height;
                }
                else if (this.properties is VideoEncodingProperties)
                {
                    return (this.properties as VideoEncodingProperties).Height;
                }

                return 0;
            }
        }

        /// <summary>
        /// The frames per seconds.
        /// </summary>
        public uint RefreshRate
        {
            get
            {
                if (this.properties is VideoEncodingProperties)
                {
                    if ((this.properties as VideoEncodingProperties).FrameRate.Denominator != 0)
                    {
                        return (this.properties as VideoEncodingProperties).FrameRate.Numerator / (this.properties as VideoEncodingProperties).FrameRate.Denominator;
                    }
                }

                return 0;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamResolution"/> class.
        /// </summary>
        /// <param name="properties"></param>
        internal StreamResolution(IMediaEncodingProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (!(properties is ImageEncodingProperties) && !(properties is VideoEncodingProperties))
            {
                throw new ArgumentException("Argument is of the wrong type. Required: " + typeof(ImageEncodingProperties).Name + " or " + typeof(VideoEncodingProperties).Name);
            }

            this.properties = properties;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The result string.</returns>
        public override string ToString()
        {
            return $"X: {Width} Y: {Height} FPS: {RefreshRate}";
        }
        #endregion
    }
}
