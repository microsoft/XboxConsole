using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    //
    // TODO TFS 769989
    //
    // Make Microsoft.Xbox.XTF.dll public and remove redundant hresult.cs and extension.cs from other assemblies
    //

    internal static class HRESULT
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_PENDING = -2147483638;
        public const int E_NOTIMPL = -2147467262;
        public const int E_FAIL = -2147467259;
        public const int E_ABORT = -2147467260;
        public const int XTF_E_NOT_A_RESPONSE = -1;

        public static bool FAILED(int hr)
        {
            return hr < 0;
        }

        public static void CHK(int hr)
        {
            if(FAILED(hr))
            {
                throw new COMException(String.Empty, hr);
            }
        }
    }
}

