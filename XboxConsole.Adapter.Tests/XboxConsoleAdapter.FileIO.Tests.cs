//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.FileIO.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterFileIoTestCategory = "Adapter.FileIO";

        private const string LocalFile = "ContentOfThisStringDoesNotMatter";

        private readonly XboxPath remoteFile = new XboxPath("xd:\\ContentOfThisStringDoesNotMatter", XboxOperatingSystem.System);

        /// <summary>
        /// Verifies that the SendFile method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSendFileThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SendFile(ConsoleAddress, LocalFile, this.remoteFile, null);
        }

        /// <summary>
        /// Verifies that the SendFile method throws an ArgumentNullException
        /// if the source file path is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendFileThrowsArgumentNullExceptionWithNullSourceFile()
        {
            this.adapter.SendFile(ConsoleAddress, null, this.remoteFile, null);
        }

        /// <summary>
        /// Verifies that the SendFile method throws an ArugmentNullException
        /// if the destination file path is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestSendFileThrowsArgumentNullExceptionWithNullDestinationFile()
        {
            this.adapter.SendFile(ConsoleAddress, LocalFile, null, null);
        }

        /// <summary>
        /// Verifies that the SendFile method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestSendFileTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SendFile(ConsoleAddress, LocalFile, this.remoteFile, null);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter SendFile(string, XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole SendFile(string, XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the SendFile method passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestSendFileCallsXdkCopyFiles()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "SendFile did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(0, recursionLevel, "SendFile should call Xdk.CopyFiles with a recursion level of zero.");
                    Assert.AreEqual(this.remoteFile.FullName, destinationPath, "SendFile didn't pass the correct destination to the Xdk.");
                    Assert.AreEqual(LocalFile, sourceSearchPath, "SendFile didn't pass the correct source to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "SendFile didn't pass the correct operating system to the Xdk.");
                    Assert.AreSame(expectedProgess, metrics, "SendFile didn't pass the correct progress object to the Xdk.");
                };

            this.adapter.SendFile(ConsoleAddress, LocalFile, this.remoteFile, null);
            Assert.IsTrue(isCorrectMethodCalled, "The SendFile method did not call the Xdk's CopyFiles method.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.SendFile(ConsoleAddress, LocalFile, this.remoteFile, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SendFile(null, LocalFile, this.remoteFile, null));
        }

        /// <summary>
        /// Verifies that the ReceiveFile method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestReceiveFileThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.ReceiveFile(ConsoleAddress, this.remoteFile, LocalFile, null);
        }

        /// <summary>
        /// Verifies that the ReceiveFile method throws an ArgumentNullException
        /// if the source file path is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReceiveFileThrowsArgumentNullExceptionWithNullSourceFile()
        {
            this.adapter.ReceiveFile(ConsoleAddress, this.remoteFile, null, null);
        }

        /// <summary>
        /// Verifies that the ReceiveFile method throws an ArugmentNullException
        /// if the destination file path is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReceiveFileThrowsArgumentNullExceptionWithNullDestinationFile()
        {
            this.adapter.ReceiveFile(ConsoleAddress, null, LocalFile, null);
        }

        /// <summary>
        /// Verifies that the ReceiveFile method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestReceiveFileTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.ReceiveFile(ConsoleAddress, this.remoteFile, LocalFile, null);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter ReceiveFile(XboxFileSytemObjectPath, string) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole ReceiveFile(XboxFileSytemObjectPath, string) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the ReceiveFile method passes the correct parameters to the Xdk.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestReceiveFileCallsXdkCopyFiles()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, ipAddress, "ReceiveFile did not pass the correct IP address to the Xdk.");
                Assert.AreEqual(0, recursionLevel, "ReceiveFile should call Xdk.CopyFiles with a recursion level of zero.");
                Assert.AreEqual(LocalFile, destinationPath, "ReceiveFile didn't pass the correct destination to the Xdk.");
                Assert.AreEqual(this.remoteFile.FullName, sourceSearchPath, "ReceiveFile didn't pass the correct source to the Xdk.");
                Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "ReceiveFile didn't pass the correct operating system to the Xdk.");
                Assert.AreSame(expectedProgess, metrics, "ReceiveFile didn't pass the correct progress object to the Xdk.");
            };

            this.adapter.ReceiveFile(ConsoleAddress, this.remoteFile, LocalFile, null);
            Assert.IsTrue(isCorrectMethodCalled, "The ReceiveFile method did not call the Xdk's CopyFiles method.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.ReceiveFile(ConsoleAddress, this.remoteFile, LocalFile, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ReceiveFile(null, this.remoteFile, LocalFile, null));
        }

        /// <summary>
        /// Verifies that the DeleteFile method throws an ObjectDisposedException
        /// if the adapter has already been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDeleteFileThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DeleteFile(ConsoleAddress, this.remoteFile);
        }

        /// <summary>
        /// Verifies that the DeleteFile method throws an ArgumentNullException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteFileThrowsArgumentNullException()
        {
            this.adapter.DeleteFile(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the DeleteFile method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestDeleteFileTurnsComExceptionsIntXboxConsoleExceptions()
        {
            this.fakeXboxXdk.DeleteFilesAction = (ipAddress, remoteSearchPath, targetOperatingSystem, recursionLevel) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.DeleteFile(ConsoleAddress, this.remoteFile);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter DeleteFile(XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole DeleteFile(XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the DeleteFile method calls the DeleteFiles method in the
        /// XDK with the correct parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestDeleteFileCallsXdkDeleteFiles()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.DeleteFilesAction = (ipAddress, remoteFileSearchPattern, targetOperatingSystem, recursionLevel) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "DeleteFiles did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(0, recursionLevel, "DeleteFiles should call Xdk.CopyFiles with a recursion level of zero.");
                    Assert.AreEqual(this.remoteFile.FullName, remoteFileSearchPattern, "DeleteFiles didn't pass the correct destination to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "DeleteFiles didn't pass the correct operating system to the Xdk.");
                };

            this.adapter.DeleteFile(ConsoleAddress, this.remoteFile);
            Assert.IsTrue(isCorrectMethodCalled, "DeleteFile didn't call the DeleteFiles method in the Xdk.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.DeleteFile(null, this.remoteFile));
        }

        /// <summary>
        /// Verifies that the SendDirectory method throws an ObjectDisposedException
        /// if the adapter has already been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSendDirectoryThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, false, null);
        }

        /// <summary>
        /// Verifies that the SendDirectory method throws an ArgumentNullException
        /// if given a null value for the sourceDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendDirectoryThrowsArgumentNullExceptionWithNullSourceDirectory()
        {
            this.adapter.SendDirectory(ConsoleAddress, null, this.remoteFile, false, null);
        }

        /// <summary>
        /// Verifies that the SendDirectory method throws an ArgumentNullException
        /// if given a null value for the destinationDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendDirectoryThrowsArgumentNullExceptionWithNullRemoteDirectory()
        {
            this.adapter.SendDirectory(ConsoleAddress, LocalFile, null, false, null);
        }

        /// <summary>
        /// Verifies that the SendDirectory method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestSendDirectoryTurnsComExceptionsIntXboxConsoleExceptions()
        {
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, true, null);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter SendDirectory(string, XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole SendDirectory(string, XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that a call to recursively send a directory passes the correct parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestSendDirectoryRecursiveCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "SendDirectory did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(-1, recursionLevel, "SendDirectory should call Xdk.CopyFiles with a recursion level of negative one.");
                    Assert.AreEqual(this.remoteFile.FullName, destinationPath, "SendDirectory didn't pass the correct destination to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "SendDirectory didn't pass the correct operating system to the Xdk.");
                    Assert.IsTrue(sourceSearchPath.IndexOf(LocalFile, StringComparison.OrdinalIgnoreCase) >= 0, "SendDirectory didn't pass the correct source path to the XDK.");
                    Assert.AreSame(expectedProgess, metrics, "SendDirectory didn't pass the correct progress object to the Xdk.");
                };

            this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, true, null);
            Assert.IsTrue(isCorrectMethodCalled, "The SendDirectory method did not call the CopyFiles method in the XDK.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, true, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SendDirectory(null, LocalFile, this.remoteFile, true, null));
        }

        /// <summary>
        /// Verifies that a call to non-recursively send a directory passes the correct parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestSendDirectoryNonRecursiveCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "SendDirectory did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(0, recursionLevel, "SendDirectory should call Xdk.CopyFiles with a recursion level of zero.");
                    Assert.AreEqual(this.remoteFile.FullName, destinationPath, "SendDirectory didn't pass the correct destination to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "SendDirectory didn't pass the correct operating system to the Xdk.");
                    Assert.IsTrue(sourceSearchPath.StartsWith(LocalFile, StringComparison.OrdinalIgnoreCase), "SendDirectory didn't pass the correct source path to the XDK.");
                    Assert.AreSame(expectedProgess, metrics, "SendDirectory didn't pass the correct progress object to the Xdk.");
                };

            this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, false, null);
            Assert.IsTrue(isCorrectMethodCalled, "The SendDirectory method did not call the CopyFiles method in the XDK.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.SendDirectory(ConsoleAddress, LocalFile, this.remoteFile, false, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.SendDirectory(null, LocalFile, this.remoteFile, false, null));
        }

        /// <summary>
        /// Verifies that the ReceiveDirectory method throws an ObjectDisposedException
        /// if the adapter has already been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestReceiveDirectoryThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, false, null);
        }

        /// <summary>
        /// Verifies that the ReceiveDirectory method throws an ArgumentNullException
        /// if given a null value for the sourceDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReceiveDirectoryThrowsArgumentNullExceptionWithNullSourceDirectory()
        {
            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, null, false, null);
        }

        /// <summary>
        /// Verifies that the ReceiveDirectory method throws an ArgumentNullException
        /// if given a null value for the destinationDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReceiveDirectoryThrowsArgumentNullExceptionWithNullRemoteDirectory()
        {
            this.adapter.ReceiveDirectory(ConsoleAddress, null, LocalFile, false, null);
        }

        /// <summary>
        /// Verifies that the ReceiveDirectory method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestReceiveDirectoryTurnsComExceptionsIntXboxConsoleExceptions()
        {
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                throw new COMException();
            };

            ShimXboxConsoleAdapterBase.AllInstances.GetDirectoryContentsStringXboxPath = (@base, ipAddress, xboxPath) =>
            {
                if (xboxPath.FullName == this.remoteFile.FullName)
                {
                    return new XboxFileSystemInfoDefinition[]
                            {
                                new XboxFileSystemInfoDefinition(0, FileAttributes.Directory, Path.Combine(xboxPath.FullName, "subdirectory"), xboxPath.OperatingSystem, 0, 0, 0),
                                new XboxFileSystemInfoDefinition(0, FileAttributes.Archive, Path.Combine(xboxPath.FullName, "file.txt"), xboxPath.OperatingSystem, 0, 0, 0),
                            };
                }
                else
                {
                    return Enumerable.Empty<XboxFileSystemInfoDefinition>();
                }
            };

            try
            {
                this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, true, null);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter ReceiveDirectory(XboxFileSytemObjectPath, string) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole ReceiveDirectory(XboxFileSytemObjectPath, string) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that a call to recursively send a directory passes the correct parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestReceiveDirectoryRecursiveCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "ReceiveDirectory did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "ReceiveDirectory didn't pass the correct operating system to the Xdk.");
                    Assert.IsTrue(sourceSearchPath.IndexOf(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase) >= 0, "ReceiveDirectory didn't pass the correct source path to the XDK.");
                    if (expectedProgess == null)
                    {
                        Assert.IsNull(metrics, "ReceiveDirectory didn't pass the a null progress object to the Xdk.");
                    }
                    else
                    {
                        Assert.IsNotNull(metrics, "ReceiveDirectory didn't pass the a non-null progress object to the Xdk.");
                    }
                };

            ShimXboxConsoleAdapterBase.AllInstances.GetDirectoryContentsStringXboxPath = (@base, ipAddress, xboxPath) =>
                {
                    if (xboxPath.FullName == this.remoteFile.FullName)
                    {
                        return new XboxFileSystemInfoDefinition[]
                            {
                                new XboxFileSystemInfoDefinition(0, FileAttributes.Directory, Path.Combine(xboxPath.FullName, "subdirectory"), xboxPath.OperatingSystem, 0, 0, 0),
                                new XboxFileSystemInfoDefinition(0, FileAttributes.Archive, Path.Combine(xboxPath.FullName, "file.txt"), xboxPath.OperatingSystem, 0, 0, 0),
                            };
                    }
                    else
                    {
                        return Enumerable.Empty<XboxFileSystemInfoDefinition>();
                    }
                };

            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, true, null);
            Assert.IsTrue(isCorrectMethodCalled, "The ReceiveDirectory method did not call the CopyFiles method in the XDK.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, true, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ReceiveDirectory(null, this.remoteFile, LocalFile, true, null));
        }

        /// <summary>
        /// Verifies that a call to non-recursively send a directory passes the correct parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestReceiveDirectoryNonRecursiveCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            IProgress<XboxFileTransferMetric> expectedProgess = null;
            this.fakeXboxXdk.CopyFilesAction = (ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, ipAddress, "ReceiveDirectory did not pass the correct IP address to the Xdk.");
                Assert.AreEqual(0, recursionLevel, "ReceiveDirectory should call Xdk.CopyFiles with a recursion level of zero.");
                Assert.AreEqual(LocalFile, destinationPath, "ReceiveDirectory didn't pass the correct destination to the Xdk.");
                Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "ReceiveDirectory didn't pass the correct operating system to the Xdk.");
                Assert.IsTrue(sourceSearchPath.IndexOf(LocalFile, StringComparison.OrdinalIgnoreCase) >= 0, "ReceiveDirectory didn't pass the correct source path to the XDK.");
                Assert.AreSame(expectedProgess, metrics, "ReceiveDirectory didn't pass the correct progress object to the Xdk.");
            };

            this.fakeXboxXdk.FindFilesFunc = (ipAddress, searchPattern, operatingSystem, recursionLevel) =>
            {
                return new XboxFileSystemInfoDefinition[]
                    {
                        new XboxFileSystemInfoDefinition(0, FileAttributes.Directory, @"xd:\directory", operatingSystem, 0, 0, 0), 
                        new XboxFileSystemInfoDefinition(0, FileAttributes.Archive, @"xd:\directory\file.txt", operatingSystem, 0, 0, 0), 
                    };
            };

            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, false, null);
            Assert.IsTrue(isCorrectMethodCalled, "The ReceiveDirectory method did not call the CopyFiles method in the XDK.");

            expectedProgess = new Progress<XboxFileTransferMetric>();
            this.adapter.ReceiveDirectory(ConsoleAddress, this.remoteFile, LocalFile, false, expectedProgess);

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.ReceiveDirectory(null, this.remoteFile, LocalFile, false, null));
        }

        /// <summary>
        /// Verifies that the DeleteDirectory methods throws an ObjectDisposedException
        /// if the adatper is already disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDeleteDirectoryThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DeleteDirectory(ConsoleAddress, this.remoteFile, false);
        }

        /// <summary>
        /// Verifies that DeleteDirectory method throws an ArgumentNullException
        /// when given an null value for the directoryToDelete parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteDirectoryThrowsArgumentNullException()
        {
            this.adapter.DeleteDirectory(ConsoleAddress, null, false);
        }

        /// <summary>
        /// Verifies that the DeleteDirectory method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestDeleteDirectoryTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.RemoveDirectoryAction = (ipAddress, sourceSearchPath, targetOperatingSystem, isRecursive) => { throw new COMException(); };
            this.fakeXboxXdk.DeleteFilesAction = (ipAddress, remoteFileSearchPattern, targetOperatingSystem, recursionLevel) => { throw new COMException(); };
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) => { throw new COMException(); };

            try
            {
                this.adapter.DeleteDirectory(ConsoleAddress, this.remoteFile, true);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter DeleteDirectory(XboxFileSytemObjectPath, bool) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole DeleteDirectory(XboxFileSytemObjectPath, bool) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the recurisve DeleteDirectory function passes the correct
        /// parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestDeleteDirectoryRecursiveCallsXdk()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.DeleteFilesAction = (ipAddress, remoteFileSearchPattern, targetOperatingSystem, recursionLevel) => { };
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) => Enumerable.Empty<XboxFileSystemInfoDefinition>();
            this.fakeXboxXdk.RemoveDirectoryAction = (ipAddress, remoteDirectoryPath, targetOperatingSystem, recursive) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "DeleteDirectory did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "SendDirectory didn't pass the correct operating system to the Xdk.");
                    Assert.IsTrue(remoteDirectoryPath.StartsWith(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase), "DeleteDirectory is trying to delete a directory outside of the given directory.");
                };

            this.adapter.DeleteDirectory(ConsoleAddress, this.remoteFile, true);
            Assert.IsTrue(isCorrectMethodCalled, "DeleteDirectory did not call the correct method in the XDK.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.DeleteDirectory(null, this.remoteFile, true));
        }

        /// <summary>
        /// Verifies that the non-recurisve DeleteDirectory function passes the correct
        /// parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestDeleteDirectoryNonRecursiveCallsXdk()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.DeleteFilesAction = (ipAddress, remoteFileSearchPattern, targetOperatingSystem, recursionLevel) => { };
            this.fakeXboxXdk.RemoveDirectoryAction = (ipAddress, remoteDirectoryPath, targetOperatingSystem, recursive) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, ipAddress, "DeleteDirectory did not pass the correct IP address to the Xdk.");
                Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "SendDirectory didn't pass the correct operating system to the Xdk.");
                Assert.IsFalse(recursive, "DeleteDirectory did not pass the recursive flag to the XDK correctly.");
                Assert.IsTrue(remoteDirectoryPath.StartsWith(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase), "DeleteDirectory is trying to delete a directory outside of the given directory.");
            };

            this.adapter.DeleteDirectory(ConsoleAddress, this.remoteFile, false);
            Assert.IsTrue(isCorrectMethodCalled, "DeleteDirectory did not call the correct method in the XDK.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.DeleteDirectory(null, this.remoteFile, false));
        }

        /// <summary>
        /// Verifies that the GetDirectoryContents method throws an ObjectDisposedException
        /// if the adapter had already been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetDirectoryContentsThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.GetDirectoryContents(ConsoleAddress, this.remoteFile);
        }

        /// <summary>
        /// Verifies that the GetDirectoryContents method throws an ArgumentNullException
        /// if a null value is given for the xboxDirectory parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDirectoryContentsThrowsArgumentNullException()
        {
            this.adapter.GetDirectoryContents(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the GetDirectoryContents method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestGetDirectoryContentsTurnsComExceptionsIntXboxConsoleExceptions()
        {
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.GetDirectoryContents(ConsoleAddress, this.remoteFile);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter GetDirectoryContents(XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole GetDirectoryContents(XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the GetDirectoryContents method correctly calls the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestGetDirectoryContentsCallsXdk()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, remoteSearchPattern, targetOperatingSystem, recursionLevels) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "GetDirectoryContents did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "GetDirectoryContents did not pass the correct operating system to the XDK.");
                    Assert.IsTrue(remoteSearchPattern.StartsWith(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase), "GetDirectoryContents did not pass the correct directory to the XDK.");
                    Assert.AreEqual(0, recursionLevels, "GetDirectoryContents did not pass a recursionLevel value of zero.");
                    return Enumerable.Empty<XboxFileSystemInfoDefinition>();
                };

            var contents = this.adapter.GetDirectoryContents(ConsoleAddress, this.remoteFile);
            Assert.IsTrue(!contents.Any(), "GetDirectoryContents returned the incorrect number of XboxFileSystemInfoDefintion objects.");
            Assert.IsTrue(isCorrectMethodCalled, "GetDirectoryContents did not call the correct Xdk method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetDirectoryContents(null, this.remoteFile));
        }

        /// <summary>
        /// Verifies that the GetFileSystemInfoDefinition method will throw an
        /// ObjectDisposedException if the adapter is already disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestGetFileSystemInfoDefinitionThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.GetFileSystemInfoDefinition(ConsoleAddress, this.remoteFile);
        }

        /// <summary>
        /// Verifies that the GetFileSystemInfoDefinition method will throw an
        /// ArgumentNullException if given a null value for the xboxFilePath
        /// parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetFileSystemInfoDefinitionThrowsArgumentNullException()
        {
            this.adapter.GetFileSystemInfoDefinition(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the GetFileSystemInfoDefinition method converts COMExceptions thrown by the XDK
        /// into XboxConsoleExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestGetFileSystemInfoDefinitionTurnsComExceptionsIntXboxConsoleExceptions()
        {
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.GetFileSystemInfoDefinition(ConsoleAddress, this.remoteFile);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter GetFileSystemInfoDefinition(XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole GetFileSystemInfoDefinition(XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the GetFileSystemInfoDefinition method calls the XDK  correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestGetFileSystemInfoDefinitionThrowsFileNotFoundException()
        {
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) =>
                {
                    Assert.AreEqual(ConsoleAddress, ipAddress, "GetDirectoryContents did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "GetDirectoryContents did not pass the correct operating system to the XDK.");
                    Assert.AreEqual(0, recursionLevel, "GetFileSystemInfoDefinition should call Xdk.FindFiles with a recursion level of zero.");
                    Assert.IsTrue(sourceSearchPath.StartsWith(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase), "GetDirectoryContents did not pass the correct directory to the XDK.");

                    return Enumerable.Empty<XboxFileSystemInfoDefinition>();
                };

            this.adapter.GetFileSystemInfoDefinition(ConsoleAddress, this.remoteFile);
        }

        /// <summary>
        /// Verifies that the GetFileSystemInfoDefinition method calls the XDK  correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestGetFileSystemInfoDefinitionCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.FindFilesFunc = (ipAddress, sourceSearchPath, targetOperatingSystem, recursionLevel) =>
            {
                isCorrectMethodCalled = true;
                Assert.AreEqual(ConsoleAddress, ipAddress, "GetDirectoryContents did not pass the correct IP address to the Xdk.");
                Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "GetDirectoryContents did not pass the correct operating system to the XDK.");
                Assert.AreEqual(0, recursionLevel, "GetFileSystemInfoDefinition should call Xdk.FindFiles with a recursion level of zero.");
                Assert.IsTrue(sourceSearchPath.StartsWith(this.remoteFile.FullName, StringComparison.OrdinalIgnoreCase), "GetDirectoryContents did not pass the correct directory to the XDK.");

                return new XboxFileSystemInfoDefinition[] { new XboxFileSystemInfoDefinition(0, FileAttributes.Archive, this.remoteFile.FullName, this.remoteFile.OperatingSystem, 0, 0, 0) };
            };

            var xboxFileInfo = this.adapter.GetFileSystemInfoDefinition(ConsoleAddress, this.remoteFile);
            Assert.IsTrue(isCorrectMethodCalled, "GetFileSystemInfoDefinition did not call the correct Xdk method.");
            Assert.IsNotNull(xboxFileInfo, "The file returned by GetFileSystemDefinition does not match the file requested.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetFileSystemInfoDefinition(null, this.remoteFile));
        }

        /// <summary>
        /// Verifies that the CreateDirectory method throws an ObjectDisposedException
        /// if the adapter has already been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestCreateDirectoryThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.CreateDirectory(ConsoleAddress, this.remoteFile);
        }

        /// <summary>
        /// Verifies that the CreateDirectory mehtod will throw an
        /// ArgumentNullException if given a null-value for the 
        /// xboxDirectoryPath parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateDirectoryThrowsArgumentNullException()
        {
            this.adapter.CreateDirectory(ConsoleAddress, null);
        }

        /// <summary>
        /// Verifies that the CreateDirectory method will catch a COMException from the XDK
        /// and wrap it in an XboxConsoleException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        public void TestCreateDirectoryTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.CreateDirectoryAction = (ipAddress, xboxDirectoryPath, targetOperatingSystem) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.CreateDirectory(ConsoleAddress, this.remoteFile);
                Assert.Fail("The XDK should have thrown a COMException");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsoleAdapter CreateDirectory(XboxFileSytemObjectPath) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole CreateDirectory(XboxFileSytemObjectPath) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the CreateDirectory method passes the correct parameters to the XDK.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterFileIoTestCategory)]
        public void TestCreateDirectoryCallsXdkCorrectly()
        {
            bool isCorrectMethodCalled = false;
            this.fakeXboxXdk.CreateDirectoryAction = (ipAddress, xboxDirectoryPath, targetOperatingSystem) =>
                {
                    isCorrectMethodCalled = true;
                    Assert.AreEqual(ConsoleAddress, ipAddress, "CreateDirectory did not pass the correct IP address to the Xdk.");
                    Assert.AreEqual(this.remoteFile.OperatingSystem, targetOperatingSystem, "CreateDirectory did not pass the correct operating system to the XDK.");
                    Assert.AreEqual(this.remoteFile.FullName, xboxDirectoryPath, "CreateDirectory did not pass the correct directory path to the XDK.");
                };

            this.adapter.CreateDirectory(ConsoleAddress, this.remoteFile);
            Assert.IsTrue(isCorrectMethodCalled, "CreateDirectory did not call the correct Xdk method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.CreateDirectory(null, this.remoteFile));
        }
    }
}
