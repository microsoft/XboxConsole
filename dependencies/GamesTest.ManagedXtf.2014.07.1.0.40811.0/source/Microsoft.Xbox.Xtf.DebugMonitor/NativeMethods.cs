using Microsoft.Xbox.XTF;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF.Diagnostics
{
    internal static class NativeMethods
    {
        // IXtfDebugMonitorClient
        [DllImport(@"XtfDebugMonitor.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateDebugMonitorClient(string pszAddress,
            ref System.Guid riid, out System.IntPtr ppvObject);
    }
}
