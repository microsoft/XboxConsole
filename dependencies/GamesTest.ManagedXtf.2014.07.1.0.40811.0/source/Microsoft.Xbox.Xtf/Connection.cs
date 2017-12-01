using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Xbox.XTF
{
    public enum ConnectionStatus
    {
        Pending,
        Ready,
        Disconnected,
    }

    public abstract class Connection : IDisposable
    {
        protected IXtfConnection BaseObject;
        private AutoResetEvent _ConnectionEvent;
        private bool disposed;

        #region Properties

        public AutoResetEvent ConnectionEvent
        {
            get { return _ConnectionEvent; }
            set { _ConnectionEvent = value; BaseObject.SetConnectionEvent(Extensions.ToHandle(value)); }
        }

        public ConnectionStatus ConnectionStatus
        {
            get
            {
                try
                {
                    if (disposed)
                    {
                        throw new ObjectDisposedException(this.GetType().FullName);
                    }
                    BaseObject.GetConnectionStatus();
                }
                catch(COMException e)
                {
                    return (HRESULT.E_PENDING == e.ErrorCode) ? ConnectionStatus.Pending : ConnectionStatus.Disconnected;
                }

                return ConnectionStatus.Ready;
            }
        }

        public Guid ExtensionId
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return Extensions.ToGuid(BaseObject.GetExtensionId());
            }
        }

        public string RemoteAddress
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return BaseObject.GetRemoteAddress();
            }
        }

        #endregion

        public Connection()
        {
        }

        internal Connection(IXtfConnection connection)
        {
            this.BaseObject = connection;
        }

        ~Connection()
        {
            this.Dispose(false);
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

        protected virtual void Dispose(bool disposing)
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
                if (disposing)
                {
                    _ConnectionEvent = null;
                }

                this.disposed = true;
            }
        }
    }
}
