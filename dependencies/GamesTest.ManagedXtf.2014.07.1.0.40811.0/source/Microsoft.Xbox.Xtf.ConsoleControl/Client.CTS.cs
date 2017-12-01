//------------------------------------------------------------------------------
// <copyright file="Client.CTS.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Xbox.XTF.Console
{
    using System;

    partial class ConsoleControlClient
    {
        public string GetConfigValue(string key)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            return this.BaseObject.GetConfigValue(key);
        }

        public void SetConfigValue(string key, string value)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            this.BaseObject.SetConfigValue(key, value);
        }

        /// <summary>
        /// Captures a screenshot on the specified console
        /// </summary>
        /// <param name="address">The address of the console to grab a screenshot on.</param>
        /// <returns>
        /// A <see cref="System.IntPtr"/> pointing to an HBITMAP containing the screenshot.
        /// When you have finished using this bitmap, use DeleteObject to free the resources
        /// associated with the HBITMAP.
        /// </returns>
        public static IntPtr CaptureScreenshot(string address)
        {
            IntPtr phBitmap = IntPtr.Zero;

            HRESULT.CHK(NativeMethods.XtfCaptureScreenshot(address, out phBitmap));

            return phBitmap;
        }
    }
}
