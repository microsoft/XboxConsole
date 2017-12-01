//------------------------------------------------------------------------------
// <copyright file="DirectoryInfoExtensionsTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.IO.Fakes;
    using System.Linq;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class to house tests for the DirectoryInfo extension methods.
    /// </summary>
    [TestClass]
    public class DirectoryInfoExtensionsTests
    {
        private const string DirectoryInfoExtensionsTestCategory = "XboxConsole.DirectoryInfoExtensions";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private XboxPath xboxPath = new XboxPath(@"xd:\directory", XboxOperatingSystem.System);

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
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubConsoleAdapterBase doesn't really need to be disposed.")]
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (@base, xboxXdk) => { };

            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
                SendDirectoryStringStringXboxPathBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDirectory, destinationDirectory, isRecursive, metrics) => { },
                SendFileStringStringXboxPathIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFile, destinationFile, metrics) => { },
                ReceiveDirectoryStringXboxPathStringBooleanIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceDiretory, destinationDirectory, isRecursive, metrics) => { },
                ReceiveFileStringXboxPathStringIProgressOfXboxFileTransferMetric = (systemIpAddress, sourceFilePath, destinationFilePath, metrics) => { },
                CreateDirectoryStringXboxPath = (systemIpAddress, path) => { },
            };

            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(DirectoryInfoExtensionsTests.ConsoleAddress),
                SystemIpAddressAndSessionKeyCombinedGet = () => DirectoryInfoExtensionsTests.ConsoleAddress,
            };
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
        /// Verifies that the CopyTo method throws if given a null value for the directoryInfo parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(DirectoryInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDirectoryInfoExtensionsCopyToArgumentNullDirectoryInfo()
        {
            DirectoryInfo directoryInfo = null;
            directoryInfo.CopyTo(this.xboxPath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(DirectoryInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDirectoryInfoExtensionsCopyToArgumentNullConsole()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"c:\testDirectory");
            directoryInfo.CopyTo(this.xboxPath, null);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a null value for the xboxPath parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(DirectoryInfoExtensionsTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDirectoryInfoExtensionsCopyToArgumentNullXboxPath()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"c:\testDirectory");
            directoryInfo.CopyTo(null, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method throws if given a directory that doesn't exist.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(DirectoryInfoExtensionsTestCategory)]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void TestDirectoryInfoExtensionsCopyToDirectoryNotFound()
        {
            ShimXboxFileSystemInfo.ExistsImplStringXboxPathFuncOfXboxFileSystemInfoDefinitionBooleanXboxConsoleAdapterBase = (address, xboxPath, existsPredicate, adapter) => true;

            DirectoryInfo directoryInfo = new DirectoryInfo(@"c:\testDirectoryDoesntExist");
            directoryInfo.CopyTo(this.xboxPath, this.XboxConsole);
        }

        /// <summary>
        /// Verifies that the CopyTo method correctly invokes the XboxFile.Copy.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The test involves work with nested data structures.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(DirectoryInfoExtensionsTestCategory)]
        public void TestDirectoryInfoExtensionsCopyToCallsXboxFileCopyCorrectly()
        {
            string sourceRootDirectory = @"c:\testDirectory";

            Dictionary<string, IEnumerable<string>> sourceDirectories = new Dictionary<string, IEnumerable<string>> 
            { 
                { @"c:\testDirectory", new string[] { "subDir1", "subDir2" } },
                { @"c:\testDirectory\subDir1", new string[0] },
                { @"c:\testDirectory\subDir2", new string[] { "subDir21" } }
            };

            Dictionary<string, IEnumerable<string>> sourceFiles = new Dictionary<string, IEnumerable<string>> 
            { 
                { @"c:\testDirectory", new string[] { "file1" } }, 
                { @"c:\testDirectory\subDir1", new string[] { "file11.txt", "file12" } }, 
                { @"c:\testDirectory\subDir2", new string[0] },
                { @"c:\testDirectory\subDir2\subDir21", new string[] { "file211.txt", "file212", "file213.jpg" } }
            };

            Collection<string> sourceFilesCopyCheck = new Collection<string> 
            { 
                @"c:\testDirectory\file1", 
                @"c:\testDirectory\subDir1\file11.txt", 
                @"c:\testDirectory\subDir1\file12", 
                @"c:\testDirectory\subDir2\subDir21\file211.txt", 
                @"c:\testDirectory\subDir2\subDir21\file212", 
                @"c:\testDirectory\subDir2\subDir21\file213.jpg", 
            };

            Collection<string> destinationFilesCopyCheck = new Collection<string> 
            { 
                @"xd:\directory\file1", 
                @"xd:\directory\subDir1\file11.txt", 
                @"xd:\directory\subDir1\file12", 
                @"xd:\directory\subDir2\subDir21\file211.txt", 
                @"xd:\directory\subDir2\subDir21\file212", 
                @"xd:\directory\subDir2\subDir21\file213.jpg", 
            };

            ShimFileInfo.AllInstances.LengthGet = FileInfo => 100;

            ShimDirectoryInfo.AllInstances.EnumerateFilesStringSearchOption = (directoryInfo, search, options) =>
                {
                    return sourceFilesCopyCheck.Select(fileName => new FileInfo(fileName));
                };

            ShimDirectoryInfo.AllInstances.GetDirectories = directoryInfo =>
                {
                    var element = sourceDirectories.SingleOrDefault(elem => string.Equals(elem.Key, directoryInfo.FullName, StringComparison.OrdinalIgnoreCase));
                    if (element.Key != null)
                    {
                        return element.Value.Select(path => new DirectoryInfo(Path.Combine(element.Key, path))).ToArray();
                    }
                    else
                    {
                        return new DirectoryInfo[0];
                    }
                };

            ShimDirectoryInfo.AllInstances.GetFiles = directoryInfo =>
                {
                    var element = sourceFiles.SingleOrDefault(elem => string.Equals(elem.Key, directoryInfo.FullName, StringComparison.OrdinalIgnoreCase));
                    if (element.Key != null)
                    {
                        return element.Value.Select(fileName => new FileInfo(Path.Combine(element.Key, fileName))).ToArray();
                    }
                    else
                    {
                        return new FileInfo[0];
                    }
                };

            ShimXboxFile.CopyStringXboxPathXboxConsoleIProgressOfXboxFileTransferMetric = (sourceFile, destinationPath, console, metrics) =>
                {
                    Assert.IsTrue(sourceFilesCopyCheck.Remove(sourceFile), string.Format(CultureInfo.CurrentCulture, "The file {0} is either not found or copied more than once!", sourceFile));
                    Assert.IsTrue(destinationFilesCopyCheck.Remove(destinationPath.FullName), string.Format(CultureInfo.CurrentCulture, "The file {0} is either not found or copied more than once!", destinationPath.FullName));
                    Assert.AreEqual(this.xboxPath.OperatingSystem, destinationPath.OperatingSystem);
                };

            bool successfullyCreatedDirectories = false;
            ShimXboxDirectory.ExistsXboxPathXboxConsole = (xboxPath, console) => false;
            ShimXboxDirectory.CreateXboxPathXboxConsole = (xboxPath, console) => { successfullyCreatedDirectories = true; };

            DirectoryInfo directory = new DirectoryInfo(sourceRootDirectory);
            var returnedValue = directory.CopyTo(this.xboxPath, this.XboxConsole);

            Assert.IsTrue(successfullyCreatedDirectories, "Didn't create necessary directory(ies) at the destination.");
            Assert.IsTrue(sourceFilesCopyCheck.Count == 0, "Didn't copy all necessary files correctly.");
            Assert.IsTrue(destinationFilesCopyCheck.Count == 0, "Didn't copy all necessary files correctly.");
            Assert.AreSame(this.XboxConsole, returnedValue.Console);
            Assert.AreEqual(this.xboxPath.FullName, returnedValue.FullName);
            Assert.AreEqual(this.xboxPath.OperatingSystem, returnedValue.OperatingSystem);
        }
    }
}
