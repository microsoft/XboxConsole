//------------------------------------------------------------------------------
// <copyright file="ExceptionFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Internal.GamesTest.Xbox
{
    /// <summary>
    /// Represents a translator of COMExeptions into XboxExceptions.
    /// </summary>
    internal static class ExceptionFactory
    {
        private const int SystemCannotFindDriveSpecifiedCOMExceptionHResult = -2147024881; // 0x8007000F

        private const int SessionKeyIncorrectCOMExceptionHResult = unchecked((int)0x8C11040B);

        /// <summary>
        /// Creates an XboxException derived type that maps to the HResult of the ExternalException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>An XboxException derived type that maps to the HResult of the ExternalException.</returns>
        public static Exception Create(string message, ExternalException innerException)
        {
            return Create(message, innerException, string.Empty);
        }

        /// <summary>
        /// Creates an XboxConsoleException derived type that maps to the HResult of the ExternalException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The name of the Xbox.</param>
        /// <returns>An XboxConsoleException derived type that maps to the HResult of the ExternalException.</returns>
        public static Exception Create(string message, ExternalException innerException, string xboxName)
        {
            if (innerException == null)
            {
                throw new ArgumentNullException("innerException");
            }

            // We need to wrap a special COM exception into a FileNotFoundException: if the drive does not exist.

            bool isComException = innerException is COMException;
            if (isComException && innerException.HResult == SystemCannotFindDriveSpecifiedCOMExceptionHResult)
            {
                return new FileNotFoundException("The system cannot find the drive specified", innerException);
            }

            string errorMessage = string.Format(CultureInfo.InvariantCulture, "{0}.  Reason: {1}", message, GetWin32ErrorMessage(innerException.ErrorCode));

            if (string.IsNullOrEmpty(xboxName))
            {
                return new XboxException(errorMessage, innerException);
            }
            else
            {
                if (isComException && innerException.HResult == SessionKeyIncorrectCOMExceptionHResult)
                {
                    return new XboxSessionKeyException(errorMessage, innerException, xboxName);
                }

                return new XboxConsoleException(errorMessage, innerException, xboxName);
            }
        }

        /// <summary>
        /// Creates an XboxInputException derived type that maps to the HResult of the ExternalException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="xboxName">The name of the Xbox.</param>
        /// <param name="gamepadId">A gamepads Id.</param>
        /// <returns>An XboxInputException derived type that maps to the HResult of the ExternalException.</returns>
        public static Exception Create(string message, ExternalException innerException, string xboxName, ulong gamepadId)
        {
            if (innerException == null)
            {
                throw new ArgumentNullException("innerException");
            }

            string errorMessage = string.Format(CultureInfo.InvariantCulture, "{0}.  Reason: {1}", message, GetWin32ErrorMessage(innerException.ErrorCode));

            return new XboxInputException(errorMessage, innerException, xboxName, gamepadId);
        }

        private static string GetWin32ErrorMessage(int errorCode)
        {
            // The Win32Exception class has the ability to translate Win32 error
            // codes into human-readable strings.  If the errorCode is not a Win32 error 
            // code then you get back a message saying something like "Unknown Error".
            Win32Exception win32Exception = new Win32Exception(errorCode);
            string message = win32Exception.Message;
            return message;
        }
    }
}
