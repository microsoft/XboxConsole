//------------------------------------------------------------------------------
// <copyright file="DirectoryInfoExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Represents extension methods for the DirectoryInfo type.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Copies a directory from a PC to an Xbox.
        /// </summary>
        /// <param name="directoryInfo">The directory info object pointing to the PC directory to copy.</param>
        /// <param name="xboxPath">The path on the Xbox to copy the directory contents to.</param>
        /// <param name="console">The Xbox console to copy the contents to.</param>
        /// <returns>An XboxDirectoryInfo object pointing to the newly copied directory.</returns>
        public static XboxDirectoryInfo CopyTo(this DirectoryInfo directoryInfo, XboxPath xboxPath, XboxConsole console)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            return CopyTo(directoryInfo, xboxPath, console, null);
        }

        /// <summary>
        /// Copies a directory from a PC to an Xbox.
        /// </summary>
        /// <param name="directoryInfo">The directory info object pointing to the PC directory to copy.</param>
        /// <param name="xboxPath">The path on the Xbox to copy the directory contents to.</param>
        /// <param name="console">The Xbox console to copy the contents to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <returns>An XboxDirectoryInfo object pointing to the newly copied directory.</returns>
        public static XboxDirectoryInfo CopyTo(this DirectoryInfo directoryInfo, XboxPath xboxPath, XboxConsole console, IProgress<XboxFileTransferMetric> metrics)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            if (directoryInfo == null)
            {
                throw new ArgumentNullException("directoryInfo");
            }

            if (xboxPath == null)
            {
                throw new ArgumentNullException("xboxPath");
            }

            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            if (!XboxDirectory.Exists(xboxPath, console))
            {
                XboxDirectory.Create(xboxPath, console);
            }

            var directorySize = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(x => x.Length);
            return directoryInfo.CopyToRecursive(xboxPath, console, directorySize, metrics);
        }

        private static XboxDirectoryInfo CopyToRecursive(this DirectoryInfo directoryInfo, XboxPath xboxPath, XboxConsole console, long totalSizeOfFolder, IProgress<XboxFileTransferMetric> metrics)
        {
            double totalTransferred = 0;
            double tempTransferred = 0;

            IProgress<XboxFileTransferMetric> newMetrics = null;

            if (metrics != null)
            {
                newMetrics = new Progress<XboxFileTransferMetric>((x) =>
                {
                    tempTransferred = x.TotalBytesTransferred;

                    metrics.Report(new XboxFileTransferMetric(x.SourceFilePath, x.TargetFilePath, x.FileSizeInBytes, x.FileBytesTransferred, totalSizeOfFolder, totalTransferred + tempTransferred));
                });
            }

            var directories = directoryInfo.GetDirectories();

            foreach (var directory in directories)
            {
                directory.CopyToRecursive(new XboxPath(Path.Combine(xboxPath.FullName, directory.Name), xboxPath.OperatingSystem), console, totalSizeOfFolder, newMetrics);

                totalTransferred += tempTransferred;
                tempTransferred = 0;
            }

            var files = directoryInfo.GetFiles();

            foreach (var file in files)
            {
                XboxFile.Copy(file.FullName, new XboxPath(Path.Combine(xboxPath.FullName, file.Name), xboxPath.OperatingSystem), console, newMetrics);
                totalTransferred += tempTransferred;
                tempTransferred = 0;
            }

            return new XboxDirectoryInfo(xboxPath, console);
        }
    }
}
