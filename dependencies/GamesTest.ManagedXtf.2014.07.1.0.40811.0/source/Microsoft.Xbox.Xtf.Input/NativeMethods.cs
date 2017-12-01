using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Input
{
    internal static class API
    {
        // IXtfInputClient
        [DllImport(@"XtfInput.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "XtfCreateInputClient", ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateInputClient(string address,
            ref System.Guid riid, out System.IntPtr ppvObject);
    }
}
