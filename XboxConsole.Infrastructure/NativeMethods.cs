//------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A static class to house P/Invoke signatures for native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// This method is used to add the directory where the XTF native DLLs are located
        /// to the search path used by Windows so that they can be found automatically.
        /// </summary>
        /// <param name="pathName">The path to the XTF native DLLs.</param>
        /// <returns>True upon success.</returns>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetDllDirectory(string pathName);

        /// <summary>
        /// Gets XTF error description for an HRESULT returned from Xbox One API.
        /// </summary>
        /// <param name="hresult">The error code.</param>
        /// <param name="errorMessageBuffer">Buffer to fill with error message.</param>
        /// <param name="errorMessageBufferLength">Length of error message written into the buffer.</param>
        /// <param name="userActionTextBuffer">Buffer to fill with action message.</param>
        /// <param name="userActionTextBufferLength">Length of action message written into the buffer.</param>
        /// <returns>The error code for conversion result.</returns>
        [DllImport(@"XtfApi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        internal static extern int XtfGetErrorText(int hresult, IntPtr errorMessageBuffer, out uint errorMessageBufferLength, IntPtr userActionTextBuffer, out uint userActionTextBufferLength);
    }
}
