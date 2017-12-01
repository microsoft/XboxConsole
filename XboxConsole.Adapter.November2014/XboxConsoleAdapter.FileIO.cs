//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.FileIO.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.November2014
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.IO;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the November 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        /// <summary>
        /// Copies a file from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFilePath">The path to the file on the PC to be copied.</param>
        /// <param name="destinationFile">The destination file to be copied to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected override void SendFileImpl(string systemIpAddress, string sourceFilePath, XboxPath destinationFile, IProgress<XboxFileTransferMetric> metrics)
        {
            if (destinationFile == null)
            {
                throw new ArgumentNullException("destinationFile");
            }

            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentNullException("sourceFilePath");
            }

            this.XboxXdk.CopyFiles(systemIpAddress, sourceFilePath, destinationFile.FullName, destinationFile.OperatingSystem, 0, metrics);
        }

        /// <summary>
        /// Copies a file from an Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFile">The source file to be copied.</param>
        /// <param name="destinationFilePath">The destination of the file on the PC.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected override void ReceiveFileImpl(string systemIpAddress, XboxPath sourceFile, string destinationFilePath, IProgress<XboxFileTransferMetric> metrics)
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentNullException("destinationFilePath");
            }

            this.XboxXdk.CopyFiles(systemIpAddress, sourceFile.FullName, destinationFilePath, sourceFile.OperatingSystem, 0, metrics);
        }

        /// <summary>
        /// Deletes a file from an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The complete path to the file to be deleted.</param>
        protected override void DeleteFileImpl(string systemIpAddress, XboxPath xboxFilePath)
        {
            if (xboxFilePath == null)
            {
                throw new ArgumentNullException("xboxFilePath");
            }

            this.XboxXdk.DeleteFiles(systemIpAddress, xboxFilePath.FullName, xboxFilePath.OperatingSystem, 0);
        }

        /// <summary>
        /// Copies a directory from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The path to the directory on the PC to be copied.</param>
        /// <param name="destinationDirectory">The destination directory on the Xbox.</param>
        /// <param name="recursive">True to copy the directory and all of its sub-directories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected override void SendDirectoryImpl(string systemIpAddress, string sourceDirectory, XboxPath destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            if (destinationDirectory == null)
            {
                throw new ArgumentNullException("destinationDirectory");
            }

            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentNullException("sourceDirectory");
            }

            int recursionLevel = recursive ? -1 : 0;
            string searchPattern = Path.Combine(sourceDirectory, "*");
            this.XboxXdk.CopyFiles(systemIpAddress, searchPattern, destinationDirectory.FullName, destinationDirectory.OperatingSystem, recursionLevel, metrics);
        }

        /// <summary>
        /// Copies a directory from the Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The Xbox diretory to copy.</param>
        /// <param name="destinationDirectory">The path to the destination directory on the PC.</param>
        /// <param name="recursive">True to recurisve delete the content in the given directory and all of its subdirectories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected override void ReceiveDirectoryImpl(string systemIpAddress, XboxPath sourceDirectory, string destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            if (sourceDirectory == null)
            {
                throw new ArgumentNullException("sourceDirectory");
            }

            if (string.IsNullOrWhiteSpace(destinationDirectory))
            {
                throw new ArgumentNullException("destinationDirectory");
            }

            if (recursive)
            {
                ulong totalBytes = 0;
                if (metrics != null)
                {
                    totalBytes = this.GetDirectorySizeRecursive(systemIpAddress, sourceDirectory);
                }

                // The January XDK will not copy empty directories, so we have to do it ourselves.
                this.ReceiveDirectoryRecursive(systemIpAddress, sourceDirectory, destinationDirectory, totalBytes, metrics);
            }
            else
            {
                try
                {
                    var contents = this.GetDirectoryContents(systemIpAddress, sourceDirectory);
                    if (contents.Any(f => !f.IsDirectory))
                    {
                        this.XboxXdk.CopyFiles(systemIpAddress, sourceDirectory.FullName, destinationDirectory, sourceDirectory.OperatingSystem, 0, metrics);
                    }
                }
                catch (FileNotFoundException)
                {
                    // If the directory exists, but is empty then the XDK will throw a FileNotFoundException.
                }
            }
        }

        /// <summary>
        /// Remove the given directory from the Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="directoryToDelete">The Xbox directory to remove.</param>
        /// <param name="recursive">True to recursivly delete the directory, its content and all of its sub-directories.</param>
        protected override void DeleteDirectoryImpl(string systemIpAddress, XboxPath directoryToDelete, bool recursive)
        {
            if (directoryToDelete == null)
            {
                throw new ArgumentNullException("directoryToDelete");
            }

            if (recursive)
            {
                // For some reason, recursive directory deleting is completely broken in the January XDK.
                // If you try to use the recursive deleting functionality exposed by the XDK to delete an empty directory
                // then it will throw a "FileNotFoundException" even if the directory exists.  
                // Therefore, we have to do the recursive delete ourselves.
                this.DeleteDirectoryRecursive(systemIpAddress, directoryToDelete);
            }
            else
            {
                this.XboxXdk.RemoveDirectory(systemIpAddress, directoryToDelete.FullName, directoryToDelete.OperatingSystem, recursive);
            }
        }

        /// <summary>
        /// Retrieves an XboxFileSystemInfoDefintion object for the specified file system object.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The file system object for a which an XboxFileSystemInfoDefinition object shall be retrieved.</param>
        /// <returns>
        /// An XboxFileSystemInfoDefinition object describing the file specified in the <paramref name="xboxFilePath" /> parameter.
        /// </returns>
        protected override XboxFileSystemInfoDefinition GetFileSystemInfoDefinitionImpl(string systemIpAddress, XboxPath xboxFilePath)
        {
            if (xboxFilePath == null)
            {
                throw new ArgumentNullException("xboxFilePath");
            }

            bool isRoot = XboxPath.IsRoot(xboxFilePath.FullName);
            string cleanedFilePath = null;

            // For disk roots we need cleanedFilePath to keep the trailing separator char to make a valid searchPattern for XboxXdk.FindFiles.
            if (!isRoot &&
                (xboxFilePath.FullName.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase) ||
                xboxFilePath.FullName.EndsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)))
            {
                cleanedFilePath = xboxFilePath.FullName.Substring(0, xboxFilePath.FullName.Length - 1);
            }
            else
            {
                cleanedFilePath = xboxFilePath.FullName;
            }

            // If the "xboxFilePath" parameter represents a directory and we just pass the string
            // straight to the "FindFiles" method, then it will return only the content of the directory.
            // By appending a "*" onto the end we can retrieve the directory itself.  However, if the path ends with a trailing slash
            // then the search pattern won't work as expected, which is why the trailing slash is removed above.  Additionally, if "xboxFilePath"
            // represents a file then this process works as well.
            string searchPattern = string.Format(CultureInfo.InvariantCulture, "{0}*", cleanedFilePath);
            var matchingFiles = this.XboxXdk.FindFiles(systemIpAddress, searchPattern, xboxFilePath.OperatingSystem, 0);

            if (isRoot)
            {
                // For disk roots we create a definition object as long as XboxXdk.FindFiles above didn't throw an exception (that means the disk root was traversed successfully)
                return new XboxFileSystemInfoDefinition(0, FileAttributes.Directory, xboxFilePath.FullName, xboxFilePath.OperatingSystem, 0, 0, 0);
            }
            else
            {
                XboxFileSystemInfoDefinition returnValue = matchingFiles.FirstOrDefault(f => f.Path.FullName.Equals(cleanedFilePath, StringComparison.OrdinalIgnoreCase));
                if (returnValue == default(XboxFileSystemInfoDefinition))
                {
                    throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Unable to locate file on the Xbox's {0} operating system.", xboxFilePath.OperatingSystem), xboxFilePath.FullName);
                }
                else
                {
                    return returnValue;
                }
            }
        }

        /// <summary>
        /// Retrieves the contents of a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectory">The complete path to the directory.</param>
        /// <returns>
        /// The contents of a directory on an Xbox.
        /// </returns>
        protected override IEnumerable<XboxFileSystemInfoDefinition> GetDirectoryContentsImpl(string systemIpAddress, XboxPath xboxDirectory)
        {
            if (xboxDirectory == null)
            {
                throw new ArgumentNullException("xboxDirectory");
            }

            string searchPattern = Path.Combine(xboxDirectory.FullName, "*");
            return this.XboxXdk.FindFiles(systemIpAddress, searchPattern, xboxDirectory.OperatingSystem, 0);
        }

        /// <summary>
        /// Recursively delete the given directory and all of its subdirectories.  Note all directories must already be empty.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="directoryToDelete">The directory to delete.</param>
        protected void DeleteDirectoryRecursive(string systemIpAddress, XboxPath directoryToDelete)
        {
            if (directoryToDelete == null)
            {
                throw new ArgumentNullException("directoryToDelete");
            }

            // The January XDK is kind of crazy.  If you try to enumerate a directory that
            // exists, but is empty, then you will get a FileNotFoundException.  If that happens
            // then we can just ignore it and try to delete the directory anyway.
            IEnumerable<XboxFileSystemInfoDefinition> directoryContents = null;
            try
            {
                directoryContents = this.GetDirectoryContents(systemIpAddress, directoryToDelete);
            }
            catch (FileNotFoundException)
            {
            }

            if (directoryContents != null)
            {
                if (directoryContents.Any(f => !f.IsDirectory))
                {
                    string searchPattern = Path.Combine(directoryToDelete.FullName, "*");
                    this.XboxXdk.DeleteFiles(systemIpAddress, searchPattern, directoryToDelete.OperatingSystem, 0);
                }

                foreach (XboxFileSystemInfoDefinition subdirectory in directoryContents.Where(f => f.IsDirectory))
                {
                    this.DeleteDirectoryRecursive(systemIpAddress, subdirectory.Path);
                }
            }

            this.XboxXdk.RemoveDirectory(systemIpAddress, directoryToDelete.FullName, directoryToDelete.OperatingSystem, false);
        }

        /// <summary>
        /// Recursively copies a directory from an Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectory">The xbox directory to be copied.</param>
        /// <param name="destinationDirectory">The destination directory on the PC.</param>
        /// <param name="totalSizeOfFolder">The total size of the initial directory.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected void ReceiveDirectoryRecursive(string systemIpAddress, XboxPath xboxDirectory, string destinationDirectory, ulong totalSizeOfFolder, IProgress<XboxFileTransferMetric> metrics)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            try
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

                var contents = this.GetDirectoryContents(systemIpAddress, xboxDirectory);
                if (contents.Any(f => !f.IsDirectory))
                {
                    this.XboxXdk.CopyFiles(systemIpAddress, xboxDirectory.FullName, destinationDirectory, xboxDirectory.OperatingSystem, 0, newMetrics);

                    totalTransferred += tempTransferred;
                    tempTransferred = 0;
                }

                foreach (XboxFileSystemInfoDefinition subdirectory in contents.Where(f => f.IsDirectory))
                {
                    string directoryName = Path.Combine(destinationDirectory, Path.GetFileNameWithoutExtension(subdirectory.Path.FullName));
                    this.ReceiveDirectoryRecursive(systemIpAddress, subdirectory.Path, directoryName, totalSizeOfFolder, newMetrics);

                    totalTransferred += tempTransferred;
                    tempTransferred = 0;
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        /// <summary>
        /// Creates a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectoryPath">The complete path to the directory to be created.</param>
        protected override void CreateDirectoryImpl(string systemIpAddress, XboxPath xboxDirectoryPath)
        {
            if (xboxDirectoryPath == null)
            {
                throw new ArgumentNullException("xboxDirectoryPath");
            }

            this.XboxXdk.CreateDirectory(systemIpAddress, xboxDirectoryPath.FullName, xboxDirectoryPath.OperatingSystem);
        }

        /// <summary>
        /// Gets the size of a folder on the console.
        /// </summary>
        /// <param name="systemIpAddress">The ip of the console.</param>
        /// <param name="sourceDirectory">The directory to the size.</param>
        /// <returns>The size in bytes of the folder.</returns>
        private ulong GetDirectorySizeRecursive(string systemIpAddress, XboxPath sourceDirectory)
        {
            var folder = this.GetDirectoryContents(systemIpAddress, sourceDirectory);
            var sum = unchecked((ulong)folder.Sum(f => unchecked((long)f.FileSize)));

            foreach (var dir in folder.Where(f => f.IsDirectory))
            {
                sum += this.GetDirectorySizeRecursive(systemIpAddress, dir.Path);
            }

            return sum;
        }
    }
}
