#region File Description
//-----------------------------------------------------------------------------
// SpineEvent
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using Spine;
#endregion


namespace WaveEngine.Spine
{
    /// <summary>
    /// This class represent a spine event
    /// </summary>
    public class SpineEvent
    {
        /// <summary>
        /// Gets the event name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the int value of the event
        /// </summary>
        public int Int { get; private set; }

        /// <summary>
        /// Gets the float value of the event
        /// </summary>
        public float Float { get; private set; }

        /// <summary>
        /// Gets the string value of the event
        /// </summary>
        public string String { get; private set; }

        /// <summary>
        /// Gets the time value of the event
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpineEvent" /> class.
        /// </summary>
        /// <param name="e">The spine event</param>
        internal SpineEvent(Event e)
        {
            this.Name = e.Data.Name;
            this.Int = e.Data.Int;
            this.Float = e.Data.Float;
            this.String = e.Data.String;
            this.Time = e.Time;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
