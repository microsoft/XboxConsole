using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xbox.XTF;

using Microsoft.Win32.SafeHandles;

namespace Microsoft.Xbox.XTF.Application
{
    public sealed partial class ApplicationClient : IDisposable
    {
        private const string UnsnapCommand = "close";

        // Use method LazyClient() instead of this member.
        private IXtfApplicationClient lazyClient;
        private bool disposed = false;

        private Guid mediumId;
        private string address;

        public ApplicationClient(Guid mediumId, string address)
        {
            this.lazyClient = null;
            this.mediumId = mediumId;
            this.address = address;
        }

        public ApplicationClient(Guid mediumId)
            : this(mediumId, null)
        {
        }

        public ApplicationClient(string address)
            : this(Guid.Empty, address)
        {
        }

        public ApplicationClient()
            : this(Guid.Empty, null)
        {
        }

        ~ApplicationClient()
        {
            this.Dispose(false);
        }

        public string Address
        {
            get
            {
                return this.address;
            }
        }

        public string GetInstalled()
        {
            string toReturn = String.Empty;

            toReturn = LazyClient().GetInstalled();

            return toReturn;
        }

        /// <summary>
        /// Deploy an application from a PC to an Xbox.
        /// </summary>
        /// <param name="deployFilePath">The path to the directory that contains the title to be deployed.</param>
        /// <param name="removeExtraFiles">True to remove files that exist on the Xbox that are not in the directory given by the <paramref name="deployFilePath"/>.</param>
        /// <param name="progressCallback">A callback object whose methods will be invoked when progress is made during the deployment process.</param>
        /// <param name="result">The result of the deployment.  Zero indicates success, all else are failure.</param>
        /// <param name="packageFullName">The packageFullName of the package that has been deployed.</param>
        /// <returns>The AUMID of the newly deployed package.</returns>
        public string Deploy(string deployFilePath, bool removeExtraFiles, DeploymentProgress progressCallback, out bool canceled, out int result, out string packageFullName)
        {
            string toReturn = String.Empty;
            int iCanceled = 0;

            toReturn = LazyClient().Deploy(deployFilePath, removeExtraFiles ? 1 : 0, progressCallback, out iCanceled, out result, out packageFullName);

            canceled = (iCanceled != 0);

            return toReturn;
        }

        public string Deploy(string deployFilePath, bool removeExtraFiles, IXtfDeployCallback callback, out bool canceled, out int result, out string packageFullName)
        {
            string toReturn = String.Empty;
            int iCanceled = 0;

            toReturn = LazyClient().Deploy(deployFilePath, removeExtraFiles ? 1 : 0, callback, out iCanceled, out result, out packageFullName);

            canceled = (iCanceled != 0);

            return toReturn;
        }

        public void Launch(string applicationUserModelId)
        {
            LazyClient().Launch(applicationUserModelId);
        }

        public uint QueryExecutionState(string packageMoniker)
        {
            return LazyClient().QueryExecutionState(packageMoniker);
        }

        public void Suspend(string packageMoniker)
        {
            LazyClient().Suspend(packageMoniker, 1);
        }

        public void Suspend(string packageMoniker, int fAsDebugger)
        {
            LazyClient().Suspend(packageMoniker, fAsDebugger);
        }

        public void Resume(string packageMoniker)
        {
            LazyClient().Resume(packageMoniker);
        }

        public void Constrain(string packageMoniker)
        {
            LazyClient().Constrain(packageMoniker);
        }

        public void Unconstrain(string packageMoniker)
        {
            LazyClient().Unconstrain(packageMoniker);
        }

        public void Terminate(string packageMoniker)
        {
            LazyClient().Terminate(packageMoniker);
        }

        public void Uninstall(string packageMoniker)
        {
            LazyClient().Uninstall(packageMoniker);
        }

        public void Snap(string applicationUserModelId)
        {
            LazyClient().Snap(applicationUserModelId);
        }

        public void Unsnap()
        {
            LazyClient().Snap(UnsnapCommand);
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

        internal IXtfApplicationClient LazyClient()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Internal resource");
            }

            if(this.lazyClient == null)
            {
                System.Guid riid = typeof(IXtfApplicationClient).GUID;
                IntPtr ppvObj;

                if(API.XtfCreateApplicationClient(this.address, ref riid, out ppvObj) < 0)
                {
                    throw new XtfApplicationNoConnectionException("Unable to connect to " + this.address);
                }

                this.lazyClient = Marshal.GetObjectForIUnknown(ppvObj) as IXtfApplicationClient;
                Marshal.Release(ppvObj);
            }

            return this.lazyClient;
        }

        #endregion
    }

    public sealed class XtfApplicationNoConnectionException : System.Exception
    {
        public XtfApplicationNoConnectionException() :
            base()
        {
        }

        public XtfApplicationNoConnectionException(string message) :
            base(message)
        {
        }

        private XtfApplicationNoConnectionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public XtfApplicationNoConnectionException(string innerError, Exception innerException) :
            base(innerError, innerException)
        {
        }
    }
}
