using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Console
{
    [Flags]
    public enum AddFlags : uint
    {
        None = 0x00000000,
        SetDefault = 0x00000001
    }

    internal class ConsoleEnumeratorCallback : IXtfEnumerateConsolesCallback
    {
        internal List<XtfConsole> consoles;

        internal ConsoleEnumeratorCallback()
        {
            this.consoles = new List<XtfConsole>();
        }

        public void OnConsoleFound(ref _XTFCONSOLEDATA consoleData)
        {
            this.consoles.Add(new XtfConsole(consoleData));
        }
    }
    
    public class ConsoleManager : IDisposable
    {
        internal IXtfConsoleManager consoleManager;
        internal ConsoleManagerCallback callback;
        private bool disposed = false;

        public event EventHandler<ConsoleEventArgs> ConsoleAdded;
        public event EventHandler<ConsoleEventArgs> ConsoleRemoved;
        public event EventHandler<ConsoleEventArgs> DefaultConsoleChanged;

        public ConsoleManager()
        {   
            this.callback = new ConsoleManagerCallback(this);
            this.callback.ConsoleAdded += this.OnConsoleAdded;
            this.callback.ConsoleRemoved += this.OnConsoleRemoved;
            this.callback.DefaultConsoleChanged += this.OnDefaultConsoleChanged;

            System.Guid riid = typeof(IXtfConsoleManager).GUID;
            IntPtr ppvObj;
            HRESULT.CHK(NativeMethods.XtfCreateConsoleManager(this.callback, ref riid, out ppvObj));
            this.consoleManager = null;
            this.consoleManager = Marshal.GetObjectForIUnknown(ppvObj) as IXtfConsoleManager;
            Marshal.Release(ppvObj);
        }

        ~ConsoleManager()
        {
            this.Dispose(false);
        }

        public static string GetDefaultAddress()
        {
            ConsoleManager cm = new ConsoleManager();
            XtfConsole c = cm.GetDefaultConsole();
            return c.Address;
        }

        public void AddConsole(string alias, string address, AddFlags flags)
        {
            this.consoleManager.AddConsole(alias, address, (uint)flags);
        }

        public void AddConsole(string alias, string address)
        {
            this.consoleManager.AddConsole(alias, address, (uint)AddFlags.None);
        }

        public void RemoveConsole(string alias)
        {
            this.consoleManager.RemoveConsole(alias);
        }

        public XtfConsole GetConsole(string alias)
        {
            XtfConsole console = null;
            try
            {
                console = new XtfConsole(this.consoleManager.GetConsole(alias));
            }
            catch (COMException)
            {
                console = null;
            }
            return console;
        }

        public List<XtfConsole> GetConsoles()
        {
            ConsoleEnumeratorCallback callback = new ConsoleEnumeratorCallback();
            consoleManager.EnumerateConsoles(callback);
            return callback.consoles;
        }

        public XtfConsole GetDefaultConsole()
        {
            XtfConsole console = null;
            try
            {
                return new XtfConsole(this.consoleManager.GetDefaultConsole());
            }
            catch (COMException)
            {
                // If an exception is thrown the default console is not set so return null.
                console = null;
            }
            return console;
        }

        public void SetDefaultConsole(string alias)
        {
            this.consoleManager.SetDefaultConsole(alias);
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
                if (null != this.consoleManager)
                {
                    Marshal.ReleaseComObject(this.consoleManager);
                    this.consoleManager = null;
                }

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

        internal void OnConsoleAdded(object sender, ConsoleEventArgs e)
        {
            EventHandler<ConsoleEventArgs> handler = this.ConsoleAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnConsoleRemoved(object sender, ConsoleEventArgs e)
        {
            EventHandler<ConsoleEventArgs> handler = this.ConsoleRemoved;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        internal void OnDefaultConsoleChanged(object sender, ConsoleEventArgs e)
        {
            EventHandler<ConsoleEventArgs> handler = this.DefaultConsoleChanged;
            if (null != handler)
            {
                handler(this, e);
            }
        }
    }
}
