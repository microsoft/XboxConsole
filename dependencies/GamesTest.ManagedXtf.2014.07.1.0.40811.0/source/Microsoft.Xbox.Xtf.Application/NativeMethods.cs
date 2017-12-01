using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Application
{
    public static class API
    {
        // IXtfApplicationClient
        [DllImport(@"XtfApplication.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "XtfCreateApplicationClient", ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateApplicationClient(string address,
            ref System.Guid riid, out System.IntPtr ppvObject);
    }
}
