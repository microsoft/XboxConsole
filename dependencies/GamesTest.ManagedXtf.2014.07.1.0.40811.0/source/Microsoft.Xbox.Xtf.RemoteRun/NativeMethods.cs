using System;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.RemoteRun
{
    internal static partial class NativeMethods
    {
        // IXtfRemoteRunClient
        [DllImport(@"XtfRemoteRun.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateRemoteRunClient(string pszAddress,
            ref System.Guid riid, out System.IntPtr ppvObject);
    }
}

