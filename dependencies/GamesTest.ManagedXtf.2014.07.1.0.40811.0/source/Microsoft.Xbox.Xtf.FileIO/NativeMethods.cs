using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.IO
{
    internal static partial class NativeMethods
    {
        // IXtfFileIOClient
        [DllImport(@"XtfFileIO.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateFileIOClient(string pszAddress,
            ref System.Guid riid, out System.IntPtr ppvObject);
    }
}
