using System;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Console
{
    internal static partial class NativeMethods
    {
        // IXtfConsoleControlClient
        [DllImport(@"XtfConsoleControl.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateConsoleControlClient(string pszAddress,
            ref System.Guid riid, out System.IntPtr ppvObject);

        [DllImport(@"XtfConsoleControl.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "XtfCaptureScreenshot", ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCaptureScreenshot(string address, out System.IntPtr phBitmap);

        /// <summary>
        /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.
        /// </summary>
        /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
        /// <returns>
        /// If the function succeeds, the return value is <c>true</c>. If the specified handle is not valid or is currently selected into a DC, the return value is <c>false</c>.
        /// </returns>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
    }
}

