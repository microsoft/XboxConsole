using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    public static class Core
    {
        internal static readonly IXtfCoreEx BaseObject;

        static Core()
        {
            GUID riid = Extensions.ToGuid(typeof(IXtfCoreEx).GUID);
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfInitialize(ref riid, out ppvObj));
            BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfCoreEx;
            Marshal.Release(ppvObj);
        }

        public static string ErrorText
        {
            get { return BaseObject.GetErrorText(); }
            set { BaseObject.SetErrorText(value); }
        }
    }
}
