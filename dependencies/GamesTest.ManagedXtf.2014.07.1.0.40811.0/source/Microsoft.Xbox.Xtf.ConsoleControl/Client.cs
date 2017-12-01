using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Console
{
    [Flags]
    public enum ShutdownConsoleFlags : uint
    {
        None = 0x00000000,
        Reboot = 0x00000001
    }

    public class ProcessInfo
    {
        internal XTFPROCESSINFO _ProcessInfo;

        public ProcessInfo(uint processId, string imageFileName)
        {
            _ProcessInfo = new XTFPROCESSINFO();
            this.ProcessId = processId;
            this.ImageFileName = imageFileName;
        }

        public uint ProcessId
        {
            get { return _ProcessInfo.dwProcessId; }
            set { _ProcessInfo.dwProcessId = value; }
        }

        public string ImageFileName
        {
            get { return _ProcessInfo.pszImageFileName; }
            set { _ProcessInfo.pszImageFileName = value; }
        }
    }

    public class RunningProcessEventArgs : EventArgs
    {
        public ProcessInfo ProcessInfo;

        internal RunningProcessEventArgs(XTFPROCESSINFO xpi)
        {
            this.ProcessInfo = new ProcessInfo(xpi.dwProcessId, xpi.pszImageFileName);
        }
    }

    public delegate void RunningProcessEventHandler(object sender, RunningProcessEventArgs e);

    internal class RunningProcessCallback : IXtfRunningProcessCallback
    {
        ConsoleControlClient _Client;
        RunningProcessEventHandler _EventHandler;

        public static RunningProcessCallback Create(ConsoleControlClient client, RunningProcessEventHandler eventHandler)
        {
            if(null == eventHandler)
            {
                return null;
            }

            return new RunningProcessCallback(client, eventHandler);
        }

        RunningProcessCallback(ConsoleControlClient client, RunningProcessEventHandler eventHandler)
        {
            _Client = client;
            _EventHandler = eventHandler;
        }

        void IXtfRunningProcessCallback.OnFoundProcess(ref XTFPROCESSINFO xpi)
        {
            _EventHandler(_Client, new RunningProcessEventArgs(xpi));
        }
    }

    public sealed partial class ConsoleControlClient : IDisposable
    {
        internal IXtfConsoleControlClient BaseObject;
        private bool disposed = false;

        public ConsoleControlClient(string address)
        {
            System.Guid riid = typeof(IXtfConsoleControlClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateConsoleControlClient(address, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfConsoleControlClient;
            Marshal.Release(ppvObj);
        }

        public ConsoleControlClient()
        {
            System.Guid riid = typeof(IXtfConsoleControlClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateConsoleControlClient(null, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfConsoleControlClient;
            Marshal.Release(ppvObj);
        }

        ~ConsoleControlClient()
        {
            this.Dispose(false);
        }

        public DateTime SystemTime
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Internal resource");
                }

                XTFSYSTEMTIME systemTime = new XTFSYSTEMTIME();
                BaseObject.GetSystemTime(out systemTime);

                try
                {
                    return new DateTime(
                        systemTime.wYear,
                        systemTime.wMonth,
                        systemTime.wDay,
                        systemTime.wHour,
                        systemTime.wMinute,
                        systemTime.wSecond,
                        systemTime.wMilliseconds);
                }
                catch
                {
                    // Swallow the exception and return the defualt value if
                    // the system does not respond with valid data.
                    return default(DateTime);
                }
            }
        }

        public void ShutdownConsole(ShutdownConsoleFlags flags)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.ShutdownConsole((uint)flags);
        }

        public bool UpdatesPending
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Internal resource");
                }
                return 0 != BaseObject.UpdatesPending();
            }
        }

        public void GetRunningProcesses(RunningProcessEventHandler eventHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.GetRunningProcesses(RunningProcessCallback.Create(this, eventHandler));
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
