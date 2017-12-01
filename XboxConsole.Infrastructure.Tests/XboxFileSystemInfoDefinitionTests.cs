//------------------------------------------------------------------------------
// <copyright file="XboxFileSystemInfoDefinitionTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to contain the tests for the XboxFileSystemInfoDefinition object.
    /// </summary>
    [TestClass]
    public class XboxFileSystemInfoDefinitionTests
    {
        private const string XboxFileSystemInfoDefinitionTestCategory = "Infrastructure.XboxFileSystemInfoDefinition";

        /// <summary>
        /// Verifies that the XboxFileSystemInfoDefinition constructor will throw an 
        /// ArgumentNullException if given a null value for the filePath parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsArgumentNullException()
        {
#pragma warning disable 168
            var notUsed = new XboxFileSystemInfoDefinition(0, FileAttributes.System, null, XboxOperatingSystem.System, 0, 0, 0);
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the XboxFileSystemInfoDefinition constructor properly initializes
        /// all of its properties.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoDefinitionTestCategory)]
        public void TestConstructorInitializesProperties()
        {
            const ulong CreationTime = 0;
            const FileAttributes FileAttributes = FileAttributes.System;
            const string FilePath = "ContentIsNotImportant";
            const XboxOperatingSystem OperatingSystem = XboxOperatingSystem.System;
            const ulong FileSize = 0;
            const ulong LastAccessTime = 0;
            const ulong LastWriteTime = 0;

            XboxFileSystemInfoDefinition infoDefinition = new XboxFileSystemInfoDefinition(CreationTime, FileAttributes, FilePath, OperatingSystem, FileSize, LastAccessTime, LastWriteTime);

            Assert.AreEqual(DateTime.FromFileTime((long)CreationTime), infoDefinition.CreationTime, "The XboxFileSystemInfoDefinition did not initialize the CreationTime property correctly.");
            Assert.AreEqual(FileAttributes, infoDefinition.FileAttributes, "The XboxFileSystemInfoDefinition did not initialize the FileAttributes property correctly.");
            Assert.AreEqual(FilePath, infoDefinition.Path.FullName, "The XboxFileSystemInfoDefinition did not initialize the Path.FullName property correctly.");
            Assert.AreEqual(OperatingSystem, infoDefinition.Path.OperatingSystem, "The XboxFileSystemInfoDefinition did not initialize the Path.OperatingSystem property correctly.");
            Assert.AreEqual(FileSize, infoDefinition.FileSize, "The XboxFileSystemInfoDefinition did not initialize the FileSize property correctly.");
            Assert.AreEqual(DateTime.FromFileTime((long)LastAccessTime), infoDefinition.LastAccessTime, "The XboxFileSystemInfoDefinition did not initialize the LastAccessTime property correctly.");
            Assert.AreEqual(DateTime.FromFileTime((long)LastWriteTime), infoDefinition.LastWriteTime, "The XboxFileSystemInfoDefinition did not initialize the LastWriteTime property correctly.");
        }
    }
}
