//------------------------------------------------------------------------------
// <copyright file="TextEventArgsTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class for housing tests for the TextEventArgs class.
    /// </summary>
    [TestClass]
    public class TextEventArgsTests
    {
        private const string TextEventArgsTestCategory = "Infrastructure.TextEventArgs";

        private const string Source = "Some source";
        private const string Message = "Some message";

        /// <summary>
        /// Verifies that the constructor (string) sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(TextEventArgsTestCategory)]
        public void TestConstructorStringSetsPropertiesCorrectly()
        {
            TextEventArgs textEventArgs = new TextEventArgs(Message);
            Assert.AreEqual(Message, textEventArgs.Message, "The Message property was not set correctly.");
            Assert.IsNull(textEventArgs.Source, "The Source property was not set correctly (should be set to null).");
        }

        /// <summary>
        /// Verifies that the constructor (string, string) sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(TextEventArgsTestCategory)]
        public void TestConstructorStringStringSetsPropertiesCorrectly()
        {
            TextEventArgs textEventArgs = new TextEventArgs(Source, Message);
            Assert.AreEqual(Message, textEventArgs.Message, "The Message property was not set correctly.");
            Assert.AreEqual(Source, textEventArgs.Source, "The Source property was not set correctly.");
        }
    }
}
