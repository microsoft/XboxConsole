//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.FileIO.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.IO;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides a default implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>
        /// Copies a file from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFilePath">The path to the file on the PC to be copied.</param>
        /// <param name="destinationFile">The destination file to be copied to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public void SendFile(string systemIpAddress, string sourceFilePath, XboxPath destinationFile, IProgress<XboxFileTransferMetric> metrics)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (destinationFile == null)
            {
                throw new ArgumentNullException("destinationFile");
            }

            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentNullException("sourceFilePath");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.SendFileImpl(systemIpAddress, sourceFilePath, destinationFile, metrics),
                string.Format(CultureInfo.InvariantCulture, "Failed to copy file. Source: '{0}' Destination: '{1}' Operating System: '{2}'", sourceFilePath, destinationFile.FullName, destinationFile.OperatingSystem.ToString()));
        }

        /// <summary>
        /// Copies a file from an Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFile">The source file to be copied.</param>
        /// <param name="destinationFilePath">The destination of the file on the PC.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public void ReceiveFile(string systemIpAddress, XboxPath sourceFile, string destinationFilePath, IProgress<XboxFileTransferMetric> metrics)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentNullException("destinationFilePath");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.ReceiveFileImpl(systemIpAddress, sourceFile, destinationFilePath, metrics),
                string.Format(CultureInfo.InvariantCulture, "Failed to copy file. Source: '{0}' Operating System: '{1}' Destination: '{2}'", sourceFile.FullName, sourceFile.OperatingSystem.ToString(), destinationFilePath));
        }

        /// <summary>
        /// Deletes a file from an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The complete path to the file to be deleted.</param>
        public void DeleteFile(string systemIpAddress, XboxPath xboxFilePath)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (xboxFilePath == null)
            {
                throw new ArgumentNullException("xboxFilePath");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DeleteFileImpl(systemIpAddress, xboxFilePath),
                string.Format(CultureInfo.InvariantCulture, "Failed to delete file '{0}'", xboxFilePath.FullName));
        }

        /// <summary>
        /// Copies a directory from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The path to the directory on the PC to be copied.</param>
        /// <param name="destinationDirectory">The destination directory on the Xbox.</param>
        /// <param name="recursive">True to recursive copy all files in the given directory and all of its subdirectories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public void SendDirectory(string systemIpAddress, string sourceDirectory, XboxPath destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentNullException("sourceDirectory");
            }

            if (destinationDirectory == null)
            {
                throw new ArgumentNullException("destinationDirectory");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.SendDirectoryImpl(systemIpAddress, sourceDirectory, destinationDirectory, recursive, metrics),
                string.Format(CultureInfo.InvariantCulture, "Failed to copy directory. Source: '{0}' Destination: '{1}' Operating System: '{2}'", sourceDirectory, destinationDirectory.FullName, destinationDirectory.OperatingSystem.ToString()));
        }

        /// <summary>
        /// Copies a directory from the Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The Xbox diretory to copy.</param>
        /// <param name="destinationDirectory">The path to the destination directory on the PC.</param>
        /// <param name="recursive">True to recursive copy all files in the given directory and all of its subdirectories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public void ReceiveDirectory(string systemIpAddress, XboxPath sourceDirectory, string destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (string.IsNullOrWhiteSpace(destinationDirectory))
            {
                throw new ArgumentNullException("destinationDirectory");
            }

            if (sourceDirectory == null)
            {
                throw new ArgumentNullException("sourceDirectory");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.ReceiveDirectoryImpl(systemIpAddress, sourceDirectory, destinationDirectory, recursive, metrics),
                string.Format(CultureInfo.InvariantCulture, "Failed to copy directory. Source: '{0}' Operating System: '{1}' Destination: '{2}'", sourceDirectory.FullName, sourceDirectory.OperatingSystem.ToString(), destinationDirectory));
        }

        /// <summary>
        /// Remove the given directory from the Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="directoryToDelete">The Xbox directory to remove.</param>
        /// <param name="recurisve">True to recursivly delete the directory, its content and all of its sub-directories.</param>
        public void DeleteDirectory(string systemIpAddress, XboxPath directoryToDelete, bool recurisve)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (directoryToDelete == null)
            {
                throw new ArgumentNullException("directoryToDelete");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.DeleteDirectoryImpl(systemIpAddress, directoryToDelete, recurisve),
                string.Format(CultureInfo.InvariantCulture, "Failed to remove directory '{0}'", directoryToDelete.FullName));
        }

        /// <summary>
        /// Retrieves the contents of a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectory">The complete path to the directory.</param>
        /// <returns>The contents of a directory on an Xbox.</returns>
        public IEnumerable<XboxFileSystemInfoDefinition> GetDirectoryContents(string systemIpAddress, XboxPath xboxDirectory)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (xboxDirectory == null)
            {
                throw new ArgumentNullException("xboxDirectory");
            }

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetDirectoryContentsImpl(systemIpAddress, xboxDirectory),
                string.Format(CultureInfo.InvariantCulture, "Failed to retrieve contents of directory '{0}'", xboxDirectory.FullName));
        }

        /// <summary>
        /// Retrieves an XboxFileSystemInfoDefintion object for the specified file system object.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The file system object for a which an XboxFileSystemInfoDefinition object shall be retrieved.</param>
        /// <returns>An XboxFileSystemInfoDefinition object describing the file specified in the <paramref name="xboxFilePath"/> parameter.</returns>
        public XboxFileSystemInfoDefinition GetFileSystemInfoDefinition(string systemIpAddress, XboxPath xboxFilePath)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (xboxFilePath == null)
            {
                throw new ArgumentNullException("xboxFilePath");
            }

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetFileSystemInfoDefinitionImpl(systemIpAddress, xboxFilePath),
                string.Format(CultureInfo.InvariantCulture, "Failed to retrieve XboxFileSystemInfoDefinition for '{0}'", xboxFilePath));
        }

        /// <summary>
        /// Creates a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectoryPath">The complete path to the directory to be created.</param>
        public void CreateDirectory(string systemIpAddress, XboxPath xboxDirectoryPath)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (xboxDirectoryPath == null)
            {
                throw new ArgumentNullException("xboxDirectoryPath");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.CreateDirectoryImpl(systemIpAddress, xboxDirectoryPath),
                string.Format(CultureInfo.InvariantCulture, "Failed to create directory '{0}'", xboxDirectoryPath.FullName));
        }

        /// <summary>
        /// Copies a file from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFilePath">The path to the file on the PC to be copied.</param>
        /// <param name="destinationFile">The destination file to be copied to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected virtual void SendFileImpl(string systemIpAddress, string sourceFilePath, XboxPath destinationFile, IProgress<XboxFileTransferMetric> metrics)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Copies a file from an Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceFile">The source file to be copied.</param>
        /// <param name="destinationFilePath">The destination of the file on the PC.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected virtual void ReceiveFileImpl(string systemIpAddress, XboxPath sourceFile, string destinationFilePath, IProgress<XboxFileTransferMetric> metrics)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Deletes a file from an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The complete path to the file to be deleted.</param>
        protected virtual void DeleteFileImpl(string systemIpAddress, XboxPath xboxFilePath)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Copies a directory from a PC to an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The path to the directory on the PC to be copied.</param>
        /// <param name="destinationDirectory">The destination directory on the Xbox.</param>
        /// <param name="recursive">True to copy the directory and all of its sub-directories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected virtual void SendDirectoryImpl(string systemIpAddress, string sourceDirectory, XboxPath destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Copies a directory from the Xbox to a PC.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="sourceDirectory">The Xbox diretory to copy.</param>
        /// <param name="destinationDirectory">The path to the destination directory on the PC.</param>
        /// <param name="recursive">True to copy the directory and all of its sub-directories.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        protected virtual void ReceiveDirectoryImpl(string systemIpAddress, XboxPath sourceDirectory, string destinationDirectory, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Remove the given directory from the Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="directoryToDelete">The Xbox directory to remove.</param>
        /// <param name="recurisve">True to recursivly delete the directory, its content and all of its sub-directories.</param>
        protected virtual void DeleteDirectoryImpl(string systemIpAddress, XboxPath directoryToDelete, bool recurisve)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Retrieves an XboxFileSystemInfoDefintion object for the specified file system object.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxFilePath">The file system object for a which an XboxFileSystemInfoDefinition object shall be retrieved.</param>
        /// <returns>An XboxFileSystemInfoDefinition object describing the file specified in the <paramref name="xboxFilePath"/> parameter.</returns>
        protected virtual XboxFileSystemInfoDefinition GetFileSystemInfoDefinitionImpl(string systemIpAddress, XboxPath xboxFilePath)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Retrieves the contents of a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectory">The complete path to the directory.</param>
        /// <returns>The contents of a directory on an Xbox.</returns>
        protected virtual IEnumerable<XboxFileSystemInfoDefinition> GetDirectoryContentsImpl(string systemIpAddress, XboxPath xboxDirectory)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Creates a directory on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="xboxDirectoryPath">The complete path to the directory to be created.</param>
        protected virtual void CreateDirectoryImpl(string systemIpAddress, XboxPath xboxDirectoryPath)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
