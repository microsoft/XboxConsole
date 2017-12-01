//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.ConsoleControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.November2014
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the November 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        private static readonly TimeSpan MaxTimeBetweenRetries = TimeSpan.FromSeconds(1.0);

        private static readonly TimeSpan MinimumDVRCaptureTime = TimeSpan.FromSeconds(6);

        private static readonly TimeSpan MaximumDVRCaptureTime = TimeSpan.FromSeconds(300);

        /// <summary>Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.</summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        protected override void RebootImpl(string systemIpAddress, TimeSpan timeout)
        {
            this.RebootImpl(systemIpAddress, systemIpAddress, timeout);
        }

        /// <summary>Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.</summary>
        /// <param name="originalSystemIpAddress">The pre-reboot "System Ip" address of the Xbox kit.</param>
        /// <param name="newSystemIpAddress">The post-reboot "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        /// <remarks>
        /// This overload should be used when the connection string to the console will have changed during the reboot
        /// such as when rebooting after changing the session key.
        /// </remarks>
        protected override void RebootImpl(string originalSystemIpAddress, string newSystemIpAddress, TimeSpan timeout)
        {
            this.XboxXdk.Reboot(originalSystemIpAddress);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.WaitUntilConsoleIsUnresponsive(originalSystemIpAddress, timeout);
            stopwatch.Stop();

            if (timeout > TimeSpan.Zero)
            {
                timeout = timeout - stopwatch.Elapsed;
                timeout = timeout > TimeSpan.Zero ? timeout : TimeSpan.Zero;
            }

            this.WaitUntilRebootIsComplete(newSystemIpAddress, timeout);
        }

        /// <summary>
        /// Shutdown the Xbox console and wait no more than the specified amount of time for the operation to complete.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the shutdown process to complete and for the Xbox console to become unresponsive.</param>
        /// <exception cref="System.TimeoutException">Thrown if the shutdown operation does not complete within the given timeout period.</exception>
        protected override void ShutdownImpl(string systemIpAddress, TimeSpan timeout)
        {
            this.XboxXdk.Shutdown(systemIpAddress);
            this.WaitUntilConsoleIsUnresponsive(systemIpAddress, timeout);
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the specified console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        protected override IntPtr CaptureScreenshotImpl(string systemIpAddress)
        {
            var bitmapPointer = this.XboxXdk.CaptureScreenshot(systemIpAddress);

            if (bitmapPointer == IntPtr.Zero)
            {
                throw new XboxConsoleException("Failed to capture screenshot.");
            }

            return bitmapPointer;
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "CaptureRecordedGameClip" method.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="outputPath">The path where recorded MP4 clip will be output.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time.</param>
        protected override void CaptureRecordedGameClipImpl(string systemIpAddress, string outputPath, uint captureSeconds)
        {
            if (captureSeconds < MinimumDVRCaptureTime.TotalSeconds || captureSeconds > MaximumDVRCaptureTime.TotalSeconds)
            {
                throw new ArgumentOutOfRangeException("captureSeconds", string.Format(CultureInfo.InvariantCulture, "captureSeconds must be between {0} and {1} seconds", (int)MinimumDVRCaptureTime.TotalSeconds, (int)MaximumDVRCaptureTime.TotalSeconds));
            }

            this.XboxXdk.CaptureRecordedGameClip(systemIpAddress, outputPath, captureSeconds);
        }

        /// <summary>
        /// Immediately after initiating a Shutdown or Reboot request, the console remains responsive
        /// for a short period of time. This method is intended to block until the kit is verified
        /// to no longer be responsive.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the console to become unresponsive.</param>
        /// <exception cref="Sytem.TimeoutException">Thrown if the console does not become unresponsive within the given timeout period.</exception>
        protected override void WaitUntilConsoleIsUnresponsive(string systemIpAddress, TimeSpan timeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Wait until console doesn't respond to pings anymore
            bool success = this.RetryOperationUntilTimeout(
                timeout,
                () =>
                {
                    return !this.RespondsToPing(systemIpAddress);
                });
            if (!success)
            {
                throw new TimeoutException("Console failed to become unresponsive during the given timeout period.");
            }

            stopwatch.Stop();
            if (timeout > TimeSpan.Zero)
            {
                timeout = timeout - stopwatch.Elapsed;
                timeout = timeout > TimeSpan.Zero ? timeout : TimeSpan.Zero;
            }

            // Also wait until console can't connect anymore
            success = this.RetryOperationUntilTimeout(
                timeout,
                () =>
                {
                    return !this.XboxXdk.CanConnect(systemIpAddress);
                });
            if (!success)
            {
                throw new TimeoutException("Console failed to become unresponsive during the given timeout period.");
            }
        }

        /// <summary>
        /// The XTF APIs do not offer a way to synchronously reboot a console therefore, we need to implement
        /// our own logic to wait for a console to finish rebooting. This method should block until it has
        /// confirmed that the console has completed rebooting or the timeout has been reached or exceeded.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the console to finish rebooting.</param>
        /// <exception cref="Sytem.TimeoutException">Thrown if the console does not become responsive within the given timeout period.</exception>
        protected override void WaitUntilRebootIsComplete(string systemIpAddress, TimeSpan timeout)
        {
            bool success = this.RetryOperationUntilTimeout(
                timeout,
                () =>
                {
                    try
                    {
                        IEnumerable<XboxProcessDefinition> runningProcesses = this.XboxXdk.GetRunningProcesses(systemIpAddress, XboxOperatingSystem.System);
                        return runningProcesses.Any(process => process.ImageFileName.IndexOf("Home.exe", StringComparison.OrdinalIgnoreCase) >= 0 || process.ImageFileName.IndexOf("oobe.exe", StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    catch (COMException)
                    {
                        // If a COMException is thrown when creating the ConsoleControlClient that means
                        // that the kit is just generally unresponsive so we need to wait and try again.
                        return false;
                    }
                });

            if (!success)
            {
                throw new TimeoutException("Console failed to become unresponsive during the given timeout period.");
            }
        }

        /// <summary>
        /// Continually performs an action until either the action succeeds (by returning true) or the timeout expires.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to continue retrying the action until timing out.</param>
        /// <param name="action">The action to continually be performed.</param>
        /// <returns>True if the action completed within the given time span, otherwise false.</returns>
        protected bool RetryOperationUntilTimeout(TimeSpan timeout, Func<bool> action)
        {
            Func<bool> loopCondition = null;
            if (timeout < TimeSpan.Zero)
            {
                loopCondition = () => true;
            }
            else
            {
                loopCondition = () => timeout > TimeSpan.Zero;
            }

            do
            {
                bool operationCompletedSuccessfully = action();
                if (operationCompletedSuccessfully)
                {
                    return true;
                }

                TimeSpan sleepTime = MaxTimeBetweenRetries;

                if (timeout >= TimeSpan.Zero && timeout < MaxTimeBetweenRetries)
                {
                    sleepTime = timeout;
                }

                Thread.Sleep(sleepTime);

                // If we received an input "timeout" of something less than zero
                // and we decrement on every iteration then we run the risk of ending
                // up decrementing "timeout" below TimeSpan.MinValue and getting
                // an Overflow exception, so we need to do this check.
                if (timeout > TimeSpan.Zero)
                {
                    timeout -= sleepTime;
                }
            }
            while (loopCondition());

            return false;
        }

        private bool RespondsToPing(string systemIpAddress)
        {
            using (Ping ping = new Ping())
            {
                PingReply pingReply = ping.Send(systemIpAddress.Split('+')[0], 1000);
                if (pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
