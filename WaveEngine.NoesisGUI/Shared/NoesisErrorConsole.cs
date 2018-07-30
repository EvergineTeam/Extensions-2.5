// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEngine.NoesisGUI
{
    /// <summary>
    /// Class for Noesis error login
    /// </summary>
    internal class NoesisErrorConsole
    {
        /// <summary>
        /// Print a Noesis error caused by a NoesisPanel
        /// </summary>
        /// <param name="componentName">The name of the component which caused the error.</param>
        /// <param name="error">The error message.</param>
        internal static void PrintPanelError(string componentName, Exception error)
        {
            NoesisErrorConsole.ConsoleWrite($"NoesisPanel error ({componentName}): {NoesisErrorConsole.GetMessageFromException(error)}");
        }

        /// <summary>
        /// Print a Noesis error caused by the NoesisService
        /// </summary>
        /// <param name="error">The error message.</param>
        internal static void PrintServiceError(Exception error)
        {
            NoesisErrorConsole.ConsoleWrite($"NoesisService error: {NoesisErrorConsole.GetMessageFromException(error)}");
        }

        private static string GetMessageFromException(Exception error)
        {
            for (Exception current = error; current != null; current = current.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(current.Message))
                {
                    return $"{current.GetType()} - {current.Message}";
                }
            }

            return "(unknown exception)";
        }

        private static void ConsoleWrite(string message)
        {
#if !UWP
            Console.WriteLine(message);
#endif
        }
    }
}
