//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.Input.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.Input.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterInputTestCategory = "Adapter.Input";
        private const string SystemIpAddress = "10.124.151.246"; // The actual IP address used here is irrelevant.

        /// <summary>
        /// Verifies that the ConnectGamepad method works correctly with valid parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        public void TestConnectGamepad()
        {
            bool invoked = false;
           
            var stubGamepad = new StubIVirtualGamepad();
            stubGamepad.Connect = () => { invoked = true; return 0; };
            this.fakeXboxXdk.CreateGamepadFunc = (i) => { return stubGamepad; };
           
            this.adapter.ConnectXboxGamepad(SystemIpAddress);

            Assert.IsTrue(invoked);
        }

        /// <summary>
        /// Verifies that the ConnectGamepad method throws an exception if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestConnectGamepadThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.ConnectXboxGamepad(SystemIpAddress);
        }

        /// <summary>
        /// Verifies that the ConnectGamepad method throws an exception if
        /// the systemIpAddress parameter is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConnectGamepadThrowsArgumentNullExceptionForNullSystemIpAddress()
        {
            this.adapter.ConnectXboxGamepad(null);
        }

        /// <summary>
        /// Verifies that the DisconnectGamepad method works correctly with valid parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        public void TestDisconnectGamepad()
        {
            bool invoked = false;
            var stubGamepad = new StubIVirtualGamepad();
            stubGamepad.Disconnect = () => { invoked = true; };
            this.fakeXboxXdk.CreateGamepadFunc = (i) => { return stubGamepad; };

            this.adapter.ConnectXboxGamepad("ip");
            this.adapter.DisconnectXboxGamepad(SystemIpAddress, 0);

            Assert.IsTrue(invoked);
        }

        /// <summary>
        /// Verifies that the DisconnectGamepad method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestDisconnectGamepadThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.DisconnectXboxGamepad(SystemIpAddress, 0);
        }

        /// <summary>
        /// Verifies that the DisconnectGamepad method throws an exception if
        /// the systemIpAddress parameter is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDisconnectGamepadThrowsArgumentNullExceptionForNullSystemIpAddress()
        {
            this.adapter.DisconnectXboxGamepad(null, 0);
        }

        /// <summary>
        /// Verifies that the SendGamepadReport method works correctly with valid parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        public void TestSendGamepadReport()
        {
            bool invoked = false;

            var stubGamepad = new StubIVirtualGamepad();
            stubGamepad.SetGamepadStateXboxGamepadState = (state) => { invoked = true; };
            this.fakeXboxXdk.CreateGamepadFunc = (i) => { return stubGamepad; };

            this.adapter.ConnectXboxGamepad(SystemIpAddress);
            this.adapter.SendGamepadReport(SystemIpAddress, 0, new XboxGamepadState());

            Assert.IsTrue(invoked);
        }

        /// <summary>
        /// Verifies that the SendGamepadReport method throws an ObjectDisposedException if
        /// the adapter has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestSendGamepadReportThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.SendGamepadReport(SystemIpAddress, 0, new XboxGamepadState());
        }

        /// <summary>
        /// Verifies that the SendGamepadReport method throws an exception if
        /// the systemIpAddress parameter is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendGamepadReportThrowsArgumentNullExceptionForNullSystemIpAddress()
        {
            this.adapter.SendGamepadReport(null, 0, new XboxGamepadState());
        }

        /// <summary>
        /// Verifies that the SendGamepadReport method throws an exception if
        /// the XboxGamepadState parameter is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterInputTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendGamepadReportThrowsArgumentNullExceptionForNullGamepadState()
        {
            this.adapter.SendGamepadReport(SystemIpAddress, 0, null);
        }
    }
}
