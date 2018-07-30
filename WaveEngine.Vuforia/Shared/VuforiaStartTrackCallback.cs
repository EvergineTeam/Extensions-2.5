// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// VuforiaStartTrackCallback. This abstraction class is needeed to support AOT on Xamarin.IOS
    /// See Reverse Callbacks section from https://developer.xamarin.com/guides/ios/advanced_topics/limitations/
    /// </summary>
    internal class VuforiaStartTrackCallback
    {
        private static TaskCompletionSource<bool> taskCompletionSource;
        private static GCHandle callbackHandle;
        private static Action<bool> internalAction;

        /// <summary>
        /// Delegate for Vuforia start tracking call
        /// </summary>
        /// <param name="result">The result of the Vuforia start tracking call</param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void StartTrackCallback(bool result);

        private StartTrackCallback callback;

        /// <summary>
        /// Gets the start trancking task
        /// </summary>
        public Task<bool> Task
        {
            get
            {
                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// Gets the start trancking callback
        /// </summary>
        public StartTrackCallback CallBack
        {
            get
            {
                return this.callback;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VuforiaStartTrackCallback"/> class.
        /// </summary>
        /// <param name="onCallbackAction">Action to be called when the callback is triggered</param>
        public VuforiaStartTrackCallback(Action<bool> onCallbackAction)
        {
            internalAction = onCallbackAction;

            this.callback = new StartTrackCallback(OnInitializeCallback);

            taskCompletionSource = new TaskCompletionSource<bool>();
            callbackHandle = GCHandle.Alloc(this.callback);
        }

#if IOS
        [ObjCRuntime.MonoPInvokeCallback(typeof(StartTrackCallback))]
#endif
        private static void OnInitializeCallback(bool result)
        {
            if (internalAction != null)
            {
                internalAction(result);
            }

            taskCompletionSource.SetResult(result);
            callbackHandle.Free();
        }
    }
}
