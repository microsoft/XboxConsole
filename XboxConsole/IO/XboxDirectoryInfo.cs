//------------------------------------------------------------------------------
// <copyright file="XboxDirectoryInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents an Xbox directory.
    /// </summary>
    public class XboxDirectoryInfo : XboxFileSystemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDirectoryInfo"/> class.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="operatingSystem">The operating system on which the directory resides.</param>
        /// <param name="console">The console on which the directory resides.</param>
        public XboxDirectoryInfo(string path, XboxOperatingSystem operatingSystem, XboxConsole console)
            : base(path, operatingSystem, console)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDirectoryInfo"/> class.
        /// </summary>
        /// <param name="xboxPath">The Xbox path to the directory.</param>
        /// <param name="console">The console on which the directory resides.</param>
        public XboxDirectoryInfo(XboxPath xboxPath, XboxConsole console)            
            : base(xboxPath, console)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDirectoryInfo"/> class.
        /// </summary>
        /// <param name="definition">The definition of the directory.</param>
        /// <param name="console">The console on which the directory resides.</param>
        internal XboxDirectoryInfo(XboxFileSystemInfoDefinition definition, XboxConsole console)
            : base(definition, console)
        {
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public XboxDirectoryInfo Parent
        {
            get
            {
                if (XboxPath.IsRoot(this.FullName))
                {
                    return null;
                }
                else
                {
                    string fullPath = this.FullName;

                    if (fullPath[fullPath.Length - 1] == Path.DirectorySeparatorChar || fullPath[fullPath.Length - 1] == Path.AltDirectorySeparatorChar)
                    {
                        fullPath = fullPath.Substring(0, fullPath.Length - 1);
                    }

                    string directoryName = XboxPath.GetDirectoryName(fullPath);
                    if (directoryName == null)
                    {
                        return null;
                    }

                    return new XboxDirectoryInfo(directoryName, this.OperatingSystem, this.Console);
                }
            }
        }

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The directory root.</value>
        public XboxDirectoryInfo Root
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FullName))
                {
                    return null;
                }
                else if (XboxPath.IsRoot(this.FullName))
                {
                    return this;
                }
                else
                {
                    string[] directory = this.FullName.Split('\\');

                    return new XboxDirectoryInfo(string.Concat(directory[0], "\\"), this.OperatingSystem, this.Console);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="XboxDirectoryInfo"/> exists.
        /// </summary>
        /// <value><c>True</c> if exists; otherwise, <c>false</c>.</value>
        public override bool Exists
        {
            get
            {
                return ExistsImpl(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, this.Console.Adapter);
            }
        }

        /// <summary>
        /// Deletes this empty directory.
        /// </summary>
        public override void Delete()
        {
            this.Delete(false);
        }

        /// <summary>
        /// Deletes the directory and, if argument recursive is true, all of its contents.
        /// </summary>
        /// <param name="recursive">Value indicating whether or not to try delete contents.</param>
        /// <exception cref="System.IO.IOException">Thrown if this object represents the root of the drive.</exception>
        public void Delete(bool recursive)
        {
            if (XboxPath.IsRoot(this.FullName))
            {
                throw new IOException("Cannot delete the drive root");
            }

            if (this.Exists)
            {
                this.Console.Adapter.DeleteDirectory(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, recursive);
            }

            this.Refresh();
        }

        /// <summary>
        /// Copies this directory and all of its subdirectories to a path on the local machine.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the source directory does not exist at the path specified.</exception>
        public override void Copy(string localPath)
        {
            this.Copy(localPath, true);
        }

        /// <summary>
        /// Copies this directory and all of its subdirectories to a path on the local machine.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the source directory does not exist at the path specified.</exception>
        public void Copy(string localPath, IProgress<XboxFileTransferMetric> metrics)
        {
            this.Copy(localPath, true, metrics);
        }

        /// <summary>
        /// Copies this directory to the specified local path.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        /// <param name="recursive">Recursively copies all subdirectories if set to <c>true</c>.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the source directory does not exist at the path specified.</exception>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the localPath does not have an Xbox origin.</exception>
        public void Copy(string localPath, bool recursive)
        {
            this.Copy(localPath, recursive, null);
        }

        /// <summary>
        /// Copies this directory to the specified local path.
        /// </summary>
        /// <param name="localPath">The path on the local machine to copy to.</param>
        /// <param name="recursive">Recursively copies all subdirectories if set to <c>true</c>.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the source directory does not exist at the path specified.</exception>
        /// <exception cref="Microsoft.Internal.GamesTest.Xbox.XboxConsoleFeatureNotSupportedException">Thrown if the localPath does not have an Xbox origin.</exception>
        public void Copy(string localPath, bool recursive, IProgress<XboxFileTransferMetric> metrics)
        {
            if (!this.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist at the path specified.");
            }

            if (XboxPath.HasXboxOrigin(localPath))
            {
                throw new XboxConsoleFeatureNotSupportedException("Not able to copy from Xbox to Xbox.");
            }

            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            this.Console.Adapter.ReceiveDirectory(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, localPath, recursive, metrics);
        }

        /// <summary>
        /// Creates a directory on an Xbox.
        /// </summary>
        public void Create()
        {
            if (!this.Exists)
            {
                this.Console.Adapter.CreateDirectory(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath);
            }
        }

        /// <summary>
        /// Moves a directory between an xbox console and a PC.
        /// </summary>
        /// <param name="destinationPath">The path to move to.</param>
        public override void MoveTo(string destinationPath)
        {
            this.Copy(destinationPath);
            this.Delete(true);
        }

        /// <summary>
        /// Returns the subdirectories of the current directory.
        /// </summary>
        /// <returns>An IEnumerable of XboxDirectoryInfo objects.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a potentially slow operation")]
        public IEnumerable<XboxDirectoryInfo> GetDirectories()
        {
            return this.GetFileSystemInfos().OfType<XboxDirectoryInfo>();
        }

        /// <summary>
        /// Returns a file list from the current directory.
        /// </summary>
        /// <returns>An IEnumerable of XboxFileInfo objects.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a potentially slow operation")]
        public IEnumerable<XboxFileInfo> GetFiles()
        {
            return this.GetFileSystemInfos().OfType<XboxFileInfo>();
        }

        /// <summary>
        /// Returns an IEnumerable of strongly typed XboxFileSystemInfo entries representing all the files and subdirectories in a directory.
        /// </summary>
        /// <returns>An IEnumerable of strongly typed XboxFileSystemInfo entries.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a potentially slow operation")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Compatability with the .NET IO naming convention")]
        public IEnumerable<XboxFileSystemInfo> GetFileSystemInfos()
        {
            return this.GetFileSystemInfos("*", false);
        }

        /// <summary>
        /// Returns an IEnumerable of strongly typed XboxFileSystemInfo entries representing all the files and subdirectories in a directory.
        /// </summary>
        /// <param name="searchPattern">Search pattern for files in the directory.</param>
        /// <param name="recursive">True if search should check recursively through child folders.</param>
        /// <returns>An IEnumerable of strongly typed XboxFileSystemInfo entries.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Compatability with the .NET IO naming convention")]
        public IEnumerable<XboxFileSystemInfo> GetFileSystemInfos(string searchPattern, bool recursive)
        {
            List<XboxFileSystemInfoDefinition> definitionObjects = this.Console.Adapter.GetDirectoryContents(this.Console.SystemIpAddressAndSessionKeyCombined, this.XboxPath, searchPattern, recursive).ToList();

            IEnumerable<XboxFileSystemInfo> directories = (from fileObject in definitionObjects
                                                           where fileObject.IsDirectory
                                                           select new XboxDirectoryInfo(fileObject, this.Console)).Cast<XboxFileSystemInfo>();

            IEnumerable<XboxFileSystemInfo> files = (from fileObject in definitionObjects
                                                     where !fileObject.IsDirectory
                                                     select new XboxFileInfo(fileObject, this.Console)).Cast<XboxFileSystemInfo>();

            return directories.Union(files);
        }

        /// <summary>
        /// Determines whether a directory exists on an Xbox.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="path">The directory to look for.</param>
        /// <param name="consoleAdapter">The Xbox console to search on.</param>
        /// <returns>A value indicating whether the directory exists on the console or not.</returns>
        internal static bool ExistsImpl(string systemIpAddress, XboxPath path, XboxConsoleAdapterBase consoleAdapter)
        {
            return XboxFileSystemInfo.ExistsImpl(systemIpAddress, path, xboxFileSystemInfoDefinition => xboxFileSystemInfoDefinition.IsDirectory, consoleAdapter);
        }
    }
}
