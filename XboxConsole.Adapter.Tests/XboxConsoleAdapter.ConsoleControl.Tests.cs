//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.ConsoleControl.Tests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Net.NetworkInformation;
    using System.Net.NetworkInformation.Fakes;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents tests for the XboxConsole adapter types.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is a Test class and its purpose is to test all functionality of a particular class, regardless of the infrastructure needed for that.")]
    public partial class XboxConsoleAdapterTests
    {
        private const string AdapterConsoleControlTestCategory = "Adapter.ConsoleControl";

        private const string CaptureScreenshotZeroPointerExceptionMessage = "Failed to capture screenshot.";

        /// <summary>
        /// Verifies that if the XDK throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.RebootAction = s =>
                {
                    throw new COMException();
                };

            try
            {
                this.adapter.Reboot(ConsoleAddress, TimeSpan.FromSeconds(1.0));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole Reboot() method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole Reboot() method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method calls the XboxXdk's Reboot method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanCallsXboxXdkReboot()
        {
            this.TestReboot(TimeSpan.FromSeconds(60.0), 2, 0);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method completes successfully
        /// when passed a timeout value of TimeSpan.MaxValue.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanTimeSpanMax()
        {
            this.TestReboot(TimeSpan.MaxValue, 3, 3);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method completes successfully
        /// when passed a timeout value of Timeout.InfiniteTimeSpan.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanTimeSpanInfinite()
        {
            this.TestReboot(Timeout.InfiniteTimeSpan, 3, 3);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method succeeds
        /// when passed a timeout value of TimeSpan.MinValue.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanTimeSpanMin()
        {
            this.TestReboot(TimeSpan.MinValue, 2, 0);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method throws a TimeoutException
        /// when passed a timeout value of TimeSpan.Zero.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestRebootTimeSpanTimeSpanZero()
        {
            this.TestReboot(TimeSpan.Zero, 2, 0);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method throws a TimeoutException
        /// if the console does not become unresponsive within its timeout window.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestRebootTimeSpanThrowsIfConsoleDoesNotBecomeUnresponsiveInTime()
        {
            this.TestReboot(TimeSpan.FromSeconds(5.0), 10, 0);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method throws a TimeoutException
        /// if the console does not become responsive within its timeout window.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestRebootTimeSpanThrowsIfConsoleDoesNotBecomeResponsiveInTime()
        {
            this.TestReboot(TimeSpan.FromSeconds(5.0), 0, 10);
        }

        /// <summary>
        /// Verifies that Reboot(TimeSpan) succeeds even if the process of checking that the
        /// kit became unresponsive succeeds, but took a little too long and then the check for 
        /// responsiveness succeeded on the first try.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestRebootTimeSpanSucceedsEvenIfCheckingUnresponsiveTakesTooLongSoLongAsTheKitIsResponsiveOnTheFirstTry()
        {
            this.TestReboot(TimeSpan.FromSeconds(2.0), 0, 0, 2500);
        }

        /// <summary>
        /// Verifies that the Reboot(TimeSpan) method throws an exception if the process of checking that the kit
        /// became unresponsive takes a little too long and the first check for responsiveness returns false.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestRebootTimeSpanThrowsIfCheckingUnresponsiveTakesTooLongAndTheKitIsNotResponsiveAgainOnTheFirstTry()
        {
            this.TestReboot(TimeSpan.FromSeconds(2.0), 0, 1, 2500);
        }

        /// <summary>
        /// Verifies that if the XDK's Shutdown function throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.ShutdownAction = _ =>
            {
                throw new COMException();
            };

            TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
            try
            {
                this.adapter.Shutdown(ConsoleAddress, timeSpan);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole Shutdown(TimeSpan) method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole Shutdown(TimeSpan) method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method calls the XboxXdk's Shutdown method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanCallsXboxXdkShutdown()
        {
            this.TestShutdown(TimeSpan.FromSeconds(5.0), 0);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method succeeds
        /// when passed a timeout value of TimeSpan.MinValue.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanTimeSpanMin()
        {
            this.TestShutdown(TimeSpan.MinValue, 5);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method throws a TimeoutException
        /// when passed a timeout value of TimeSpan.Zero and the kit has not shutdown
        /// by the time it does its first check.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestShutdownTimeSpanTimeSpanZeroThrowsIfShutdownIsNotCompleteOnFirstCheck()
        {
            this.TestShutdown(TimeSpan.Zero, 5);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method succeeds
        /// when passed a timeout value of TimeSpan.Zero and the kit has 
        /// shutdown by the time it does its first check.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanTimeSpanZeroSucceedsIfShutdownIsCompleteOnFirstCheck()
        {
            this.TestShutdown(TimeSpan.Zero, 0);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method succeeds
        /// when passed a timeout value of TimeSpan.MaxValue.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanTimeSpanMax()
        {
            this.TestShutdown(TimeSpan.MaxValue, 2);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method succeeds
        /// when passed a timeout value of Timeout.InfiniteTimeSpan.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestShutdownTimeSpanTimeSpanInfinite()
        {
            this.TestShutdown(Timeout.InfiniteTimeSpan, 2);
        }

        /// <summary>
        /// Verifies that the Shutdown(TimeSpan) method throws a TimeoutException
        /// when the kit does not become unresponsive in the given time out window.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(TimeoutException))]
        public void TestShutdownTimeSpanThrowsTimeoutExceptionIfDoesNotBecomeUnresponsiveInTime()
        {
            this.TestShutdown(TimeSpan.FromSeconds(5.0), 10);
        }

        /// <summary>
        /// Verifies that if the XDK's CaptureScreenshot method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureScreenshotTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.CaptureScreenshotFunc = _ =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.CaptureScreenshot(ConsoleAddress);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole Adapter CaptureScreenshot method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole CaptureScreenshot method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the CaptureScreenshot()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestCaptureScreenshotThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.CaptureScreenshot(ConsoleAddress);
        }

        /// <summary>
        /// Verifies that the adapter's CaptureScreenshot() method
        /// calls the XboxXdk's CaptureScreenshot method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureScreenshotCallsXdkCaptureScreenshot()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.CaptureScreenshotFunc = ipAddress =>
            {
                isCorrectFunctionCalled = true;
                return IntPtr.Zero;
            };

            try
            {
                this.adapter.CaptureScreenshot(ConsoleAddress);
            }
            catch (XboxConsoleException)
            {
            }

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's CaptureScreenshot function failed to call the XboxXdk's CaptureScreenshot function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.CaptureScreenshot(null));
        }

        /// <summary>
        /// Verifies that the adapter throws an exception if the test it receives from the XDK is null.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureScreenshotThrowsWithZeroOutput()
        {
            this.fakeXboxXdk.CaptureScreenshotFunc = ipAddress => IntPtr.Zero;

            try
            {
                var bitmapPointer = this.adapter.CaptureScreenshot(ConsoleAddress);
                Assert.Fail("Null received from the XDK didn't throw an expected exception.");
            }
            catch (XboxConsoleException ex)
            {
                Assert.AreEqual(CaptureScreenshotZeroPointerExceptionMessage, ex.Message, "IntPtr.Zero received from the XDK didn't throw an expected exception.");
            }
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on a non-zero IntPtr.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureScreenshot()
        {
            var expectedIntPtr = new IntPtr(12345678);
            this.fakeXboxXdk.CaptureScreenshotFunc = ipAddress =>
            {
                return expectedIntPtr;
            };

            var actualIntPtr = this.adapter.CaptureScreenshot(ConsoleAddress);
            Assert.AreEqual(expectedIntPtr, actualIntPtr, "The returned value from CaptureScreenshot did not match expected value.");
        }

        /// <summary>
        /// Verifies that if the XDK's CaptureRecordedGameClip method throws a COMException, then the
        /// XboxConsoleAdapter will wrap that into an XboxConsoleException with the COMException as its inner exception.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to simulate XDK")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all exception types so that I can verify we capture the correct exception type.")]
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureRecordedGameClipTurnsComExceptionIntoXboxConsoleException()
        {
            this.fakeXboxXdk.CaptureRecordedGameClipAction = (ipAddress, filePath, captureSeconds) =>
            {
                throw new COMException();
            };

            try
            {
                this.adapter.CaptureRecordedGameClip(ConsoleAddress, null, 10);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(XboxConsoleException), "The XboxConsole Adapter CaptureRecordedGameClip method did not convert a COMException into an XboxConsoleException.");
                Assert.IsInstanceOfType(ex.InnerException, typeof(COMException), "The XboxConsole Adapter CaptureRecordedGameClip method did not include the COMException as the inner exception of the XboxConsoleException that it threw.");
            }
        }

        /// <summary>
        /// Verifies that an ObjectDisposedException is thrown if the CaptureRecordedGameClip()
        /// method is called after the object has been disposed.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestCaptureRecordedGameClipThrowsObjectDisposedException()
        {
            this.adapter.Dispose();
            this.adapter.CaptureRecordedGameClip(ConsoleAddress, null, 10);
        }

        /// <summary>
        /// Verifies that the adapter's CaptureRecordedGameClip() method
        /// calls the XboxXdk's CaptureRecordedGameClip method.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureRecordedGameClipCallsXdkCaptureRecordedGameClip()
        {
            bool isCorrectFunctionCalled = false;
            this.fakeXboxXdk.CaptureRecordedGameClipAction = (ipAddress, filePath, captureSeconds) =>
            {
                isCorrectFunctionCalled = true;
            };

            try
            {
                this.adapter.CaptureRecordedGameClip(ConsoleAddress, null, 10);
            }
            catch (XboxConsoleException)
            {
            }

            Assert.IsTrue(isCorrectFunctionCalled, "The adapter's CaptureRecordedGameClip function failed to call the XboxXdk's CaptureRecordedGameClip function.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.CaptureScreenshot(null));
        }

        /// <summary>
        /// Verifies that the adapter correctly passes on the parameters.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        public void TestCaptureRecordedGameClip()
        {
            string expectedFilePath = @"C:\";
            uint expectedCaptureSeconds = 123;

            this.fakeXboxXdk.CaptureRecordedGameClipAction = (ipAddress, filePath, captureSeconds) =>
            {
                Assert.AreEqual(ConsoleAddress, ipAddress, "The adapter did not pass the expected ipaddress to the XboxXdk level.");
                Assert.AreEqual(expectedFilePath, filePath, "The adapter did not pass the expected file path to the XboxXdk level.");
                Assert.AreEqual(expectedCaptureSeconds, captureSeconds, "The adapter did not pass the expected captureSeconds to the XboxXdk level.");
            };

            this.adapter.CaptureRecordedGameClip(ConsoleAddress, expectedFilePath, expectedCaptureSeconds);
        }

        /// <summary>
        /// Verifies that the adapter throws an ArgumentException when called with number of seconds less than 6.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCaptureRecordedGameClipThrowsArgumentExceptionOnSecondsLessThanMin()
        {
            this.fakeXboxXdk.CaptureRecordedGameClipAction = (ipAddress, filePath, captureSeconds) =>
            {
            };

            this.adapter.CaptureRecordedGameClip(ConsoleAddress, null, 4);
        }

        /// <summary>
        /// Verifies that the adapter throws an ArgumentException when called with number of seconds greater than 300.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory(AdapterConsoleControlTestCategory)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCaptureRecordedGameClipThrowsArgumentExceptionOnSecondsGreaterThanMax()
        {
            this.fakeXboxXdk.CaptureRecordedGameClipAction = (ipAddress, filePath, captureSeconds) =>
            {
            };

            this.adapter.CaptureRecordedGameClip(ConsoleAddress, null, 356);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Throwing COMExceptions to simulate the behavior of the XDK.")]
        private void TestReboot(TimeSpan timeout, int numTimesToBeResponsive, int numTimesToBeUnresponsive, int canConnectSleepTime = 0)
        {
            bool isRebootCalled = false;
            bool correctIpAddressPassedToReboot = false;
            this.fakeXboxXdk.RebootAction = ipAddress =>
            {
                correctIpAddressPassedToReboot = ipAddress == XboxConsoleAdapterTests.ConsoleAddress;
                isRebootCalled = true;
            };

            int numCanConnectCalls = 0;
            this.fakeXboxXdk.CanConnectFunc = _ =>
            {
                Thread.Sleep(canConnectSleepTime);
                if (numCanConnectCalls++ < numTimesToBeResponsive)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            ShimPingReply.AllInstances.StatusGet = _ =>
            {
                if (numCanConnectCalls++ < numTimesToBeResponsive)
                {
                    return IPStatus.Success;
                }
                else
                {
                    return IPStatus.TimedOut;
                }
            };

            int numGetRunningProcCalls = 0;
            this.fakeXboxXdk.GetRunningProcessesFunc = (_0, _1) =>
            {
                if (numGetRunningProcCalls++ < numTimesToBeUnresponsive)
                {
                    throw new COMException();
                }
                else
                {
                    return new XboxProcessDefinition[] 
                    { 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 0, "file0.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 1, "file1.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 2, "Home.exe"), 
                        new XboxProcessDefinition(XboxOperatingSystem.Title, 3, "file2.exe") 
                    };
                }
            };

            this.adapter.Reboot(ConsoleAddress, timeout);
            Assert.IsTrue(isRebootCalled, "The adapter's Reboot method must call the XboxXdk's Reboot method.");
            Assert.IsTrue(correctIpAddressPassedToReboot, "The adapter passed the wrong IP Address to the XboxXdk's Reboot method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.Reboot(null, timeout));
        }
        
        private void TestShutdown(TimeSpan timeout, int numTimesToBeResponsive)
        {
            bool isShutdownCalled = false;
            bool correctIpAddressPassedToShutdown = false;
            this.fakeXboxXdk.ShutdownAction = ipAddress =>
                {
                    correctIpAddressPassedToShutdown = ipAddress == XboxConsoleAdapterTests.ConsoleAddress;
                    isShutdownCalled = true;
                };

            int numCanConnectCalls = 0;
            this.fakeXboxXdk.CanConnectFunc = _ =>
                {
                    if (numCanConnectCalls++ < numTimesToBeResponsive)
                    {
                        return true;
                    }
                    else
                    {
                        numCanConnectCalls = 0; // need it reset back to zero for another shim (below), which is called later from inside the tested method (this.adapter.Shutdown(timeout))
                        return false;
                    }
                };

            ShimPingReply.AllInstances.StatusGet = _ =>
                {
                    if (numCanConnectCalls++ < numTimesToBeResponsive)
                    {
                        return IPStatus.Success;
                    }
                    else
                    {
                        return IPStatus.TimedOut;
                    }
                };

            this.adapter.Shutdown(ConsoleAddress, timeout);
            Assert.IsTrue(isShutdownCalled, "The adapter's Shutdown method must call the XboxXdk's Shutdown method.");
            Assert.IsTrue(correctIpAddressPassedToShutdown, "The adapter passed the wrong IP Address to the XboxXdk's Shutdown method.");

            this.VerifyThrows<ArgumentNullException>(() => this.adapter.Shutdown(null, timeout));
        }
    }
}
