// <auto-generated />

using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    public class Error
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////
        private class NoesisException: Exception
        {
            public NoesisException(string message): base(message) { }
            public NoesisException(Exception e): base("", e) { }
        }

        public static void Check()
        {
            if (Pending) throw Retrieve();
        }

        private static bool Pending
        {
            get { return _pendingException != null; }
        }

        private static Exception Retrieve()
        {
            Exception e = _pendingException;
            _pendingException = null;

            // extend pending exception info with the callstack of the Error.Check() caller
            return new NoesisException(e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetNativePendingError(System.Exception exception)
        {
            // Do not overwrite if there is already an exception pending
            if (_pendingException == null)
            {
                // store exception that have just occurred
                _pendingException = exception;

                Noesis_CppSetPendingError(exception.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        internal static void RegisterCallback()
        {
            Noesis_RegisterErrorCallback(_errorCallback);
        }

        private delegate void ErrorCallback([MarshalAs(UnmanagedType.LPWStr)]string desc);
        private static ErrorCallback _errorCallback = SetPendingError;

        [MonoPInvokeCallback(typeof(ErrorCallback))]
        private static void SetPendingError(string desc)
        {
            // Do not overwrite if there is already an exception pending
            if (_pendingException == null)
            {
                // create exception with the exact callstack where error occurred
                _pendingException = new NoesisException(desc);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static Exception _pendingException = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////
        [DllImport(Library.Name)]
        private static extern void Noesis_RegisterErrorCallback(ErrorCallback errorCallback);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        [DllImport(Library.Name)]
        private static extern void Noesis_CppSetPendingError([MarshalAs(UnmanagedType.LPWStr)]string message);
    }
}

