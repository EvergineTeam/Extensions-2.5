// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// VuforiaInitializedCallback. This abstraction class is needeed to support AOT on Xamarin.IOS
    /// See Reverse Callbacks section from https://developer.xamarin.com/guides/ios/advanced_topics/limitations/
    /// </summary>
    internal class VuforiaInitializedCallback
    {
        private static TaskCompletionSource<bool> taskCompletionSource;
        private static GCHandle callbackHandle;
        private static Action<bool> internalAction;

        /// <summary>
        /// Delegate for Vuforia initialization
        /// </summary>
        /// <param name="result">The result of the initialization</param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void InitCallback(bool result);

        private InitCallback callback;

        /// <summary>
        /// Gets the initialization task
        /// </summary>
        public Task<bool> Task
        {
            get
            {
                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// Gets the initialization callback
        /// </summary>
        public InitCallback CallBack
        {
            get
            {
                return this.callback;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VuforiaInitializedCallback"/> class.
        /// </summary>
        /// <param name="onCallbackAction">Action to be called when the callback is triggered</param>
        public VuforiaInitializedCallback(Action<bool> onCallbackAction)
        {
            internalAction = onCallbackAction;

            this.callback = new InitCallback(OnInitializeCallback);

            taskCompletionSource = new TaskCompletionSource<bool>();
            callbackHandle = GCHandle.Alloc(this.callback);
        }

#if IOS
        [ObjCRuntime.MonoPInvokeCallback(typeof(InitCallback))]
#endif
        private static void OnInitializeCallback(bool result)
        {
            if (taskCompletionSource != null)
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
}
