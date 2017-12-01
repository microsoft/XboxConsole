//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.Debug.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterDebugTestCategory = "Adapter.Debug";

        /// <summary>
        /// Verifies that if the XDK's GetRunningProcesses(XboxOperatingSystem) function throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterDebugTestCategory)]
        public void TestGetRunningProcessesTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.GetRunningProcessesFunc = (_0, _1) =>
            {
                throw new COMException();
            };

            try
            {
                var processes = this.adapter.GetRunningProcesses(ConsoleAddress, XboxOperatingSystem.Title);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole GetRunningProcesses(XboxOperatingSystem) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole GetRunningProcesses(XboxOperatingSystem) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the GetRunningProcesses(XboxOperatingSystem) method calls the XboxXdk's GetRunningProcesses(XboxOperatingSystem) method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterDebugTestCategory)]
        public void TestGetRunningProcessesCallsXboxXdkGetRunningProcesses()
        {
            bool isGetRunningProcessesCalled = false;
            bool correctIpAddressPassed = false;
            bool correctOperatingSystemPassed = false;

            var xboxProcesses = new XboxProcessDefinition[] 
                    { 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 0, "file0.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 1, "file1.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 2, "Home.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 3, "file2.exe") 
                    };

            this.fakeXboxXdk.GetRunningProcessesFunc = (ipAddress, operatingSystem) =>
            {
                correctIpAddressPassed = ipAddress == XboxConsoleAdapterTests.ConsoleAddress;
                correctOperatingSystemPassed = operatingSystem == XboxOperatingSystem.Title;
                isGetRunningProcessesCalled = true;
                return new XboxProcessDefinition[] 
                    {
                        xboxProcesses[0],
                        xboxProcesses[1],
                        xboxProcesses[2],
                        xboxProcesses[3]
                    };
            };

            var processes = this.adapter.GetRunningProcesses(ConsoleAddress, XboxOperatingSystem.Title);
            Assert.IsTrue(isGetRunningProcessesCalled, "The adapter's GetRunningProcesses(XboxOperatingSystem) method must call the XboxXdk's GetRunningProcesses(..) method.");
            Assert.IsTrue(correctIpAddressPassed, "The adapter passed wrong IP Address to the XboxXdk's GetRunningProcesses(..) method.");
            Assert.IsTrue(correctOperatingSystemPassed, "The adapter passed wrong Operating System to the XboxXdk's GetRunningProcesses(..) method.");
            Assert.IsTrue(processes.SequenceEqual(xboxProcesses), "The adapter didn't return correct enumeration of processes.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.GetRunningProcesses(null, XboxOperatingSystem.Title));
        }
    }
}
