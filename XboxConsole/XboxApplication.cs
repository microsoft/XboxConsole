//------------------------------------------------------------------------------
// <copyright file="XboxApplication.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Describes an application installed on an Xbox console.
    /// </summary>
    public class XboxApplication : XboxItem
    {
        /// <summary>
        /// Initializes a new instance of the XboxApplication class.
        /// </summary>
        /// <param name="definition">An object that defines this application.</param>
        /// <param name="package">The package that this application is associated with.</param>
        /// <param name="console">The console that this application is associated with.</param>
        internal XboxApplication(XboxApplicationDefinition definition, XboxPackage package, XboxConsole console)
            : base(console)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            
            this.Definition = definition;
            this.Package = package;
        }

        /// <summary>
        /// Gets the Application User Model Id for this application.  This is equal to "PackageFamilyName!ApplicationId".
        /// </summary>
        public string Aumid 
        {
            get
            {
                return this.Definition.Aumid;
            }
        }

        /// <summary>
        /// Gets the name of the application as defined by the package manifest.
        /// </summary>
        public string ApplicationId
        {
            get
            {
                return this.Definition.ApplicationId;
            }
        }

        /// <summary>
        /// Gets the package object associated with this application.
        /// </summary>
        public XboxPackage Package { get; private set; }

        /// <summary>
        /// Gets the definition object backing this application object.
        /// This is a property and not a field so that it can be shimmed in the Unit Tests.
        /// </summary>
        internal XboxApplicationDefinition Definition { get; private set; }

        /// <summary>
        /// Launches this application.
        /// </summary>
        public void Launch()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.LaunchApplication(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Launches this application with command line arguments.
        /// </summary>
        /// <param name="arguments">Command-line arguments to launch with.</param>
        public void Launch(string arguments)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.LaunchApplication(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition, arguments);
        }

        /// <summary>
        /// Snaps the specified application.
        /// </summary>
        /// <exception cref="XboxSnapException">Thrown when the application was unable to be snapped.</exception>
        public void Snap()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.SnapApplication(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }
    }
}
