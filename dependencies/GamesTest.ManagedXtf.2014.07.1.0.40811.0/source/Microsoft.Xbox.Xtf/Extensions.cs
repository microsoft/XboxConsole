using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    //
    // TODO TFS 769989
    //
    // Make Microsoft.Xbox.XTF.dll public and remove redundant hresult.cs and extension.cs from other assemblies
    //

    internal static partial class Extensions
    {
        public static System.Guid ToGuid(Microsoft.Xbox.XTF.GUID guid)
        {
            return new System.Guid(
                guid.Data1,
                guid.Data2,
                guid.Data3,
                guid.Data4[0],
                guid.Data4[1],
                guid.Data4[2],
                guid.Data4[3],
                guid.Data4[4],
                guid.Data4[5],
                guid.Data4[6],
                guid.Data4[7]);
        }

        public static Microsoft.Xbox.XTF.GUID ToGuid(System.Guid guid)
        {
            byte[] bytes = guid.ToByteArray();

            return new Microsoft.Xbox.XTF.GUID()
            {
                Data1 = (uint)BitConverter.ToInt32(bytes, 0),
                Data2 = (ushort)BitConverter.ToInt16(bytes, 4),
                Data3 = (ushort)BitConverter.ToInt16(bytes, 6),
                Data4 = new byte[8]
                {
                    bytes[8],
                    bytes[9],
                    bytes[10],
                    bytes[11],
                    bytes[12],
                    bytes[13],
                    bytes[14],
                    bytes[15],
                },
            };
        }

        public static IntPtr ToHandle(EventWaitHandle handle)
        {
            return (null == handle) ? IntPtr.Zero : ToHandle(handle.SafeWaitHandle);
        }

        public static IntPtr ToHandle(SafeWaitHandle handle)
        {
            return (null == handle) ? IntPtr.Zero : ToHandle(handle);
        }

        public static AutoResetEvent ToAutoResetEvent(IntPtr handle)
        {
            if(IntPtr.Zero == handle)
            {
                return null;
            }

            AutoResetEvent value = new AutoResetEvent(false);
            value.SafeWaitHandle = new SafeWaitHandle(handle, false);

            return value;
        }

        public static uint ToTimeout(TimeSpan? time)
        {
            return time.HasValue ? ToTimeout(time.Value) : uint.MaxValue;
        }

        public static uint ToTimeout(TimeSpan time)
        {
            return (uint)time.TotalMilliseconds;
        }

        public static void ReleaseComObject<T>(ref T obj) where T : class
        {
            if(null != obj)
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
    }
}
