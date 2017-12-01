//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapter.PackageManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.July2014
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;

    /// <summary>
    /// This class represents an implemenation of the XboxConsoleAdapter
    /// that is supported by the July 2014 version of the XDK.
    /// </summary>
    internal partial class XboxConsoleAdapter : XboxConsoleAdapterBase
    {
        private COMExceptionWhenConnectingHandler comExceptionWhenConnectingHandler = new COMExceptionWhenConnectingHandler();

        /// <summary>
        /// Connects to the console and retrieves the collection of installed packages.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <returns>The collection of packages installed on the console.</returns>
        protected override IEnumerable<XboxPackageDefinition> GetInstalledPackagesImpl(string systemIpAddress)
        {
            // In the July 2014 XDK the format of the string returned by the XDK is a JSON object with this schema:
            // {"Packages":[{"FullName":"Achievements_1.0.1308.7000_x64__8wekyb3d8bbwe","Applications":[{"Aumid":"Achievements_8wekyb3d8bbwe!App"}]}]}
            string xdkOutput = this.XboxXdk.GetInstalledPackages(systemIpAddress);

            List<XboxPackageDefinition> returnValue = new List<XboxPackageDefinition>();
            InstalledPackages installedPackages;
            try
            {
                using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Unicode.GetBytes(xdkOutput), XmlDictionaryReaderQuotas.Max))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(InstalledPackages));
                    installedPackages = (InstalledPackages)serializer.ReadObject(jsonReader);
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new XboxConsoleException("Failed to parse installed packages string.", ex);
            }
            catch (SerializationException ex)
            {
                throw new XboxConsoleException("Failed to parse installed packages string.", ex);
            }

            foreach (var package in installedPackages.Packages)
            {
                string packageFullName = package.FullName;
                XboxPackageDefinition newPackage = new XboxPackageDefinition(packageFullName, package.Applications.Select(app => app.Aumid));
                returnValue.Add(newPackage);
            }

            return returnValue;
        }

        /// <summary>
        /// Launches the given application.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The application to be launched.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        protected override void LaunchApplicationImpl(string systemIpAddress, XboxApplicationDefinition application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.XboxXdk.LaunchApplication(systemIpAddress, application.Aumid);
        }

        /// <summary>
        /// Launches the given application with command line arguments.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The application to be launched.</param>
        /// <param name="arguments">Command line arguments to pass to the package executable.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        protected override void LaunchApplicationImpl(string systemIpAddress, XboxApplicationDefinition application, string arguments)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (string.IsNullOrEmpty(arguments))
            {
                // Passing null or empty string reverts to regular LaunchApplication behavior
                this.XboxXdk.LaunchApplication(systemIpAddress, application.Aumid);
            }
            else
            {
                // Passing valid arguments concatenates them to AUMID with a space separator (we trust native Xtf to handle this properly)
                this.XboxXdk.LaunchApplication(systemIpAddress, string.Format(CultureInfo.InvariantCulture, "{0} {1}", application.Aumid, arguments));
            }
        }

        /// <summary>
        /// Terminates the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be terminated.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        protected override void TerminatePackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.TerminatePackage(systemIpAddress, package.FullName);
        }

        /// <summary>
        /// Resumes execution of a suspended package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be resumed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        protected override void ResumePackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.ResumePackage(systemIpAddress, package.FullName);
        }

        /// <summary>
        /// Suspends the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be suspended.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        protected override void SuspendPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.SuspendPackage(systemIpAddress, package.FullName);
        }

        /// <summary>
        /// Constrains the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be constrained.</param>
        protected override void ConstrainPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.ConstrainPackage(systemIpAddress, package.FullName);
        }

        /// <summary>
        /// Unconstrains a constrained package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package to be unconstrained.</param>
        protected override void UnconstrainPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.UnconstrainPackage(systemIpAddress, package.FullName);
        }

        /// <summary>
        /// Snaps the given application.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="application">The application to be snapped.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="application"/> parameter is null.</exception>
        protected override void SnapApplicationImpl(string systemIpAddress, XboxApplicationDefinition application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.XboxXdk.SnapApplication(systemIpAddress, application.Aumid);
        }

        /// <summary>
        /// Unsnaps the currently snapped application.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        protected override void UnsnapApplicationImpl(string systemIpAddress)
        {
            this.XboxXdk.UnsnapApplication(systemIpAddress);
        }

        /// <summary>
        /// Retrieves the execution state of the given package.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="package">The package for which the execution state shall be retrieved.</param>
        /// <returns>The current execution state of the package given by the <paramref name="package"/> parameter.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the <paramref name="package"/> parameter is null.</exception>
        /// <exception cref="XboxConsoleException">Thrown if the execution state value returned by the XDK does not match one of the expected values.</exception>
        protected override PackageExecutionState QueryPackageExecutionStateImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            uint xdkValue = this.comExceptionWhenConnectingHandler.CheckForCOMExceptionWhenConnecting(() => { return this.XboxXdk.QueryPackageExecutionState(systemIpAddress, package.FullName); }, systemIpAddress);

            // Interpretation of the following values comes from the XDK team's sources.
            // The following C++ header files:
            //
            // $\sdpublic\xbox\sra\internal\sdk\inc\appmodeltools.h 
            // $\xbox\base\appmodel\appmodeltools\inc\appmodeltools.h 
            // 
            // define an enum containing constants we are using to distinguish package states:
            //
            // typedef enum {
            //    AMT_PACKAGE_STATE_UNKNOWN,
            //    AMT_PACKAGE_STATE_RUNNING,
            //    AMT_PACKAGE_STATE_SUSPENDING,
            //    AMT_PACKAGE_STATE_SUSPENDED,
            //    AMT_PACKAGE_STATE_TERMINATED,
            //    AMT_PACKAGE_STATE_CONSTRAINED,
            // } AMT_PACKAGE_STATE;
            switch (xdkValue)
            {
                case 0:
                    return PackageExecutionState.Unknown;
                case 1:
                    return PackageExecutionState.Running;
                case 2:
                    return PackageExecutionState.Suspending;
                case 3:
                    return PackageExecutionState.Suspended;
                case 4:
                    return PackageExecutionState.Terminated;
                case 5:
                    return PackageExecutionState.Constrained;
                default:
                    throw new XboxConsoleException(string.Format(CultureInfo.InvariantCulture, "Unknown package execution state value of '{0}'", xdkValue));
            }
        }

        /// <summary>
        /// Push deploys file to the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        protected override Task<XboxPackageDefinition> DeployPushImplAsync(string systemIpAddress, string deployFilePath, bool removeExtraFiles, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            return this.DeployPushImplAsync(systemIpAddress, deployFilePath, removeExtraFiles, CancellationToken.None, progressMetric, progressError, progressExtraFile);
        }

        /// <summary>
        /// Push deploys file to the console.
        /// </summary>
        /// <param name="systemIpAddress">The "System Ip" address of the Xbox kit.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the deployment to complete.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        protected async override Task<XboxPackageDefinition> DeployPushImplAsync(string systemIpAddress, string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            if (deployFilePath == null)
            {
                throw new ArgumentNullException("deployFilePath");
            }

            // In the July 2014 XDK the format of the string returned by the XDK is a JSON object with this schema:
            // {"Applications":["XboxConsole.XboxSample_zjr0dfhgjwvde!App"],"Identity":{"FullName":"XboxConsole.XboxSample_1.0.0.0_x64__zjr0dfhgjwvde"}}
            string xdkOutput = await this.XboxXdk.DeployPushAsync(systemIpAddress, deployFilePath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile);

            DeploymentPackage package;
            try
            {
                using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Unicode.GetBytes(xdkOutput), XmlDictionaryReaderQuotas.Max))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DeploymentPackage));
                    package = (DeploymentPackage)serializer.ReadObject(jsonReader);
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new XboxConsoleException("Failed to parse installed packages string.", ex);
            }
            catch (SerializationException ex)
            {
                throw new XboxConsoleException("Failed to parse installed packages string.", ex);
            }

            string packageFullName = package.Identity.FullName;
            string aumid = package.Applications.FirstOrDefault();

            return new XboxPackageDefinition(packageFullName, package.Applications);
        }

        /// <summary>
        /// Uninstall a package from a given console based on its package full name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to be affected.</param>
        /// <param name="package">The package to be uninstalled.</param>
        protected override void UninstallPackageImpl(string systemIpAddress, XboxPackageDefinition package)
        {
            if (string.IsNullOrWhiteSpace(systemIpAddress))
            {
                throw new ArgumentNullException("systemIpAddress");
            }

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.XboxXdk.UninstallPackage(systemIpAddress, package.FullName);
        }

        [DataContract]
        private class Package
        {
            [DataMember]
            public string FullName { get; set; }

            [DataMember]
            public Application[] Applications { get; set; }
        }

        [DataContract]
        private class Application
        {
            [DataMember]
            public string Aumid { get; set; }
        }

        [DataContract]
        private class InstalledPackages
        {
            [DataMember]
            public Package[] Packages { get; set; }
        }

        [DataContract]
        private class PackageIdentity
        {
            [DataMember]
            public string FullName { get; set; }
        }

        [DataContract]
        private class DeploymentPackage
        {
            [DataMember]
            public string[] Applications { get; set; }

            [DataMember]
            public PackageIdentity Identity { get; set; }
        }
    }
}
