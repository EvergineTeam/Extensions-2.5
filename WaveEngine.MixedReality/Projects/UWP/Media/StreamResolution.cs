// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using Windows.Media.MediaProperties;
#endregion

namespace WaveEngine.MixedReality.Media
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
        /// Gets the width resolution.
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
        /// Gets the height resolution.
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
        /// Gets the frames per seconds.
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
        /// <param name="properties">The properties</param>
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
            return $"X: {this.Width} Y: {this.Height} FPS: {this.RefreshRate}";
        }
        #endregion
    }
}
