using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xbox.XTF;
using Microsoft.Xbox.XTF.Console;

using Microsoft.Win32.SafeHandles;

namespace Microsoft.Xbox.XTF.Input
{    
    [FlagsAttribute]
    public enum GamepadButtons : ushort
    {
        Right_thumbstick  = 0x8000,
        Left_thumbstick   = 0x4000,
        Right_shoulder    = 0x2000,
        Left_shoulder     = 0x1000,
        Dpad_right        = 0x800,
        Dpad_left         = 0x400,
        Dpad_down         = 0x200,
        Dpad_up           = 0x100,
        Y                 = 0x80,
        X                 = 0x40,
        B                 = 0x20,
        A                 = 0x10,
        View              = 0x8,
        Menu              = 0x4,
        Nexus             = 0x2,
        Enroll            = 0x1,
    }


    internal class InputClient : IDisposable
    {
        // Use method LazyClient() instead of this member.
        private IXtfInputClient lazyClient;
        private bool disposed = false;

        private Guid mediumId;
        private string address;

        public InputClient(Guid mediumId, string address)
        {
            this.lazyClient = null;
            this.mediumId = mediumId;
            this.address = address;

            if (this.address == null)
            {
                this.address = ConsoleManager.GetDefaultAddress();
            }
        }

        public InputClient(Guid mediumId)
            : this(mediumId, null)
        {
        }

        public InputClient(string address)
            : this(Guid.Empty, address)
        {
        }

        public InputClient()
            : this(Guid.Empty, null)
        {
        }

        ~InputClient()
        {
            this.Dispose(false);
        }

        public ulong ConnectGamepad()
        {
            ulong controllerId;

            LazyClient().ConnectGamepad(out controllerId);

            return controllerId;
        }

        public void DisconnectGamepad(ulong controllerId)
        {
            LazyClient().DisconnectGamepad(controllerId);
        }

        public void SendGamepadReport(ulong controllerId, GAMEPAD_REPORT report)
        {
            LazyClient().SendGamepadReport(controllerId, report);
        }



        #region IDisposable members

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

        #endregion

        #region Private members

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (null != this.lazyClient)
                {
                    Extensions.ReleaseComObject(ref this.lazyClient);
                }

                //
                // Since the garbage collector's Finalize() runs on
                // a background thread, managed objects are not safe
                // to reference.  Only clean up managed objects if this
                // is being explicitly disposed.
                //
                if (disposing)
                {
                    this.address = null;
                }

                this.disposed = true;
            }
        }

        private IXtfInputClient LazyClient()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            if(this.lazyClient == null)
            {
                System.Guid riid = typeof(IXtfInputClient).GUID;
                IntPtr ppvObj;

                if(API.XtfCreateInputClient(this.address, ref riid, out ppvObj) < 0)
                {
                    throw new XtfInputNoConnectionException("Unable to connect to " + this.address);
                }

                this.lazyClient = Marshal.GetObjectForIUnknown(ppvObj) as IXtfInputClient;
                Marshal.Release(ppvObj);
            }

            return this.lazyClient;
        }

        #endregion
    }

    public sealed class XtfInputNoConnectionException : System.Exception
    {
        public XtfInputNoConnectionException() :
            base()
        {
        }

        public XtfInputNoConnectionException(string message) :
            base(message)
        {
        }

        private XtfInputNoConnectionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public XtfInputNoConnectionException(string innerError, Exception innerException) :
            base(innerError, innerException)
        {
        }
    }

    public sealed class XtfInputException : System.Exception
    {
        public XtfInputException() :
            base()
        {
        }

        public XtfInputException(string message) :
            base(message)
        {
        }

        private XtfInputException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public XtfInputException(string innerError, Exception innerException) :
            base(innerError, innerException)
        {
        }
    }

}
