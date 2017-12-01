//------------------------------------------------------------------------------
// <copyright file="XboxProcess.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Represents a process on an Xbox console.
    /// </summary>
    public class XboxProcess : XboxItem
    {
        private object syncRoot = new object();
        private EventHandler<TextEventArgs> textReceived;

        /// <summary>
        /// Initializes a new instance of the XboxProcess class.
        /// </summary>
        /// <param name="definition">An object that defines this XboxProcess.</param>
        /// <param name="console">The console that this XboxProcess is associated with.</param>
        internal XboxProcess(XboxProcessDefinition definition, XboxConsole console)
            : base(console)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            this.Definition = definition;
        }

        /// <summary>
        /// Occurs when a text (output debug string) is received. Use this.StartDebug(...) and this.StopDebug(...) to start and stop receiving events.
        /// </summary>
        public event EventHandler<TextEventArgs> TextReceived
        {
            add
            {
                this.AddEvent<TextEventArgs>(ref this.textReceived, value, this.OnTextReceivedSubscribeImpl);
            }

            remove
            {
                this.RemoveEvent<TextEventArgs>(ref this.textReceived, value, this.OnTextReceivedUnsubscribeImpl);
            }
        }

        /// <summary>
        /// Gets the Xbox operating system the process is running in.
        /// </summary>
        public XboxOperatingSystem OperatingSystem 
        {
            get
            {
                return this.Definition.OperatingSystem;
            }
        }

        /// <summary>
        /// Gets the Id (PID) of the Xbox process.
        /// </summary>
        public uint ProcessId 
        {
            get
            {
                return this.Definition.ProcessId;
            }
        }

        /// <summary>
        /// Gets the file name of the Xbox process.
        /// </summary>
        public string ImageFileName 
        { 
            get
            {
                return this.Definition.ImageFileName;
            }
        }

        /// <summary>
        /// Gets or sets the definition object backing this object.
        /// This is a property and not a field so that it can be shimmed in the Unit Tests.
        /// </summary>
        private XboxProcessDefinition Definition { get; set; }

        /// <summary>
        /// Runs an executable on the Xbox.
        /// </summary>
        /// <param name="console">The console to run the executable on.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the application on.</param>
        public static void Run(XboxConsole console, string fileName, string arguments, XboxOperatingSystem operatingSystem)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            Run(console, fileName, arguments, operatingSystem, null);
        }

        /// <summary>
        /// Runs an executable on the Xbox.
        /// </summary>
        /// <param name="console">The console to run the executable on.</param>
        /// <param name="fileName">The path to an executable to start.</param>
        /// <param name="arguments">The command-line arguments to pass into the executable.</param>
        /// <param name="operatingSystem">The <see cref="Microsoft.Internal.GamesTest.Xbox.XboxOperatingSystem"/> to run the executable on.</param>
        /// <param name="outputReceivedCallback">A callback method that will be called when there is output from the process.</param>
        public static void Run(XboxConsole console, string fileName, string arguments, XboxOperatingSystem operatingSystem, Action<string> outputReceivedCallback)
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            if (console == null)
            {
                throw new ArgumentNullException("console");
            }

            if (console.IsDisposed)
            {
                throw new ObjectDisposedException("console");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is not valid", "fileName");
            }

            console.Adapter.RunExecutable(console.SystemIpAddressAndSessionKeyCombined, fileName, arguments, operatingSystem, outputReceivedCallback);
        }

        private void AddEvent<T>(ref EventHandler<T> eventBacking, EventHandler<T> value, Action subscribeAction) where T : EventArgs
        {
            lock (this.syncRoot)
            {
                if (eventBacking == null)
                {
                    eventBacking += value;
                    subscribeAction();
                }
                else
                {
                    eventBacking += value;
                }
            }
        }

        private void RemoveEvent<T>(ref EventHandler<T> eventBacking, EventHandler<T> value, Action unsubscribeAction) where T : EventArgs
        {
            lock (this.syncRoot)
            {
                eventBacking -= value;

                if (eventBacking == null)
                {
                    unsubscribeAction();
                }
            }
        }

        private void TextReceivedInternalHandler(object sender, TextEventArgs eventArgs)
        {
            if (this.textReceived != null)
            {
                this.textReceived(sender, eventArgs);
            }
        }

        private void OnTextReceivedSubscribeImpl()
        {
            this.Console.Adapter.StartDebug(this.Console.SystemIpAddressAndSessionKeyCombined, this.OperatingSystem, this.ProcessId, this.TextReceivedInternalHandler);
        }

        private void OnTextReceivedUnsubscribeImpl()
        {
            this.Console.Adapter.StopDebug(this.Console.SystemIpAddressAndSessionKeyCombined, this.OperatingSystem, this.ProcessId);
        }
    }
}
