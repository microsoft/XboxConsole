//------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Xbox.XTF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This is class represents an Xtf message being exchanged between a client and a server.
    /// This .cs file is intended to contain changes made to the Message class by the GTO CTS team.
    /// </summary>
    public sealed partial class Message : IDisposable
    {
        /// <summary>
        /// Intiailizes a new instance of the Message class.
        /// </summary>
        /// <param name="messageId">The Xtf Message id for the new Message.</param>
        /// <param name="messageType">The Xtf MessageType for this message.</param>
        public Message(uint messageId, MessageType messageType)
        {
            this._Info = new XTFMSGINFO();
            this._Info.MessageId = messageId;
            this._Info.MessageType = (uint)messageType;

            GUID riid = Extensions.ToGuid(typeof(IXtfMessage).GUID);
            IntPtr ppvObj;

            Core.BaseObject.CreateMessage(ref this._Info, ref riid, out ppvObj);

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfMessage;
            Marshal.Release(ppvObj);
        }

        /// <summary>
        /// Sets an array of bytes to be sent along with this message.
        /// </summary>
        /// <param name="index">The index that describes which message buffer is to be set.</param>
        /// <param name="buffer">The content of the message buffer.</param>
        /// <param name="flags">Flags that describe the buffer.</param>
        public void SetMessageBuffer(uint index, byte[] buffer, MessageBufferFlags flags)
        {
            this.BaseObject.SetMessageBuffer(index, ref buffer[0], (uint)buffer.Length, (uint)flags);
        }

        /// <summary>
        /// Gets an array of bytes that had previously been stored in a message.
        /// </summary>
        /// <param name="index">The index that describes which message buffer should be retrieved.</param>
        /// <param name="flags">Flags that describe the retrieved buffer.</param>
        /// <returns>An array of bytes that had previously been stored in a message.</returns>
        public byte[] GetMessageBuffer(uint index, out MessageBufferFlags flags)
        {
            unsafe
            {
                IntPtr bufferPointer = IntPtr.Zero;
                IntPtr ppBuffer = new IntPtr((void*)&bufferPointer);
                uint localFlags;
                uint bufferSize;
                this.BaseObject.GetMessageBuffer(index, ppBuffer, out bufferSize, out localFlags);

                flags = (MessageBufferFlags)localFlags;

                byte[] returnValue = new byte[bufferSize];
                Marshal.Copy(bufferPointer, returnValue, 0, (int)bufferSize);

                return returnValue;
            }
        }
    }
}
