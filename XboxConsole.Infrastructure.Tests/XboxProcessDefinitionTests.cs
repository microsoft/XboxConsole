//------------------------------------------------------------------------------
// <copyright file="XboxProcessDefinitionTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class for housing tests for the XboxProcessDefinition class.
    /// </summary>
    [TestClass]
    public class XboxProcessDefinitionTests
    {
        private const string ProcessDefinitionTestCategory = "Infrastructure.XboxProcessDefinition";

        private const XboxOperatingSystem OperatingSystem = XboxOperatingSystem.Title;
        private const uint ProcessId = 123;
        private const string ImageFileName = "process.exe";

        /// <summary>
        /// Verifies that the constructor sets the ImageFileName property to an empty string if the imageFileName parameter passed in is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ProcessDefinitionTestCategory)]
        public void TestConstructorSetsImageFileNamePropertyToEmptyWhenParameterIsNull()
        {
            XboxProcessDefinition process = new XboxProcessDefinition(OperatingSystem, ProcessId, null);
            Assert.AreEqual(string.Empty, process.ImageFileName, "The constructor did not set the ImageFileName to string.Empty as expected.");
        }

        /// <summary>
        /// Verifies that the constructor sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ProcessDefinitionTestCategory)]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            XboxProcessDefinition process = new XboxProcessDefinition(OperatingSystem, ProcessId, ImageFileName);

            Assert.AreEqual(OperatingSystem, process.OperatingSystem, "The OperatingSystem property was not set correctly.");
            Assert.AreEqual(ProcessId, process.ProcessId, "The ProcessId property was not set correctly.");
            Assert.AreEqual(ImageFileName, process.ImageFileName, "The ImageFileName property was not set correctly.");
        }
    }
}
