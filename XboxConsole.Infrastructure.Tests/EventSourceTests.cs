//------------------------------------------------------------------------------
// <copyright file="EventSourceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the XboxConsoleEventSource.
    /// </summary>
    [TestClass]
    public class EventSourceTests
    {
        /// <summary>
        /// Verifies that the XboxConsoleEventSource is marked up correctly.
        /// </summary>
        [TestMethod]
        public void EventSourceVerificationTest()
        {
            EventSourceAnalyzer.InspectAll(XboxConsoleEventSource.Logger);
        }
    }
}
