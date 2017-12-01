using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.XTF
{
    public sealed class ServerEventArgs : EventArgs
    {
        public readonly Server Server;
        public readonly Channel Channel;
        public readonly ConnectionStatus ConnectionStatus;

        public ServerEventArgs(IXtfServer server, IXtfChannel channel, ConnectionStatus connectionStatus)
        {
            Server = new Server(server);
            Channel = new Channel(channel);
            ConnectionStatus = connectionStatus;
        }
    }

    public delegate void ServerEventHandler(object sender, ServerEventArgs e);

    internal class ServerCallback : IXtfServerCallback
    {
        private readonly ServerEventHandler EventHandler;

        public static ServerCallback Create(ServerEventHandler eventHandler)
        {
            if(null == eventHandler)
            {
                return null;
            }

            return new ServerCallback(eventHandler);
        }

        ServerCallback(ServerEventHandler eventHandler)
        {
            EventHandler = eventHandler;
        }

        void IXtfServerCallback.OnChannelConnected(IXtfServer pServer, IXtfChannel pChannel)
        {
            ServerEventArgs e = new ServerEventArgs(pServer, pChannel, ConnectionStatus.Ready);
            EventHandler(e.Server, e);
        }

        void IXtfServerCallback.OnChannelDisconnected(IXtfServer pServer, IXtfChannel pChannel)
        {
            ServerEventArgs e = new ServerEventArgs(pServer, pChannel, ConnectionStatus.Disconnected);
            EventHandler(e.Server, e);
        }
    }

    public sealed class Server : IDisposable
    {
        internal IXtfServer BaseObject;
        private bool disposed = false;

        internal Server(IXtfServer Server)
        {
            BaseObject = Server;
        }

        public Server(Guid extensionId, string serverName, ServerEventHandler eventHandler, MessageDispatcher messageDispatcher)
        {
            ServerCallback serverCallback = ServerCallback.Create(eventHandler);
            XtfMessageDispatcher xtfMessageDispatcher = new XtfMessageDispatcher(messageDispatcher);
            GUID exid = Extensions.ToGuid(extensionId);
            GUID riid = Extensions.ToGuid(typeof(IXtfServer).GUID);
            IntPtr ppvObj;

            Core.BaseObject.CreateServer(serverName, ref exid, serverCallback, xtfMessageDispatcher, ref riid, out ppvObj);

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfServer;
            Marshal.Release(ppvObj);
        }

        ~Server()
        {
            this.Dispose(false);
        }

        public Guid ExtensionId
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return Extensions.ToGuid(BaseObject.GetExtensionId()); }
        }

        public string Name
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetServerName();
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetServerName(value);
            }
        }

        public object Context
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetServerContext();
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetServerContext(value);
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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Server \"{0}\" {1}", Name, ExtensionId);
        }
    }
}
