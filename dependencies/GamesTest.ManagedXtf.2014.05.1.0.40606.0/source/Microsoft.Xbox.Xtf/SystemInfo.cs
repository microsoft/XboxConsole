using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    [Flags]
    public enum IPAddressTypes : uint
    {
        None = 0,
        All = 0xFFFFFFFF,

        Tools = 0x80000000,
        System = 0x40000000,
        PartitionMask = 0xC0000000,

        IPv6 = 0x20000000,
        IPv4 = 0x10000000,
        IPVersionMask = 0x30000000,
    }

    public struct IPAddressEntry
    {
        public IPAddressTypes AddressType;
        public IPAddress Address;
        public IPAddress SubnetMask;
    }

    internal class IPAddressList : List<IPAddressEntry>, IXtfEnumIPAddressesCallback
    {
        public void OnEnumIPAddress(ref XTFIPADDRENTRY pAddressEntry)
        {
            IPAddressEntry entry = new IPAddressEntry()
            {
                AddressType = (IPAddressTypes)pAddressEntry.dwAddressType,
            };

            if (IPAddress.TryParse(pAddressEntry.pszAddress, out entry.Address) &&
                IPAddress.TryParse(pAddressEntry.pszSubnetMask, out entry.SubnetMask))
            {
                Add(entry);
            }
        }
    }

    public class SystemInfo : IDisposable
    {
        internal IXtfSystemInfo BaseObject;
        private bool disposed = false;

        public SystemInfo()
        {
            GUID riid;
            IntPtr ppvObj;

            riid = Extensions.ToGuid(typeof(IXtfSystemInfo).GUID);
            Core.BaseObject.GetSystemInfo(ref riid, out ppvObj);
            BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfSystemInfo;
            Marshal.Release(ppvObj);
        }

        public SystemInfo(string address, TimeSpan? timeout)
        {
            IXtfSystemClient client;
            GUID riid;
            IntPtr ppvObj;

            riid = Extensions.ToGuid(typeof(IXtfSystemClient).GUID);
            Core.BaseObject.CreateSystemClient(address, Extensions.ToTimeout(timeout), ref riid, out ppvObj);
            client = Marshal.GetObjectForIUnknown(ppvObj) as IXtfSystemClient;

            riid = Extensions.ToGuid(typeof(IXtfSystemInfo).GUID);
            client.GetSystemInfo(ref riid, out ppvObj);
            BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfSystemInfo;
            Marshal.Release(ppvObj);

            Extensions.ReleaseComObject(ref client);
        }

        ~SystemInfo()
        {
            this.Dispose(false);
        }

        public IEnumerable<IPAddressEntry> GetIPAddresses()
        {
            return GetIPAddresses(IPAddressTypes.All);
        }

        public IEnumerable<IPAddressEntry> GetIPAddresses(IPAddressTypes types)
        {
            IPAddressList list = new IPAddressList();
            BaseObject.EnumIPAddresses((uint)types, list);
            return list;
        }

        public void Dispose()
        {
            this.Dispose(true);

            //
            // Avoid cleaning up twice.  Calling Dispose(true)
            // implies that managed objects will be cleaned up as well
            // as unmanaged objects so the garbage collector will
            // have no work to do when finalizing this object.
            //
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                Extensions.ReleaseComObject(ref BaseObject);

                //
                // Since the garbage collector's Finalize() runs on
                // a background thread, managed objects are not safe
                // to reference.  Only clean up managed objects if this
                // is being explicitly disposed.
                //
                // if (disposing)
                // {
                //    // ... Clean up managed objects
                // }
                //

                this.disposed = true;
            }
        }
    }
}
