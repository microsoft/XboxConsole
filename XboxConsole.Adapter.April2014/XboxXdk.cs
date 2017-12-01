//------------------------------------------------------------------------------
// <copyright file="XboxXdk.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.April2014
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Xbox.Input;
    using Microsoft.Xbox.XTF;
    using Microsoft.Xbox.XTF.Application;
    using Microsoft.Xbox.XTF.Console;
    using Microsoft.Xbox.XTF.Diagnostics;
    using Microsoft.Xbox.XTF.IO;
    using Microsoft.Xbox.XTF.RemoteRun;
    using Microsoft.Xbox.XTF.User;

    /// <summary>
    /// Implementation of XboxXdk used in XboxConsoleAdapter.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class XboxXdk : XboxXdkBase
    {
        private const int SuspendNonDebugMode = 0;
        private const int SuspendDebugMode = 1;

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        public override string DefaultConsole
        {
            get
            {
                using (ConsoleManager manager = new ConsoleManager())
                {
                    var console = manager.GetDefaultConsole();
                    return console != null ? console.Address : null;
                }
            }

            set
            {
                using (ConsoleManager manager = new ConsoleManager())
                {
                    // try to find the console in the "neighbourhood" list by its tools IP address
                    var console = manager.GetConsoles().FirstOrDefault(c => c.Address.Equals(value, StringComparison.OrdinalIgnoreCase));
                    if (console == null)
                    {
                        // add one to the "neighborhood" if it was not found, create a unique alias for it
                        manager.AddConsole(string.Format(CultureInfo.InvariantCulture, "devkit_{0}", value), value, AddFlags.SetDefault);
                    }
                    else
                    {
                        manager.SetDefaultConsole(console.Alias);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a string describing all of the applications installed on the console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to retrieve the list of installed packages from.</param>
        /// <returns>A string describing all of the applications installed on the console.</returns>
        public override string GetInstalledPackages(string systemIpAddress)
        {
            using (ApplicationClient appClient = new ApplicationClient(systemIpAddress))
            {
                return appClient.GetInstalled();
            }
        }

        /// <summary>
        /// Gets the list of processes running on a console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        public override IEnumerable<XboxProcessDefinition> GetRunningProcesses(string systemIpAddress, XboxOperatingSystem operatingSystem)
        {
            IList<XboxProcessDefinition> runningProcesses = new List<XboxProcessDefinition>();
            RunningProcessEventHandler runningProcessEventHandler = (sender, runningProcessEventArgs) =>
            {
                runningProcesses.Add(new XboxProcessDefinition(operatingSystem, runningProcessEventArgs.ProcessInfo.ProcessId, runningProcessEventArgs.ProcessInfo.ImageFileName));
            };

            using (ConsoleControlClient console = new ConsoleControlClient(this.GetOperatingSystemConnectionString(systemIpAddress, operatingSystem)))
            {
                console.GetRunningProcesses(runningProcessEventHandler);
            }

            return runningProcesses;
        }

        /// <summary>
        /// Returns true if a console can be connected to (responsive).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        /// <returns>The indication if a console can be connected to.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "notUsed", Justification = "Calling a property is needed to enforce throwing an exception, the returned value is irrelevant.")]
        public override bool CanConnect(string systemIpAddress)
        {
            try
            {
                using (ConsoleControlClient console = new ConsoleControlClient(systemIpAddress))
                {
                    DateTime notUsed = console.SystemTime; // Need to call something to enforce it to throw an exception when it cannot connect.
                }

                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        /// <summary>
        /// Reboots the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        public override void Reboot(string systemIpAddress)
        {
            using (ConsoleControlClient console = new ConsoleControlClient(systemIpAddress))
            {
                console.ShutdownConsole(ShutdownConsoleFlags.Reboot);
            }
        }

        /// <summary>
        /// Shutdowns the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        public override void Shutdown(string systemIpAddress)
        {
            using (ConsoleControlClient console = new ConsoleControlClient(systemIpAddress))
            {
                console.ShutdownConsole(ShutdownConsoleFlags.None);
            }
        }

        /// <summary>
        /// Launches the application with the given Application Usage Model Id.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the application will be launched.</param>
        /// <param name="aumid">The AUMID of the application to launch.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void LaunchApplication(string systemIpAddress, string aumid)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Launch(aumid);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Terminates the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to terminate.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void TerminatePackage(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Terminate(packageFullName);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Suspends the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to suspend.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void SuspendPackage(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Suspend(packageFullName, SuspendNonDebugMode);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Resumes the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to resume.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void ResumePackage(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Resume(packageFullName);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Constrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the package will be constrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to constrain.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void ConstrainPackage(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Constrain(packageFullName);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }            
        }

        /// <summary>
        /// Unconstrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the package will be unconstrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to unconstrain.</param>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override void UnconstrainPackage(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Unconstrain(packageFullName);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }            
        }

        /// <summary>
        /// Snaps the application with the given Application Usage Model Id.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the application will be snapped.</param>
        /// <param name="aumid">The AUMID of the application to snapped.</param>
        public override void SnapApplication(string systemIpAddress, string aumid)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Snap(aumid);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new XboxSnapException("Unable to snap the application.", ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Unsnaps the currently snapped application.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the application will be unsnanpped.</param>
        public override void UnsnapApplication(string systemIpAddress)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    client.Unsnap();
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Retrieves the execution state of the package with the given Package Full Name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the state of the package will be retrieved.</param>
        /// <param name="packageFullName">The Package Full Name of the package to resume.</param>
        /// <returns>
        /// A value representing the current execution state of the given package.
        /// </returns>
        /// <exception cref="CannotConnectException">Thrown when not able to connect to the console.</exception>
        public override uint QueryPackageExecutionState(string systemIpAddress, string packageFullName)
        {
            try
            {
                using (ApplicationClient client = new ApplicationClient(systemIpAddress))
                {
                    return client.QueryExecutionState(packageFullName);
                }
            }
            catch (XtfApplicationNoConnectionException ex)
            {
                throw new CannotConnectException(string.Format(CultureInfo.InvariantCulture, "Unable to connect to {0}.", systemIpAddress), ex, systemIpAddress);
            }
        }

        /// <summary>
        /// Copies a file from either a PC to an Xbox or from an Xbox to a PC.  The direction of the copy is dependent on the paths
        /// passed to <paramref name="sourceFilePath" /> and <paramref name="destinationFilePath" />.  If one of the paths starts with
        /// the letter "x", then it is considered to be the path to the file on the Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to copy a file from or to.</param>
        /// <param name="sourceFilePath">The path to the file to be copied.  If this file resides on the Xbox then the path must start with the letter "x".  For example, if the
        /// file is on the Xbox's D:\ drive, then the path must start with "XD:\".</param>
        /// <param name="destinationFilePath">The complete destination path for the file.  If the destination is intended to be on the Xbox then this path must start with the letter "x".
        /// For example, if you wish to copy the file to the D:\ drive, then this path must start with "XD:\".</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox that you wish to copy the file from or to.</param>
        /// <param name="recursionLevel">The number of levels of recursion to use when looking for files to copy.  Pass -1 to indicate a completely recursive copy.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public override void CopyFiles(string systemIpAddress, string sourceFilePath, string destinationFilePath, XboxOperatingSystem targetOperatingSystem, int recursionLevel, IProgress<XboxFileTransferMetric> metrics)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                throw new ArgumentException("sourceFilePath must not be null or empty", "sourceFilePath");
            }

            using (FileIOClient fileIoClient = new FileIOClient(this.GetOperatingSystemConnectionString(systemIpAddress, targetOperatingSystem)))
            {
                CopyFileStartEventHandler copyFileStartHandler = (sender, args) => { };
                CopyFileProgressEventHandler copyFileProgressEventHandler = null;
                if (metrics == null)
                {
                    copyFileProgressEventHandler = (sender, args) => { };
                }
                else
                {
                    double totalBytes = 0;
                    if (sourceFilePath.StartsWith("X", StringComparison.OrdinalIgnoreCase) || sourceFilePath.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                    {
                        totalBytes = unchecked((ulong)this.FindFiles(systemIpAddress, sourceFilePath, targetOperatingSystem, 0).Sum((f) => unchecked((long)f.FileSize)));
                    }
                    else
                    {
                        var folder = new DirectoryInfo(sourceFilePath);

                        if (folder.Exists)
                        {
                            totalBytes = folder.EnumerateFiles("*").Sum(f => f.Length);
                        }
                        else
                        {
                            var file = new System.IO.FileInfo(sourceFilePath);

                            if (file.Exists)
                            {
                                totalBytes = file.Length;
                            }
                            else
                            {
                                throw new FileNotFoundException("Unable to find Source path provided.");
                            }
                        }
                    }

                    Dictionary<string, ulong> filesTransferProgress = new Dictionary<string, ulong>();
                    copyFileProgressEventHandler = (sender, args) =>
                        {
                            lock (filesTransferProgress)
                            {
                                if (filesTransferProgress.ContainsKey(args.SourceFileName))
                                {
                                    filesTransferProgress[args.SourceFileName] = args.BytesCopied;
                                }
                else
                {
                                    filesTransferProgress.Add(args.SourceFileName, args.BytesCopied);
                                }

                                metrics.Report(new XboxFileTransferMetric(
                                    sourceFilePath: args.SourceFileName,
                                    targetFilePath: args.TargetFileName,
                                    fileSize: args.FileSize,
                                    fileBytesTransferred: args.BytesCopied,
                                    totalSize: totalBytes,
                                    totalBytesTransferred: unchecked((ulong)filesTransferProgress.Sum(f => unchecked((long)f.Value)))));
                            }
                        };
                }

                // The idea to pass "-1" as the includeAttributes came straight from the XTF team's source code.  They are doing the same thing in the "xbcp" tool.
                // Passing "FileAttributes.System | FileAttributes.Hidden" as the excludeAttributes came straight from the XTF team's source code.  They are doing the same thing in the "xbcp" tool.
                fileIoClient.CopyFiles(sourceFilePath, (FileAttributes)(-1), FileAttributes.System | FileAttributes.Hidden, (uint)recursionLevel, destinationFilePath, CopyFileFlags.None, copyFileStartHandler, copyFileProgressEventHandler);
            }
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="systemIpAddress">The IP address.</param>
        /// <param name="remoteDirectoryPath">The remote directory path.</param>
        /// <param name="targetOperatingSystem">The target operating system.</param>
        public override void CreateDirectory(string systemIpAddress, string remoteDirectoryPath, XboxOperatingSystem targetOperatingSystem)
        {
            using (FileIOClient fileIoClient = new FileIOClient(this.GetOperatingSystemConnectionString(systemIpAddress, targetOperatingSystem)))
            {
                fileIoClient.CreateDirectory(remoteDirectoryPath);
            }
        }

        /// <summary>
        /// Deletes files from an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the files will be deleted.</param>
        /// <param name="remoteFileSearchPattern">The search path for the files to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the files will be deleted.</param>
        /// <param name="recursionLevel">The number of levels of recursion to use when searching for files to delete.</param>
        public override void DeleteFiles(string systemIpAddress, string remoteFileSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevel)
        {
            using (FileIOClient fileIoClient = new FileIOClient(this.GetOperatingSystemConnectionString(systemIpAddress, targetOperatingSystem)))
            {
                FindFileEventHandler findFileHandler = (sender, args) => { };
                fileIoClient.DeleteFiles(remoteFileSearchPattern, (FileAttributes)(-1), FileAttributes.System | FileAttributes.Hidden, (uint)recursionLevel, DeleteFileFlags.None, findFileHandler);
            }
        }

        /// <summary>
        /// Removes a directory from an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which the directory will be deleted.</param>
        /// <param name="remoteDestinationPath">The complete path to the directory to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the directory will be removed.</param>
        /// <param name="recursive">A flag to indicate whether or not to recursively delete the contents of the given directory and all
        /// of its children.</param>
        public override void RemoveDirectory(string systemIpAddress, string remoteDestinationPath, XboxOperatingSystem targetOperatingSystem, bool recursive)
        {
            using (FileIOClient fileIoClient = new FileIOClient(this.GetOperatingSystemConnectionString(systemIpAddress, targetOperatingSystem)))
            {
                FindFileEventHandler findFileHandler = (sender, args) => { };
                fileIoClient.RemoveDirectory(remoteDestinationPath, recursive ? RemoveDirectoryFlags.Force : RemoveDirectoryFlags.None, findFileHandler);
            }
        }

        /// <summary>
        /// Retrieves a collection of files on an Xbox that match the given search pattern.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console on which to search for files.</param>
        /// <param name="remoteSearchPattern">The pattern used to search for files.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox to search for files.</param>
        /// <param name="recursionLevels">The number of recursion levels to use while searching for matches.</param>
        /// <returns>
        /// A enumeration of the files that match the given search pattern.
        /// </returns>
        public override IEnumerable<XboxFileSystemInfoDefinition> FindFiles(string systemIpAddress, string remoteSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevels)
        {
            List<XboxFileSystemInfoDefinition> returnValue = new List<XboxFileSystemInfoDefinition>();
            using (FileIOClient fileIoClient = new FileIOClient(this.GetOperatingSystemConnectionString(systemIpAddress, targetOperatingSystem)))
            {
                FindFileEventHandler findFileHandler = (sender, args) =>
                    {
                        returnValue.Add(CreateXboxFileSystemInfoDefintion(args.FileInfo, targetOperatingSystem));
                    };

                fileIoClient.FindFiles(remoteSearchPattern, (FileAttributes)(-1), FileAttributes.System | FileAttributes.Hidden, (uint)recursionLevels, FindFileFlags.None, findFileHandler);
            }

            return returnValue;
        }

        /// <summary>
        /// Creates an Xbox debug monitor client.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The Xbox debug monitor client.</returns>
        public override IXboxDebugMonitorClient CreateDebugMonitorClient(string systemIpAddress, XboxOperatingSystem operatingSystem)
        {
            return new XboxDebugMonitorClient(this.GetOperatingSystemConnectionString(systemIpAddress, operatingSystem));
        }

        /// <summary>
        /// Queries for and returns the system IP address of a console identified by the specified tools IP address.
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of a console.</param>
        /// <returns>The system IP address of the console.</returns>
        public override string GetSystemIpAddress(string systemIpAddress)
        {
            // Getting System IP address via Microsoft.Xbox.XTF.SystemInfo is not working in August QFE3 
            // (it throws an exception saying "An existing connection was forcibly closed by the remote host. (Exception from HRESULT: 0x80072746)").
            // As a temporary solution, one of the title test teams has requested that instead of throwing a not-supported exception they would rather prefer 
            // returning an invalid IP address.
            return IPAddress.None.ToString();
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        public override string GetConfigValue(string systemIpAddress, string key)
        {
            using (ConsoleControlClient console = new ConsoleControlClient(systemIpAddress))
            {
                return console.GetConfigValue(key);
            }
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        public override void SetConfigValue(string systemIpAddress, string key, string value)
        {
            using (ConsoleControlClient console = new ConsoleControlClient(systemIpAddress))
            {
                console.SetConfigValue(key, value);
            }
        }

        /// <summary>
        /// Creates a virtual gamepad for the console.
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of the console.</param>
        /// <returns>An IVirtualGamepad instance.</returns>
        public override IVirtualGamepad CreateXboxGamepad(string systemIpAddress)
        {
            return new XboxGamepadAdapter(systemIpAddress, new XtfGamepad(systemIpAddress));
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of the console.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the deployment to complete.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is a json string describing the deployed package.</returns>
        public override async Task<string> DeployPushAsync(string systemIpAddress, string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            return await Task.Run(() =>
            {
                DeploymentProgress progressHandler = null;
                if (progressMetric != null || progressExtraFile != null || progressError != null)
                {
                    // Create some variables that will be used by every progress update
                    ulong filesTransferred = 0;
                    double bytesTransferred = 0;
                    double totalFiles = 0;
                    double totalBytes = 0;

                    progressHandler = new DeploymentProgress(
                        initializedCallback: null,
                        changeEvaluationCallback: null,
                        metricsCallback: (e) =>
                            {
                                totalFiles = e.TotalFiles;
                                totalBytes = e.TotalBytes;
                            },
                        fileProgressCallback: (e) =>
                            {
                                // When data1 == data2, that means that we've finished sending the data for this file
                                double tempBytes = 0;
                                if (e.BytesTransferred == e.FileSize)
                                {
                                    bytesTransferred += e.FileSize;
                                    tempBytes = bytesTransferred;
                                    ++filesTransferred;
                                }
                                else
                                {
                                    tempBytes = bytesTransferred + e.BytesTransferred;
                                }

                                if (progressMetric != null)
                                {
                                    progressMetric.Report(new XboxDeploymentMetric(
                                        filePath: e.FilePath,
                                        fileBytesTransferred: e.BytesTransferred,
                                        fileSizeInBytes: e.FileSize,
                                        percentageFileBytesTransferred: (e.FileSize == 0.0) ? 0.0 : (double)e.BytesTransferred / (double)e.FileSize,
                                        totalBytes: totalBytes,
                                        totalBytesTransferred: tempBytes,
                                        percentageTotalBytesTransferred: (totalBytes == 0.0) ? 0.0 : tempBytes / totalBytes,
                                        totalFilesTransferred: filesTransferred,
                                        totalFiles: totalFiles));
                                }
                            },
                        errorCallback: (e) =>
                            {
                                if (progressError != null)
                                {
                                    progressError.Report(new XboxDeploymentError(e.Error));
                                }
                            },
                        extraFileDetectedCallback: (e) =>
                            {
                                if (progressExtraFile != null)
                                {
                                    progressExtraFile.Report(new XboxDeploymentExtraFile(
                                        filePath: e.FilePath,
                                        extraFileDetected: true,
                                        extraFileRemoved: false));
                                }
                            },
                        extraFileRemovedCallback: (e) =>
                            {
                                if (progressExtraFile != null)
                                {
                                    progressExtraFile.Report(new XboxDeploymentExtraFile(
                                        filePath: e.FilePath,
                                        extraFileDetected: false,
                                        extraFileRemoved: true));
                                }
                            });
                }

                using (ApplicationClient appClient = new ApplicationClient(systemIpAddress))
                {
                    int result;
                    string fullPackageName;
                    bool cancelled;
                    try
                    {
                        return appClient.Deploy(deployFilePath, removeExtraFiles, progressHandler, out cancelled, out result, out fullPackageName);
                    }
                    catch (COMException e)
                    {
                        throw new XboxDeployException("The build could not be successfully deployed.", e, appClient.Address);
                    }
                    catch (FileLoadException e)
                    {
                        throw new XboxDeployException("The build could not be successfully deployed.", e, appClient.Address);
                    }
                }
            });
        }

        /// <summary>
        /// Runs an executable on the console.
        /// </summary>
        /// <param name="systemIpAddress">The tools IP address of the console.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputRecievedCallback">A callback method that will be called when there is output from the process.</param>
        public override void RunExecutable(string systemIpAddress, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputRecievedCallback)
        {
            var file = Path.GetFileName(fileName);
            var path = Path.GetDirectoryName(fileName);

            RunFlags flags = RunFlags.None;
            uint period = 0;

            RemoteRunEventHandler outputHandler = null;
            if (outputRecievedCallback != null)
            {
                flags = RunFlags.RedirectOutput;
                outputHandler = (o, e) =>
                {
                    outputRecievedCallback(e.Text);
                };
            }

            using (RemoteRunClient console = new RemoteRunClient(this.GetOperatingSystemConnectionString(systemIpAddress, operatingSystem)))
            {
                console.Run(string.Format(CultureInfo.InvariantCulture, "{0} {1}", file, arguments), path, flags, period, null, outputHandler);
            }
        }

        /// <summary>
        /// Gets the list of users on a console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        /// <returns>An enumeration of XboxUserDefinition instances.</returns>
        public override IEnumerable<XboxUserDefinition> GetUsers(string systemIpAddress)
        {
            using (UserClient console = new UserClient(systemIpAddress))
            {
                return console.ListUsers().Select(x => new XboxUserDefinition(x.UserId, x.EmailAddress, x.Gamertag, x.SignedIn));
            }
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        public override uint AddGuestUser(string systemIpAddress)
        {
            using (UserClient console = new UserClient(systemIpAddress))
            {
                return console.AddSponseredUser();
            }
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        public override XboxUserDefinition AddUser(string systemIpAddress, string emailAddress)
        {
            using (UserClient console = new UserClient(systemIpAddress))
            {
                var id = console.AddUser(emailAddress);

                var user = console.ListUsers().FirstOrDefault(x => x.UserId == id);

                if (user == null)
                {
                    throw new XboxConsoleException(string.Format(CultureInfo.InvariantCulture, "Unable to confirm that the user with email address {0} was added", emailAddress), systemIpAddress);
                }

                return new XboxUserDefinition(user.UserId, user.EmailAddress, user.Gamertag, user.SignedIn);
            }
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        public override void DeleteAllUsers(string systemIpAddress)
        {
            using (UserClient console = new UserClient(systemIpAddress))
            {
                console.DeleteAllUsers();
            }
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        public override void DeleteUser(string systemIpAddress, XboxUserDefinition user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "user cannot be null");
            }

            using (UserClient console = new UserClient(systemIpAddress))
            {
                console.DeleteUser(user.EmailAddress);
            }
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        public override XboxUserDefinition SignInUser(string systemIpAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "user cannot be null");
            }

            using (UserClient console = new UserClient(systemIpAddress))
            {
                console.SignInUserId(user.UserId, password, storePassword);

                var consoleUser = console.ListUsers().FirstOrDefault(x => x.UserId == user.UserId);

                if (consoleUser == null)
                {
                    throw new XboxConsoleException(string.Format(CultureInfo.InvariantCulture, "Unable to confirm that the user with email address {0} was signed-in", user.EmailAddress), systemIpAddress);
                }

                return new XboxUserDefinition(consoleUser.UserId, consoleUser.EmailAddress, consoleUser.Gamertag, consoleUser.SignedIn);
            }
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        public override XboxUserDefinition SignOutUser(string systemIpAddress, XboxUserDefinition user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "user cannot be null");
            }

            using (UserClient console = new UserClient(systemIpAddress))
            {
                console.SignOutUser(user.EmailAddress);

                var consoleUser = console.ListUsers().FirstOrDefault(x => x.UserId == user.UserId);

                if (consoleUser == null)
                {
                    throw new XboxConsoleException(string.Format(CultureInfo.InvariantCulture, "Unable to confirm that the user with email address {0} was signed-in", user.EmailAddress), systemIpAddress);
                }

                return new XboxUserDefinition(consoleUser.UserId, consoleUser.EmailAddress, consoleUser.Gamertag, consoleUser.SignedIn);
            }
        }

        /// <summary>
        /// Pairs a controller to a user on a console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of console.</param>
        /// <param name="userId">The user id of the user to pair.</param>
        /// <param name="controllerId">The controller of the id to pair.</param>
        public override void PairControllerToUser(string systemIpAddress, uint userId, ulong controllerId)
        {
            using (UserClient console = new UserClient(systemIpAddress))
            {
                console.PairControllerWithUser(userId, controllerId);
            }
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the specified console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        public override IntPtr CaptureScreenshot(string systemIpAddress)
        {
            return ConsoleControlClient.CaptureScreenshot(systemIpAddress);
        }

        /// <summary>
        /// Uninstall a package from a given console based on its package full name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to be affected.</param>
        /// <param name="packageFullName">The Pacakge Full Name of the package to be uninstalled.</param>
        public override void UninstallPackage(string systemIpAddress, string packageFullName)
        {
            using (ApplicationClient client = new ApplicationClient(systemIpAddress))
            {
                client.Uninstall(packageFullName);
            }
        }

        /// <summary>
        /// A method for converting a Microsoft.Xbox.XTF.IO.FileInfo object into an XboxFileSystemInfoDefintion object.
        /// </summary>
        /// <param name="fileInfo">An XTF object for describing a file.</param>
        /// <param name="xboxOperatingSystem">The operating system from which the file came.</param>
        /// <returns>An XboxFileSystemInfoDefinition object describing the same file given by the <paramref name="fileInfo"/> parameter.</returns>
        private XboxFileSystemInfoDefinition CreateXboxFileSystemInfoDefintion(Microsoft.Xbox.XTF.IO.FileInfo fileInfo, XboxOperatingSystem xboxOperatingSystem)
        {
            return new XboxFileSystemInfoDefinition(fileInfo.CreationTime, fileInfo.FileAttributes, fileInfo.FileName, xboxOperatingSystem, fileInfo.FileSize, fileInfo.LastAccessTime, fileInfo.LastWriteTime);
        }

        /// <summary>
        /// Creates a connection string for an Xbox and a specific operating system on that Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the Xbox.</param>
        /// <param name="targetOperatingSystem">The operating system to connect to.</param>
        /// <returns>A connection string that can be used to connect to a specific operating system on an Xbox.</returns>
        private string GetOperatingSystemConnectionString(string systemIpAddress, XboxOperatingSystem targetOperatingSystem)
        {
            switch (targetOperatingSystem)
            {
                case XboxOperatingSystem.System:
                    return string.Format(CultureInfo.InvariantCulture, "{0}/system", systemIpAddress);
                case XboxOperatingSystem.Title:
                    return string.Format(CultureInfo.InvariantCulture, "{0}/title", systemIpAddress);
            }

            throw new ArgumentException("Invalid XboxOperatingSystem specified", "targetOperatingSystem");
        }
    }
}
