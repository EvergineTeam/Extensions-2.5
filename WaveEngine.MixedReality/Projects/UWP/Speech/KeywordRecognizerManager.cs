// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common;
using Windows.Media.SpeechRecognition;
using static WaveEngine.MixedReality.Speech.KeywordRecognizerService;
#endregion

namespace WaveEngine.MixedReality.Speech
{
    /// <summary>
    /// KeywordRecognizer listens to speech input and attempts to match uttered phrases to a list of registered keywords.
    /// </summary>
    internal class KeywordRecognizerManager : IKeywordRecognizer
    {
        /// <summary>
        /// SpeechRecgonizer from UWP API.
        /// </summary>
        private SpeechRecognizer recognizer;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The current grammar.
        /// </summary>
        private string[] keywords;

        /// <summary>
        /// Occurs when a keyword has been recognized.
        /// </summary>
        public event KeywordResult OnKeywordRecognized;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether where the keywordRecognizer is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        public string[] Keywords
        {
            get
            {
                return this.keywords;
            }

            set
            {
                this.keywords = value;
                this.UpdateKeywordlist(this.keywords);
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordRecognizerManager"/> class.
        /// </summary>
        public KeywordRecognizerManager()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="KeywordRecognizerManager"/> class.
        /// </summary>
        ~KeywordRecognizerManager()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        public void Initialize()
        {
            if (this.recognizer != null)
            {
                this.recognizer.ContinuousRecognitionSession.Completed -= this.OnContinuousRecognitionSessionCompleted;
                this.recognizer.ContinuousRecognitionSession.ResultGenerated -= this.OnContinuousRecognitionSessionResult;
            }

            this.recognizer = new SpeechRecognizer();
            this.recognizer.ContinuousRecognitionSession.Completed += this.OnContinuousRecognitionSessionCompleted;
            this.recognizer.ContinuousRecognitionSession.ResultGenerated += this.OnContinuousRecognitionSessionResult;
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        public void Terminate()
        {
            this.Stop();
            this.Dispose(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public async void Start()
        {
            if (!this.IsRunning)
            {
                try
                {
                    await this.recognizer.ContinuousRecognitionSession.StartAsync();
                    this.IsRunning = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("KeywordRecognizer: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public async void Stop()
        {
            if (this.IsRunning)
            {
                try
                {
                    await this.recognizer.ContinuousRecognitionSession.CancelAsync();
                    this.IsRunning = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("KeywordRecognizer: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when speech recognizer has a result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void OnContinuousRecognitionSessionResult(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (this.OnKeywordRecognized != null)
            {
                string text = args.Result.Text;
                var confidence = args.Result.Confidence;
                this.OnKeywordRecognized(new KeywordRecognizerResult(text, (ConfidenceLevel)confidence));
            }
        }

        /// <summary>
        /// Occurs when speech recognizer has completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void OnContinuousRecognitionSessionCompleted(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
        }

        /// <summary>
        /// Update the keywork list
        /// </summary>
        /// <param name="keywords">Keywords to add.</param>
        private async void UpdateKeywordlist(string[] keywords)
        {
            if (this.recognizer != null)
            {
                if (this.IsRunning)
                {
                    throw new Exception("You cannot change the grammar while the service is running. Stop it before.");
                }

                // Clear previews constraints
                if (this.recognizer.Constraints.Count > 0)
                {
                    this.recognizer.Constraints.Clear();
                }

                try
                {
                    // Add new list
                    this.recognizer.Constraints.Add(new SpeechRecognitionListConstraint(keywords));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Keyword recognizer: " + ex.Message);
                }

                // Compile the constraint.
                SpeechRecognitionCompilationResult result = await this.recognizer.CompileConstraintsAsync();

                if (result.Status != SpeechRecognitionResultStatus.Success)
                {
                    this.IsRunning = false;
                    System.Diagnostics.Debug.WriteLine("Unable to compile grammar.");
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
