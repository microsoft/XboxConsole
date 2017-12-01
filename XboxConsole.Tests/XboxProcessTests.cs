//------------------------------------------------------------------------------
// <copyright file="XboxProcessTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxProcess class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is a test class and the objects are disposed in the TestCleanup method.")]
    [TestClass]
    public partial class XboxProcessTests
    {
        private const string XboxProcessTestCategory = "XboxConsole.XboxProcess";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private const string ProcessRunTestFileName = "TestFileName.exe";
        private const string ProcessRunTestArguments = "TestArguments.";
        private const XboxOperatingSystem ProcessRunTestOperatingSystem = XboxOperatingSystem.Title;
        private readonly IPAddress processRunTestIPAddress = IPAddress.Parse(ConsoleAddress);
        private Action<string> processRunTestAction;

        private XboxProcess xboxProcess;
        private XboxProcessDefinition processDefinition;

        private IDisposable shimsContext;
        private XboxConsole xboxConsole;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            this.processDefinition = new XboxProcessDefinition(XboxOperatingSystem.Title, 123, "process.exe");

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
            {
                var myShim = new ShimXboxConsole(console)
                {
                    AdapterGet = () => new StubXboxConsoleAdapterBase(null),
                    SystemIpAddressGet = () => IPAddress.Parse(XboxProcessTests.ConsoleAddress),
                    XboxGamepadsGet = () => new List<Input.XboxGamepad>()
                };
            };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
            {
            };

            this.processRunTestAction = new Action<string>((str) => { });
            this.xboxConsole = new XboxConsole((IPAddress)null);
            this.xboxProcess = new XboxProcess(this.processDefinition, this.xboxConsole);
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.xboxConsole.Dispose();
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that the static method XboxProcess.Run() calls the Adapter.RunExecutable( ... ) method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        public void TestProcessRunCallsAdapterRunExecutable()
        {
            bool isCorrectMethodCalled;
            ShimXboxConsoleAdapterBase.AllInstances.RunExecutableStringStringStringXboxOperatingSystemActionOfString = 
                (adapter, toolsIP, fileName, arguments, operatingSystem, outputReceivedCallback) =>
                {
                    isCorrectMethodCalled = true;
                };

            isCorrectMethodCalled = false;
            XboxProcess.Run(this.xboxConsole, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem);
            Assert.IsTrue(isCorrectMethodCalled, "The static XboxProcess.Run() method didn't call the adapter's RunExecutable( ... ).");

            isCorrectMethodCalled = false;
            XboxProcess.Run(this.xboxConsole, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem, this.processRunTestAction);
            Assert.IsTrue(isCorrectMethodCalled, "The static XboxProcess.Run() method didn't call the adapter's RunExecutable( ... ).");
        }

        /// <summary>
        /// Verifies that the static method XboxProcess.Run() passes the correct values to the Adapter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        public void TestProcessRunPassesCorrectValues()
        {
            ShimXboxProcess.RunXboxConsoleStringStringXboxOperatingSystemActionOfString = 
                (console, fileName, arguments, operatingSystem, outputReceivedCallback) =>
                {
                    Type processType = typeof(XboxProcess);
                    CheckRunExecutableValueEquality(processType, console.SystemIpAddress.ToString(), fileName, arguments, operatingSystem, outputReceivedCallback);
                };

            ShimXboxConsoleAdapterBase.AllInstances.RunExecutableStringStringStringXboxOperatingSystemActionOfString =
                (adapter, toolsIP, fileName, arguments, operatingSystem, outputReceivedCallback) =>
                {
                    Type adapterType = typeof(XboxConsoleAdapterBase);
                    CheckRunExecutableValueEquality(adapterType, toolsIP, fileName, arguments, operatingSystem, outputReceivedCallback);
                };

            ShimXboxXdkBase.AllInstances.RunExecutableStringStringStringXboxOperatingSystemActionOfString =
                (x, toolsIP, fileName, arguments, operatingSystem, outputReceivedCallback) =>
                {
                    Type xdkType = typeof(XboxXdkBase);
                    CheckRunExecutableValueEquality(xdkType, toolsIP, fileName, arguments, operatingSystem, outputReceivedCallback);
                };

            using (this.xboxConsole = new XboxConsole(this.processRunTestIPAddress))
            {
                XboxProcess.Run(this.xboxConsole, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem, this.processRunTestAction);
                this.processRunTestAction = null;
                XboxProcess.Run(this.xboxConsole, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem);
            }
        }

        /// <summary>
        /// Verifies that the static method XboxProcess.Run() throws an exception when a null XboxConsole is provided.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestProcessRunThrowsOnNullConsole()
        {
            XboxProcess.Run(null, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem);
        }

        /// <summary>
        /// Verifies that the static method XboxProcess.Run() throws an exception when an invalid fileName is provided.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestProcessRunThrowsOnIncorrectFilePath()
        {
            XboxProcess.Run(this.xboxConsole, string.Empty, ProcessRunTestArguments, ProcessRunTestOperatingSystem);
        }

        /// <summary>
        /// Verifies that the static method XboxProcess.Run() throws an exception when a disposed XboxConsole is provided.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestProcessRunAfterDispose()
        {
            this.xboxConsole = new XboxConsole(this.processRunTestIPAddress);
            this.xboxConsole.Dispose();
            XboxProcess.Run(this.xboxConsole, ProcessRunTestFileName, ProcessRunTestArguments, ProcessRunTestOperatingSystem);
        }

        /// <summary>
        /// Verifies that the subscribing to TextReceived event calls Adapter's StartDebug, unsubscribing calls Adapter's StopDebug, and triggering calls the event handler.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        public void TestTextReceivedCallsAdapterStartDebugStopDebug()
        {
            bool isStartDebugCalled = false;
            bool isStopDebugCalled = false;
            bool isTextReceived0 = false;
            bool isTextReceived1 = false;
            bool isTextReceived2 = false;
            EventHandler<TextEventArgs> eventHandler = null;

            ShimXboxConsoleAdapterBase.AllInstances.StartDebugStringXboxOperatingSystemUInt32EventHandlerOfTextEventArgs = (adapter, systemIpAddress, operatingSystem, processId, handler) =>
                {
                    isStartDebugCalled = true;
                    eventHandler = handler;
                };

            ShimXboxConsoleAdapterBase.AllInstances.StopDebugStringXboxOperatingSystemUInt32 = (adapter, systemIpAddress, operatingSystem, processId) =>
                {
                    isStopDebugCalled = true;
                };

            EventHandler<TextEventArgs> textReceived0 = (sender, args) => 
                {
                    isTextReceived0 = true;
                };

            EventHandler<TextEventArgs> textReceived1 = (sender, args) =>
                {
                    isTextReceived1 = true;
                };

            EventHandler<TextEventArgs> textReceived2 = (sender, args) =>
                {
                    isTextReceived2 = true;
                };

            this.xboxProcess = new XboxProcess(this.processDefinition, this.xboxConsole);

            this.xboxProcess.TextReceived += textReceived0;
            this.xboxProcess.TextReceived += textReceived1;
            this.xboxProcess.TextReceived += textReceived2;
            Assert.IsTrue(isStartDebugCalled, "Adapter's StartDebug was not called.");
            Assert.IsFalse(isTextReceived0, "TextReceived event handler (0) should not be called.");
            Assert.IsFalse(isTextReceived1, "TextReceived event handler (1) should not be called.");
            Assert.IsFalse(isTextReceived2, "TextReceived event handler (2) should not be called.");
            Assert.IsFalse(isStopDebugCalled, "Adapter's StopDebug should not be called.");

            isStartDebugCalled = false;
            isStopDebugCalled = false;
            isTextReceived0 = false;
            isTextReceived1 = false;
            isTextReceived2 = false;

            // call a handler as well to test the event
            eventHandler(new object(), new TextEventArgs("Some source", "Some message"));
            Assert.IsFalse(isStartDebugCalled, "Adapter's StartDebug should not be called.");
            Assert.IsTrue(isTextReceived0, "TextReceived event handler (0) was not called.");
            Assert.IsTrue(isTextReceived1, "TextReceived event handler (1) was not called.");
            Assert.IsTrue(isTextReceived2, "TextReceived event handler (2) was not called.");
            Assert.IsFalse(isStopDebugCalled, "Adapter's StopDebug should not be called.");

            isStartDebugCalled = false;
            isStopDebugCalled = false;
            isTextReceived0 = false;
            isTextReceived1 = false;
            isTextReceived2 = false;

            this.xboxProcess.TextReceived -= textReceived0;
            this.xboxProcess.TextReceived -= textReceived1;
            this.xboxProcess.TextReceived -= textReceived2;
            Assert.IsFalse(isStartDebugCalled, "Adapter's StartDebug should not be called.");
            Assert.IsFalse(isTextReceived0, "TextReceived event handler (0) should not be called.");
            Assert.IsFalse(isTextReceived1, "TextReceived event handler (1) should not be called.");
            Assert.IsFalse(isTextReceived2, "TextReceived event handler (2) should not be called.");
            Assert.IsTrue(isStopDebugCalled, "Adapter's StopDebug was not called.");
        }

        /// <summary>
        /// Verifies that the XboxProcess class will throw an ArgumentNullException
        /// if given a null value for the processDefinition parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxProcessThrowsArgumentNullExceptionWithNullProcessDefinition()
        {
#pragma warning disable 168
            XboxProcess notUsed = new XboxProcess(null, this.xboxConsole);
            Assert.IsNotNull(notUsed, "XboxProcess constructor did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the XboxProcess class will throw an ArgumentNullException
        /// if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxProcessTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxProcessThrowsArgumentNullExceptionWithNullConsole()
        {
#pragma warning disable 168
            XboxProcess notUsed = new XboxProcess(this.processDefinition, null);
            Assert.IsNotNull(notUsed, "XboxProcess constructor did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        private void CheckRunExecutableValueEquality(Type objectType, string toolsIP, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputReceivedCallback)
        {
            const string FailedToPassSameValue = "Failed to pass in the same {0} to the {1}.";
            Assert.AreEqual(ConsoleAddress, toolsIP, FailedToPassSameValue, "toolsIP", objectType);
            Assert.AreEqual(ProcessRunTestFileName, fileName, FailedToPassSameValue, "fileName", objectType);
            Assert.AreEqual(ProcessRunTestArguments, arguments, FailedToPassSameValue, "arguments", objectType);
            Assert.AreEqual(ProcessRunTestOperatingSystem, operatingSystem, FailedToPassSameValue, "operatingSystem", objectType);
            Assert.AreSame(this.processRunTestAction, outputReceivedCallback, FailedToPassSameValue, "outputReceivedCallback", objectType);
        }
    }
}