using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Xbox.XTF
{
    public sealed class Channel : Connection
    {
        internal new IXtfChannel BaseObject;
        MessageDispatcher _MessageDispatcher;
        private bool disposed = false;

        internal Channel(IXtfChannel channel) : base(channel)
        {
            this.BaseObject = channel;
        }

        public Channel(string address, Guid extensionId, TimeSpan? timeout)
        {
            GUID exid = Extensions.ToGuid(extensionId);
            GUID riid = Extensions.ToGuid(typeof(IXtfChannel).GUID);
            IntPtr ppvObj;

            Core.BaseObject.CreateChannel(address, ref exid, Extensions.ToTimeout(timeout), ref riid, out ppvObj);

            base.BaseObject = this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfChannel;
            Marshal.Release(ppvObj);
        }

        ~Channel()
        {
            this.Dispose(false);
        }

        public void Dispatch()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            BaseObject.Dispatch();
        }

        public object Context
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return BaseObject.GetChannelContext();
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                BaseObject.SetChannelContext(value);
            }
        }

        public uint ChannelId
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                uint local = 0, remote = 0;
                BaseObject.GetChannelId(out local, out remote);
                return local;
            }
        }

        public uint RemoteChannelId
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                uint local = 0, remote = 0;
                BaseObject.GetChannelId(out local, out remote);
                return remote;
            }
        }

        public MessageDispatcher MessageDispatcher
        {
            get { return _MessageDispatcher; }
        }

        public uint OptimalMessageSize
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return BaseObject.GetOptimalMessageSize();
            }
        }

        public AutoResetEvent GetReceiveEvent(uint requestId)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            return Extensions.ToAutoResetEvent(BaseObject.GetReceiveEvent(requestId));
        }

        public void SetReceiveEvent(uint requestId, EventWaitHandle value)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            BaseObject.SetReceiveEvent(requestId, Extensions.ToHandle(value));
        }

        public Message Receive(uint requestId, TimeSpan? timeout)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            IXtfMessage message;
            XTFMSGINFO content;

            try
            {
                GUID riid = Extensions.ToGuid(typeof(IXtfMessage).GUID);
                IntPtr ppvObj;

                BaseObject.Receive(requestId, Extensions.ToTimeout(timeout), out content, ref riid, out ppvObj);

                message = Marshal.GetObjectForIUnknown(ppvObj) as IXtfMessage;
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

            return new Message(message);
        }

        public uint Send(Message message, TimeSpan? timeout)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message", "Cannot be null");
            }

            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            return BaseObject.Send(message.BaseObject, Extensions.ToTimeout(timeout));
        }

        public uint SetMessageDispatcher(MessageDispatcher messageDispatcher, MessageDispatcherFlags flags)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            XtfMessageDispatcher xtfMessageDispatcher = new XtfMessageDispatcher(messageDispatcher);
            return BaseObject.SetMessageDispatcher(xtfMessageDispatcher, (uint)flags);
        }

        protected override void Dispose(bool disposing)
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
                    _MessageDispatcher = null;
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Channel {0}:{1}", ChannelId, RemoteChannelId);
        }
    }
}
