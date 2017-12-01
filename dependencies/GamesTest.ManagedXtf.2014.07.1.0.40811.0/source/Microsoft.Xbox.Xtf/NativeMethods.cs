using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    internal static partial class NativeMethods
    {
        [DllImport(@"XtfCore.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfInitialize(ref GUID riid, out System.IntPtr ppvObject);
    }
}
