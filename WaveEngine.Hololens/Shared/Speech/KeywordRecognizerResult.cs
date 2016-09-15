#region File Description
//-----------------------------------------------------------------------------
// KeywordRecognizerResult
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Hololens.Speech
{
    public sealed class KeywordRecognizerResult : EventArgs
    {
        /// <summary>
        /// The text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The confidence.
        /// </summary>
        public ConfidenceLevel Confidence { get; private set; }

        /// <summary>
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
