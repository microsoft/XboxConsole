using System;
using System.Collections.Generic;

namespace Microsoft.Xbox.XTF
{
    [Flags]
    public enum MessageDispatcherFlags : uint
    {
        DisableThread = 0x00000001,
        DisableResult = 0x00000002,

        Mask = DisableThread | DisableResult,
    }

    public sealed class MessageEventArgs : EventArgs
    {
        IXtfChannel _XtfChannel;
        IXtfMessage _XtfMessage;
        XTFMSGINFO _XtfMessageInfo;

        Channel _Channel;
        Message _Message;

        internal MessageEventArgs(IXtfChannel channel, IXtfMessage message, XTFMSGINFO messageInfo)
        {
            _XtfChannel = channel;
            _XtfMessage = message;
            _XtfMessageInfo = messageInfo;
        }

        public Channel Channel
        {
            get
            {
                if(null == _Channel)
                {
                    _Channel = new Channel(_XtfChannel);
                }

                return _Channel;
            }
        }

        public Message Message
        {
            get
            {
                if(null == _Message)
                {
                    _Message = new Message(_XtfMessage, _XtfMessageInfo);
                }

                return _Message;
            }
        }
    }

    public delegate void MessageHandler(object sender, MessageEventArgs e);

    internal class XtfMessageDispatcher : IXtfMessageDispatcher
    {
        public readonly MessageDispatcher MessageDispatcher;

        public XtfMessageDispatcher(MessageDispatcher messageDispatcher)
        {
            MessageDispatcher = messageDispatcher;
        }

        public void DispatchMessage(IXtfChannel pChannel, IXtfMessage pMessage, ref XTFMSGINFO pMessageInfo)
        {
            MessageDispatcher.DispatchMessage(pChannel, pMessage, ref pMessageInfo);
        }
    }

    public sealed class MessageDispatcher : Dictionary<uint, MessageHandler>
    {
        public MessageDispatcher()
        {
        }

        public MessageDispatcher(IEnumerable<KeyValuePair<uint, MessageHandler>> source)
        {
            AddRange(source);
        }

        public void Add(MessageType messageType, uint messageId, MessageHandler messageHandler)
        {
            uint key = CreateKey(messageType, messageId);
            Add(key, messageHandler);
        }

        public void AddRange(IEnumerable<KeyValuePair<uint, MessageHandler>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "Cannot be null");
            }

            foreach(KeyValuePair<uint, MessageHandler> pair in source)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public static uint CreateKey(MessageType messageType, uint messageId)
        {
            return ((uint)messageType << 16) | (uint)messageId;
        }

        internal void DispatchMessage(IXtfChannel channel, IXtfMessage message, ref XTFMSGINFO content)
        {
            uint key = CreateKey((MessageType)content.MessageType, content.MessageId);
            MessageHandler messageHandler = this[key];
            messageHandler(this, new MessageEventArgs(channel, message, content));
        }
    }
}
