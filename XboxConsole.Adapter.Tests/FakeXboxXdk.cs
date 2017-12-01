//------------------------------------------------------------------------------
// <copyright file="FakeXboxXdk.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.IO;

    /// <summary>
    /// Fake implementation of XboxXdk used in Unit Testing.
    /// </summary>
    internal class FakeXboxXdk : XboxXdkBase
    {
        /// <summary>
        /// Gets or sets a custom func to shim the property (get).
        /// </summary>
        public Func<string> DefaultConsoleGetFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom action to shim the property (set).
        /// </summary>
        public Action<string> DefaultConsoleSetAction { get; set; }

        /// <summary>
        /// Gets or sets a custom func to shim the method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by design.")]
        public Func<string, XboxOperatingSystem, IEnumerable<XboxProcessDefinition>> GetRunningProcessesFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom func to shim the method.
        /// </summary>
        public Func<string, bool> CanConnectFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom action to shim the method.
        /// </summary>
        public Action<string> RebootAction { get; set; }

        /// <summary>
        /// Gets or sets a custom action to shim the method.
        /// </summary>
        public Action<string> ShutdownAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetInstalledPackages" method.
        /// </summary>
        public Func<string, string> GetInstalledPackagesFunc { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "LaunchApplication" method.
        /// </summary>
        public Action<string, string> LaunchApplicationAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "TerminatePackage" method.
        /// </summary>
        public Action<string, string> TerminatePackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "ResumePackage" method.
        /// </summary>
        public Action<string, string> ResumePackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "SuspendPackage" method.
        /// </summary>
        public Action<string, string> SuspendPackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "ConstrainPackage" method.
        /// </summary>
        public Action<string, string> ConstrainPackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "UnconstrainPackage" method.
        /// </summary>
        public Action<string, string> UnconstrainPackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "UninstallPackage" method.
        /// </summary>
        public Action<string, string> UninstallPackageAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "SnapApplication" method.
        /// </summary>
        public Action<string, string> SnapApplicationAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "UnsnapApplication" method.
        /// </summary>
        public Action<string> UnsnapApplicationAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "QueryPackageExecutionStateFunc" method.
        /// </summary>
        public Func<string, string, uint> QueryPackageExecutionStateFunc { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "CopyFile" method.
        /// </summary>
        public Action<string, string, string, XboxOperatingSystem, int, IProgress<XboxFileTransferMetric>> CopyFilesAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "CreateDirectory" action.
        /// </summary>
        public Action<string, string, XboxOperatingSystem> CreateDirectoryAction { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "DeleteFile" method.
        /// </summary>
        public Action<string, string, XboxOperatingSystem, int> DeleteFilesAction { get; set; }

        /// <summary>
        /// Gets or sets the shim for the "RemoveDirectory" method.
        /// </summary>
        public Action<string, string, XboxOperatingSystem, bool> RemoveDirectoryAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "FindFiles" method.
        /// </summary>
        public Func<string, string, XboxOperatingSystem, int, IEnumerable<XboxFileSystemInfoDefinition>> FindFilesFunc { get; set; }

        /// <summary>
        /// Gets or sets the action to shim the "CreateDebugMonitorClient" method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", 
            Justification = "We need to implement unit tests referencing this code")]
        public Func<string, XboxOperatingSystem, IXboxDebugMonitorClient> CreateDebugMonitorClientFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom func to shim the method.
        /// </summary>
        public Func<string, string, string> GetConfigValueFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom action to shim the method.
        /// </summary>
        public Action<string, string, string> SetConfigValueAction { get; set; }

        /// <summary>
        /// Gets or sets a custom func to shim the CreateGamepad method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "We need to implement unit tests referencing this code")]
        public Func<string, IVirtualGamepad> CreateGamepadFunc { get; set; }

        /// <summary>
        /// Gets or sets a custom func to shim the "DeployPushAsync" method.
        /// </summary>
        public Func<string, string, bool, CancellationToken, IProgress<XboxDeploymentMetric>, IProgress<XboxDeploymentError>, IProgress<XboxDeploymentExtraFile>, Task<string>> DeployPushAsyncFunc { get; set; }
        
        /// <summary>
        /// Gets or sets the function to shim the "GetInstalledPackages" method.
        /// </summary>
        public Func<string, IEnumerable<XboxUserDefinition>> GetUsersFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "AddGuestUser" method.
        /// </summary>
        public Func<string, uint> AddGuestUserFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "AddUser" method.
        /// </summary>
        public Func<string, string, XboxUserDefinition> AddUserFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "DeleteAllUsers" method.
        /// </summary>
        public Action<string> DeleteAllUsersAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "DeleteUser" method.
        /// </summary>
        public Action<string, XboxUserDefinition> DeleteUserAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "SignInUser" method.
        /// </summary>
        public Func<string, XboxUserDefinition, string, bool, XboxUserDefinition> SignInUserFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "SignOutUser" method.
        /// </summary>
        public Func<string, XboxUserDefinition, XboxUserDefinition> SignOutUserFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetInstalledPackages" method.
        /// </summary>
        public Action<string, uint, ulong> PairControllerToUserFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "AddLocalUsersToParty" method.
        /// </summary>
        public Action<string, uint, string, string[]> AddLocalUsersToPartyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "InviteToParty" method.
        /// </summary>
        public Action<string, uint, string, string[]> InviteToPartyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "RemoveLocalUsersFromParty" method.
        /// </summary>
        public Action<string, uint, string[]> RemoveLocalUsersFromPartyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetPartyId" method.
        /// </summary>
        public Func<string, uint, string> GetPartyIdFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetPartyMembers" method.
        /// </summary>
        public Func<string, uint, string[]> GetPartyMembersFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "AcceptInviteToParty" method.
        /// </summary>
        public Action<string, string, string> AcceptInviteToPartyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "DeclineInviteToParty" method.
        /// </summary>
        public Action<string, string, string> DeclineInviteToPartyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "CaptureScreenshot" method.
        /// </summary>
        public Func<string, IntPtr> CaptureScreenshotFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "CaptureRecordedGameClip" method.
        /// </summary>
        public Action<string, string, uint> CaptureRecordedGameClipAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetConsoleInfo" method.
        /// </summary>
        public Func<string, XboxConsoleInfo> GetConsoleInfoFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "RegisterPackage" method.
        /// </summary>
        public Func<string, string, XboxPackageDefinition> RegisterPackageFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "UnregisterPackage" method.
        /// </summary>
        public Action<string, string> UnregisterPackageAction { get; set; }

        /// <summary>
        /// Gets or sets the function to shim the "GetAvailableSpaceForAppInstallation" method.
        /// </summary>
        public Func<string, string, ulong> GetAvailableSpaceForAppInstallationFunc { get; set; }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        public override string DefaultConsole
        {
            get
            {
                if (this.DefaultConsoleGetFunc != null)
                {
                    return this.DefaultConsoleGetFunc();
                }
                else
                {
                    throw new NotImplementedException("DefaultConsoleGetFunc is not set.");
                }
            }

            set
            {
                if (this.DefaultConsoleSetAction != null)
                {
                    this.DefaultConsoleSetAction(value);
                }
                else
                {
                    throw new NotImplementedException("DefaultConsoleSetAction is not set.");
                }
            }
        }

        /// <summary>
        /// Gets the list of processes running on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        public override IEnumerable<XboxProcessDefinition> GetRunningProcesses(string ipAddress, XboxOperatingSystem operatingSystem)
        {
            if (this.GetRunningProcessesFunc != null)
            {
                return this.GetRunningProcessesFunc(ipAddress, operatingSystem);
            }
            else
            {
                throw new NotImplementedException("GetRunningProcessesFunc is not set.");
            }
        }

        /// <summary>
        /// Returns true if a console can be connected to (responsive).
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        /// <returns>The indication if a console can be connected to.</returns>
        public override bool CanConnect(string ipAddress)
        {
            if (this.CanConnectFunc != null)
            {
                return this.CanConnectFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("CanConnectFunc is not set.");
            }
        }

        /// <summary>
        /// Reboots the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void Reboot(string ipAddress)
        {
            if (this.RebootAction != null)
            {
                this.RebootAction(ipAddress);
            }
            else
            {
                throw new NotImplementedException("RebootAction is not set.");
            }
        }

        /// <summary>
        /// Shutdowns the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void Shutdown(string ipAddress)
        {
            if (this.ShutdownAction != null)
            {
                this.ShutdownAction(ipAddress);
            }
            else
            {
                throw new NotImplementedException("ShutdownAction is not set.");
            }
        }

        /// <summary>
        /// Get a string describing all packages installed on the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        /// <returns>A string describing all packages installed on the console.</returns>
        public override string GetInstalledPackages(string ipAddress)
        {
            if (this.GetInstalledPackagesFunc != null)
            {
                return this.GetInstalledPackagesFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("GetInstalledPackagesFunc is not set.");
            }
        }

        /// <summary>
        /// Launches the given application on the given console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="aumid">The aumid of the application to be launched.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void LaunchApplication(string ipAddress, string aumid)
        {
            if (this.LaunchApplicationAction != null)
            {
                this.LaunchApplicationAction(ipAddress, aumid);
            }
            else
            {
                throw new NotImplementedException("LaunchApplicationAction is not set.");
            }
        }

        /// <summary>
        /// Terminates the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to terminate.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void TerminatePackage(string ipAddress, string packageFullName)
        {
            if (this.TerminatePackageAction != null)
            {
                this.TerminatePackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("TerminatePackageAction is not set.");
            }
        }

        /// <summary>
        /// Suspends the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to suspend.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void SuspendPackage(string ipAddress, string packageFullName)
        {
            if (this.SuspendPackageAction != null)
            {
                this.SuspendPackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("SuspendPackageAction is not set.");
            }
        }

        /// <summary>
        /// Resumes the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be launched.</param>
        /// <param name="packageFullName">The Package Full Name of the package to resume.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void ResumePackage(string ipAddress, string packageFullName)
        {
            if (this.ResumePackageAction != null)
            {
                this.ResumePackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("ResumePackageAction is not set.");   
            }
        }

        /// <summary>
        /// Constrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be constrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to constrain.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void ConstrainPackage(string ipAddress, string packageFullName)
        {
            if (this.ConstrainPackageAction != null)
            {
                this.ConstrainPackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("ConstrainPackageAction is not set.");
            }
        }

        /// <summary>
        /// Unconstrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be unconstrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to unconstrain.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void UnconstrainPackage(string ipAddress, string packageFullName)
        {
            if (this.UnconstrainPackageAction != null)
            {
                this.UnconstrainPackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("UnconstrainPackageAction is not set.");
            }
        }

        /// <summary>
        /// Uninstall the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be unconstrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to uninstall.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void UninstallPackage(string ipAddress, string packageFullName)
        {
            if (this.UninstallPackageAction != null)
            {
                this.UninstallPackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("UninstallPackageAction is not set.");
            }
        }

        /// <summary>
        /// Snaps the given application on the given console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="aumid">The aumid of the application to be launched.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void SnapApplication(string ipAddress, string aumid)
        {
            if (this.SnapApplicationAction != null)
            {
                this.SnapApplicationAction(ipAddress, aumid);
            }
            else
            {
                throw new NotImplementedException("SnapApplicationAction is not set.");
            }
        }

        /// <summary>
        /// Unsnaps the snapped application on the given console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void UnsnapApplication(string ipAddress)
        {
            if (this.UnsnapApplicationAction != null)
            {
                this.UnsnapApplicationAction(ipAddress);
            }
            else
            {
                throw new NotImplementedException("UnsnapApplicationAction is not set.");
            }
        }

        /// <summary>
        /// Retrieves the execution state of the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the state of the package will be retrieved.</param>
        /// <param name="packageFullName">The Package Full Name of the package to resume.</param>
        /// <returns>
        /// A value representing the current execution state of the given package.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override uint QueryPackageExecutionState(string ipAddress, string packageFullName)
        {
            if (this.QueryPackageExecutionStateFunc != null)
            {
                return this.QueryPackageExecutionStateFunc(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("QueryPackageExecutionStateFunc is not set.");
            }
        }

        /// <summary>
        /// Copies files that match the search pattern from either a PC to an Xbox or from an Xbox to a PC.  The direction of the copy is dependent on the paths
        /// passed to <paramref name="sourceSearchPath"/> and <paramref name="destinationPath"/>.  If one of the paths starts with
        /// the letter "x", then it is considered to be the path to the file on the Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console to copy a file from or to.</param>
        /// <param name="sourceSearchPath">The search pattern of the files to be copied.  If the files resides on the Xbox then the path must start with the letter "x".  For example, if the 
        /// file is on the Xbox's D:\ drive, then the path must start with "XD:\".</param>
        /// <param name="destinationPath">The complete destination path for the file.  If the destination is intended to be on the Xbox then this path must start with the letter "x".
        /// For example, if you wish to copy the file to the D:\ drive, then this path must start with "XD:\".</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox that you wish to copy the files from or to.</param>
        /// <param name="recursionLevel">The number of levels of recursion to use when searching for files.</param>
        /// <param name="metrics">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        public override void CopyFiles(string ipAddress, string sourceSearchPath, string destinationPath, XboxOperatingSystem targetOperatingSystem, int recursionLevel, IProgress<XboxFileTransferMetric> metrics)
        {
            if (this.CopyFilesAction != null)
            {
                this.CopyFilesAction(ipAddress, sourceSearchPath, destinationPath, targetOperatingSystem, recursionLevel, metrics);
            }
            else
            {
                throw new NotImplementedException("CopyFileAction is not set.");   
            }
        }

        /// <summary>
        /// Creates a directory on the Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which to create the directory.</param>
        /// <param name="destinationDirectoryPath">The path to the directory to be created.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox where the directory shall be created.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void CreateDirectory(string ipAddress, string destinationDirectoryPath, XboxOperatingSystem targetOperatingSystem)
        {
            if (this.CreateDirectoryAction != null)
            {
                this.CreateDirectoryAction(ipAddress, destinationDirectoryPath, targetOperatingSystem);
            }
            else
            {
                throw new NotImplementedException("CreateDirectoryAction is not set.");   
            }
        }

        /// <summary>
        /// Deletes files from an Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the files will be deleted.</param>
        /// <param name="remoteFileSearchPattern">The search path for the files to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the files will be deleted.</param>
        /// <param name="recursionLevel">The number of levels of recursion to use when searching for files to delete.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void DeleteFiles(string ipAddress, string remoteFileSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevel)
        {
            if (this.DeleteFilesAction != null)
            {
                this.DeleteFilesAction(ipAddress, remoteFileSearchPattern, targetOperatingSystem, recursionLevel);
            }
            else
            {
                throw new NotImplementedException("DeleteFileAction is not set.");   
            }
        }

        /// <summary>
        /// Removes a directory from an Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the directory will be deleted.</param>
        /// <param name="remoteFilePath">The complete path to the directory to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the directory will be removed.</param>
        /// <param name="recursive">A flag to indicate whether or not to recursively delete the contents of the given directory and all
        /// of its children.</param>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override void RemoveDirectory(string ipAddress, string remoteFilePath, XboxOperatingSystem targetOperatingSystem, bool recursive)
        {
            if (this.RemoveDirectoryAction != null)
            {
                this.RemoveDirectoryAction(ipAddress, remoteFilePath, targetOperatingSystem, recursive);
            }
            else
            {
                throw new NotImplementedException("RemoveDirectoryAction is not set.");   
            }
        }

        /// <summary>
        /// Retrieves a collection of files on an Xbox that match the given search pattern.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which to search for files.</param>
        /// <param name="remoteSearchPattern">The pattern used to search for files.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox to search for files.</param>
        /// <param name="recursionLevels">The number of recursion levels to use while searching for matches.</param>
        /// <returns>
        /// A enumeration of the files that match the given search pattern.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if a delegate replacing the implementation of this method has not been provided.</exception>
        public override IEnumerable<XboxFileSystemInfoDefinition> FindFiles(string ipAddress, string remoteSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevels)
        {
            if (this.FindFilesFunc != null)
            {
                return this.FindFilesFunc(ipAddress, remoteSearchPattern, targetOperatingSystem, recursionLevels);
            }
            else
            {
                throw new NotImplementedException("FindFilesFunc is not set.");
            }
        }

        /// <summary>
        /// Creates an Xbox debug monitor client.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The Xbox debug monitor client.</returns>
        public override IXboxDebugMonitorClient CreateDebugMonitorClient(string ipAddress, XboxOperatingSystem operatingSystem)
        {
            if (this.CreateDebugMonitorClientFunc != null)
            {
                return this.CreateDebugMonitorClientFunc(ipAddress, operatingSystem);
            }
            else
            {
                throw new NotImplementedException("CreateDebugMonitorClientFunc is not set.");
            }
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="ipAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        public override string GetConfigValue(string ipAddress, string key)
        {
            if (this.GetConfigValueFunc != null)
            {
                return this.GetConfigValueFunc(ipAddress, key);
            }
            else
            {
                throw new NotImplementedException("GetConfigValueFunc is not set.");
            }
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="ipAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        public override void SetConfigValue(string ipAddress, string key, string value)
        {
            if (this.SetConfigValueAction != null)
            {
                this.SetConfigValueAction(ipAddress, key, value);
            }
            else
            {
                throw new NotImplementedException("SetConfigValueAction is not set.");
            }
        }

        /// <summary>
        /// Creates a XboxGamepad.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>A gamepad.</returns>
        public override IVirtualGamepad CreateXboxGamepad(string ipAddress)
        {
            if (this.CreateGamepadFunc != null)
            {
                return this.CreateGamepadFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("CreateGamepadFunc is not set.");
            }
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the deployment to complete.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is a json string describing the deployed package.</returns>
        public override Task<string> DeployPushAsync(string ipAddress, string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            if (this.DeployPushAsyncFunc != null)
            {
                return this.DeployPushAsyncFunc(ipAddress, deployFilePath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile);
            }
            else
            {
                throw new NotImplementedException("DeployPushAsyncFunc is not set.");
            }
        }

        /// <summary>
        /// Gets the list of users on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <returns>The enumeration of XboxUserDefinition instances.</returns>
        public override IEnumerable<XboxUserDefinition> GetUsers(string ipAddress)
        {
            if (this.GetUsersFunc != null)
            {
                return this.GetUsersFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("GetUsersFunc is not set.");
            }
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        public override uint AddGuestUser(string ipAddress)
        {
            if (this.AddGuestUserFunc != null)
            {
                return this.AddGuestUserFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("AddGuestUserFunc is not set.");
            }
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        public override XboxUserDefinition AddUser(string ipAddress, string emailAddress)
        {
            if (this.AddUserFunc != null)
            {
                return this.AddUserFunc(ipAddress, emailAddress);
            }
            else
            {
                throw new NotImplementedException("AddUserFunc is not set.");
            }
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        public override void DeleteAllUsers(string ipAddress)
        {
            if (this.DeleteAllUsersAction != null)
            {
                this.DeleteAllUsersAction(ipAddress);
            }
            else
            {
                throw new NotImplementedException("DeleteAllUsersAction is not set.");
            }
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        public override void DeleteUser(string ipAddress, XboxUserDefinition user)
        {
            if (this.DeleteUserAction != null)
            {
                this.DeleteUserAction(ipAddress, user);
            }
            else
            {
                throw new NotImplementedException("DeleteUserAction is not set.");
            }
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        public override XboxUserDefinition SignInUser(string ipAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            if (this.SignInUserFunc != null)
            {
                return this.SignInUserFunc(ipAddress, user, password, storePassword);
            }
            else
            {
                throw new NotImplementedException("SignInUserFunc is not set.");
            }
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        public override XboxUserDefinition SignOutUser(string ipAddress, XboxUserDefinition user)
        {
            if (this.SignOutUserFunc != null)
            {
                return this.SignOutUserFunc(ipAddress, user);
            }
            else
            {
                throw new NotImplementedException("SignOutUserAction is not set.");
            }
        }

        /// <summary>
        /// Creates a party for the given title ID (if one does not exist) and adds the given local users to it.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to add other users to the party.</param>
        /// <param name="localUserXuidsToAdd">User XUIDs to add to the party.</param>
        public override void AddLocalUsersToParty(string ipAddress, uint titleId, string actingUserXuid, string[] localUserXuidsToAdd)
        {
            if (this.AddLocalUsersToPartyFunc != null)
            {
                this.AddLocalUsersToPartyFunc(ipAddress, titleId, actingUserXuid, localUserXuidsToAdd);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Invites the given users on behalf of the acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to invite other users to the party.</param>
        /// <param name="remoteUserXuidsToInvite">Remote user XUIDs to invite to the party.</param>
        public override void InviteToParty(string ipAddress, uint titleId, string actingUserXuid, string[] remoteUserXuidsToInvite)
        {
            if (this.InviteToPartyFunc != null)
            {
                this.InviteToPartyFunc(ipAddress, titleId, actingUserXuid, remoteUserXuidsToInvite);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Removes the given users from the party belonging to the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="localUserXuidsToRemove">Local user XUIDs to remove from the party.</param>
        public override void RemoveLocalUsersFromParty(string ipAddress, uint titleId, string[] localUserXuidsToRemove)
        {
            if (this.RemoveLocalUsersFromPartyFunc != null)
            {
                this.RemoveLocalUsersFromPartyFunc(ipAddress, titleId, localUserXuidsToRemove);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Returns the party ID belonging to the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get the associated party ID for.</param>
        /// <returns>ID of existing party used to accept or decline an invitation to the party.</returns>
        public override string GetPartyId(string ipAddress, uint titleId)
        {
            if (this.GetPartyIdFunc != null)
            {
                return this.GetPartyIdFunc(ipAddress, titleId);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Lists both the current members and the reserved members of the party belonging to given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get party members for.</param>
        /// <returns>Party member user XUIDs, which may contain a mix of local and remote users.</returns>
        public override string[] GetPartyMembers(string ipAddress, uint titleId)
        {
            if (this.GetPartyMembersFunc != null)
            {
                return this.GetPartyMembersFunc(ipAddress, titleId);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Accepts the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to accept the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public override void AcceptInviteToParty(string ipAddress, string actingUserXuid, string partyId)
        {
            if (this.AcceptInviteToPartyFunc != null)
            {
                this.AcceptInviteToPartyFunc(ipAddress, actingUserXuid, partyId);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Declines the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to decline the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public override void DeclineInviteToParty(string ipAddress, string actingUserXuid, string partyId)
        {
            if (this.DeclineInviteToPartyFunc != null)
            {
                this.DeclineInviteToPartyFunc(ipAddress, actingUserXuid, partyId);
            }
            else
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Pairs a controller to a user on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="userId">The user id of the user to pair.</param>
        /// <param name="controllerId">The controller of the id to pair.</param>
        public override void PairControllerToUser(string ipAddress, uint userId, ulong controllerId)
        {
            if (this.PairControllerToUserFunc != null)
            {
                this.PairControllerToUserFunc(ipAddress, userId, controllerId);
            }
            else
            {
                throw new NotImplementedException("PairControllerToUserFunc is not set.");
            }
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the specified console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        public override IntPtr CaptureScreenshot(string ipAddress)
        {
            if (this.CaptureScreenshotFunc != null)
            {
                return this.CaptureScreenshotFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("CaptureScreenshotFunc is not set.");
            }
        }

        /// <summary>
        /// Captures an MP4 clip using the GameDVR service and writes it to the specified output path.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="outputPath">Full path of the MP4 file to create.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time (between 6 and 300).</param>
        public override void CaptureRecordedGameClip(string ipAddress, string outputPath, uint captureSeconds)
        {
            if (this.CaptureRecordedGameClipAction != null)
            {
                this.CaptureRecordedGameClipAction(ipAddress, outputPath, captureSeconds);
            }
            else
            {
                throw new NotImplementedException("CaptureScreenshotFunc is not set.");
            }
        }

        /// <summary>
        /// Gets the information about a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>A XboxConsoleInfo containing information about the console.</returns>
        public override XboxConsoleInfo GetConsoleInfo(string ipAddress)
        {
            if (this.GetConsoleInfoFunc != null)
            {
                return this.GetConsoleInfoFunc(ipAddress);
            }
            else
            {
                throw new NotImplementedException("GetConsoleInfoFunc is not set.");
            }
        }

        /// <summary>
        /// Registers a package deployed to the title scratch drive. 
        /// </summary>
        /// <param name="ipAddress">The ip address of the Xbox kit.</param>
        /// <param name="packagePath">The relative path on the consoles scratch drive to the package.</param>
        /// <returns>The package definition object that describes the package.</returns>
        public override XboxPackageDefinition RegisterPackage(string ipAddress, string packagePath)
        {
            if (this.RegisterPackageFunc != null)
            {
                return this.RegisterPackageFunc(ipAddress, packagePath);
            }
            else
            {
                throw new NotImplementedException("GetConsoleInfoFunc is not set.");
            }
        }

        /// <summary>
        /// Unregisters a package deployed to the title scratch drive.
        /// </summary>
        /// <param name="ipAddress">The ip address of the Xbox kit.</param>
        /// <param name="packageFullName">The Package Full Name of the package to be unregistered.</param>
        public override void UnregisterPackage(string ipAddress, string packageFullName)
        {
            if (this.UnregisterPackageAction != null)
            {
                this.UnregisterPackageAction(ipAddress, packageFullName);
            }
            else
            {
                throw new NotImplementedException("CaptureScreenshotFunc is not set.");
            }
        }

        /// <summary>
        /// Gets the available space that is available for app installation.
        /// </summary>
        /// <param name="ipAddress">The IP address of the Xbox kit.</param>
        /// <param name="storageName">The name of the storage device to check. Allowed values are "internal" and null. </param>
        /// <returns>The number of bytes of freespace on the storage device on the specified console.</returns>
        public override ulong GetAvailableSpaceForAppInstallation(string ipAddress, string storageName)
        {
            if (this.GetAvailableSpaceForAppInstallationFunc != null)
            {
                return this.GetAvailableSpaceForAppInstallationFunc(ipAddress, storageName);
            }
            else
            {
                throw new NotImplementedException("GetConsoleInfoFunc is not set.");
            }
        }
    }
}
