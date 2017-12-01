//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.ConsoleControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;

    /// <summary>
    /// A thin layer over the XTF provided managed API.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.</summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        public void Reboot(string systemIpAddress, TimeSpan timeout)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);
            
            this.PerformXdkAction(
                systemIpAddress,
                () => this.RebootImpl(systemIpAddress, timeout),
                "Failed to reboot Xbox.");
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
        public void Reboot(string originalSystemIpAddress, string newSystemIpAddress, TimeSpan timeout)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(originalSystemIpAddress);

            this.PerformXdkAction(
                originalSystemIpAddress,
                () => this.RebootImpl(originalSystemIpAddress, newSystemIpAddress, timeout),
                "Failed to reboot Xbox.");
        }

        /// <summary>
        /// Shutdown the Xbox console and wait no more than the specified amount of time for the operation to complete.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the shutdown process to complete and for the Xbox console to become unresponsive.</param>
        /// <exception cref="System.TimeoutException">Thrown if the shutdown operation does not complete within the given timeout period.</exception>
        public void Shutdown(string systemIpAddress, TimeSpan timeout)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.ShutdownImpl(systemIpAddress, timeout),
                "Failed to shutdown Xbox.");
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the specified console.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        public IntPtr CaptureScreenshot(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc<IntPtr>(
                systemIpAddress,
                () => this.CaptureScreenshotImpl(systemIpAddress),
                "Failed to capture screenshot.");
        }

        /// <summary>
        /// Captures an MP4 clip using the GameDVR service and writes to specified output path.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="outputPath">Full path of the MP4 file to create.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time (between 6 and 300).</param>
        public void CaptureRecordedGameClip(string systemIpAddress, string outputPath, uint captureSeconds)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.CaptureRecordedGameClipImpl(systemIpAddress, outputPath, captureSeconds),
                "Failed to capture DVR clip.");
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "Reboot" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        protected virtual void RebootImpl(string systemIpAddress, TimeSpan timeout)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "Reboot" method.
        /// </summary>
        /// <param name="originalSystemIpAddress">The pre-reboot "System Ip" address of the Xbox kit.</param>
        /// <param name="newSystemIpAddress">The post-reboot "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        /// <remarks>
        /// This overload should be used when the connection string to the console will have changed during the reboot
        /// such as when rebooting after changing the session key.
        /// </remarks>
        protected virtual void RebootImpl(string originalSystemIpAddress, string newSystemIpAddress, TimeSpan timeout)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "Shutdown" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the shutdown process to complete and for the Xbox console to become unresponsive.</param>
        /// <exception cref="System.TimeoutException">Thrown if the shutdown operation does not complete within the given timeout period.</exception>
        protected virtual void ShutdownImpl(string systemIpAddress, TimeSpan timeout)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
        
        /// <summary>
        /// Provides the adapter-specific implementation of the "CaptureScreenshot" method.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        protected virtual IntPtr CaptureScreenshotImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "CaptureRecordedGameClip" method.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console.</param>
        /// <param name="outputPath">The path where recorded MP4 clip will be output.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time, between 6 and 300.</param>
        protected virtual void CaptureRecordedGameClipImpl(string systemIpAddress, string outputPath, uint captureSeconds)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Immediately after initiating a Shutdown or Reboot request, the console remains responsive
        /// for a short period of time.  This method is intended to block until the kit is verified
        /// to no longer be responsive.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the console to become unresponsive.</param>
        /// <exception cref="System.OperationCanceledException">Thrown when the caller cancels this waiting operation.</exception>
        protected virtual void WaitUntilConsoleIsUnresponsive(string systemIpAddress, TimeSpan timeout)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// The XTF APIs do not offer a way to synchronously reboot a console therefore, we need to implement
        /// our own logic to wait for a console to finish rebooting.  This method should block until it has
        /// confirmed that the console has completed rebooting or the timeout has been reached or exceeded.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="timeout">The maximum amount of time to wait for the console to finish rebooting.</param>
        /// <exception cref="System.OperationCanceledException">Thrown when the caller cancels this waiting operation.</exception>
        protected virtual void WaitUntilRebootIsComplete(string systemIpAddress, TimeSpan timeout)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
