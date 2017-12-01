//------------------------------------------------------------------------------
// <copyright file="DeploymentProgress.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Xbox.XTF.Application
{
    using System;

    /// <summary>
    /// A simple class that implements the IXtfDeployCallback interface so that users
    /// of this class don't need to have a reference to the Xtf Primary Interop Assembly.
    /// </summary>
    public class DeploymentProgress : IXtfDeployCallback
    {
        private Action<ChangeEvaluationCallbackArgs> changeEvaluationCallback;
        private Action<ErrorCallbackArgs> errorCallback;
        private Action<ExtraFileDetectedCallbackArgs> extraFileDetectedCallback;
        private Action<ExtraFileRemovedCallbackArgs> extraFileRemovedCallback;
        private Action<FileProgressCallbackArgs> fileProgressCallback;
        private Action initializedCallback;
        private Action<MetricsCallbackArgs> metricsCallback;

        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="changeEvaluationCallback">This callback invoked when XTF is evaluating which files need to be transferred or deleted.</param>
        /// <param name="errorCallback">This callback invoked when deployment has encountered an error.</param>
        /// <param name="extraFileDetectedCallback">This callback invoked when an extra file has been detected.</param>
        /// <param name="extraFileRemovedCallback">This callback invoked to report that an extra file on the console has been removed.</param>
        /// <param name="fileProgressCallback">This callback invoked to report progress in deploying a file to the console.</param>
        /// <param name="initializedCallback">This callback invoked when deploy is beginning.</param>
        /// <param name="metricsCallback">This callback invoked when deployment metrics are reported.</param>
        public DeploymentProgress(
            Action<ChangeEvaluationCallbackArgs> changeEvaluationCallback,
            Action<ErrorCallbackArgs> errorCallback,
            Action<ExtraFileDetectedCallbackArgs> extraFileDetectedCallback,
            Action<ExtraFileRemovedCallbackArgs> extraFileRemovedCallback,
            Action<FileProgressCallbackArgs> fileProgressCallback,
            Action initializedCallback,
            Action<MetricsCallbackArgs> metricsCallback)
        {
            this.changeEvaluationCallback = changeEvaluationCallback;
            this.errorCallback = errorCallback;
            this.extraFileDetectedCallback = extraFileDetectedCallback;
            this.extraFileRemovedCallback = extraFileRemovedCallback;
            this.fileProgressCallback = fileProgressCallback;
            this.initializedCallback = initializedCallback;
            this.metricsCallback = metricsCallback;
        }

        /// <summary>
        /// Callback invoked when XTF is evaluating which files need to be transferred or deleted. 
        /// </summary>
        /// <remarks>
        /// This callback is invoked when the deployment system is evaluating which files need to be deployed to the console or deleted
        /// from the console. You can use this callback to update a progress indicator. Note that if you are deploying a game or app
        /// that has not been registered on the target console, then all files must be deployed, and this callback will not be invoked
        /// at all. 
        /// </remarks>
        /// <param name="ullFilesProcessed">Number of files that have been evaluated so far.</param>
        /// <param name="ullTotalFiles">Total number of files to be evaluated.</param>
        public void OnDeployChangeEvaluation(ulong ullFilesProcessed, ulong ullTotalFiles)
        {
            if (this.changeEvaluationCallback != null)
            {
                this.changeEvaluationCallback(new ChangeEvaluationCallbackArgs(ullFilesProcessed, ullTotalFiles));
            }
        }

        /// <summary>
        /// Callback invoked when deployment has encountered an error. 
        /// </summary>
        /// <param name="hrError">An error HRESULT describing the error encountered in deployment.</param>
        /// <remarks><c>OnDeployError</c> is not used by the current implementation, but is reserved for future use.</remarks>
        public void OnDeployError(int hrError)
        {
            if (this.errorCallback != null)
            {
                this.errorCallback(new ErrorCallbackArgs(hrError));
            }
        }

        /// <summary>
        /// Callback invoked when an extra file has been detected. 
        /// </summary>
        /// <param name="pszFilePath">The absolute path to the extra file, on the console.</param>
        /// <remarks>
        /// Called once for every file found on the console that is not on the deploying PC. 
        /// </remarks>
        public void OnDeployExtraFileDetected(string pszFilePath)
        {
            if (this.extraFileDetectedCallback != null)
            {
                this.extraFileDetectedCallback(new ExtraFileDetectedCallbackArgs(pszFilePath));
            }
        }

        /// <summary>
        /// Callback invoked to report that an extra file on the console has been removed. 
        /// </summary>
        /// <param name="pszFilePath">Path to the extra file that was removed.</param>
        /// <remarks>
        /// If the fRemoveExtraFiles parameter to Deploy was set to TRUE, this callback is invoked
        /// when a file is deleted from the console.  
        /// </remarks>
        public void OnDeployExtraFileRemoved(string pszFilePath)
        {
            if (this.extraFileRemovedCallback != null)
            {
                this.extraFileRemovedCallback(new ExtraFileRemovedCallbackArgs(pszFilePath));
            }
        }

        /// <summary>
        /// Callback invoked to report progress in deploying a file to the console 
        /// </summary>
        /// <param name="pszFilePath">File path of the file.</param>
        /// <param name="ullBytesTransferred">Bytes transferred so far.</param>
        /// <param name="ullFileSize">Total size of the file being transferred, in bytes.</param>
        /// <remarks>
        /// <para>
        /// This callback is invoked every time that a buffer block is successfully transferred for a file. Note
        /// that the transfer process uses multiple threads, and your code must ensure that it is correlating
        /// progress messages by pszFilePath and protecting against the usual multi-threading issues. 
        /// </para>
        /// <para>
        /// This callback is always invoked once at the start of the deployment, and once at the end of deployment,
        /// so that if a zero-length file is being deployed, both invocations will report that zero bytes out of
        /// zero bytes have been transferred. 
        /// </para>
        /// </remarks>
        public void OnDeployFileProgress(string pszFilePath, ulong ullBytesTransferred, ulong ullFileSize)
        {
            if (this.fileProgressCallback != null)
            {
                this.fileProgressCallback(new FileProgressCallbackArgs(pszFilePath, ullBytesTransferred, ullFileSize));
            }
        }

        /// <summary>
        /// Callback invoked when deploy is beginning. 
        /// </summary>
        /// <remarks>
        /// This callback is invoked to indicate that the deployment system has successfully initialized and is beginning
        /// the deployment process. If your code needs to store a starting time to measure deployment time, or to
        /// initialize a progress dialog, this is a good place to do it. 
        /// </remarks>
        public void OnDeployInitialized()
        {
            if (this.initializedCallback != null)
            {
                this.initializedCallback();
            }
        }

        /// <summary>
        /// Callback invoked when deployment metrics are reported. 
        /// </summary>
        /// <param name="ullTotalFiles">Number of files that will be deployed.</param>
        /// <param name="ullTotalBytes">Number of bytes that will be transferred during deployment.</param>
        /// <remarks>
        /// This callback is invoked once at the end of the evaluation process, to report how many files will be deployed
        /// to the console and the total number of bytes to be transferred. 
        /// </remarks>
        public void OnDeployMetrics(ulong ullTotalFiles, ulong ullTotalBytes)
        {
            if (this.metricsCallback != null)
            {
                this.metricsCallback(new MetricsCallbackArgs(ullTotalFiles, ullTotalBytes));
            }
        }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment change evaluations.
    /// </summary>
    public class ChangeEvaluationCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the ChangeEvaluationCallbackArgs class.
        /// </summary>
        /// <param name="filesProcessed">Number of files that have been evaluated so far.</param>
        /// <param name="totalFiles">Total number of files to be evaluated.</param>
        public ChangeEvaluationCallbackArgs(ulong filesProcessed, ulong totalFiles)
        {
            this.FilesProcessed = filesProcessed;
            this.TotalFiles = totalFiles;
        }

        /// <summary>
        /// Gets the number of files that have been evaluated so far.
        /// </summary>
        public ulong FilesProcessed { get; private set; }
        /// <summary>
        /// Gets the total number of files to be evaluated.
        /// </summary>
        public ulong TotalFiles { get; private set; }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment errors.
    /// </summary>
    public class ErrorCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="error">An error HRESULT describing the error encountered in deployment.</param>
        public ErrorCallbackArgs(int error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets an error HRESULT describing the error encountered in deployment.
        /// </summary>
        public int Error { get; private set; }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment detection of extra files.
    /// </summary>
    public class ExtraFileDetectedCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="filePath">The absolute path to the extra file, on the console.</param>
        public ExtraFileDetectedCallbackArgs(string filePath)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets the absolute path to the extra file, on the console.
        /// </summary>
        public string FilePath { get; private set; }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment removal of extra files.
    /// </summary>
    public class ExtraFileRemovedCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="filePath">Path to the extra file that was removed.</param>
        public ExtraFileRemovedCallbackArgs(string filePath)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets the path to the extra file that was removed.
        /// </summary>
        public string FilePath { get; private set; }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment file progress.
    /// </summary>
    public class FileProgressCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="filePath">File path of the file.</param>
        /// <param name="bytesTransferred">Bytes transferred so far.</param>
        /// <param name="fileSize">Total size of the file being transferred, in bytes.</param>
        public FileProgressCallbackArgs(string filePath, ulong bytesTransferred, ulong fileSize)
        {
            this.FilePath = filePath;
            this.BytesTransferred = bytesTransferred;
            this.FileSize = fileSize;
        }

        /// <summary>
        /// Gets the file path of the file.
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// Gets the bytes transferred so far.
        /// </summary>
        public ulong BytesTransferred { get; private set; }
        /// <summary>
        /// Gets the total size of the file being transferred, in bytes.
        /// </summary>
        public ulong FileSize { get; private set; }
    }

    /// <summary>
    /// A class that representing callback arguments for deployment metrics.
    /// </summary>
    public class MetricsCallbackArgs
    {
        /// <summary>
        /// Initializes a new instance of the DeploymentProgress class.
        /// </summary>
        /// <param name="totalFiles">Number of files that will be deployed.</param>
        /// <param name="totalBytes">Number of bytes that will be transferred during deployment.</param>
        public MetricsCallbackArgs(ulong totalFiles, ulong totalBytes)
        {
            this.TotalFiles = totalFiles;
            this.TotalBytes = totalBytes;
        }

        /// <summary>
        /// Gets the number of files that will be deployed.
        /// </summary>
        public ulong TotalFiles { get; private set; }
        /// <summary>
        /// Gets the number of bytes that will be transferred during deployment.
        /// </summary>
        public ulong TotalBytes { get; private set; }
    }
}

