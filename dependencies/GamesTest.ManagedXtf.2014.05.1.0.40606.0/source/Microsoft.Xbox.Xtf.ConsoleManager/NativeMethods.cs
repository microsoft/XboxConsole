using System;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Console
{
    internal static partial class NativeMethods
    {
        // IXtfConsoleManager
        [DllImport(@"XtfConsoleManager.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateConsoleManager(IXtfConsoleManagerCallback callback, ref System.Guid riid, out System.IntPtr ppvObject);
    }
}
