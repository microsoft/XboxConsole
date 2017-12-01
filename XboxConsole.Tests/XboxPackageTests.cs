//------------------------------------------------------------------------------
// <copyright file="XboxPackageTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Microsoft.Internal.GamesTest.Xbox.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class housing tests for the XboxPackage class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is a test class and the objects are disposed in the TestCleanup method.")]
    [TestClass]
    public class XboxPackageTests
    {
        private const string XboxPackageTestCategory = "XboxConsole.XboxPackage";

        private const string ConsoleAddress = "10.124.151.244"; // The actual IP address used here is irrelevant.

        private const string PackageFamilyName = "NuiView.ERA_8wekyb3d8bbwe";
        private const string PackageFullName = "NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe";
        private const string ApplicationId = "NuiView.ERA";
        private const string Aumid = PackageFamilyName + "!" + ApplicationId;
        private static readonly string[] Aumids = { Aumid };

        private readonly TimeSpan timeToWaitForPolling = TimeSpan.FromSeconds(10);

        private XboxPackage xboxPackage;
        private XboxPackageDefinition packageDefinition;

        private IDisposable shimsContext;
        private XboxConsole xboxConsole;

        /// <summary>
        /// Called once before each test to setup shim and stub objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.shimsContext = ShimsContext.Create();

            this.packageDefinition = new XboxPackageDefinition(PackageFullName, PackageFamilyName, Aumids);

            ShimXboxConsole.ConstructorIPAddress = (console, address) =>
                {
                    var myShim = new ShimXboxConsole(console)
                        {
                            AdapterGet = () => new StubXboxConsoleAdapterBase(null),
                            SystemIpAddressGet = () => IPAddress.Parse(XboxPackageTests.ConsoleAddress),
                            XboxGamepadsGet = () => new List<GamesTest.Xbox.Input.XboxGamepad>()
                        };
                };

            ShimXboxConsoleAdapterBase.ConstructorXboxXdkBase = (adapter, xboxXdk) =>
                {
                };

            this.xboxConsole = new XboxConsole((IPAddress)null);
            this.xboxPackage = new XboxPackage(this.packageDefinition, this.xboxConsole);
        }

        /// <summary>
        /// Called once after each test to clean up shim and stub objects.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.xboxConsole.Dispose();
            this.shimsContext.Dispose();
        }

        /// <summary>
        /// Verifies that the ExecutionState property calls the adapter's QueryPackageExecutionState method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStateCallsAdapterQueryPackageExecutionState()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
                return 0;
            };

            var notUsed = this.xboxPackage.ExecutionState;
            Assert.IsTrue(isCorrectMethodCalled, "The ExecutionState property didn't call the adapter's QueryPackageExecutionState(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Suspend method calls the adapter's SuspendPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestSuspendCallsAdapterSuspendPackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.SuspendPackageStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Suspend();
            Assert.IsTrue(isCorrectMethodCalled, "The Suspend() method didn't call the adapter's SuspendPackage(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Resume method calls the adapter's ResumePackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestResumeCallsAdapterResumePackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.ResumePackageStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Resume();
            Assert.IsTrue(isCorrectMethodCalled, "The Resume() method didn't call the adapter's ResumePackage(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Terminate method calls the adapter's TerminatePackage method and passes the correct
        /// AUMID value as the parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestTerminateCallsAdapterTerminatePackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.TerminatePackageStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Terminate();
            Assert.IsTrue(isCorrectMethodCalled, "The Terminate() method didn't call the adapter's TerminatePackage(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Constrain method calls the adapter's ConstrainPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestConstrainCallsAdapterConstrainPackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.ConstrainPackageStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Constrain();
            Assert.IsTrue(isCorrectMethodCalled, "The Constrain() method didn't call the adapter's ConstrainPackage(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Unconstrain method calls the adapter's UnconstrainPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestUnconstrainCallsAdapterUnconstrainPackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.UnconstrainPackageStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Unconstrain();
            Assert.IsTrue(isCorrectMethodCalled, "The Unconstrain() method didn't call the adapter's UnconstrainPackage(systemIpAddress, package).");
        }

        /// <summary>
        /// Verifies that the Unsnap method calls the adapter's UnsnapApplication method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestUnsnapCallsAdapterUnsnapApplication()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.UnsnapApplicationString = (adapter, systemIpAddress) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Unsnap();
            Assert.IsTrue(isCorrectMethodCalled, "The Unsnap() method didn't call the adapter's UnsnapApplication(systemIpAddress).");
        }

        /// <summary>
        /// Verifies that the Uninstall method calls the adapter's UninstallPackage method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestUninstallCallsAdapterUninstallPackage()
        {
            bool isCorrectMethodCalled = false;
            ShimXboxConsoleAdapterBase.AllInstances.UninstallPackageStringXboxPackageDefinition = (adapter, systemIpAddress, packageDefinition) =>
            {
                isCorrectMethodCalled = true;
            };

            this.xboxPackage.Uninstall();
            Assert.IsTrue(isCorrectMethodCalled, "The Uninstall() method didn't call the adapter's UninstallPackage(systemIpAddress).");
        }

        /// <summary>
        /// Verifies that the XboxPackage class will throw an ArgumentNullException
        /// if given a null value for the packageDefinition parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxPackageThrowsArgumentNullExceptionWithNullPackageDefinition()
        {
#pragma warning disable 168
            XboxPackage notUsed = new XboxPackage(null, this.xboxConsole);
            Assert.IsNotNull(notUsed, "XboxPackage constructor did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that the XboxPackage class will throw an ArgumentNullException
        /// if given a null value for the console parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXboxPackageThrowsArgumentNullExceptionWithNullConsole()
        {
#pragma warning disable 168
            XboxPackage notUsed = new XboxPackage(this.packageDefinition, null);
            Assert.IsNotNull(notUsed, "XboxPackage constructor did not throw an ArgumentNullException as expected.");
#pragma warning restore 168
        }

        /// <summary>
        /// Verifies that subscribing and unsubscribing to ExecutionStateChanged starts and stops polling the execution state.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStatePollingStartsAndStops()
        {
            EventHandler<PackageExecutionStateChangedEventArgs> handler = (o, e) =>
            {
                // Nothing needed here
            };

            this.TestExecutionStatePolling(
                () =>
                {
                    this.xboxPackage.ExecutionStateChanged += handler;
                },
                () =>
                {
                    this.xboxPackage.ExecutionStateChanged -= handler;
                });
        }

        /// <summary>
        /// Verifies that ExecutionStateChanged polling ends with multiple subscribers.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStatePollingStartsAndStopsMultipleHandlers()
        {
            EventHandler<PackageExecutionStateChangedEventArgs> handler1 = (o, e) =>
            {
                // Nothing needed here.
            };
            EventHandler<PackageExecutionStateChangedEventArgs> handler2 = (o, e) =>
            {
                // Nothing needed here.
            }; 
            EventHandler<PackageExecutionStateChangedEventArgs> handler3 = (o, e) =>
            {
                // Nothing needed here.
            };

            this.TestExecutionStatePolling(
                () =>
                {
                    this.xboxPackage.ExecutionStateChanged += handler1;
                    this.xboxPackage.ExecutionStateChanged += handler2;
                    this.xboxPackage.ExecutionStateChanged += handler3;
                },
                () =>
                {
                    this.xboxPackage.ExecutionStateChanged -= handler1;
                    this.xboxPackage.ExecutionStateChanged -= handler2;
                    this.xboxPackage.ExecutionStateChanged -= handler3;
                });
        }

        /// <summary>
        /// Verifies that ExecutionStateChanged is only raised when the state changes.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStateEventFiresOnNewState()
        {
            PackageExecutionState state = PackageExecutionState.Constrained;
            int numberOfTimesStateChanged = 0;
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                switch (state)
                {
                    case PackageExecutionState.Constrained:
                        ++numberOfTimesStateChanged;
                        state = PackageExecutionState.Running;
                        break;
                    case PackageExecutionState.Running:
                        ++numberOfTimesStateChanged;
                        state = PackageExecutionState.Suspended;
                        break;
                    case PackageExecutionState.Suspended:
                        ++numberOfTimesStateChanged;
                        state = PackageExecutionState.Suspending;
                        break;
                    case PackageExecutionState.Suspending:
                        ++numberOfTimesStateChanged;
                        state = PackageExecutionState.Terminated;
                        break;
                    case PackageExecutionState.Terminated:
                        ++numberOfTimesStateChanged;
                        state = PackageExecutionState.Unknown;
                        break;
                    case PackageExecutionState.Unknown:
                    default:
                        break;
                }

                return state;
            };

            int numberOfTimesHandlerEntered = 0;
            EventHandler<PackageExecutionStateChangedEventArgs> handler = (o, e) =>
            {
                ++numberOfTimesHandlerEntered;
            };

            this.xboxPackage.ExecutionStateChanged += handler;

            Thread.Sleep(this.timeToWaitForPolling);
            Assert.AreEqual(numberOfTimesStateChanged, numberOfTimesHandlerEntered, "The handler was not called for each time that the state changed.");
        }

        /// <summary>
        /// Verifies that ExecutionStateChanged calls multiple event handlers.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStateEventMultipleHandlers()
        {
            PackageExecutionState state = PackageExecutionState.Constrained;
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                if (state == PackageExecutionState.Constrained)
                {
                    state = PackageExecutionState.Running;
                }
                else
                {
                    state = PackageExecutionState.Constrained;
                }

                return state;
            };

            bool handler1Entered = false;
            bool handler2Entered = false;

            EventHandler<PackageExecutionStateChangedEventArgs> handler1 = (o, e) =>
            {
                handler1Entered = true;
            };
            EventHandler<PackageExecutionStateChangedEventArgs> handler2 = (o, e) =>
            {
                handler2Entered = true;
            };

            this.xboxPackage.ExecutionStateChanged += handler1;
            this.xboxPackage.ExecutionStateChanged += handler2;

            Thread.Sleep(this.timeToWaitForPolling);

            // Ensuring multiple handlers are called.
            Assert.IsTrue(handler1Entered, "The handler was not called after subscribing.");
            Assert.IsTrue(handler2Entered, "The handler was not called after subscribing.");

            this.xboxPackage.ExecutionStateChanged -= handler2;

            handler1Entered = false;
            handler2Entered = false;
            Thread.Sleep(this.timeToWaitForPolling);

            // Ensuring that after a handler unsubscribes, the other handlers still get called,
            // and after unsubscribing, that handler doesn't get called.
            Assert.IsTrue(handler1Entered, "The handler was not called after another handler unsubscribed.");
            Assert.IsFalse(handler2Entered, "The handler was called after subscribing.");            
        }

        /// <summary>
        /// Verifies that ExecutionStateChanged polling continues after a CannotConnectExceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStateEventHandlesConnectionException()
        {
            bool firstQueryFinished = false;
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                if (!firstQueryFinished)
                {
                    firstQueryFinished = true;
                    throw new CannotConnectException();
                }
                else
                {
                    return PackageExecutionState.Constrained;
                }
            };

            bool handlerEntered = false;
            EventHandler<PackageExecutionStateChangedEventArgs> handler = (o, e) =>
            {
                handlerEntered = true;
            };

            this.xboxPackage.ExecutionStateChanged += handler;

            Thread.Sleep(this.timeToWaitForPolling);
            Assert.IsTrue(handlerEntered, "The handler was not called after the exception was thrown.");
        }

        /// <summary>
        /// Verifies that ExecutionStateChanged notifies about exceptions.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(XboxPackageTestCategory)]
        public void TestExecutionStateEventExceptionNotification()
        {
            XboxException queryException = new XboxException();
            bool firstQueryFinished = false;
            bool pollingContinued = false;
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                if (!firstQueryFinished)
                {
                    firstQueryFinished = true;
                    throw queryException;
                }
                else
                {
                    pollingContinued = true;
                }

                return PackageExecutionState.Constrained;
            };

            bool handlerWasCalled = false;
            PackageExecutionState newState = PackageExecutionState.Terminated;
            Exception eventException = null;
           
            EventHandler<PackageExecutionStateChangedEventArgs> handler = (o, e) =>
            {
                handlerWasCalled = true;
                newState = e.NewState;
                eventException = e.Exception;
            };

            this.xboxPackage.ExecutionStateChanged += handler;

            Thread.Sleep(this.timeToWaitForPolling);

            Assert.IsFalse(pollingContinued, "The polling continued after the exception.");
            Assert.IsTrue(handlerWasCalled, "The handler was not called after the exception.");
            Assert.AreEqual(PackageExecutionState.Unknown, newState, "The EventArgs didn't have the expected new package state.");
            Assert.IsNotNull(eventException, "The EventArgs didn't have the expected exception");
            Assert.AreSame(queryException, eventException, "The EventArgs didn't have the expected exception");
        }

        private void TestExecutionStatePolling(Action subscriptionAction, Action unsubscriptionAction)
        {
            ManualResetEventSlim hasMethodBeenCalled = new ManualResetEventSlim(false);
            ShimXboxConsoleAdapterBase.AllInstances.QueryPackageExecutionStateStringXboxPackageDefinition = (adapter, systemIpAddress, package) =>
            {
                hasMethodBeenCalled.Set();
                return 0;
            };

            subscriptionAction();

            if (!hasMethodBeenCalled.Wait(this.timeToWaitForPolling))
            {
                Assert.Fail("Subscribing to the ExecutionStateChanged event never caused the adapter's QueryPackageExecutionState(systemIpAddress, package) to be called.");
            }

            // Verify that it is called a second time to ensure that it is actually polled.
            hasMethodBeenCalled.Reset();
            if (!hasMethodBeenCalled.Wait(this.timeToWaitForPolling))
            {
                Assert.Fail("Subscribing to the ExecutionStateChanged event only caused the adapter's QueryPackageExecutionState(systemIpAddress, package) to be called once.");
            }

            unsubscriptionAction();

            hasMethodBeenCalled.Reset();
            if (hasMethodBeenCalled.Wait(this.timeToWaitForPolling))
            {
                Assert.Fail("Unsubscribing to the ExecutionStateChanged event still caused the adapter's QueryPackageExecutionState(systemIpAddress, package) to be called.");
            }
        }
    }
}
