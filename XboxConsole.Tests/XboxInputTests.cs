//------------------------------------------------------------------------------
// <copyright file="XboxInputTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class of tests for Input.Gamepad class.
    /// </summary>
    [TestClass]
    public class XboxInputTests
    {
        private const string XboxInputTestCategory = "XboxConsole.Input";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

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
            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null));
            this.shimXboxConsole = new ShimXboxConsole
            {
                AdapterGet = () => this.shimAdapter,
                SystemIpAddressGet = () => IPAddress.Parse(XboxInputTests.ConsoleAddress),
                SystemIpAddressStringGet = () => IPAddress.Parse(XboxInputTests.ConsoleAddress).ToString(),
                SystemIpAddressAndSessionKeyCombinedGet = () => XboxInputTests.ConsoleAddress,
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
        /// Validates that id is set after initializing a new Gamepad.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadIdSetToInvalidOnConstruction()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            Assert.IsNotNull(gamepad, "Failed to create gamepad");
            Assert.IsNull(gamepad.Id, "id of gamepad should be null when not connected.");
        }

        /// <summary>
        /// Validates that id is set after connecting a new Gamepad.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsolAdapterBase object is not important.")]
        public void TestXboxGamepadIdSetToValidAfterConnect()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            Assert.IsNotNull(gamepad, "Failed to create gamepad.");
            gamepad.Connect();

            Assert.IsTrue(gamepad.Id >= 0, "Received a bad id, or failed to connect gamepad");
        }

        /// <summary>
        /// Validates that id is set after disconnecting a Gamepad.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsolAdapterBase object is not important.")]
        public void TestXboxGamepadIdSetToInvalidAfterDisconnect()
        {
            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
                DisconnectXboxGamepadStringUInt64 = (systemIpAddress, gamepadId) => { }
            };

            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            Assert.IsNotNull(gamepad, "Failed to create gamepad.");
            gamepad.Connect();
            Assert.IsTrue(gamepad.Id >= 0, "Received a bad id, or failed to connect gamepad");

            gamepad.Disconnect();

            Assert.IsNull(gamepad.Id, "Id not set to null after disconnecting");
        }

        /// <summary>
        /// Checks that exception is thrown when passing null to SetGamepadState.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxGamepadSetGamepadStateWithNullState()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);

            gamepad.SetXboxGamepadState(null);
        }

        /// <summary>
        /// Validates that parameters passed through SetGamepadState are valid.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The StubXboxConsolAdapterBase object is not important.")]
        public void TestXboxGamepadSendGamepadStateCallsAdapterSendGamepadReportCorrectly()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            var gamepadState = new XboxGamepadState();

            bool adapterCalled = false;
            
            this.shimAdapter = new ShimXboxConsoleAdapterBase(new StubXboxConsoleAdapterBase(null))
            {
                SendGamepadReportStringUInt64XboxGamepadState = (systemIpAddress, gamepadId, adapterGamepadState)
                    =>
                {
                    adapterCalled = true;
                    Assert.AreEqual(this.XboxConsole.SystemIpAddressString, systemIpAddress, "SystemIpAddress did not match the one from the XboxConsole");
                    Assert.AreEqual(gamepad.Id, gamepadId, "Gamepad Id is not equal to one being set");
                    Assert.AreEqual(gamepadState, adapterGamepadState, "GamepadState not equal to one being set");
                }
            };
            gamepad.Connect();
            gamepad.SetXboxGamepadState(gamepadState);

            Assert.IsTrue(adapterCalled, "Adapter SetGamepadState never called");
        }

        /// <summary>
        /// Verifies Connect throws an exception when already connected.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadConnectThrowsWhenAlreadyConnected()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            gamepad.Connect();

            this.VerifyGamepadThrowsFunc<XboxInputException>(
                () => gamepad.Connect(),
                "Connect did not throw an exception when already connected.");
        }

        /// <summary>
        /// Verifies Disconnect throws an exception when not connected.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadDisconnectThrowsWhenNotConnected()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);

            this.VerifyGamepadThrowsFunc<XboxInputException>(
                () => gamepad.Disconnect(), 
                "Disconnect did not throw an exception when not connected.");
        }

        /// <summary>
        /// Verifies SetGamepadState throws an exception when not connected.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadSetXboxGamepadStateThrowsWhenNotConnected()
        {
            var gamepad = new GamesTest.Xbox.Input.XboxGamepad(this.XboxConsole);
            
            XboxGamepadState state = new XboxGamepadState();

            this.VerifyGamepadThrowsFunc<XboxInputException>(
                () => gamepad.SetXboxGamepadState(state),
                "SetGamepadState did not throw an exception when not connected.");
        }

        /// <summary>
        /// Verifies XboxGamepadState throws an exception for invalid range values.
        /// </summary>        
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test method")]
        public void TestXboxGamepadStateInvalidRanges()
        {
            List<Action> badValues = new List<Action>()
            {
                () => new XboxGamepadState() { LeftThumbstickX = -2.0f },
                () => new XboxGamepadState() { LeftThumbstickX = 12.0f },
                () => new XboxGamepadState() { LeftThumbstickY = -4.0f },
                () => new XboxGamepadState() { LeftThumbstickY = 55.0f },
                () => new XboxGamepadState() { LeftTrigger = -0.4f },
                () => new XboxGamepadState() { LeftTrigger = 1.2f },
                () => new XboxGamepadState() { RightThumbstickX = -2.0f },
                () => new XboxGamepadState() { RightThumbstickX = 12.0f },
                () => new XboxGamepadState() { RightThumbstickY = -4.0f },
                () => new XboxGamepadState() { RightThumbstickY = 55.0f },
                () => new XboxGamepadState() { RightTrigger = -0.4f },
                () => new XboxGamepadState() { RightTrigger = 1.2f }
            };

            foreach (var badValue in badValues)
            {
                this.VerifyGamepadThrowsFunc<ArgumentOutOfRangeException>(badValue, "Did not throw exception for invalid range.");
            }
        }

        /// <summary>
        /// Verifies XboxGamepadState accepts valid value ranges.
        /// </summary>        
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test method")]
        public void TestXboxGamepadStateValidRanges()
        {
            List<Action> validValues = new List<Action>()
            {
                () => new XboxGamepadState() { LeftThumbstickX = -1.0f },
                () => new XboxGamepadState() { LeftThumbstickX = 1.0f },
                () => new XboxGamepadState() { LeftThumbstickX = 0.0f },
                () => new XboxGamepadState() { LeftThumbstickY = -1.0f },
                () => new XboxGamepadState() { LeftThumbstickY = 1.0f },
                () => new XboxGamepadState() { LeftThumbstickY = 0.0f },
                () => new XboxGamepadState() { LeftTrigger = -0.0f },
                () => new XboxGamepadState() { LeftTrigger = 1.0f },
                () => new XboxGamepadState() { LeftTrigger = 0.0f },
                () => new XboxGamepadState() { RightThumbstickX = -1.0f },
                () => new XboxGamepadState() { RightThumbstickX = 1.0f },
                () => new XboxGamepadState() { RightThumbstickX = 0.0f },
                () => new XboxGamepadState() { RightThumbstickY = -1.0f },
                () => new XboxGamepadState() { RightThumbstickY = 1.0f },
                () => new XboxGamepadState() { RightThumbstickY = 0.0f },
                () => new XboxGamepadState() { RightTrigger = -0.0f },
                () => new XboxGamepadState() { RightTrigger = 1.0f }
            };

            foreach (var validValue in validValues)
            {
                validValue();
            }
        }

        /// <summary>
        /// Verifies XboxGamepadState Equals with similar states.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadStateEqualsWithSimilar()
        {
            var gamepad1 = new XboxGamepadState()
            {
                Buttons = XboxGamepadButtons.A,
                LeftTrigger = 1.0f,
                RightTrigger = 1.0f,
                LeftThumbstickX = -1.0f,
                LeftThumbstickY = 0.3f,
                RightThumbstickX = 0.1f,
                RightThumbstickY = -0.2f
            };

            var gamepad2 = new XboxGamepadState()
            {
                Buttons = XboxGamepadButtons.A,
                LeftTrigger = 1.0f,
                RightTrigger = 1.0f,
                LeftThumbstickX = -1.0f,
                LeftThumbstickY = 0.3f,
                RightThumbstickX = 0.1f,
                RightThumbstickY = -0.21f
            };

            Assert.AreEqual(gamepad1, gamepad2, "Gamepads are not equal");
        }

        /// <summary>
        /// Verifies XboxGamepadState Equals with different states.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxInputTestCategory)]
        public void TestXboxGamepadStateEqualsWithDifferent()
        {
            var gamepad1 = new XboxGamepadState()
            {
                Buttons = XboxGamepadButtons.A
            };

            var gamepad2 = new XboxGamepadState()
            {
                Buttons = XboxGamepadButtons.B
            };

            Assert.AreNotEqual(gamepad1, gamepad2, "Gamepads are equal");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design.")]
        private void VerifyGamepadThrowsFunc<T>(Action action, string exceptionMessage)
        {
            bool exceptionThrown = false;
            
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(T))
                {
                    exceptionThrown = true;
                }
            }

            Assert.IsTrue(exceptionThrown, exceptionMessage);
        }
    }
}
