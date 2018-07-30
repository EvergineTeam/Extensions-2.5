// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.MixedReality.Speech
{
    /// <summary>
    /// Interface that encapsulates the keyword recognizer
    /// </summary>
    internal interface IKeywordRecognizer : IDisposable
    {
        /// <summary>
        /// Gets or sets the current grammar.
        /// </summary>
        string[] Keywords { get; set; }

        /// <summary>
        /// Gets a value indicating whether the recognizer is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Occurs when a keyword has been recognized.
        /// </summary>
        event KeywordRecognizerService.KeywordResult OnKeywordRecognized;

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        void Terminate();

        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void Start();

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void Stop();
    }
}
