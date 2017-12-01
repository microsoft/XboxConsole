using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.RemoteRun
{
    [Flags]
    public enum RunFlags : uint
    {
        None = 0x00000000,
        Wait = 0x00000001,
        GuestUser = 0x00000002,
        RedirectInput = 0x80000000,
        RedirectOutput = 0x40000000,
        RedirectBoth = RedirectInput | RedirectOutput,
    }

    public class RemoteRunEventArgs : EventArgs
    {
        public string Text;

        internal RemoteRunEventArgs()
        {
        }

        internal RemoteRunEventArgs(string text)
        {
            Text = text;
        }
    }

    public delegate void RemoteRunEventHandler(object sender, RemoteRunEventArgs e);

    internal class RemoteRunCallback : IXtfRemoteRunCallback
    {
        RemoteRunClient _Client;
        RemoteRunEventHandler _InputHandler;
        RemoteRunEventHandler _OutputHandler;

        public static RemoteRunCallback Create(RemoteRunClient client, RemoteRunEventHandler inputHandler, RemoteRunEventHandler outputHandler)
        {
            if((null == inputHandler) && (null == outputHandler))
            {
                return null;
            }

            return new RemoteRunCallback(client, inputHandler, outputHandler);
        }

        RemoteRunCallback(RemoteRunClient client, RemoteRunEventHandler inputHandler, RemoteRunEventHandler outputHandler)
        {
            _Client = client;
            _InputHandler = inputHandler;
            _OutputHandler = outputHandler;
        }

        void IXtfRemoteRunCallback.Input(out string text)
        {
            if(null == _InputHandler)
            {
                text = null;
                return;
            }

            RemoteRunEventArgs e = new RemoteRunEventArgs();
            _InputHandler(_Client, e);
            text = e.Text;
        }

        void IXtfRemoteRunCallback.Output(string text)
        {
            if(null == _OutputHandler)
            {
                return;
            }

            RemoteRunEventArgs e = new RemoteRunEventArgs(text);
            _OutputHandler(_Client, e);
        }
    }

    public sealed class RemoteRunClient : IDisposable
    {
        public const uint DefaultRunPeriod = 100;

        internal IXtfRemoteRunClient BaseObject;
        private bool disposed = false;

        public RemoteRunClient(string address)
        {
            System.Guid riid = typeof(IXtfRemoteRunClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateRemoteRunClient(address, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfRemoteRunClient;
            Marshal.Release(ppvObj);
        }

        public RemoteRunClient()
        {
            System.Guid riid = typeof(IXtfRemoteRunClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateRemoteRunClient(null, ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfRemoteRunClient;
            Marshal.Release(ppvObj);
        }

        ~RemoteRunClient()
        {
            this.Dispose(false);
        }

        public void Run(string commandLine, string workingDirectory, RunFlags flags, uint period, RemoteRunEventHandler inputHandler, RemoteRunEventHandler outputHandler)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            BaseObject.Run(commandLine, workingDirectory, (uint)flags, period, RemoteRunCallback.Create(this, inputHandler, outputHandler));
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
