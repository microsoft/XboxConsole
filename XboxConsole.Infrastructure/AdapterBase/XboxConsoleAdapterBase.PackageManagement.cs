//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.PackageManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides default a implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    internal abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        /// <summary>
        /// Gets the list of all packages currently installed on the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <returns>The list of all packages currently installed on the console.</returns>
        public IEnumerable<XboxPackageDefinition> GetInstalledPackages(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.GetInstalledPackagesImpl(systemIpAddress),
                "Failed to retrieve installed applications.");
        }

        /// <summary>
        /// Launches the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The package to be launched.</param>
        public void LaunchApplication(string systemIpAddress, XboxApplicationDefinition application)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }
            
            this.PerformXdkAction(
                systemIpAddress,
                () => this.LaunchApplicationImpl(systemIpAddress, application),
                string.Format(CultureInfo.InvariantCulture, "Failed to launch package with AUMID: {0}", application.Aumid));
        }

        /// <summary>
        /// Launches the given package with command line arguments.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The package to be launched.</param>
        /// <param name="arguments">Command line arguments to pass to the package executable.</param>
        public void LaunchApplication(string systemIpAddress, XboxApplicationDefinition application, string arguments)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.LaunchApplicationImpl(systemIpAddress, application, arguments),
                string.Format(CultureInfo.InvariantCulture, "Failed to launch package with AUMID: {0} and arguments: {1}", application.Aumid, arguments));
        }

        /// <summary>
        /// Terminates the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be terminated.</param>
        public void TerminatePackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.TerminatePackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to terminate package: {0}", package.FullName));
        }

        /// <summary>
        /// Suspends the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be suspended.</param>
        public void SuspendPackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.SuspendPackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to Suspend package: {0}", package.FullName));
        }

        /// <summary>
        /// Resumes execution of a suspended package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be resumed.</param>
        public void ResumePackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.ResumePackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to Resume package: {0}", package.FullName));
        }

        /// <summary>
        /// Constrains the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be constrained.</param>
        public void ConstrainPackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.ConstrainPackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to Constrain package: {0}", package.FullName));
        }

        /// <summary>
        /// Unconstrains a constrained package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be unconstrained.</param>
        public void UnconstrainPackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.UnconstrainPackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to Unconstrain package: {0}", package.FullName));
        }

        /// <summary>
        /// Snaps the given application.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The application to be snapped.</param>
        public void SnapApplication(string systemIpAddress, XboxApplicationDefinition application)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.PerformXdkAction(
                systemIpAddress,
                () => this.SnapApplicationImpl(systemIpAddress, application),
                string.Format(CultureInfo.InvariantCulture, "Failed to snap application with AUMID: {0}", application.Aumid));
        }

        /// <summary>
        /// Unsnaps the given application.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        public void UnsnapApplication(string systemIpAddress)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.UnsnapApplicationImpl(systemIpAddress),
                "Failed to unsnap application");
        }

        /// <summary>
        /// Retrieves the execution state of the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package for which the execution state shall be retrieved.</param>
        /// <returns>The current execution state of the package given by the <paramref name="package"/> parameter.</returns>
        public PackageExecutionState QueryPackageExecutionState(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.QueryPackageExecutionStateImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to query execution state of package: {0}", package.FullName));
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        public Task<XboxPackageDefinition> DeployPushAsync(string systemIpAddress, string deployFilePath, bool removeExtraFiles, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            return this.PerformXdkFunc(
                systemIpAddress,
                () => this.DeployPushImplAsync(systemIpAddress, deployFilePath, removeExtraFiles, progressMetric, progressError, progressExtraFile),
                "Failed to deploy package");
        }

        /// <summary>
        /// Uninstall a package from a given console based on its package full name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to be affected.</param>
        /// <param name="package">The package to be uninstalled.</param>
        public void UninstallPackage(string systemIpAddress, XboxPackageDefinition package)
        {
            this.ThrowIfDisposed();
            this.ThrowIfInvalidSystemIpAddress(systemIpAddress);

            this.PerformXdkAction(
                systemIpAddress,
                () => this.UninstallPackageImpl(systemIpAddress, package),
                string.Format(CultureInfo.InvariantCulture, "Failed to uninstall package with full name '{0}'", package == null || string.IsNullOrEmpty(package.FullName) ? "null" : package.FullName));
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "ResumePackage" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be resumed.</param>
        protected virtual void ResumePackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "QueryPackageExecutionState" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package for which the execution state shall be retrieved.</param>
        /// <returns>The current execution state of the package given by the <paramref name="package"/> parameter.</returns>
        protected virtual PackageExecutionState QueryPackageExecutionStateImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "SuspendPackage" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be suspended.</param>
        protected virtual void SuspendPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "TerminatePackage" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be terminated.</param>
        protected virtual void TerminatePackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "GetInstalledPackages" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <returns>The list of all packages currently installed on the console.</returns>
        protected virtual IEnumerable<XboxPackageDefinition> GetInstalledPackagesImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "LaunchApplication" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The package to be launched.</param>
        protected virtual void LaunchApplicationImpl(string systemIpAddress, XboxApplicationDefinition application)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "LaunchApplication" method (with arguments).
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The package to be launched.</param>
        /// <param name="arguments">Command line arguments to pass to the package executable.</param>
        protected virtual void LaunchApplicationImpl(string systemIpAddress, XboxApplicationDefinition application, string arguments)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "ConstrainPackage" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be constrained.</param>
        protected virtual void ConstrainPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "UnconstrainPackage" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be unconstrained.</param>
        protected virtual void UnconstrainPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "DeployPushAsync" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        protected virtual Task<XboxPackageDefinition> DeployPushImplAsync(string systemIpAddress, string deployFilePath, bool removeExtraFiles, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "SnapApplication" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The application to be launched.</param>
        protected virtual void SnapApplicationImpl(string systemIpAddress, XboxApplicationDefinition application)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Provides the adapter-specific implementation of the "UnsnapApplication" method.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        protected virtual void UnsnapApplicationImpl(string systemIpAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Uninstall a package from a given console based on its package full name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to be affected.</param>
        /// <param name="package">The package to be uninstalled.</param>
        protected virtual void UninstallPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }
    }
}
