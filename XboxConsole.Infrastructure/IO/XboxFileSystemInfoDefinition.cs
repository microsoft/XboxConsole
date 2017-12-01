//------------------------------------------------------------------------------
// <copyright file="XboxFileSystemInfoDefinition.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class for defining an object on the Xbox file system.
    /// </summary>
    [DebuggerDisplay("{Path.FullName}, {FileAttributes}")]
    internal class XboxFileSystemInfoDefinition
    {
        /// <summary>
        /// Initializes a new instance of the XboxFileSystemInfoDefinition class.
        /// </summary>
        /// <param name="creationTime">The creation time represented by ticks.</param>
        /// <param name="fileAttributes">The attributes describing this file system object.</param>
        /// <param name="filePath">The path to the file system object.</param>
        /// <param name="operatingSystem">The Xbox operating system on which this file resides.</param>
        /// <param name="fileSize">The size of this file system object.</param>
        /// <param name="lastAccessTime">The last time this file was accessed represented by ticks.</param>
        /// <param name="lastWriteTime">The last time this file was written to represented by ticks.</param>
        public XboxFileSystemInfoDefinition(
            ulong creationTime,
            FileAttributes fileAttributes,
            string filePath,
            XboxOperatingSystem operatingSystem,
            ulong fileSize,
            ulong lastAccessTime,
            ulong lastWriteTime)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            this.CreationTime = DateTime.FromFileTime((long)creationTime);
            this.FileAttributes = fileAttributes;
            this.Path = new XboxPath(filePath, operatingSystem);
            this.FileSize = fileSize;
            this.LastAccessTime = DateTime.FromFileTime((long)lastAccessTime);
            this.LastWriteTime = DateTime.FromFileTime((long)lastWriteTime);
        }

        /// <summary>
        /// Gets the creation time of this file system object.
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Gets the attributes describing this file system object.
        /// </summary>
        public FileAttributes FileAttributes { get; private set; }

        /// <summary>
        /// Gets the complete path to this file system object.
        /// </summary>
        public XboxPath Path { get; private set; }

        /// <summary>
        /// Gets the size of this file system object.
        /// </summary>
        public ulong FileSize { get; private set; }

        /// <summary>
        /// Gets the last access time of this file system object.
        /// </summary>
        public DateTime LastAccessTime { get; private set; }

        /// <summary>
        /// Gets the last write time of this file system object.
        /// </summary>
        public DateTime LastWriteTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this defintion represents a directory.
        /// </summary>
        public bool IsDirectory
        {
            get
            {
                return this.FileAttributes.HasFlag(FileAttributes.Directory);
            }
        }
    }
}
