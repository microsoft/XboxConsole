//------------------------------------------------------------------------------
// <copyright file="XboxXdkBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.IO;

    /// <summary>
    /// Functional facade of interop assemblies of XDK (XTF).
    /// </summary>
    internal abstract class XboxXdkBase
    {
        protected const string NotSupportedMessage = "The method is not supported for this XDK.";

        private static readonly object staticInitializationLock = new object();
        private static bool isStaticInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxXdkBase"/> class.
        /// </summary>
        protected XboxXdkBase()
        {
            if (!isStaticInitialized)
            {
                lock (staticInitializationLock)
                {
                    if (!isStaticInitialized)
                    {
                        this.SetNativeDllSearchPath(this.PathToXdkBin);
                        this.SubscribeToAssemblyResolve(this.PathToXdkBin);
                        isStaticInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        public virtual string DefaultConsole
        {
            get
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }

            set
            {
                throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Gets the path to the "bin" folder of XDK.
        /// </summary>
        private string PathToXdkBin
        {
            get
            {
                return Path.Combine(Environment.GetEnvironmentVariable("DurangoXdk"), "bin");
            }
        }

        /// <summary>
        /// Gets the list of processes running on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessDefinition instances.</returns>
        public abstract IEnumerable<XboxProcessDefinition> GetRunningProcesses(string ipAddress, XboxOperatingSystem operatingSystem);

        /// <summary>
        /// Returns true if a console can be connected to (responsive).
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <returns>The indication if a console can be connected to.</returns>
        public abstract bool CanConnect(string ipAddress);

        /// <summary>
        /// Reboots the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        public abstract void Reboot(string ipAddress);

        /// <summary>
        /// Shutdowns the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        public abstract void Shutdown(string ipAddress);

        /// <summary>
        /// Gets a string describing the packages installed on the console
        /// specified by the given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console from which to retrieve the list of installed applications.</param>
        /// <returns>A string describing the packages installed on the console
        /// specified by the given IP address.</returns>
        public abstract string GetInstalledPackages(string ipAddress);

        /// <summary>
        /// Launches the application with the given Application User Model Id.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the application will be launched.</param>
        /// <param name="aumid">The AUMID of the application to launch.</param>
        public abstract void LaunchApplication(string ipAddress, string aumid);

        /// <summary>
        /// Terminates the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be terminated.</param>
        /// <param name="packageFullName">The Package Full Name of the package to terminate.</param>
        public abstract void TerminatePackage(string ipAddress, string packageFullName);

        /// <summary>
        /// Suspends the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be suspended.</param>
        /// <param name="packageFullName">The Package Full Name of the package to suspend.</param>
        public abstract void SuspendPackage(string ipAddress, string packageFullName);

        /// <summary>
        /// Resumes the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be resumed.</param>
        /// <param name="packageFullName">The Package Full Name of the package to resume.</param>
        public abstract void ResumePackage(string ipAddress, string packageFullName);

        /// <summary>
        /// Constrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be constrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to constrain.</param>
        public virtual void ConstrainPackage(string ipAddress, string packageFullName)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Unconstrains the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the package will be unconstrained.</param>
        /// <param name="packageFullName">The Package Full Name of the package to unconstrain.</param>
        public virtual void UnconstrainPackage(string ipAddress, string packageFullName)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Snaps the application with the given Application User Model Id.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the application will be snapped.</param>
        /// <param name="aumid">The AUMID of the application to launch.</param>
        public virtual void SnapApplication(string ipAddress, string aumid)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Unsnaps the currently snapped application.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the application will be unsnapped.</param>
        public virtual void UnsnapApplication(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Retrieves the execution state of the package with the given Package Full Name.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the state of the package will be retrieved.</param>
        /// <param name="packageFullName">The Package Full Name of the package for which to retrieve execution state.</param>
        /// <returns>A value representing the current execution state of the given package.</returns>
        public abstract uint QueryPackageExecutionState(string ipAddress, string packageFullName);

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
        public abstract void CopyFiles(string ipAddress, string sourceSearchPath, string destinationPath, XboxOperatingSystem targetOperatingSystem, int recursionLevel, IProgress<XboxFileTransferMetric> metrics);

        /// <summary>
        /// Creates a directory on the Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which to create the directory.</param>
        /// <param name="destinationDirectoryPath">The path to the directory to be created.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox where the directory shall be created.</param>
        public abstract void CreateDirectory(string ipAddress, string destinationDirectoryPath, XboxOperatingSystem targetOperatingSystem);

        /// <summary>
        /// Deletes files from an Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the files will be deleted.</param>
        /// <param name="remoteFileSearchPattern">The search path for the files to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the files will be deleted.</param>
        /// <param name="recursionLevel">The number of levels of recursion to use when searching for files to delete.</param>
        public abstract void DeleteFiles(string ipAddress, string remoteFileSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevel);

        /// <summary>
        /// Removes a directory from an Xbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which the directory will be deleted.</param>
        /// <param name="remoteDirectoryPath">The complete path to the directory to be deleted.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox from which the directory will be removed.</param>
        /// <param name="recursive">A flag to indicate whether or not to recursively delete the contents of the given directory and all 
        /// of its children.</param>
        public abstract void RemoveDirectory(string ipAddress, string remoteDirectoryPath, XboxOperatingSystem targetOperatingSystem, bool recursive);

        /// <summary>
        /// Retrieves a collection of files on an Xbox that match the given search pattern.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console on which to search for files.</param>
        /// <param name="remoteSearchPattern">The pattern used to search for files.</param>
        /// <param name="targetOperatingSystem">The operating system on the Xbox to search for files.</param>
        /// <param name="recursionLevels">The number of recursion levels to use while searching for matches.</param>
        /// <returns>A enumeration of the files that match the given search pattern.</returns>
        public abstract IEnumerable<XboxFileSystemInfoDefinition> FindFiles(string ipAddress, string remoteSearchPattern, XboxOperatingSystem targetOperatingSystem, int recursionLevels);

        /// <summary>
        /// Creates an gamepad for the specified console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <returns>An IVirtualGamepad instance.</returns>
        /// <remarks>Does not connect the gamepad.</remarks>
        public virtual IVirtualGamepad CreateXboxGamepad(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Connects the gamepad to the console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <returns>The Id of the XboxGamepad.</returns>
        public virtual int ConnectXboxGamepad(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Disconnects the XboxGamepad from the console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <param name="xboxGamepadId">The Id of the XboxGamepad to disconnect.</param>
        public virtual void DisconnectXboxGamepad(string ipAddress, int xboxGamepadId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Disconnects all XboxGamepads from the console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        public virtual void DisconnectAllXboxGamepads(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Sets the state of the XboxGamepad.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <param name="xboxGamepadId">The Id of the XboxGamepad to set the state of.</param>
        /// <param name="report">The state to set the XboxGamepad to.</param>
        public virtual void SendGamepadReport(string ipAddress, int xboxGamepadId, XboxGamepadState report)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Creates an Xbox debug monitor client.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The Xbox debug monitor client.</returns>
        public virtual IXboxDebugMonitorClient CreateDebugMonitorClient(string ipAddress, XboxOperatingSystem operatingSystem)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Queries for and returns the system IP address of a console identified by the specified tools IP address.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of a console.</param>
        /// <returns>The system IP address of the console.</returns>
        public virtual string GetSystemIpAddress(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Queries for and returns a value of an Xbox configuration property (see xbconnect command line utility).
        /// </summary>
        /// <param name="ipAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <returns>The configuration property value.</returns>
        public virtual string GetConfigValue(string ipAddress, string key)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Sets an Xbox configuration property to the specified value (see xbconnect command line utility).
        /// </summary>
        /// <param name="ipAddress">The tools IP address of a console.</param>
        /// <param name="key">The configuration property name.</param>
        /// <param name="value">The configuration property value.</param>
        public virtual void SetConfigValue(string ipAddress, string key, string value)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
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
        public virtual Task<string> DeployPushAsync(string ipAddress, string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Runs an executable on the console.
        /// </summary>
        /// <param name="ipAddress">The tools IP address of the console.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputRecievedCallback">A callback method that will be called when there is output from the process.</param>
        public virtual void RunExecutable(string ipAddress, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputRecievedCallback)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Gets the list of users on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <returns>The enumeration of XboxUserDefinition instances.</returns>
        public virtual IEnumerable<XboxUserDefinition> GetUsers(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Pairs a controller to a user on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="userId">The user id of the user to pair.</param>
        /// <param name="controllerId">The controller of the id to pair.</param>
        public virtual void PairControllerToUser(string ipAddress, uint userId, ulong controllerId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Pairs a controller to a user on a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of console.</param>
        /// <param name="userId">The user id of the user to pair.</param>
        /// <param name="controllerId">The controller of the id to pair.</param>
        public virtual void PairControllerToUserExclusive(string ipAddress, uint userId, ulong controllerId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the specified console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>A pointer to the location in memory of the uncompressed frame buffer captured off the console.</returns>
        public virtual IntPtr CaptureScreenshot(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Captures an MP4 clip using the GameDVR service and writes it to the specified output path.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="outputPath">Full path of the MP4 file to create.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time (between 6 and 300).</param>
        public virtual void CaptureRecordedGameClip(string ipAddress, string outputPath, uint captureSeconds)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Uninstall a package from a given console based on its package full name.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the console to be affected.</param>
        /// <param name="packageFullName">The Pacakge Full Name of the package to be uninstalled.</param>
        public virtual void UninstallPackage(string systemIpAddress, string packageFullName)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Adds a guest user.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>The user id of the added guest user.</returns>
        public virtual uint AddGuestUser(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="emailAddress">The email address of the user to be added.</param>
        /// <returns>An XboxUserDefinition of the added user.</returns>
        public virtual XboxUserDefinition AddUser(string ipAddress, string emailAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes all users from the console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <remarks>Signed-in users are signed out before being removed from the console.</remarks>
        public virtual void DeleteAllUsers(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes the specified user from the console. 
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to be removed.</param>
        /// <remarks>A signed-in user is signed out before being removed from the console.</remarks>
        public virtual void DeleteUser(string ipAddress, XboxUserDefinition user)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Signs the given user into Xbox Live.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password of the for signing in. If a password has been stored on the console, <c>null</c> can be passed in.</param>
        /// <param name="storePassword">If <c>true</c>, saves the given password on the console for later use.</param>
        /// <returns>An XboxUserDefinition of the signed-in user.</returns>
        public virtual XboxUserDefinition SignInUser(string ipAddress, XboxUserDefinition user, string password, bool storePassword)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Signs out the given user.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="user">The user to sign-out.</param>
        /// <returns>An XboxUserDefinition of the signed-out user.</returns>
        public virtual XboxUserDefinition SignOutUser(string ipAddress, XboxUserDefinition user)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Creates a party for the given title ID (if one does not exist) and adds the given local users to it.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to add other users to the party.</param>
        /// <param name="localUserXuidsToAdd">User XUIDs to add to the party.</param>
        public virtual void AddLocalUsersToParty(string ipAddress, uint titleId, string actingUserXuid, string[] localUserXuidsToAdd)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Invites the given users on behalf of the acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="actingUserXuid">Acting user XUID on whose behalf to invite other users to the party.</param>
        /// <param name="remoteUserXuidsToInvite">Remote user XUIDs to invite to the party.</param>
        public virtual void InviteToParty(string ipAddress, uint titleId, string actingUserXuid, string[] remoteUserXuidsToInvite)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Removes the given users from the party belonging to the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to manage a party for.</param>
        /// <param name="localUserXuidsToRemove">Local user XUIDs to remove from the party.</param>
        public virtual void RemoveLocalUsersFromParty(string ipAddress, uint titleId, string[] localUserXuidsToRemove)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Returns the party ID belonging to the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get the associated party ID for.</param>
        /// <returns>ID of existing party used to accept or decline an invitation to the party.</returns>
        public virtual string GetPartyId(string ipAddress, uint titleId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Lists both the current members and the reserved members of the party belonging to given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="titleId">Title ID of the title to get party members for.</param>
        /// <returns>Party member user XUIDs, which may contain a mix of local and remote users.</returns>
        public virtual string[] GetPartyMembers(string ipAddress, uint titleId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Accepts the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to accept the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public virtual void AcceptInviteToParty(string ipAddress, string actingUserXuid, string partyId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Declines the party invitation on behalf of the given acting user to the party associated with the given title ID.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="actingUserXuid">XUID of acting user on whose behalf to decline the invitation.</param>
        /// <param name="partyId">Title ID of the party created by another user to accept the invitation to.</param>
        public virtual void DeclineInviteToParty(string ipAddress, string actingUserXuid, string partyId)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Gets the information about a console.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <returns>A XboxConsoleInfo containing information about the console.</returns>
        public virtual XboxConsoleInfo GetConsoleInfo(string ipAddress)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Registers a package.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="packagePath">The relative path on the consoles scratch drive to the package.</param>
        /// <returns>The package definition object that describes the package.</returns>
        public virtual XboxPackageDefinition RegisterPackage(string ipAddress, string packagePath)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Unregisters a package.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="packageFullName">The Pacakge Full Name of the package to be unregistered.</param>
        public virtual void UnregisterPackage(string ipAddress, string packageFullName)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Gets the available space that is available for app installation.
        /// </summary>
        /// <param name="ipAddress">The IP address of the console.</param>
        /// <param name="storageName">The name of the storage device to check. Allowed values are "internal" and null. </param>
        /// <returns>The number of bytes of freespace on the storage device on the specified console.</returns>
        public virtual ulong GetAvailableSpaceForAppInstallation(string ipAddress, string storageName)
        {
            throw new XboxConsoleFeatureNotSupportedException(NotSupportedMessage);
        }

        private static byte[] CreateRawAssembly(Stream streamFromResources)
        {
            if (streamFromResources == null || !streamFromResources.CanSeek || streamFromResources.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                int count;
                var buffer = new byte[4096];
                do
                {
                    count = streamFromResources.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, count);
                } 
                while (count != 0);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Sets native Dll search path.
        /// </summary>
        /// <param name="durangoXdkBinDirectory">The complete path to the XDK's "bin" directory.</param>
        private void SetNativeDllSearchPath(string durangoXdkBinDirectory)
        {
            if (!NativeMethods.SetDllDirectory(durangoXdkBinDirectory))
            {
                throw new InitializationFailedException("Call to Win32 function \"SetDllDirectory\" failed.  Failed to set the directory used to modify the search path for native DLLs.");
            }
        }

        /// <summary>
        /// Subscribes to current AppDomain's AssemblyResolve event.
        /// </summary>
        /// <param name="durangoXdkBinDirectory">The name of the XDK binary directory environment variable.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "We need to manually load the XTF assemblies from the XDK.")]
        private void SubscribeToAssemblyResolve(string durangoXdkBinDirectory)
        {
            // There are 2 ways we get the Xtf assemblies.
            // 1. For some versions of the Xdk, all of the managed wrappers were shipped with the XDK.
            //    In these circumstances, we have not embedded resources in our adapters, and we will
            //    need to find those on disk in the Durango Bin folder.
            // 2. For some versions of the Xdk, we have shipped a NuGet package for ManagedXtf. These
            //    assemblies should be included as embedded resources in the adapter assemblies.
            //    The ManagedXtf package may include updates that do not ship with any similarly named
            //    assemblies in the XDK, so give preference to the embedded resources instead of any
            //    on-disk assemblies of the same name.
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                AssemblyName assemblyName = new AssemblyName(args.Name);
                string fileName = assemblyName.Name + ".dll";
                string filePath = Path.Combine(durangoXdkBinDirectory, fileName);
                Assembly assembly = null;

                // use embedded resources from the adapter.
                var adapterAssembly = Assembly.GetAssembly(this.GetType());
                var fqpath = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.GetType().Namespace, fileName);
                try
                {
                    using (var streamFromResources = adapterAssembly.GetManifestResourceStream(fqpath))
                    {
                        var rawAssembly = CreateRawAssembly(streamFromResources);
                        if (rawAssembly != null)
                        {
                            assembly = Assembly.Load(rawAssembly);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // If the resource is not found, we want to return null.
                    assembly = null;
                }
                catch (BadImageFormatException)
                {
                    // If the resource is malformed, we want to return null.
                    // This occurs for both GetManifestResourceStream and Load with incompatible runtime requirements.
                    assembly = null;
                }

                // If the assembly was not loaded from the adapter, and it is on disk, load it from disk.
                if (assembly == null && File.Exists(filePath))
                {
                    assembly = Assembly.LoadFrom(filePath);
                }

                return assembly;
            };
        }
    }
}
