//------------------------------------------------------------------------------
// <copyright file="XboxFileSystemInfoTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.QualityTools.Testing.Fakes.Shims;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class to house tests for the XboxFileSystemInfo class.
    /// </summary>
    [TestClass]
    public class XboxFileSystemInfoTests
    {
        private const string XboxFileSystemInfoTestCategory = "XboxConsole.XboxFileSystemInfo";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private IDisposable shimsContext;

        private XboxFileSystemInfo fileSystemInfo;

        private ShimXboxConsoleAdapterBase shimAdapter;

        private ShimXboxFileSystemInfo xboxFileSystemInfoShim;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsoleAdapterBase doesn't really need to be disposed.")]
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxDirectoryInfo.ConstructorXboxFileSystemInfoDefinitionXboxConsole = (info, definition, console) =>
                {
                    var xboxItemShim = new ShimXboxItem(info)
                        {
                            ConsoleGet = () => new XboxConsole((IPAddress)null),
                        };
                    this.xboxFileSystemInfoShim = new ShimXboxFileSystemInfo(info)
                        {
                            DefinitionGet = () => info.Console.Adapter.GetFileSystemInfoDefinition(console.SystemIpAddress.ToString(), new XboxPath("xd:\\someDirectory", XboxOperatingSystem.System))
                        };
                };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (@base, xboxXdk) => { };

            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
            };

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
                {
                    var myShim = new ShimXboxConsole(console)
                        {
                            AdapterGet = () => this.shimAdapter,
                            SystemIpAddressGet = () => IPAddress.Parse(XboxFileSystemInfoTests.ConsoleAddress)
                        };
                };

            this.fileSystemInfo = new XboxDirectoryInfo((XboxFileSystemInfoDefinition)null, new XboxConsole((IPAddress)null));
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that the CreationTime property uses the "Definition" property
        /// to ensure that the XboxFileSystemInfoDefintion object is always retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoCreationTimeUsesDefinitionProperty()
        {
            bool success = false;
            DateTime fakeCreationTime = DateTime.Now;
            this.xboxFileSystemInfoShim.DefinitionGet = () =>
                {
                    success = true;
                    return new ShimXboxFileSystemInfoDefinition()
                        {
                            CreationTimeGet = () => fakeCreationTime
                        };
                };

            DateTime creationTime = this.fileSystemInfo.CreationTime;
            Assert.IsTrue(success);
            Assert.AreEqual(fakeCreationTime, creationTime);
        }

        /// <summary>
        /// Verifies that the CreationTime property will throw a FileNotFoundException
        /// if the adapter can not find the file.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileSystemCreationTimeThrowsFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
            {
                throw new FileNotFoundException();
            };

            var notUsed = this.fileSystemInfo.CreationTime;
            Assert.Fail("Property accessor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the IsDirectory property uses the "Definition" property
        /// to ensure that the XboxFileSystemInfoDefintion object is always retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoIsDirectoryUsesDefinitionProperty()
        {
            bool success = false;
            this.xboxFileSystemInfoShim.DefinitionGet = () =>
            {
                success = true;
                return new ShimXboxFileSystemInfoDefinition()
                {
                    IsDirectoryGet = () => true
                };
            };

            bool isDirectory = this.fileSystemInfo.IsDirectory;
            Assert.IsTrue(success);
            Assert.IsTrue(isDirectory);
        }

        /// <summary>
        /// Verifies that the IsDirectory property will throw a FileNotFoundException
        /// if the adapter can not find the file.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileSystemIsDirectoryThrowsFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
            {
                throw new FileNotFoundException();
            };

            var notUsed = this.fileSystemInfo.IsDirectory;
            Assert.Fail("Property accessor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the IsReadOnly property uses the "Definition" property
        /// to ensure that the XboxFileSystemInfoDefintion object is always retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoIsReadOnlyUsesDefinitionProperty()
        {
            bool success = false;
            this.xboxFileSystemInfoShim.DefinitionGet = () =>
            {
                success = true;
                return new ShimXboxFileSystemInfoDefinition()
                {
                    FileAttributesGet = () => FileAttributes.ReadOnly
                };
            };

            bool isReadOnly = this.fileSystemInfo.IsReadOnly;
            Assert.IsTrue(success);
            Assert.IsTrue(isReadOnly);
        }

        /// <summary>
        /// Verifies that the IsReadOnly property will throw a FileNotFoundException
        /// if the adapter can not find the file.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileSystemIsReadOnlyThrowsFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
            {
                throw new FileNotFoundException();
            };

            var notUsed = this.fileSystemInfo.IsReadOnly;
            Assert.Fail("Property accessor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the LastWriteTime property uses the "Definition" property
        /// to ensure that the XboxFileSystemInfoDefintion object is always retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoLastWriteTimeUsesDefinitionProperty()
        {
            bool success = false;
            DateTime fakeLastWriteTime = DateTime.Now;
            this.xboxFileSystemInfoShim.DefinitionGet = () =>
            {
                success = true;
                return new ShimXboxFileSystemInfoDefinition()
                {
                    LastWriteTimeGet = () => fakeLastWriteTime
                };
            };

            DateTime lastWriteTimeGet = this.fileSystemInfo.LastWriteTime;
            Assert.IsTrue(success);
            Assert.AreEqual(fakeLastWriteTime, lastWriteTimeGet);
        }

        /// <summary>
        /// Verifies that the LastWriteTime property will throw a FileNotFoundException
        /// if the adapter can not find the file.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileSystemLastWriteTimeThrowsFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
            {
                throw new FileNotFoundException();
            };

            var notUsed = this.fileSystemInfo.LastWriteTime;
            Assert.Fail("Property accessor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the Length property uses the "Definition" property
        /// to ensure that the XboxFileSystemInfoDefintion object is always retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoLengthUsesDefinitionProperty()
        {
            bool success = false;
            const long FakeLength = 1234L;
            this.xboxFileSystemInfoShim.DefinitionGet = () =>
            {
                success = true;
                return new ShimXboxFileSystemInfoDefinition()
                {
                    FileSizeGet = () => (ulong)FakeLength
                };
            };

            long length = this.fileSystemInfo.Length;
            Assert.IsTrue(success);
            Assert.AreEqual(FakeLength, length);
        }

        /// <summary>
        /// Verifies that the Length property will throw a FileNotFoundException
        /// if the adapter can not find the file.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileSystemLengthThrowsFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
            {
                throw new FileNotFoundException();
            };

            var notUsed = this.fileSystemInfo.Length;
            Assert.Fail("Property accessor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the Name property works correctly for directories.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoNameAsDirectory()
        {
            ShimXboxFileSystemInfo.AllInstances.FullNameGet = info => @"xd:\directory";

            string objectName = this.fileSystemInfo.Name;
            Assert.AreEqual("directory", objectName);
        }

        /// <summary>
        /// Verifies that the Name property works correctly for directories with a trailing slash.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoNameAsDirectoryWithTrailingSlash()
        {
            ShimXboxFileSystemInfo.AllInstances.FullNameGet = info => @"xd:\directory\";

            string objectName = this.fileSystemInfo.Name;
            Assert.AreEqual("directory", objectName);
        }

        /// <summary>
        /// Verifies that the Name property works correctly for files.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoNameAsFile()
        {
            ShimXboxFileSystemInfo.AllInstances.FullNameGet = info => @"xd:\directory\foo.txt";

            string objectName = this.fileSystemInfo.Name;
            Assert.AreEqual("foo.txt", objectName);
        }

        /// <summary>
        /// Verifies that the Name property works correctly for root directories.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemInfoNameAsRoot()
        {
            ShimXboxFileSystemInfo.AllInstances.FullNameGet = info => @"xd:\";

            string objectName = this.fileSystemInfo.Name;
            Assert.AreEqual(@"xd:\", objectName);
        }

        /// <summary>
        /// Verifies that the Exists property correctly handles a FileNotFoundException
        /// being thrown by the adapter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileSystemInfoTestCategory)]
        public void TestXboxFileSystemExistsIsFalseWithFileNotFoundException()
        {
            this.shimAdapter.GetFileSystemInfoDefinitionStringXboxPath = (systemIpAddress, path) =>
                {
                    throw new FileNotFoundException();
                };

            Assert.IsFalse(this.fileSystemInfo.Exists);
        }
    }
}
