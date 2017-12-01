//------------------------------------------------------------------------------
// <copyright file="ExceptionFactoryTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Internal.GamesTest.Xbox.Exceptions.Tests
{
    /// <summary>
    /// Represents tests for the ExceptionFactory class.
    /// </summary>
    [TestClass]
    public class ExceptionFactoryTests
    {
        private const string ExceptionFactoryTestCategory = "Infrastructure.ExceptionFactory";

        private const int DataUnavailableErrorCode = -2147483638;
        private const int OperationAbortedErrorCode = -2147467260;
        private const int ElementNotFoundErrorCode = -2147023728;

        private const string ExceptionMessage = "Exception Test Message";
        private const string ConsoleName = "10.124.151.44";

        private COMException missingDataException;
        private COMException operationAbortedException;
        private COMException elementNotFoundException;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "We're doing this on purpose to simulate exceptoins thrown by the XDK."), TestInitialize]
        public void TestInitialize()
        {
            this.missingDataException = new COMException(string.Empty, DataUnavailableErrorCode);
            this.operationAbortedException = new COMException(string.Empty, OperationAbortedErrorCode);
            this.elementNotFoundException = new COMException(string.Empty, ElementNotFoundErrorCode);
        }

        /// <summary>
        /// Verifies that a COMException representing a missing data exception is properly converted to an XboxConsoleException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ExceptionFactoryTestCategory)]
        public void TestMissingDataException()
        {
            this.ExceptionHelper<XboxConsoleException>(this.missingDataException, ConsoleName, null);
        }

        /// <summary>
        /// Verifies that a COMException representing an Operation Aborted exception is properly converted to an XboxConsoleException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ExceptionFactoryTestCategory)]
        public void TestOperationAbortedException()
        {
            this.ExceptionHelper<XboxConsoleException>(this.operationAbortedException, ConsoleName, null);
        }

        /// <summary>
        /// Verifies that a COMException representing an Element Not Found exception is properly converted to an XboxConsoleException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ExceptionFactoryTestCategory)]
        public void TestElementNotFoundException()
        {
            this.ExceptionHelper<XboxConsoleException>(this.elementNotFoundException, ConsoleName, null);
        }

        /// <summary>
        /// Verifies that a COMException representing an Operation Aborted exception is properly converted to an XboxInputException.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ExceptionFactoryTestCategory)]
        public void TestOperationAbortedGamepadException()
        {
            this.ExceptionHelper<XboxInputException>(this.operationAbortedException, ConsoleName, 0);
        }

        private void ExceptionHelper<T>(COMException xtfException, string consoleIpAddress, ulong? gamepadId)
        {
            Exception createdException = null;
            if (string.IsNullOrEmpty(consoleIpAddress))
            {
                createdException = ExceptionFactory.Create(ExceptionMessage, xtfException);
            }
            else if (!gamepadId.HasValue)
            {
                createdException = ExceptionFactory.Create(ExceptionMessage, xtfException, consoleIpAddress);
            }
            else
            {
                createdException = ExceptionFactory.Create(ExceptionMessage, xtfException, consoleIpAddress, gamepadId.Value);
            }

            Assert.IsNotNull(createdException);
            Assert.IsInstanceOfType(createdException, typeof(XboxConsoleException));
            Assert.IsTrue(createdException.Message.StartsWith(ExceptionMessage, StringComparison.Ordinal));
            Assert.AreEqual(xtfException, createdException.InnerException);

            var consoleException = createdException as XboxConsoleException;
            if (consoleIpAddress != null)
            {
                Assert.AreEqual(consoleIpAddress, consoleException.XboxName, "XboxName are not equal for XboxConsoleException");
            }

            var gamepadException = createdException as XboxInputException;
            if (gamepadException != null && gamepadId.HasValue)
            {
                Assert.AreEqual(gamepadId.Value, gamepadException.XboxGamepadId, "GamepadId are not equal for XboxInputException");
            }
        }
    }
}
