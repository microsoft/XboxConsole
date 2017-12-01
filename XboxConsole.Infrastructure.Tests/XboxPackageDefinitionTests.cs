//------------------------------------------------------------------------------
// <copyright file="XboxPackageDefinitionTests.cs" company="Microsoft">
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
    public class XboxPackageDefinitionTests
    {
        private const string PackageDefinitionTestCategory = "Infrastructure.XboxPackageDefinition";

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;
        private static readonly string[] Aumids = { Aumid };

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the Package Full Name is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithNullPackageFullName()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(null, PackageFamilyName, Aumids);
        }

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the Package Family Name is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithNullPackageFamilyName()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, null, Aumids);
        }

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if Aumids is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithNullApplications()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, null);
        }

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the Package Full Name is empty.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithEmptyPackageFullName()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(string.Empty, PackageFamilyName, Aumids);
        }

        /// <summary>
        /// Verifies that the constructor throws an ArgumentNullException if the Package Family Name is empty.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsWithEmptyPackageFamilyName()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, string.Empty, Aumids);
        }

        /// <summary>
        /// Verifies that the constructor sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(PackageDefinitionTestCategory)]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            XboxPackageDefinition package = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);
            Assert.AreEqual(PackageFullName, package.FullName, "The FullName property was not set correctly.");
            Assert.AreEqual(PackageFamilyName, package.FamilyName, "The FamilyName property was not set correctly.");
            CollectionAssert.AreEquivalent(Aumids, package.ApplicationDefinitions.Select(x => x.Aumid).ToList(), "The Applications AumIds property was not set correctly.");
            CollectionAssert.AreEquivalent(new string[] { ApplicationId }, package.ApplicationDefinitions.Select(x => x.ApplicationId).ToList(), "The Applications ApplicationId property was not set correctly.");
        }
    }
}
