//------------------------------------------------------------------------------
// <copyright file="XboxPathTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class to contain the tests for the XboxPath object.
    /// </summary>
    [TestClass]
    public class XboxPathTests
    {
        private const string XboxPathTestCategory = "Infrastructure.XboxPath";

        private IEnumerable<TestInputData<string, bool>> PathsToValidate
        {
            get
            {
                yield return new TestInputData<string, bool>(@"xd:<>\test", false);
                yield return new TestInputData<string, bool>(@"test", true);
                yield return new TestInputData<string, bool>(@"..\test", true);
                yield return new TestInputData<string, bool>(@"..\test\foo.txt", true);
                foreach (TestInputData<string, bool> pathToValidate in this.PathsForOriginCheck)
                {
                    yield return pathToValidate;
                }
            }
        }

        private IEnumerable<TestInputData<string, bool>> PathsForOriginCheck
        {
            get
            {
                yield return new TestInputData<string, bool>(@"c:\test", false);
                yield return new TestInputData<string, bool>(@"xd:\test", true);
                yield return new TestInputData<string, bool>(@"xd:", true);
                yield return new TestInputData<string, bool>(@"xd:\", true);
                yield return new TestInputData<string, bool>(@"xd:\directory", true);
                yield return new TestInputData<string, bool>(@"Xd:\directory", true);
                yield return new TestInputData<string, bool>(@"XD:\directory", true);
                yield return new TestInputData<string, bool>(@"xD:\directory", true);
                yield return new TestInputData<string, bool>(@"d:\", false);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:", true);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\", true);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\foo.txt", true);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\files\foo.txt", true);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe]:\", false);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe]:\foo.txt", false);
                yield return new TestInputData<string, bool>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe]:\files\foo.txt", false);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < 300; ++i)
                {
                    builder.Append("a");
                }

                yield return new TestInputData<string, bool>(builder.ToString(), false);
            }
        }

        private IEnumerable<TestInputData<string, string>> PathsForGetDirectoryName
        {
            get
            {
                yield return new TestInputData<string, string>(@"xd:\parentDirectory\directory", @"xd:\parentDirectory");
                yield return new TestInputData<string, string>(@"xd:\parentDirectory\directory\", @"xd:\parentDirectory\directory");
                yield return new TestInputData<string, string>(@"xd:\parentDirectory", @"xd:\");
                yield return new TestInputData<string, string>(@"xd:\parentDirectory\file.txt", @"xd:\parentDirectory");
                yield return new TestInputData<string, string>(@"xd:\", @"xd:\");
                yield return new TestInputData<string, string>(@"xd:", null);
                yield return new TestInputData<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\", @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\");
                yield return new TestInputData<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\parentDirectory", @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\");
                yield return new TestInputData<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\parentDirectory\directory", @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\parentDirectory");
            }
        }

        private IEnumerable<TestInputData<Tuple<string, string>, string>> PathStringsForCombine
        {
            get
            {
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:\directory", "foo.txt"), @"xd:\directory\foo.txt");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:\directory\", "foo.txt"), @"xd:\directory\foo.txt");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:\directory\", "subdirectory"), @"xd:\directory\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:\directory\", @"..\subdirectory"), @"xd:\directory\..\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:\", "directory"), @"xd:\directory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"xd:", "directory"), @"xd:\directory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"c:\test", "directory"), @"xd:\doesntMatter", typeof(ArgumentException));

                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory", "foo.txt"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\foo.txt");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\", "foo.txt"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\foo.txt");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory", "subdirectory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\", "subdirectory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\", @"..\subdirectory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\..\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory", @"..\subdirectory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory\..\subdirectory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\", "directory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory");
                yield return new TestInputData<Tuple<string, string>, string>(new Tuple<string, string>(@"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:", "directory"), @"{VgbView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe}:\directory");
            }
        }

        /// <summary>
        /// Verifies that the IsValidPath method returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathIsValidPathMethod()
        {
            this.RunDataDrivenTest(
                this.PathsToValidate,
                path => XboxPath.IsValidPath(path));
        }

        /// <summary>
        /// Verifies that the IsValid property returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathIsValidProperty()
        {
            this.RunDataDrivenTest(
                this.PathsToValidate,
                path => new XboxPath(path, XboxOperatingSystem.System).IsValid);
        }

        /// <summary>
        /// Verifies that the HasXboxOrigin method returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathHasXboxOrigin()
        {
            this.RunDataDrivenTest(
                this.PathsForOriginCheck,
                path => XboxPath.HasXboxOrigin(path));
        }

        /// <summary>
        /// Verifies that the GetDirectoryName method returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathGetDirectoryName()
        {
            this.RunDataDrivenTest(
                this.PathsForGetDirectoryName,
                path => XboxPath.GetDirectoryName(path));
        }

        /// <summary>
        /// Verifies that the Combine(string, string) method returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathCombineStrings()
        {
            this.RunDataDrivenTest(
                this.PathStringsForCombine,
                (input) => XboxPath.Combine(input.Item1, input.Item2));
        }

        /// <summary>
        /// Verifies that the Combine(XboxPath, XboxPath) method returns the correct
        /// value for given input path values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestXboxPathCombineXboxPathObjects()
        {
            this.RunDataDrivenTest(
                this.PathStringsForCombine.Select(
                    pathStrings => new TestInputData<Tuple<XboxPath, XboxPath>, XboxPath>(new Tuple<XboxPath, XboxPath>(new XboxPath(pathStrings.Item1.Item1, XboxOperatingSystem.System), new XboxPath(pathStrings.Item1.Item2, XboxOperatingSystem.System)), new XboxPath(pathStrings.Item2, XboxOperatingSystem.System), pathStrings.ExpectedException)),
                (input) => XboxPath.Combine(input.Item1, input.Item2),
                (expected, actual) => expected.FullName == actual.FullName && expected.OperatingSystem == actual.OperatingSystem);
        }

        /// <summary>
        /// Verifies that the XboxPath constructor
        /// throws an ArgumentNullException if a null value is given
        /// for the filePath argument.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsArgumentNullException()
        {
#pragma warning disable 168
            var notUsed = new XboxPath(null, XboxOperatingSystem.System);
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the constructor for the XboxPath
        /// initializes its properties correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        public void TestConstructorInitializesProperties()
        {
            const string FakeFilePath = "ContentNotImportant";
            const XboxOperatingSystem OperatingSystem = XboxOperatingSystem.System;
            XboxPath path = new XboxPath(FakeFilePath, OperatingSystem);

            Assert.AreEqual(FakeFilePath, path.FullName, "The constructor for the XboxPath class did not initialize the FilePath property correctly.");
            Assert.AreEqual(OperatingSystem, path.OperatingSystem, "The constructor for the XboxPath class did not initialize the OperatingSystem property correctly.");
        }

        /// <summary>
        /// Verifies that the HasXboxOrigin method throws an ArgumentNullException
        /// if given a null value for the "path" parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPathTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxPathHasXboxOriginNullArgument()
        {
            XboxPath.HasXboxOrigin(null);
        }

        private void RunDataDrivenTest<TInput, TExpectedValue>(IEnumerable<TestInputData<TInput, TExpectedValue>> inputValues, Func<TInput, TExpectedValue> testMethod, Func<TExpectedValue, TExpectedValue, bool> actualValueComparison = null)
        {
            List<TInput> failedCases = new List<TInput>();
            foreach (TestInputData<TInput, TExpectedValue> input in inputValues)
            {
                bool testPassed = true;
                TExpectedValue expectedValue = input.Item2;
                TExpectedValue actualValue = default(TExpectedValue);
                try
                {
                    actualValue = testMethod(input.Item1);

                    if (input.ExpectedException != null)
                    {
                        testPassed = false;
                        failedCases.Add(input.Item1);
                    }
                    else
                    {
                        if (actualValueComparison != null)
                        {
                            testPassed = actualValueComparison(expectedValue, actualValue);
                        }
                        else
                        {
                            testPassed = object.Equals(actualValue, expectedValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    testPassed = input.ExpectedException != null && e.GetType() == input.ExpectedException;
                }

                if (!testPassed)
                {
                    failedCases.Add(input.Item1);
                }
            }

            Assert.IsFalse(failedCases.Any(), "The following inputs failed:\n{0}", string.Join("\n", failedCases));
        }

        private class TestInputData<T1, T2> : Tuple<T1, T2>
        {
            public TestInputData(T1 item1, T2 item2)
                : base(item1, item2)
            {
            }

            public TestInputData(T1 item1, T2 item2, Type expectedException)
                : this(item1, item2)
            {
                this.ExpectedException = expectedException;
            }

            public Type ExpectedException { get; private set; }
        }
    }
}
