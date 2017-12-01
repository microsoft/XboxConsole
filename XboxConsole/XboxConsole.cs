//------------------------------------------------------------------------------
// <copyright file="XboxConsole.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Internal.GamesTest.Xbox.Configuration;
    using Microsoft.Internal.GamesTest.Xbox.Deployment;
    using Microsoft.Internal.GamesTest.Xbox.Input;
    using Microsoft.Internal.GamesTest.Xbox.IO;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Represents a Durango console.
    /// </summary>
    public class XboxConsole : DisposableObject
    {
        /// <summary>
        /// Initializes static members of the XboxConsole class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Since this static constructor is doing more than initializing static fields, but is actually effecting global state, this rule is appropriate to suppress.")]
        static XboxConsole()
        {
            TelemetrySink.StartTelemetry();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                TelemetrySink.StopTelemetry();
            };

            XboxConsoleEventSource.Logger.ModuleLoaded(Process.GetCurrentProcess().ProcessName, WindowsIdentity.GetCurrent().Name, Dns.GetHostEntry("localhost").HostName, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the XboxConsole class with default console address.
        /// </summary>
        public XboxConsole()
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            string defaultConsole = DefaultConsole;
            if (defaultConsole == null)
            {
                throw new XboxException("Default XboxConsole constructor requires default console set on this machine, but it could not find it. Please make sure default console is set.");
            }

            string[] splitString = this.SplitDefaultConsoleString(defaultConsole);
            this.SystemIpAddressString = this.GetIPAddressFromConnectionString(splitString[0]).ToString();
            this.OriginalConnectionString = defaultConsole;

            if (splitString.Length >= 2)
            {
                this.SessionKey = splitString[1];
            }

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the XboxConsole class.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the Xbox kit.</param>
        public XboxConsole(IPAddress systemIpAddress)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            if (systemIpAddress == null)
            {
                throw new ArgumentNullException("systemIpAddress");
            }

            this.SystemIpAddressString = systemIpAddress.ToString();
            this.OriginalConnectionString = this.SystemIpAddressString;

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the XboxConsole class.
        /// </summary>
        /// <param name="systemIpAddress">The IP address of the Xbox kit.</param>
        /// <param name="sessionKey">
        /// The session key of the Xbox kit. The session key can either be null,
        /// string.Empty, or an alphanumeric string 31 characters or less.
        /// </param>
        public XboxConsole(IPAddress systemIpAddress, string sessionKey)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            if (systemIpAddress == null)
            {
                throw new ArgumentNullException("systemIpAddress");
            }

            this.SystemIpAddressString = systemIpAddress.ToString();
            this.SessionKey = sessionKey;
            this.OriginalConnectionString = this.SystemIpAddressAndSessionKeyCombined;

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the XboxConsole class.
        /// </summary>
        /// <param name="connectionString">The network name or IP address of the Xbox kit including optional session key appended with '+' separator.
        /// The network name is an alphanumeric string of 15 characters or less that cannot start with a digit.
        /// The session key is an alphanumeric string of 31 characters or less.
        /// </param>
        public XboxConsole(string connectionString)
        {
            XboxConsoleEventSource.Logger.ObjectCreated(XboxConsoleEventSource.GetCurrentConstructor());

            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            this.OriginalConnectionString = connectionString;

            int sessionKeySep = connectionString.IndexOf('+');

            if (sessionKeySep != -1)
            {
                // If connection string includes session key, remove it from connection string and store separately
                this.SessionKey = connectionString.Substring(sessionKeySep + 1);
                connectionString = connectionString.Substring(0, sessionKeySep);
            }

            this.Initialize();

            this.SystemIpAddressString = this.GetIPAddressFromConnectionString(connectionString).ToString();
            }

        /// <summary>
        /// Gets or sets the default console address.
        /// </summary>
        public static string DefaultConsole
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                using (var adapter = XboxConsoleAdapterFactory.CreateAdapterForInstalledXdk())
                {
                    return adapter.DefaultConsole;
                }
            }

            set
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("value");
                }

                using (var adapter = XboxConsoleAdapterFactory.CreateAdapterForInstalledXdk())
                {
                    adapter.DefaultConsole = value;
                }
            }
        }

        /// <summary>
        /// Gets the system IP address for this console.
        /// </summary>
        public IPAddress SystemIpAddress
        {
            get
            {
                this.ThrowIfDisposed();
                return IPAddress.Parse(this.SystemIpAddressString);
            }
        }

        /// <summary>
        /// Gets the system IP address for this console.
        /// </summary>
        [Obsolete("Use SystemIpAddress instead")]
        public IPAddress ToolsIpAddress
        {
            get
            {
                this.ThrowIfDisposed();

                return this.SystemIpAddress;
            }
        }

        /// <summary>
        /// Gets the connection string with which the XboxConsole object was originally initialized.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                this.ThrowIfDisposed();

                return this.OriginalConnectionString;
            }
        }

        /// <summary>
        /// Gets the Session key for this console. The session key can either
        /// be null, string.Empty, or an alphanumeric string 31 characters or less.
        /// </summary>
        public string SessionKey
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return this.SessionKeyString;
            }

            private set
            {
                // Settor is private because we don't want to mislead the user into thinking that they can change the
                // SessionKey without going through the Configuration property.
                if (string.IsNullOrEmpty(value) || System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-zA-Z0-9]{0,31}$"))
                {
                    this.SessionKeyString = value;
                }
                else
                {
                    throw new ArgumentException("value must be either null or an alphanumeric string of 31 characters or less.", "value");
                }
            }
        }

        /// <summary>
        /// Gets the configuration of this console.
        /// </summary>
        public IXboxConfiguration Configuration
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return new ReadOnlyXboxConfiguration(settingKey => this.Adapter.GetConfigValue(this.SystemIpAddressAndSessionKeyCombined, settingKey));
            }
        }

        /// <summary>
        /// Gets the ConsoleId of the console.
        /// </summary>
        public string ConsoleId
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return this.Adapter.GetConsoleInfo(this.SystemIpAddressAndSessionKeyCombined).ConsoleId;
            }
        }

        /// <summary>
        /// Gets the DeviceId of the console.
        /// </summary>
        public string DeviceId
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return this.Adapter.GetConsoleInfo(this.SystemIpAddressAndSessionKeyCombined).DeviceId;
            }
        }

        /// <summary>
        /// Gets the host name of the console.
        /// </summary>
        public string HostName
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return this.Adapter.GetConsoleInfo(this.SystemIpAddressAndSessionKeyCombined).HostName;
            }
        }

        /// <summary>
        /// Gets the CertType of the console.
        /// </summary>
        public XboxCertTypes CertType
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                return this.Adapter.GetConsoleInfo(this.SystemIpAddressAndSessionKeyCombined).CertType;
            }
        }

        /// <summary>
        /// Gets the list of all packages currently installed on the console.
        /// </summary>
        public IEnumerable<XboxPackage> InstalledPackages
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                IEnumerable<XboxPackageDefinition> packageDefinitions = this.Adapter.GetInstalledPackages(this.SystemIpAddressAndSessionKeyCombined);

                if (packageDefinitions == null)
                {
                    return Enumerable.Empty<XboxPackage>();
                }

                return packageDefinitions.Select(def => new XboxPackage(def, this));
            }
        }

        /// <summary>
        /// Gets a list of users on this console.
        /// </summary>
        public IEnumerable<XboxUser> Users
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                this.ThrowIfDisposed();

                var users = this.Adapter.GetUsers(this.SystemIpAddressAndSessionKeyCombined);

                if (users == null)
                {
                    return Enumerable.Empty<XboxUser>();
                }

                return users.Select(x => new XboxUser(this, x));
            }
        }

        /// <summary>
        /// Gets the session key for this console. This is the backing field for SessionKey.
        /// </summary>
        internal string SessionKeyString { get; private set; }

        /// <summary>
        /// Gets the system IP address for this console. This is the backing field for SystemIpAddress.
        /// </summary>
        internal string SystemIpAddressString { get; private set; }

        /// <summary>
        /// Gets the original connection string initialized with.
        /// </summary>
        internal string OriginalConnectionString { get; private set; }

        /// <summary>
        /// Gets the combined system IP address and session key for passing into the adapter.
        /// </summary>
        internal string SystemIpAddressAndSessionKeyCombined
        {
            get
            {
                if (string.IsNullOrEmpty(this.SessionKeyString))
                {
                    return this.SystemIpAddressString;
                }
                else
                {
                    return this.SystemIpAddressString + '+' + this.SessionKeyString;
                }
            }
        }

        /// <summary>
        /// Gets the Xbox console adapter to use.
        /// </summary>
        internal XboxConsoleAdapterBase Adapter { get; private set; }

        /// <summary>
        /// Gets the XboxGamepads associated with the console. Internal to be accessed by the shim framework.
        /// </summary>
        internal List<XboxGamepad> XboxGamepads { get; private set; }

        /// <summary>
        /// Gets the list of processes running on the console.
        /// </summary>
        /// <param name="operatingSystem">The Xbox operating system.</param>
        /// <returns>The enumeration of XboxProcessInfo instances.</returns>
        public IEnumerable<XboxProcess> GetRunningProcesses(XboxOperatingSystem operatingSystem)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            return this.Adapter.GetRunningProcesses(this.SystemIpAddressAndSessionKeyCombined, operatingSystem).Select(definition => new XboxProcess(definition, this));
        }

        /// <summary>
        /// Reboot the Xbox console and wait indefinitely for the reboot 
        /// process to complete.
        /// </summary>
        public void Reboot()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Reboot(Timeout.InfiniteTimeSpan);
        }

        /// <summary>Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.</summary>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        public void Reboot(TimeSpan timeout)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Adapter.Reboot(this.SystemIpAddressAndSessionKeyCombined, timeout);
        }

        /// <summary>
        /// Reboot the Xbox console and wait indefinitely for the reboot 
        /// process to complete.
        /// </summary>
        /// <param name="configurationToApply">The configuration to apply to the console before it reboots.</param>
        public void Reboot(XboxConfiguration configurationToApply)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Reboot(configurationToApply, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Reboot the Xbox console and wait no more than the specified amount of time for the console to become responsive again.
        /// </summary>
        /// <param name="configurationToApply">The configuration to apply to the console before it reboots.</param>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to become
        /// responsive again after initiating the reboot sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the reboot operation does not complete within the given timeout period.</exception>
        public void Reboot(XboxConfiguration configurationToApply, TimeSpan timeout)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            var originalIp = this.SystemIpAddressAndSessionKeyCombined;
            this.ApplyConfiguration(configurationToApply);
            var newIp = this.SystemIpAddressAndSessionKeyCombined;
            this.Adapter.Reboot(originalIp, newIp, timeout);
        }

        /// <summary>
        /// Shutdown the Xbox console and wait indefinitely for the operation to complete.
        /// </summary>
        public void Shutdown()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Shutdown(Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Shutdown the Xbox console and wait no more than the specified amount of time for the operation to complete.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for the Xbox console to complete the shutdown sequence.</param>
        /// <exception cref="System.TimeoutException">Thrown if the shutdown operation does not complete within the given timeout period.</exception>
        public void Shutdown(TimeSpan timeout)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Adapter.Shutdown(this.SystemIpAddressAndSessionKeyCombined, timeout);
        }

        /// <summary>
        /// Creates an XboxGamepad for this console.
        /// </summary>
        /// <returns>A new XboxGamepad.</returns>
        /// <remarks>
        /// Does not attempt to connect the XboxGamepad, and does not 
        /// provide control over a physical controller.
        /// </remarks>
        public XboxGamepad CreateXboxGamepad()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            var gamepad = new XboxGamepad(this);
            this.XboxGamepads.Add(gamepad);

            return gamepad;
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        public Task<XboxPackage> DeployPushAsync(string deployFilePath, bool removeExtraFiles)
        {
            this.ThrowIfDisposed();

            return this.DeployPushAsync(deployFilePath, removeExtraFiles, null, null, null);
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the deployment to complete.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        public Task<XboxPackage> DeployPushAsync(string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            return this.DeployPushAsync(deployFilePath, removeExtraFiles, cancellationToken, null, null, null);
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        public async Task<XboxPackage> DeployPushAsync(string deployFilePath, bool removeExtraFiles, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            if (!Directory.Exists(deployFilePath))
            {
                throw new DirectoryNotFoundException(string.Format("Directory \"{0}\" does not exist.", deployFilePath));
            }

            var packageDefinition = await this.Adapter.DeployPushAsync(this.SystemIpAddressAndSessionKeyCombined, deployFilePath, removeExtraFiles, progressMetric, progressError, progressExtraFile);
            if (packageDefinition == null)
            {
                throw new XboxConsoleException("Adapter returned an unexpected value");
            }

            return new XboxPackage(packageDefinition, this);
        }

        /// <summary>
        /// Push deploys loose files to the console.
        /// </summary>
        /// <param name="deployFilePath">The path to the folder to deploy.</param>
        /// <param name="removeExtraFiles"><c>true</c> to remove any extra files, <c>false</c> otherwise.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the deployment to complete.</param>
        /// <param name="progressMetric">The progress handler that the calling app uses to receive progress updates about metrics. This may be null.</param>
        /// <param name="progressError">The progress handler that the calling app uses to receive progress updates about errors. This may be null.</param>
        /// <param name="progressExtraFile">The progress handler that the calling app uses to receive progress updates about extra files. This may be null.</param>
        /// <returns>The task object representing the asynchronous operation whose result is the deployed package.</returns>
        public async Task<XboxPackage> DeployPushAsync(string deployFilePath, bool removeExtraFiles, CancellationToken cancellationToken, IProgress<XboxDeploymentMetric> progressMetric, IProgress<XboxDeploymentError> progressError, IProgress<XboxDeploymentExtraFile> progressExtraFile)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            if (!Directory.Exists(deployFilePath))
            {
                throw new DirectoryNotFoundException(string.Format("Directory \"{0}\" does not exist.", deployFilePath));
            }

            var packageDefinition = await this.Adapter.DeployPushAsync(this.SystemIpAddressAndSessionKeyCombined, deployFilePath, removeExtraFiles, cancellationToken, progressMetric, progressError, progressExtraFile);
            if (packageDefinition == null)
            {
                throw new XboxConsoleException("Adapter returned an unexpected value");
            }

            return new XboxPackage(packageDefinition, this);
        }

        /// <summary>
        /// Registers a package located on the TitleScratch drive.
        /// </summary>
        /// <param name="scratchPath">Relative path to the package on the TitleScratch drive, omitting the root specification.</param>
        /// <returns>An XboxPackage object that allows you to manipulate the package.</returns>
        public XboxPackage RegisterPackage(XboxPath scratchPath)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            if (scratchPath == null)
            {
                throw new ArgumentNullException("scratchPath");
            }

            return new XboxPackage(this.Adapter.RegisterPackage(this.SystemIpAddressAndSessionKeyCombined, scratchPath.FullName), this);
        }

        /// <summary>
        /// Returns the amount of space in bytes available for installation of packages.
        /// Currently specifying the storage device is not supported, future versions may add an overload with XboxPath.
        /// </summary>
        /// <returns>Space available in bytes.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method is appropriate because the Xbox has to be queried every time.")]
        public ulong GetAvailableSpaceForAppInstallation()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            return this.Adapter.GetAvailableSpaceForAppInstallation(this.SystemIpAddressAndSessionKeyCombined, null);
        }

        /// <summary>
        /// Captures a screenshot from the frame buffer of the console.
        /// </summary>
        /// <returns>A BitmapSource containing the uncompressed frame buffer captured off the current console.</returns>
        public BitmapSource CaptureScreenshot()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            IntPtr pointerToHBitmap = this.Adapter.CaptureScreenshot(this.SystemIpAddressAndSessionKeyCombined);
            BitmapSource managedBitmap = null;

            try
            {
                managedBitmap = Imaging.CreateBitmapSourceFromHBitmap(pointerToHBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (ArgumentNullException ex)
            {
                // Occurs when passing a IntPtr.Zero into CreateBitmapSourceFromHBitmap.
                // Shouldn't ever happen because the adapter checks for this.
                throw new XboxConsoleException("Failed to capture screenshot.", ex);
            }
            finally
            {
                NativeMethods.DeleteObject(pointerToHBitmap);
            }

            return managedBitmap;
        }

        /// <summary>
        /// Captures an MP4 clip using the GameDVR service and writes to specified output path.
        /// </summary>
        /// <param name="outputPath">Full path of the MP4 file to create.</param>
        /// <param name="captureSeconds">How many seconds to capture backward from current time (between 6 and 300).</param>
        public void CaptureRecordedGameClip(string outputPath, uint captureSeconds)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException("outputPath must not be null or empty.", "outputPath");
            }

            this.Adapter.CaptureRecordedGameClip(this.SystemIpAddressAndSessionKeyCombined, outputPath, captureSeconds);
        }

        /// <summary>
        /// Adds a user to the console.
        /// </summary>
        /// <param name="emailAddress">The email address of the user to add.</param>
        /// <returns>An XboxUser representing the added user.</returns>
        public XboxUser AddUser(string emailAddress)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            var user = this.Adapter.AddUser(this.SystemIpAddressAndSessionKeyCombined, emailAddress);

            if (user == null)
            {
                throw new XboxConsoleException("Failed to verify that user was added.");
            }

            return new XboxUser(this, user);
        }

        /// <summary>
        /// Adds a guest user to the console.
        /// </summary>
        /// <returns>The user id of the added guest user.</returns>
        public uint AddGuestUser()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            return this.Adapter.AddGuestUser(this.SystemIpAddressAndSessionKeyCombined);
        }

        /// <summary>
        /// Deletes the specified user from the console.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        public void DeleteUser(XboxUser user)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user", "user cannot be null");
            }

            this.Adapter.DeleteUser(this.SystemIpAddressAndSessionKeyCombined, user.Definition);
        }

        /// <summary>
        /// Deletes all users from the console.
        /// </summary>
        public void DeleteAllUsers()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.ThrowIfDisposed();

            this.Adapter.DeleteAllUsers(this.SystemIpAddressAndSessionKeyCombined);
        }

        /// <summary>
        /// Disposes of managed references hold by this class.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            List<XboxConsoleException> exceptionList = new List<XboxConsoleException>(this.XboxGamepads.Count);

            if (this.XboxGamepads.Any())
            {
                foreach (var gamepad in this.XboxGamepads.Where(g => g.IsConnected))
                {
                    try
                    {
                        gamepad.Disconnect();
                    }
                    catch (XboxConsoleException ex)
                    {
                        // We need to grab the exception since if we don't
                        // at least attempt to disconnect every gamepad,
                        // the console can quickly become unresponsive.
                        System.Diagnostics.Debug.WriteLine(ex);
                        exceptionList.Add(ex);
                    }

                    // This is needed because bug # 697097
                    Thread.Sleep(200);
                }

                // Allow gamepads sufficient time to disconnect; related to bug # 710723
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            try
            {
                this.Adapter.Dispose();
            }
            catch (XboxConsoleException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                exceptionList.Add(ex);
            }

            if (exceptionList.Count > 0)
            {
                throw new XboxAggregateException("Exceptions occured during disposal.", this.SystemIpAddressAndSessionKeyCombined, exceptionList);
            }
        }

        /// <summary>
        /// Performs initialization common to all construction methods.
        /// </summary>
        private void Initialize()
        {
            this.Adapter = XboxConsoleAdapterFactory.CreateAdapterForInstalledXdk();
            this.XboxGamepads = new List<XboxGamepad>();
        }

        /// <summary>
        /// Splits the default console string into it's constituent parts.
        /// </summary>
        /// <param name="defaultConsole">The default console string.</param>
        /// <returns>A string[] of length 2 where the first entry is the ip address and the second entry is the access key.</returns>
        private string[] SplitDefaultConsoleString(string defaultConsole)
        {
            return defaultConsole.Split('+');
        }

        /// <summary>
        /// Attempts to get an IP address from its string representation or a host name.
        /// </summary>
        /// <param name="connectionString">IP address or host name to parse into an IPAddress object.</param>
        /// <returns>Parsed IP address.</returns>
        private IPAddress GetIPAddressFromConnectionString(string connectionString)
        {
            IPAddress systemIpAddress = null;

            if (!IPAddress.TryParse(connectionString, out systemIpAddress))
            {
                // Resolve IP address from host name if passed host name
                systemIpAddress = Dns.GetHostAddresses(connectionString).FirstOrDefault(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            if (systemIpAddress == null)
            {
                throw new ArgumentException("Could not connect using IP address or host name passed in the connection string.", "connectionString");
            }

            return systemIpAddress;
        }

        /// <summary>
        /// Helper function to apply configuration before rebooting.
        /// </summary>
        /// <param name="configuration">Configuration to apply.</param>
        private void ApplyConfiguration(XboxConfiguration configuration)
        {
            configuration.SetSettingValues((key, value) =>
            {
                if (value != null)
                {
                    this.Adapter.SetConfigValue(this.SystemIpAddressAndSessionKeyCombined, key, value);
                }
            });

            if (configuration.SessionKey != null)
            {
                this.SessionKey = configuration.SessionKey;
            }
        }

        /// <summary>
        /// Contains all native helper methods used by XboxConsole.
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject([In] IntPtr hObject);
        }
    }
}
