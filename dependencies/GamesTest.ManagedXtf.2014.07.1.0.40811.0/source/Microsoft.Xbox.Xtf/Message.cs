using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.Xbox.XTF
{
    public static class MessageId
    {
        public const uint Min = 0x01;
        public const uint Max = 0xFF;
    }

    public enum MessageType : uint
    {
        None = 0x00,
        Event = 0x01,
        Request = 0x02,
        RequestData = 0x03,
        Response = 0x04,

        Min = None,
        Max = Response,
    }

    public static class RequestId
    {
        public const uint None = 0x00000000;
        public const uint Min = 0x00000001;
        public const uint Max = 0x0000FFFF;
        public const uint Any = 0xFFFFFFFF;
    }

    [Flags]
    public enum MessageBufferFlags : uint
    {
        Binary = 0x00000000,
        Ansi = 0x00000001,
        Unicode = 0x00000002,
        Attach = 0x80000000,

        FormatMask = Binary | Ansi | Unicode,
        Mask = Binary | Ansi | Unicode | Attach,
    }

    public static class MessageBufferIndex
    {
        public const int MaxCount = 32;
    }

    public enum MessageStatus
    {
        Ready,
        Pending,
        Error,
    }

    public sealed partial class Message : IDisposable
    {
        internal IXtfMessage BaseObject;
        internal XTFMSGINFO _Info;
        internal AutoResetEvent _Event;
        private bool disposed = false;

        internal Message(IXtfMessage message)
            : this(message, message.GetMessageInfo())
        {
        }

        private void InitializeBaseObject()
        {
            XTFMSGINFO _Info = new XTFMSGINFO();
            GUID riid = Extensions.ToGuid(typeof(IXtfMessage).GUID);
            IntPtr ppvObj;

            Core.BaseObject.CreateMessage(ref _Info, ref riid, out ppvObj);

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfMessage;
            Marshal.Release(ppvObj);
        }

        public Message()
        {
            InitializeBaseObject();
        }

        public Message(MessageType type, int messageId)
        {
            InitializeBaseObject();

            _Info.MessageType = (uint)type;
            _Info.MessageId = (uint)messageId;
            BaseObject.SetMessageInfo(ref _Info);
        }

        public Message(IXtfMessage message, XTFMSGINFO info)
        {
            BaseObject = message;
            _Info = info;
        }

        ~Message()
        {
            this.Dispose(false);
        }

        public uint MessageId
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return _Info.MessageId;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _Info.MessageId = value;
                BaseObject.SetMessageInfo(ref _Info);
            }
        }

        public MessageType MessageType
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return (MessageType)_Info.MessageType;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _Info.MessageType = (uint)value; BaseObject.SetMessageInfo(ref _Info);
            }
        }

        public uint RequestId
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return _Info.RequestId;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _Info.RequestId = value;
                BaseObject.SetMessageInfo(ref _Info);
            }
        }

        public int Result
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return _Info.Result;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _Info.Result = value;
                BaseObject.SetMessageInfo(ref _Info);
            }
        }

        public object MessageContext
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetMessageContext();
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetMessageContext(value);
            }
        }

        public object RequestContext
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return BaseObject.GetRequestContext();
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                BaseObject.SetRequestContext(value);
            }
        }

        public AutoResetEvent Event
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                return _Event;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                _Event = value; BaseObject.SetMessageEvent(Extensions.ToHandle(value));
            }
        }

        public MessageStatus Status
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                try
                {
                    BaseObject.GetMessageStatus();
                }
                catch(COMException e)
                {
                    return (HRESULT.E_PENDING == e.ErrorCode) ? MessageStatus.Pending : MessageStatus.Error;
                }

                return MessageStatus.Ready;
            }
        }

        public void SetMessageBuffer(uint index, byte[] data)
        {
            BaseObject.SetMessageBuffer(index, ref data[0], Convert.ToUInt32(data.Length), 0);
        }

        public void SetMessageBuffer(uint index, String str)
        {
            this.SetMessageBuffer(index, Encoding.Unicode.GetBytes(str + Char.MinValue));
        }

        public void Dispose()
        {
            this.Dispose(true);
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
                if (disposing)
                {
                    _Event = null;
                }

                this.disposed = true;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Message {0}:{1}", _Info.MessageType, _Info.MessageId);
        }
    }
}

