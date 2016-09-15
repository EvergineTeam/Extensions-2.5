#region File Description
//-----------------------------------------------------------------------------
// DeviceResources
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Hololens.Internals
{
    // A base class that tracks resources allocated by native code. This class is
    // used to release COM references to DirectX resources.
    public abstract class Disposer : IDisposable
    {
        /// <summary>
        /// The collection of disposable objects.
        /// </summary>
        private SharpDX.DisposeCollector DisposeCollector;

        #region Properties

        /// <summary>
        /// Indicates whether this instance is already disposed.
        /// </summary>
        protected internal bool IsDisposed { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize a new instance of the <see cref="Disposer"/> class.
        /// </summary>
        protected internal Disposer()
        {
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Releases resources allocated by native code (unmanaged resources).
        /// All disposable objects that were added to the collection will be disposed of
        /// when this method is called.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Disposes all IDisposable object resources in the collection of disposable objects.
        /// </summary>
        /// <param name="disposeManagedResources">Since this class exists to dispose of unmanaged resources, the disposeManagedResources parameter is ignored.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            // If the DisposeCollector exists, have it dispose of all COM objects.
            if (!IsDisposed && DisposeCollector != null)
            {
                DisposeCollector.Dispose();
            }

            // The DisposeCollector is done, and can be discarded.
            DisposeCollector = null;
        }

        /// <summary>
        /// Adds an IDisposable object to the collection of disposable objects.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="objectToDispose">Object to dipose.</param>
        /// <returns>The disposed object.</returns>
        protected internal T ToDispose<T>(T objectToDispose)
        {
            // If objectToDispose is not null, add it to the collection.
            if (!ReferenceEquals(objectToDispose, null))
            {
                // Create DisposeCollector if it doesn't already exist.
                if (DisposeCollector == null)
                {
                    DisposeCollector = new SharpDX.DisposeCollector();
                    IsDisposed = false;
                }

                return DisposeCollector.Collect(objectToDispose);
            }

            // Otherwise, return a default instance of type T.
            return default(T);
        }

        /// <summary>
        /// Disposes of an IDisposable object immediately and also removes the object from the
        /// collection of disposable objects.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="objectToDispose">Object to dispose.</param>
        protected internal void RemoveAndDispose<T>(ref T objectToDispose)
        {
            // If objectToDispose is not null, and if the DisposeCollector is available, have 
            // the DisposeCollector get rid of objectToDispose.
            if (!ReferenceEquals(objectToDispose, null) && (DisposeCollector != null))
            {
                DisposeCollector.RemoveAndDispose(ref objectToDispose);
            }
        }

        /// <summary>
        /// Removes an IDisposable object from the collection of disposable objects. Does not
        /// dispose of the object before removing it.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="objectToRemove">Object to remove.</param>
        protected internal void RemoveToDispose<T>(T objectToRemove)
        {
            // If objectToRemove is not null, have the DisposeCollector forget about it.
            if (!ReferenceEquals(objectToRemove, null) && (DisposeCollector != null))
            {
                DisposeCollector.Remove(objectToRemove);
            }
        }

        #endregion
    }
}
