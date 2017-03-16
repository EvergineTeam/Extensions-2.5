#region File Description
//-----------------------------------------------------------------------------
// VuforiaStartTackCallback
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// VuforiaStartTackCallback. This abstraction class is needeed to support AOT on Xamarin.IOS
    /// See Reverse Callbacks section from https://developer.xamarin.com/guides/ios/advanced_topics/limitations/
    /// </summary>
    internal class VuforiaStartTackCallback
    {
        private static TaskCompletionSource<bool> taskCompletionSource;
        private static GCHandle callbackHandle;
        private static Action<bool> internalAction;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void StartTrackCallback(bool result);

        private StartTrackCallback callback;

        public Task<bool> Task
        {
            get
            {
                return taskCompletionSource.Task;
            }
        }

        public StartTrackCallback CallBack
        {
            get
            {
                return this.callback;
            }
        }

        public VuforiaStartTackCallback(Action<bool> onCallbackAction)
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
