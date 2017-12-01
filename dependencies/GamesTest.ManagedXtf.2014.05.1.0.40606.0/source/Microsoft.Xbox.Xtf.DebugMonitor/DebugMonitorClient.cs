using Microsoft.Xbox.XTF;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF.Diagnostics
{
    [Flags]
    public enum StartFlags
    {
        None = 0x00000000
    }

    [Flags]
    public enum StopFlags
    {
        None = 0x00000000
    }

    public class DebugMonitorClient : IDisposable
    {
        internal IXtfDebugMonitorClient debugClient;
        private bool disposed = false;
        private DebugMonitorCallback callback = null;

        public DebugMonitorClient(string address)
        {
            System.Guid riid = typeof(IXtfDebugMonitorClient).GUID;
            IntPtr ppvObj;

            int hr = NativeMethods.XtfCreateDebugMonitorClient(address, ref riid, out ppvObj);
            if (hr < 0)
            {
                throw new COMException("Unable to create object.", hr);
            }

            this.debugClient = Marshal.GetObjectForIUnknown(ppvObj) as IXtfDebugMonitorClient;
            Marshal.Release(ppvObj);

            this.callback = new DebugMonitorCallback(debugClient);
        }

        public DebugMonitorClient()
        {
            System.Guid riid = typeof(IXtfDebugMonitorClient).GUID;
            IntPtr ppvObj;

            int hr = NativeMethods.XtfCreateDebugMonitorClient(null, ref riid, out ppvObj);
            if (hr < 0)
            {
                throw new COMException("Unable to create object.", hr);
            }

            this.debugClient = Marshal.GetObjectForIUnknown(ppvObj) as IXtfDebugMonitorClient;
            Marshal.Release(ppvObj);

            this.callback = new DebugMonitorCallback(debugClient);
        }

        ~DebugMonitorClient()
        {
            this.Dispose(false);
        }

        public void Start(uint processId, StartFlags flags, OutputDebugStringEventHandler handler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            this.callback.EventHandler = handler;

            debugClient.Start(processId, (uint)flags, this.callback);
        }

        public void Stop(uint processId, StopFlags flags)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            debugClient.Stop(processId, (uint)flags);

            this.callback.EventHandler = null;
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
                Marshal.ReleaseComObject(this.debugClient);

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
