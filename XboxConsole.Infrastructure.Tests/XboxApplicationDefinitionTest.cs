//------------------------------------------------------------------------------
// <copyright file="XboxApplicationDefinitionTest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class for housing tests for the XboxPackageDefinition class.
    /// </summary>
    [TestClass]
    public class XboxApplicationDefinitionTest
    {
        private const string ApplicationDefinitionTestCategory = "Infrastructure.XboxApplicationDefinition";

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the AMUID is empty.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ApplicationDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithEmptyAumid()
        {
            XboxApplicationDefinition package = new XboxApplicationDefinition(string.Empty);
        }

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the AMUID is empty.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ApplicationDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorThrowsWithImproperlyFormattedAumid()
        {
            XboxApplicationDefinition package = new XboxApplicationDefinition("WrongFormat");
        }

        /// <summary>
        /// Verifies that the constructor sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(ApplicationDefinitionTestCategory)]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            XboxApplicationDefinition applicationDefinition = new XboxApplicationDefinition(Aumid);
            Assert.AreEqual(ApplicationId, applicationDefinition.ApplicationId, "The ApplicationId property was not set correctly.");
            Assert.AreEqual(PackageFamilyName, applicationDefinition.PackageFamilyName, "The PackageFamilyName property was not set correctly.");
            Assert.AreEqual(Aumid, applicationDefinition.Aumid, "The Aumids property was not set correctly.");
        }
    }
}
