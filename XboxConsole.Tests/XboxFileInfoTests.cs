//------------------------------------------------------------------------------
// <copyright file="XboxFileInfoTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to house tests for the XboxFileInfo class.
    /// </summary>
    [TestClass]
    public class XboxFileInfoTests
    {
        private const string XboxFileInfoTestCategory = "XboxConsole.XboxFileInfo";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private XboxPath xboxFilePath = new XboxPath(@"xd:\TestFile", XboxOperatingSystem.System);
        private IDisposable shimsContext;
        private ShimXboxConsole shimXboxConsole;
        private ShimXboxConsoleAdapterBase shimAdapter;

        private XboxConsole XboxConsole
        {
            get
            {
                return this.shimXboxConsole.Instance;
            }
        }

        /// <summary>
        /// Initializes this class before each test is executed.
        /// </summary>
        [TestInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsolAdapterBase object is not important.")]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();
            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (@base, xboxXdk) => { };

            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
                SendFileStringStringXboxPathIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFile, destinationFile, metrics) => { },
                ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { },
            };

            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(XboxFileInfoTests.ConsoleAddress),
                SystemIpAddressAndSessionKeyCombinedGet = () => XboxFileInfoTests.ConsoleAddress,
            };
        }

        /// <summary>
        /// Cleans up after each test is executed.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that an XboxFileInfo can be created with the proper input parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void XboxFileInfoConstructorTest()
        {
            var fileInfo = new XboxFileInfo(this.xboxFilePath.FullName, this.xboxFilePath.OperatingSystem, this.XboxConsole);

            Assert.IsNotNull(fileInfo);
            Assert.AreEqual(this.xboxFilePath.FullName, fileInfo.FullName);
            Assert.AreEqual(this.xboxFilePath.OperatingSystem, fileInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that an XboxFileInfo can be created with the proper input parameters (XboxPath).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void XboxFileInfoConstructorXboxPathTest()
        {
            var fileInfo = new XboxFileInfo(this.xboxFilePath, this.XboxConsole);

            Assert.IsNotNull(fileInfo);
            Assert.AreEqual(this.xboxFilePath.FullName, fileInfo.FullName);
            Assert.AreEqual(this.xboxFilePath.OperatingSystem, fileInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that an XboxFileInfo can be created with a valid XboxFileSystemInfoDefintion and XboxConsole as its parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoInternalConstructor()
        {
            XboxFileSystemInfoDefinition fileSystemInfo = new XboxFileSystemInfoDefinition(0, FileAttributes.Normal, this.xboxFilePath.FullName, this.xboxFilePath.OperatingSystem, 0, 0, 0);
            var fileInfo = new XboxFileInfo(fileSystemInfo, this.XboxConsole);

            Assert.IsNotNull(fileInfo);
            Assert.AreEqual(this.xboxFilePath.FullName, fileInfo.FullName);
            Assert.AreEqual(this.xboxFilePath.OperatingSystem, fileInfo.OperatingSystem);
        }

        /// <summary>
        /// Verifies that the Directory property returns the correct value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoGetDirectory()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parentDirectory\file", XboxOperatingSystem.System)
            };

            var fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@"xd:\parentDirectory", fileInfo.Directory.FullName);
        }

        /// <summary>
        /// Verifies that the DirectoryName property returns the correct value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoGetDirectoryName()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parentDirectory\file", XboxOperatingSystem.System)
            };

            var fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@"xd:\parentDirectory", fileInfo.DirectoryName);
        }

        /// <summary>
        /// Verifies that the Extension property returns the correct value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoGetExtension()
        {
            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parentDirectory\file.txt", XboxOperatingSystem.System)
            };

            var fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(@".txt", fileInfo.Extension);

            xboxFileShim.PathGet = () => new XboxPath(@"xd:\parentDirectory\file", XboxOperatingSystem.System);
            fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            Assert.AreEqual(string.Empty, fileInfo.Extension);
        }

        /// <summary>
        /// Verifies that the Delete method does a delete.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoDelete()
        {
            string path = @"xd:\parentDirectory\file";
            XboxOperatingSystem operatingSystem = XboxOperatingSystem.System;

            bool success = false;
            this.shimAdapter.DeleteFileStringXboxPath = (systemIpAddress, xboxPath) =>
            {
                Assert.IsTrue(path.Equals(xboxPath.FullName, StringComparison.OrdinalIgnoreCase), "The path is not the correct path.");
                Assert.IsTrue(operatingSystem == xboxPath.OperatingSystem, "The operating system is not the correct operating system.");
                success = true;
            };

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(path, operatingSystem)
            };
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            XboxFileInfo fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            fileInfo.Delete();
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that the Delete method calls refresh after deleting.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoDeleteCallsRefresh()
        {
            bool calledDeleteFile = false;
            this.shimAdapter.DeleteFileStringXboxPath = (systemIpAddress, path) =>
            {
                calledDeleteFile = true;
            };

            bool success = false;
            ShimXboxFileSystemInfo.AllInstances.Refresh = info =>
            {
                success = true;
                Assert.IsTrue(calledDeleteFile);
            };

            ShimXboxFileSystemInfoDefinition xboxFileShim = new ShimXboxFileSystemInfoDefinition()
            {
                PathGet = () => new XboxPath(@"xd:\parentDirectory\file", XboxOperatingSystem.System)
            };
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            XboxFileInfo fileInfo = new XboxFileInfo(xboxFileShim, this.XboxConsole);
            fileInfo.Delete();
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Verifies that a FileNotFoundException is thrown if a copy call is made from a file that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestXboxFileInfoCopySourceFileDoesNotExist()
        {
            this.shimAdapter.ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { throw new FileNotFoundException(); };
            XboxFileInfo fileInfo = new XboxFileInfo(@"xd:\file", XboxOperatingSystem.System, this.XboxConsole);
            fileInfo.Copy(@"c:\parentDirectory\file");
        }

        /// <summary>
        /// Verifies that a DirectoryNotFoundException is thrown if a copy call is made to a directory that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void TestXboxFileInfoCopyTargetDirectoryDoesNotExist()
        {
            this.shimAdapter.ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { throw new DirectoryNotFoundException(); };
            XboxFileInfo fileInfo = new XboxFileInfo(@"xd:\file", XboxOperatingSystem.System, this.XboxConsole);
            fileInfo.Copy(@"c:\parentDirectoryThatDoesNotExist\file");
        }

        /// <summary>
        /// Verifies that an ArgumentNullException is thrown if a copy call is made to a null directory.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileInfoCopyTargetDirectoryNull()
        {
            this.shimAdapter.ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { throw new ArgumentNullException("destinationFilePath"); };
            XboxFileInfo fileInfo = new XboxFileInfo(new ShimXboxFileSystemInfoDefinition(), this.XboxConsole);
            fileInfo.Copy(null);
        }

        /// <summary>
        /// Verifies that the default copy operation performs a copy.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        public void TestXboxFileInfoCopy()
        {
            XboxFileInfo fileInfo = new XboxFileInfo(
                new ShimXboxFileSystemInfoDefinition()
                {
                    PathGet = () => new XboxPath(@"xd:\parentDirectory\file", XboxOperatingSystem.System)
                },
                this.XboxConsole);

            string expectedDestination = @"c:\parentDirectory\file";
            IProgress<XboxFileTransferMetric> expectedMetric = null;

            bool success = false;
            this.shimAdapter.ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => 
            { 
                success = true;

                Assert.AreEqual(fileInfo.FullName, sourceFilePath.FullName, "Copy didn't pass the expected source file path to the adapter.");
                Assert.AreEqual(expectedDestination, destinationFilePath, "Copy didn't pass the expected destination file path to the adapter.");
                Assert.AreSame(expectedMetric, metrics, "Copy didn't pass the expected destination file path to the adapter.");
            };
            fileInfo.Copy(expectedDestination);
            Assert.IsTrue(success);

            fileInfo.Copy(expectedDestination, null);

            expectedMetric = new Progress<XboxFileTransferMetric>();
            fileInfo.Copy(expectedDestination, expectedMetric);
        }

        /// <summary>
        /// Verifies that the XboxFileInfo constructor throws an ArgumentNullException if given a null value
        /// for the "path" parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileInfoThrowsNullPathArgument()
        {
            XboxFileInfo dirInfo = new XboxFileInfo(null, XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxFileInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxFileInfo constructor throws an ArgumentNullException if given a null
        /// value for the "console" parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxFileInfoThrowsNullConsoleArgument()
        {
            XboxFileInfo dirInfo = new XboxFileInfo(this.xboxFilePath.FullName, XboxOperatingSystem.System, null);
            Assert.IsNotNull(dirInfo, "The XboxFileInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxFileInfo constructor throws if given a non-Xbox path.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestXboxFileInfoThrowsInvalidXboxFileArgument()
        {
            XboxFileInfo dirInfo = new XboxFileInfo(@"e:\\FakeFile", XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxFileInfo constructor should have thrown an exception.");
        }

        /// <summary>
        /// Verifies that the XboxDiretoryInfo constructor throws if given a path with invalid characters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxFileInfoTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestXboxFileInfoThrowsWithInvalidCharactersInPath()
        {
            XboxFileInfo dirInfo = new XboxFileInfo(@"xd:\<>FakeFile", XboxOperatingSystem.System, this.XboxConsole);
            Assert.IsNotNull(dirInfo, "The XboxFileInfo constructor should have thrown an exception.");
        }
    }
}
