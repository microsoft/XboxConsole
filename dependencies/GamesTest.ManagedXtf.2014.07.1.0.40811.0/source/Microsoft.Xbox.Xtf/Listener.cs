using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Xbox.XTF
{
    public sealed class Listener : IDisposable
    {
        internal IXtfListener BaseObject;
        AutoResetEvent _AcceptEvent;
        private bool disposed = false;

        public Listener(Guid extensionId)
        {
            GUID riid = Extensions.ToGuid(typeof(IXtfListener).GUID);
            GUID exid = Extensions.ToGuid(extensionId);
            IntPtr ppvObj;

            Core.BaseObject.CreateListener(null, ref exid, null, 0, ref riid, out ppvObj);

            this.BaseObject = Marshal.GetObjectForIUnknown (ppvObj) as IXtfListener;
            Marshal.Release(ppvObj);
        }

        ~Listener()
        {
            this.Dispose(false);
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

        public string Name
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetListenerName();
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetListenerName(value);
            }
        }

        public object Context
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetListenerContext();
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetListenerContext(value);
            }
        }

        public Channel Accept(TimeSpan? timeout)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            IXtfChannel channel;

            try
            {
                GUID riid = Extensions.ToGuid(typeof(IXtfChannel).GUID);
                IntPtr ppvObj;

                BaseObject.Accept(Extensions.ToTimeout(timeout), ref riid, out ppvObj);

                channel = Marshal.GetObjectForIUnknown(ppvObj) as IXtfChannel;
                Marshal.Release(ppvObj);
            }
            catch(COMException e)
            {
                if(HRESULT.E_PENDING == e.ErrorCode)
                {
                    return null;
                }

                throw;
            }

            return new Channel(channel);
        }

        public AutoResetEvent AcceptEvent
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return _AcceptEvent;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _AcceptEvent = value;
                BaseObject.SetAcceptEvent(Extensions.ToHandle(value));
            }
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
            if (!disposed)
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
                    _AcceptEvent = null;
                }

                this.disposed = true;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Listener \"{0}\" {1}", Name, ExtensionId);
        }
    }
}
