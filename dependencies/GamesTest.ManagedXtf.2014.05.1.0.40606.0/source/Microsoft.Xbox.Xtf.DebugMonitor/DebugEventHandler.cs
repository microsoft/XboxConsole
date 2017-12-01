using System;
using Microsoft.Xbox.XTF;

namespace Microsoft.Xbox.XTF.Diagnostics
{
    public class OutputDebugStringEventArgs
    {
        public uint ProcessId { get; private set; }
        public string DebugString { get; private set; }

        public OutputDebugStringEventArgs(uint processId, String value)
        {
            this.ProcessId = processId;
            this.DebugString = value;
        }
    }

    public delegate void OutputDebugStringEventHandler(object sender, OutputDebugStringEventArgs e);

    internal class DebugMonitorCallback : IXtfDebugMonitorCallback
    {
        private IXtfDebugMonitorClient client;
        private OutputDebugStringEventHandler outputDebugStringHandler;

        public DebugMonitorCallback(IXtfDebugMonitorClient client)
        {
            this.client = client;
        }

        public OutputDebugStringEventHandler EventHandler
        {
            get
            {
                lock(this)
                {
                    return this.outputDebugStringHandler;
                }
            }

            set
            {
                lock(this)
                {
                    this.outputDebugStringHandler = value;
                }
            }
        }

        void IXtfDebugMonitorCallback.OnOutputDebugString(ref XTF_OUTPUT_DEBUG_STRING_DATA data)
        {
            lock(this)
            {
                if(null != this.outputDebugStringHandler)
                {
                    this.outputDebugStringHandler(this, new OutputDebugStringEventArgs(data.dwProcessId, data.pszDbgStr));
                }
            }
        }
    }
}
