//------------------------------------------------------------------------------
// <copyright file="XboxUserTests.cs" company="Microsoft">
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
    /// Represents tests for the XboxUser type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is test code and the object is disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxUserTests
    {
        private const string XboxUserTestCategory = "XboxConsole.XboxUser";
        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private IDisposable shimsContext;
        private XboxConsole xboxConsole;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
            {
                var myShim = new ShimXboxConsole(console)
                {
                    AdapterGet = () => new StubXboxConsoleAdapterBase(null),
                    SystemIpAddressGet = () => IPAddress.Parse(XboxUserTests.ConsoleAddress),
                    SystemIpAddressStringGet = () => this.xboxConsole.SystemIpAddress.ToString(),
                    XboxGamepadsGet = () => new List<Input.XboxGamepad>(),
                };
            };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
            {
            };

            this.xboxConsole = new XboxConsole((IPAddress)null);
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
        /// Verifies that the PairWith methods call the adapter's PairGamepadToUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestPairWithMethodsCallAdapterPairGamepadToUser()
        {
            var gamepad = new XboxGamepad(this.xboxConsole);
            gamepad.Connect();

            XboxUser user = new XboxUser(this.xboxConsole, 0, null, null, false);

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                isCorrectMethodCalled = true;
            };

            user.PairWithPhysicalController(0);
            Assert.IsTrue(isCorrectMethodCalled, "The PairWithPhysicalController method didn't call the adapter's PairGamepadToUser(systemIpAddress, gamepadId, userId).");

            isCorrectMethodCalled = false;

            user.PairWithVirtualController(gamepad);
            Assert.IsTrue(isCorrectMethodCalled, "The PairWithVirtualController method didn't call the adapter's PairGamepadToUser(systemIpAddress, gamepadId, userId).");
        }

        /// <summary>
        /// Verifies that the PairWith methods call the adapter's PairGamepadToUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestPairWithMethodsPassOnIds()
        {
            const uint ExpectedUserId = 12345;
            const ulong ExpectedGamepadId = 9876543210;
            var gamepad = new XboxGamepad(this.xboxConsole);

            XboxUser user = new XboxUser(this.xboxConsole, ExpectedUserId, null, null, false);

            ShimXboxConsoleAdapterBase.AllInstances.ConnectXboxGamepadString = (_, __) => ExpectedGamepadId;

            gamepad.Connect();

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.AreEqual(ExpectedGamepadId, gamepadId, "The XboxUser did not pass in the expected id parameter");
                Assert.AreEqual(ExpectedUserId, userId, "The XboxUser did not pass in the expected id parameter");
            };

            user.PairWithPhysicalController(ExpectedGamepadId);

            user.PairWithVirtualController(gamepad);
        }

        /// <summary>
        /// Verifies that the PairWithVirtualController method throws an ArgumentNullException when a null gamepad is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPairWithVirtualControllerThrowsArgumentNullExceptions()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, null, null, false);

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.Fail("Test should never have gotten to the adapters method.");
            };

            user.PairWithVirtualController(null);
        }

        /// <summary>
        /// Verifies that the PairWithVirtualController method throws an ArgumentException when a gamepad with a null id is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPairWithVirtualControllerThrowsArgumentExceptions()
        {
            XboxGamepad gamepad = new XboxGamepad(this.xboxConsole);
            XboxUser user = new XboxUser(this.xboxConsole, 0, null, null, false);

            ShimXboxConsoleAdapterBase.AllInstances.PairGamepadToUserStringUInt64UInt32 = (adapter, systemIpAddress, gamepadId, userId) =>
            {
                Assert.Fail("Test should never have gotten to the adapters method.");
            };

            user.PairWithVirtualController(gamepad);
        }

        /// <summary>
        /// Verifies that the constructors of the XboxUser class correctly set the properties.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestConstructorsCorrectlySetProperties()
        {
            const uint ExpectedId = 12345;
            const string ExpectedEmail = "Test Email String";
            const string ExpectedGamerTag = "Test Gamertag";

            bool expectedIsSignedInValue = false;

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag));

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue));

            expectedIsSignedInValue = true;

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue));

            XboxUserDefinition definition = new XboxUserDefinition(ExpectedId, ExpectedEmail, ExpectedGamerTag, expectedIsSignedInValue);

            this.TestXboxUserProperties(
                ExpectedId,
                ExpectedEmail,
                ExpectedGamerTag,
                expectedIsSignedInValue,
                () => new XboxUser(this.xboxConsole, definition));
        }

        /// <summary>
        /// Verifies that the SignIn method calls the adapters SignInUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignInCallsAdapterSignIn()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, null, null, false);

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                isCorrectMethodCalled = true;
                return new XboxUserDefinition(0, null, null, false);
            };

            user.SignIn(null, false);
            Assert.IsTrue(isCorrectMethodCalled, "The SignIn method didn't call the adapter's SignInUser(systemIpAddress, userDefinition, password, storePassword).");
        }

        /// <summary>
        /// Verifies that the SignIn method passes on its parameters correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignInPassesArguments()
        {
            const uint ExpectedUserId = 54;
            const string ExpectedEmailAddress = "TestAddress@test.test";
            const string ExpectedGamerTag = "TestGamerTag";
            const string ExpectedPassword = "TestPassword";
            const bool ExpectedIsSignedInValue = true;

            bool expectedStorePassword = false;

            XboxUser user = new XboxUser(this.xboxConsole, ExpectedUserId, ExpectedEmailAddress, ExpectedGamerTag, !ExpectedIsSignedInValue);

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                Assert.AreSame(user.Definition, userDefinition, "The passed in XboxUserDefinition was not the same as the expected user definition.");
                Assert.AreEqual(ExpectedPassword, password, "The passed in password string was not the same as the expected password string.");
                Assert.AreEqual(expectedStorePassword, storePassword, "The passed in storePassword value was not the same as the expected storePassword value.");

                return new XboxUserDefinition(userDefinition.UserId, userDefinition.EmailAddress, userDefinition.Gamertag, ExpectedIsSignedInValue);
            };

            user.SignIn(ExpectedPassword, expectedStorePassword);
            Assert.AreEqual(ExpectedIsSignedInValue, user.IsSignedIn, "The SignOut method didn't set the users IsSignedIn property based on return value.");

            expectedStorePassword = true;
            user.SignIn(ExpectedPassword, expectedStorePassword);
        }

        /// <summary>
        /// Verifies that the SignIn method handles a null return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignInHandlesNullReturn()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, "TestEmailAddress", null, false);

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                return null;
            };

            user.SignIn(null, false);
        }

        /// <summary>
        /// Verifies that the SignIn method handles an incorrect return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignInHandlesIncorrectReturn()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, "TestEmailAddress", null, false);

            ShimXboxConsoleAdapterBase.AllInstances.SignInUserStringXboxUserDefinitionStringBoolean = (adapter, systemIpAddress, userDefinition, password, storePassword) =>
            {
                return new XboxUserDefinition(0, "DifferentEmail", null, false);
            };

            user.SignIn(null, false);
        }

        /// <summary>
        /// Verifies that the SignOut method calls the adapters SignOutUser method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignOutInvokesAdapterSignOut()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, null, null, false);

            bool isCorrectMethodCalled = false;

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                isCorrectMethodCalled = true;
                return new XboxUserDefinition(0, null, null, false);
            };

            user.SignOut();
            Assert.IsTrue(isCorrectMethodCalled, "The SignOut method didn't call the adapter's SignOutUser(systemIpAddress, userDefinition).");
        }

        /// <summary>
        /// Verifies that the SignOut method passes on its parameters correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        public void TestSignOutPassesArguments()
        {
            const uint ExpectedUserId = 54;
            const string ExpectedEmailAddress = "TestAddress@test.test";
            const string ExpectedGamerTag = "TestGamerTag";

            const bool ExpectedIsSignedInValue = false;

            XboxUser user = new XboxUser(this.xboxConsole, ExpectedUserId, ExpectedEmailAddress, ExpectedGamerTag, !ExpectedIsSignedInValue);

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                Assert.AreSame(user.Definition, userDefinition, "The passed in XboxUserDefinition was not the same as the expected user definition.");

                return new XboxUserDefinition(userDefinition.UserId, userDefinition.EmailAddress, userDefinition.Gamertag, ExpectedIsSignedInValue);
            };

            user.SignOut();
            Assert.AreEqual(ExpectedIsSignedInValue, user.IsSignedIn, "The SignOut method didn't set the users IsSignedIn property based on return value.");
        }

        /// <summary>
        /// Verifies that the SignOut method handles a null return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignOutHandlesNullReturn()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, "TestEmailAddress", null, false);

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                return null;
            };

            user.SignOut();
        }

        /// <summary>
        /// Verifies that the SignOut methods handles an incorrect return value.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxUserTestCategory)]
        [ExpectedException(typeof(XboxSignInException))]
        public void TestSignOutHandlesIncorrectReturn()
        {
            XboxUser user = new XboxUser(this.xboxConsole, 0, "TestEmailAddress", null, false);

            ShimXboxConsoleAdapterBase.AllInstances.SignOutUserStringXboxUserDefinition = (adapter, systemIpAddress, userDefinition) =>
            {
                return new XboxUserDefinition(0, "DifferentEmail", null, false);
            };

            user.SignOut();
        }

        private void TestXboxUserProperties(uint expectedId, string expectedEmail, string expectedGamerTag, bool expectedIsSignedInValue, Func<XboxUser> constructorFunc)
        {
            XboxUser user = constructorFunc();

            Assert.AreEqual(expectedId, user.UserId, "UserId Property did not match expected value");
            Assert.AreEqual(expectedEmail, user.EmailAddress, "EmailAddress Property did not match expected value");
            Assert.AreEqual(expectedGamerTag, user.GamerTag, "GamerTag Property did not match expected value");
            Assert.AreEqual(expectedIsSignedInValue, user.IsSignedIn, "IsSignedIn Property did not match expected value");
        }
    }
}
