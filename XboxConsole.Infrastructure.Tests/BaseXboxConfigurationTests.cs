//------------------------------------------------------------------------------
// <copyright file="BaseXboxConfigurationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System.Reflection;
    using Microsoft.Internal.GamesTest.Xbox.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class for housing tests for the BaseXboxConfiguration class.
    /// </summary>
    [TestClass]
    public class BaseXboxConfigurationTests
    {
        private const string BaseXboxConfigurationTestCategory = "Infrastructure.BaseXboxConfiguration";

        private const string SandboxIdKey = "SandboxId";

        /// <summary>
        /// Verifies that the constructor sets the properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(BaseXboxConfigurationTestCategory)]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            BaseXboxConfiguration configuration = new BaseXboxConfiguration();

            this.TestSetting<string>(configuration.SandboxIdSetting, SandboxIdKey);
        }

        private void TestSetting<T>(XboxConfigurationSetting<T> setting, string key)
        {
            Assert.IsNotNull(setting, "The setting {0} is null.", key);
            Assert.AreEqual(setting.Key, key, "The setting has key {1} while the key {0} is expected.", key, setting.Key);
            Assert.IsNull(setting.StringValue, "The StringValue property of setting {0} is not set to null.");
            Assert.IsNotNull(setting.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance), "The non-public instance Value property of setting {0} does not exist.", key);
            Assert.AreEqual(setting.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance).PropertyType, typeof(T), "The Value property of setting {0} is not an instance of {1} type.", key, typeof(T).Name);
            Assert.AreEqual(setting.Value, default(T), "The Value property of setting {0} is not set to the default value.");
        }
    }
}
