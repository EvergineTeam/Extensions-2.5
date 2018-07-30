// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.MixedReality.Speech
{
    /// <summary>
    /// KeywordRecognizerResult
    /// </summary>
    public sealed class KeywordRecognizerResult : EventArgs
    {
        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        public ConfidenceLevel Confidence { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordRecognizerResult"/> class.
        /// Initilizes a new instance of the <see cref="KeywordRecognizerResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="confidence">The simplified confidence.</param>
        public KeywordRecognizerResult(string text, ConfidenceLevel confidence)
        {
            this.Text = text;
            this.Confidence = confidence;
        }
    }
}
