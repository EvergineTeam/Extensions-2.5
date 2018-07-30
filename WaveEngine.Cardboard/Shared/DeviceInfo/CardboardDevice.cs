// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// The cardboard device
    /// </summary>
    public class CardboardDevice
    {
        /// <summary>
        /// Gets the with
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Gets the with
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// Gets the with in meters
        /// </summary>
        public float WidthMeters { get; private set; }

        /// <summary>
        /// Gets the height in meters
        /// </summary>
        public float HeightMeters { get; private set; }

        /// <summary>
        /// Gets the bevel in meters
        /// </summary>
        public float BevelMeters { get; private set; }

        /// <summary>
        /// Gets the default Android
        /// </summary>
        public static CardboardDevice DefaultAndroid { get; private set; }

        /// <summary>
        /// Gets the default iOS
        /// </summary>
        public static CardboardDevice DefaultIOS { get; private set; }

        static CardboardDevice()
        {
            DefaultAndroid = new CardboardDevice()
            {
                WidthMeters = 0.110f,
                HeightMeters = 0.062f,
                BevelMeters = 0.004f
            };

            DefaultIOS = new CardboardDevice()
            {
                WidthMeters = 0.1038f,
                HeightMeters = 0.0584f,
                BevelMeters = 0.004f
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardboardDevice" /> class.
        /// </summary>
        public CardboardDevice()
        {
            this.Width = WaveServices.Platform.ScreenWidth;
            this.Height = WaveServices.Platform.ScreenHeight;
        }
    }
}
