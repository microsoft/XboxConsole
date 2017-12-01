namespace Microsoft.Xbox.XTF
{
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        // IXtfCreateProcessInfoExClient
        [DllImport(@"ProcessInfoClient.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        public static extern int XtfCreateProcessInfoExClient(ref System.Guid riid, out System.IntPtr ppvObject);
        
    }
}

