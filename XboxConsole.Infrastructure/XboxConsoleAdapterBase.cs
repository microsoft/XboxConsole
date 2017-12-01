//------------------------------------------------------------------------------
// <copyright file="XboxConsoleAdapterBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Adapter
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using Microsoft.Internal.GamesTest.Utilities;
    using Microsoft.Internal.GamesTest.Xbox.Exceptions;
    using Microsoft.Internal.GamesTest.Xbox.Interfaces;

    /// <summary>
    /// The base class for all XboxConsole adapters.  This class provides default a implementation
    /// for all parts of the Xbox Console API, even if they are not supported by one particular
    /// version of the XDK (in which case an exception is thrown).  It is assumed that the adapter
    /// for each version of the XDK will override the pieces of functionality that are available or
    /// different in that particular build.
    /// </summary>
    public abstract partial class XboxConsoleAdapterBase : DisposableObject
    {
        private const string NotSupportedMessage = "The feature that you are trying to use is not supported by the version of the XDK installed on your machine.";

        private static bool isStaticInitialized = false;
        private static object staticInitializationLock = new object();

        /// <summary>
        /// Initializes a new instance of the XboxConsoleAdapterBase class.
        /// </summary>
        /// <param name="toolsIpAddress">The "Tools Ip" address of the Xbox kit.</param>
        /// <param name="xboxXdk">The XboxXdk functional facade implementation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Required by design.")]
        protected XboxConsoleAdapterBase(IPAddress toolsIpAddress, IXboxXdk xboxXdk)
        {
            if (toolsIpAddress == null)
            {
                throw new ArgumentNullException("toolsIpAddress");
            }

            if (xboxXdk == null)
            {
                throw new ArgumentNullException("xboxXdk");
            }

            this.XboxXdk = xboxXdk;
            this.ThrowIfXdkNotInstalled();

            if (!isStaticInitialized)
            {
                lock (staticInitializationLock)
                {
                    if (!isStaticInitialized)
                    {
                        this.Initialize();
                        isStaticInitialized = true;
                    }
                }
            }

            this.ToolsIpAddress = toolsIpAddress;
        }

        /// <summary>
        /// Gets the tools IP address of the kit to be controlled.
        /// </summary>
        public IPAddress ToolsIpAddress { get; private set; }

        /// <summary>
        /// Gets the XboxXdk functional facade implementation.
        /// </summary>
        protected IXboxXdk XboxXdk { get; private set; }

        /// <summary>
        /// Gets the path to the root folder of XDK.
        /// </summary>
        protected virtual string PathToXdk
        {
            get 
            {
                return Environment.GetEnvironmentVariable("DurangoXdk");
            }
        }

        /// <summary>
        /// Gets the path to the "bin" folder of XDK.
        /// </summary>
        protected virtual string PathToXdkBin
        {
            get
            {
                return Path.Combine(this.PathToXdk, "bin");
            }
        }

        /// <summary>
        /// Throws XdkNotFoundException if XDK is not properly installed.
        /// </summary>
        protected virtual void ThrowIfXdkNotInstalled()
        {
            if (string.IsNullOrWhiteSpace(this.PathToXdk))
            {
                throw new XdkNotFoundException("Failed to find a value for the \"DurangoXdk\" environment variable. Ensure that the XDK is installed.");
            }

            string xdkBinDir = this.PathToXdkBin;
            if (!Directory.Exists(xdkBinDir))
            {
                throw new XdkNotFoundException(string.Format(CultureInfo.InvariantCulture, "Failed to find the expected directory containing XDK binary files. Ensure this directory exists: '{0}'", xdkBinDir));
            }
        }

        /// <summary>
        /// Initializes the adapter.
        /// </summary>
        protected virtual void Initialize()
        {
            this.SetNativeDllSearchPath(this.PathToXdkBin);
            this.SubscribeToAssemblyResolve(this.PathToXdkBin);
        }

        /// <summary>
        /// Sets native Dll search path.
        /// </summary>
        /// <param name="durangoXdkBinDirectory">The complete path to the XDK's "bin" directory.</param>
        protected void SetNativeDllSearchPath(string durangoXdkBinDirectory)
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
        protected void SubscribeToAssemblyResolve(string durangoXdkBinDirectory)
        {
            // We have purposely chosen not to include the Microsoft.Xbox.Xtf.* assemblies
            // with this library because at the time of this writing Durango is still
            // super-secret and we don't want to be distributing the platform team's
            // code.  Therefore, we need to locate the assemblies on the user's
            // machine at run-time.
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                AssemblyName assemblyName = new AssemblyName(args.Name);
                string fileName = assemblyName.Name + ".dll";
                string filePath = Path.Combine(durangoXdkBinDirectory, fileName);
                if (!File.Exists(filePath))
                {
                    return null;
                }
                else
                {
                    Assembly a = Assembly.LoadFrom(filePath);
                    return a;
                }
            };
        }
    }
}
