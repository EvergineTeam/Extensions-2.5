#region File Description
//-----------------------------------------------------------------------------
// KeywordRecognizerService
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common;
#endregion

namespace WaveEngine.Hololens.Speech
{
    /// <summary>
    /// 
    /// </summary>
    public class KeywordRecognizerService : Service, IDisposable
    {
        /// <summary>
        /// The platform keyword recognizer
        /// </summary>
        private IKeywordRecognizer keywordRecognizer;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Custom delegate used in OnkeywordRecognized event.
        /// </summary>
        /// <param name="result"></param>
        public delegate void KeywordResult(KeywordRecognizerResult result);

        /// <summary>
        /// Occurs when a keyword has been recognized.
        /// </summary>
        public event KeywordResult OnKeywordRecognized
        {
            add
            {
                this.keywordRecognizer.OnKeywordRecognized += value;
            }

            remove
            {
                this.keywordRecognizer.OnKeywordRecognized -= value;
            }
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.keywordRecognizer != null;
            }
        }

        /// <summary>
        /// Where the keywordRecognizer is running or not.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("HoloLens is not available.");
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="KeywordRecognizerService" /> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get
            {
                if (!this.IsConnected)
                {
                    return false;
                }

                return this.disposed;
            }
        }

        public string[] Keywords
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("HoloLens is not available.");
                }

                return this.keywordRecognizer.Keywords;
            }

            set
            {
                if (this.IsConnected)
                {
                    this.keywordRecognizer.Keywords = value;
                }
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordRecognizerService"/> class.
        /// </summary>
        public KeywordRecognizerService()
        {
#if UWP
            this.keywordRecognizer = new KeywordRecognizerManager(); 
#endif
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="KeywordRecognizerService"/> class.
        /// </summary>
        ~KeywordRecognizerService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (this.IsConnected)
            {
                this.keywordRecognizer.Initialize();
            }
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        protected override void Terminate()
        {
            if (this.IsConnected)
            {
                this.keywordRecognizer.Terminate();
            }
                        
            this.Dispose();
        }

#endregion

#region Public Methods
        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void Start()
        {
            if (!this.IsConnected)
            {
                throw new Exception("HoloLens is not available.");
            }

            this.keywordRecognizer.Start();
        }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void Stop()
        {
            if (!this.IsConnected)
            {
                throw new Exception("HoloLens is not available.");
            }

            this.keywordRecognizer.Stop();
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.IsConnected)
                    {
                        this.keywordRecognizer.Dispose();
                        this.keywordRecognizer = null;
                    }                    
                }

                this.disposed = true;
            }
        }
#endregion
    }
}
