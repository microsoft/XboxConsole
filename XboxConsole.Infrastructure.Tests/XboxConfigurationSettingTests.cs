//------------------------------------------------------------------------------
// <copyright file="XboxConfigurationSettingTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using Microsoft.Internal.GamesTest.Xbox.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class for housing tests for the XboxConfigurationSetting(T) class.
    /// </summary>
    [TestClass]
    public class XboxConfigurationSettingTests
    {
        private const string XboxConfigurationSettingTestCategory = "Infrastructure.XboxConfigurationSetting";

        private const string CorrectKey = "SettingKey";
        private const string CorrectStringValue = "123";
        private const string CorrectRefTypeValue = "123";
        private const int CorrectValTypeValue = 123;

        /// <summary>
        /// Verifies that the constructor sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConfigurationSettingTestCategory)]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            XboxConfigurationSetting<int> setting = new XboxConfigurationSetting<int>(CorrectKey);

            Assert.AreEqual(CorrectKey, setting.Key, "The Key property was not set correctly.");
            Assert.IsNull(setting.StringValue, "The StringValue property was not set to null.");
            Assert.AreEqual(default(int), setting.Value, "The Value property was not set to the default value.");
        }

        /// <summary>
        /// Verifies that the StringValue property sets the StringValue and Value properties correctly when Value is of reference type (T).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConfigurationSettingTestCategory)]
        public void TestStringValuePropertyRefType()
        {
            this.TestStringValueProperty<string>(CorrectStringValue, CorrectRefTypeValue);
            this.TestStringValueProperty<string>(null, null);
        }

        /// <summary>
        /// Verifies that the StringValue property sets the StringValue and Value properties correctly when Value is of value type (T).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConfigurationSettingTestCategory)]
        public void TestStringValuePropertyValueType()
        {
            this.TestStringValueProperty<int>(CorrectStringValue, CorrectValTypeValue);
            this.TestStringValueProperty<int>(null, default(int));
        }

        /// <summary>
        /// Verifies that the Value property sets the Value and StringValue properties correctly when Value is of reference type (T).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConfigurationSettingTestCategory)]
        public void TestValuePropertyRefType()
        {
            this.TestValueProperty<string>(CorrectStringValue, CorrectRefTypeValue);
            this.TestValueProperty<string>(null, null);
        }

        /// <summary>
        /// Verifies that the Value property sets the Value and StringValue properties correctly when Value is of value type (T).
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxConfigurationSettingTestCategory)]
        public void TestValuePropertyValueType()
        {
            this.TestValueProperty<int>(CorrectStringValue, CorrectValTypeValue);
            this.TestValueProperty<int>(default(int).ToString(), default(int));
        }

        private void TestStringValueProperty<T>(string correctStringValue, T correctValue)
        {
            XboxConfigurationSetting<T> setting = new XboxConfigurationSetting<T>(CorrectKey);
            setting.StringValue = correctStringValue;

            Assert.AreEqual(correctStringValue, setting.StringValue, "The StringValue property was not set correctly.");
            Assert.AreEqual(correctValue, setting.Value, "The Value property was not set correctly.");
        }

        private void TestValueProperty<T>(string correctStringValue, T correctValue)
        {
            XboxConfigurationSetting<T> setting = new XboxConfigurationSetting<T>(CorrectKey);
            setting.Value = correctValue;

            Assert.AreEqual(correctValue, setting.Value, "The Value property was not set correctly.");
            Assert.AreEqual(correctStringValue, setting.StringValue, "The StringValue property was not set correctly.");
        }
    }
}
