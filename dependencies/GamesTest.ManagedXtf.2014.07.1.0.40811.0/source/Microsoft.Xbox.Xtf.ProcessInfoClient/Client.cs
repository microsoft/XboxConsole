namespace Microsoft.Xbox.XTF
{
    using System;
    using System.Runtime.InteropServices;

    public class ProcessInfoEx
    {
        internal XTFPROCESSINFOEX XtfProcessInfoEx;

        public ProcessInfoEx(uint processId, 
                             string processName, 
                             ulong workingSetSize, 
                             uint handleCount, 
                             uint threadCount, 
                             ulong pageFileUsage, 
                             uint pageFaultCount, 
                             ulong systemCpuTime, 
                             ulong processCpuTime)
        {
            this.XtfProcessInfoEx = new XTFPROCESSINFOEX();
            this.ProcessId = processId;
            this.ProcessName = processName;
            this.WorkingSetSize = workingSetSize;
            this.HandleCount = handleCount;
            this.ThreadCount = threadCount;
            this.PageFaultCount = pageFaultCount; 
            this.PageFileUsage = pageFileUsage;
            this.SystemCpuTime = systemCpuTime;
            this.ProcessCpuTime = processCpuTime; 
        }

        public uint ProcessId
        {
            get { return this.XtfProcessInfoEx.processID; }
            set { this.XtfProcessInfoEx.processID = value; }
        }

        public string ProcessName
        {
            get { return this.XtfProcessInfoEx.ProcessName; }
            set { this.XtfProcessInfoEx.ProcessName = value; }
        }

        public ulong WorkingSetSize
        {
            get { return this.XtfProcessInfoEx.WorkingSetSize; }
            set { this.XtfProcessInfoEx.WorkingSetSize = value; }
        }

        public uint HandleCount
        {
            get { return this.XtfProcessInfoEx.HandleCount; }
            set { this.XtfProcessInfoEx.HandleCount = value; }
        }

        public uint ThreadCount
        {
            get { return this.XtfProcessInfoEx.ThreadCount; }
            set { this.XtfProcessInfoEx.ThreadCount = value; }
        }

        public ulong PageFileUsage
        {
            get { return this.XtfProcessInfoEx.PageFileUsage; }
            set { this.XtfProcessInfoEx.PageFileUsage = value; }
        }
        public uint PageFaultCount
        {
            get { return this.XtfProcessInfoEx.PageFaultCount; }
            set { this.XtfProcessInfoEx.PageFaultCount = value; }
        }

        public ulong ProcessCpuTime
        {
            get { return this.XtfProcessInfoEx.ProcessCpuTime; }
            set { this.XtfProcessInfoEx.ProcessCpuTime = value; }
        }
        public ulong SystemCpuTime
        {
            get { return this.XtfProcessInfoEx.SystemCpuTime; }
            set { this.XtfProcessInfoEx.SystemCpuTime = value; }
        }
    }

    public class RunningProcessExEventArgs : EventArgs
    {
        public ProcessInfoEx ProcessInfoEx;

        internal RunningProcessExEventArgs(XTFPROCESSINFOEX processInfoEx)
        {
            this.ProcessInfoEx = new ProcessInfoEx(
                processInfoEx.processID,
                processInfoEx.ProcessName,
                processInfoEx.WorkingSetSize,
                processInfoEx.HandleCount,
                processInfoEx.ThreadCount, 
                processInfoEx.PageFileUsage, 
                processInfoEx.PageFaultCount, 
                processInfoEx.SystemCpuTime,
                processInfoEx.ProcessCpuTime);
        }
    }

    public delegate void RunningProcessExEventHandler(object sender, RunningProcessExEventArgs e);

    internal class RunningProcessExCallback : IXtfProcessExCallback
    {
        GetProcessInfoEx _Client;
        RunningProcessExEventHandler _EventHandler;

        public static RunningProcessExCallback Create(GetProcessInfoEx client, RunningProcessExEventHandler eventHandler)
        {
            if (null == eventHandler)
            {
                return null;
            }

            return new RunningProcessExCallback(client, eventHandler);
        }

        public RunningProcessExCallback(GetProcessInfoEx client, RunningProcessExEventHandler eventHandler)
        {
            this._Client = client;
            this._EventHandler = eventHandler;
        }

        void IXtfProcessExCallback.OnFoundProcess(XTFPROCESSINFOEX xpi)
        {
            this._EventHandler(this._Client, new RunningProcessExEventArgs(xpi));
        }
    }

    public sealed class GetProcessInfoEx : IDisposable
    {
        private bool _disposed;
        internal IXtfProcessInfoExClient BaseObject;


        public GetProcessInfoEx()
        {
            this._disposed = false;

            System.Guid riid = typeof(IXtfProcessInfoExClient).GUID;
            IntPtr ppvObj;

            HRESULT.CHK(NativeMethods.XtfCreateProcessInfoExClient(ref riid, out ppvObj));

            this.BaseObject = Marshal.GetObjectForIUnknown(ppvObj) as IXtfProcessInfoExClient;
            Marshal.Release(ppvObj);
        }

        ~GetProcessInfoEx()
        {
            this.Dispose(false);
        }

        public bool CheckConnectionStatus(String address, out int operatingSystemName)
        {
            int canConnect = 0;

            this.BaseObject.CheckConnectionStatus(address, out canConnect, out operatingSystemName);

            if (canConnect != 0)
            {
                return true;
            }
            return false; 
        }

        public void GetRunningProcessesEx(String address, RunningProcessExEventHandler eventHandler)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            this.BaseObject.GetRunningProcessesEx(address, new RunningProcessExCallback(this, eventHandler));
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
            if (!this._disposed)
            {
                Extensions.ReleaseComObject(ref this.BaseObject);

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

                this._disposed = true;
            }
        }
    }
}
