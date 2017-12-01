//------------------------------------------------------------------------------
// <copyright file="XboxPackage.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Internal.GamesTest.Xbox.Telemetry;

    /// <summary>
    /// Describes a package installed on an Xbox console.
    /// Here is a link to a great video that explains the difference between Package Full Name and Package Family Name.
    /// http://www.youtube.com/watch?v=8cvSNT9ho58.
    /// </summary>
    public class XboxPackage : XboxItem
    {
        private readonly TimeSpan executionStatePollingTime = TimeSpan.FromSeconds(1);
        private readonly TimeSpan executionStatePollingEndingTime = TimeSpan.FromSeconds(15);
        private object syncRoot = new object();
        private EventHandler<PackageExecutionStateChangedEventArgs> executionStateChanged;
        private Task executionStatePollingTask;
        private CancellationTokenSource executionStatePollingCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the XboxPackage class.
        /// </summary>
        /// <param name="definition">An object that defines this package.</param>
        /// <param name="console">The console that this package is associated with.</param>
        internal XboxPackage(XboxPackageDefinition definition, XboxConsole console)
            : base(console)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            this.Definition = definition;

            this.Applications = definition.ApplicationDefinitions.Select(appDef => new XboxApplication(appDef, this, console));
        }

        /// <summary>
        /// Occurs when a change in the package state is detected.
        /// </summary>
        public event EventHandler<PackageExecutionStateChangedEventArgs> ExecutionStateChanged
        {
            add
            {
                this.AddEvent<PackageExecutionStateChangedEventArgs>(ref this.executionStateChanged, value, this.OnExecutionStateChangedSubscribeImpl);
            }

            remove
            {
                this.RemoveEvent<PackageExecutionStateChangedEventArgs>(ref this.executionStateChanged, value, this.OnExecutionStateChangedUnsubscribeImpl);
            }
        }

        /// <summary>
        /// Gets a string consisting of different pieces of information concatenated together with underscores.
        ///     PackageName_VersionNumber_ProcessorArchitecture_ResourceId_PublisherId
        ///     Consider this example:
        ///     NuiView.ERA_1.0.0.0_neutral__8wekyb3d8bbwe
        ///     Here we have:
        ///     PackageName = NuiView.ERA
        ///     Version = 1.0.0.0
        ///     ProcessorArchitecture = neutral
        ///     ResourceId = {blank}.  Note that this element is optional and is blank in this example.
        ///     PublisherId = 8wekyb3d8bbwe.
        /// </summary>
        public string FullName
        {
            get
            {
                return this.Definition.FullName;
            }
        }

        /// <summary>
        /// Gets a string consisting of two different pieces of information concatenated together with underscores.
        ///     PackageName_PublisherId
        ///     Consider this example:
        ///     NuiView.ERA_8wekyb3d8bbwe
        ///     Here we have:
        ///     PackageName = NuiView.ERA
        ///     PublisherId = 8wekyb3d8bbwe.
        /// </summary>
        public string FamilyName
        {
            get
            {
                return this.Definition.FamilyName;
            }
        }

        /// <summary>
        /// Gets the list of applications inside this package.
        /// </summary>
        public IEnumerable<XboxApplication> Applications { get; private set; }

        /// <summary>
        /// Gets the current execution state of this package.
        /// </summary>
        public PackageExecutionState ExecutionState
        {
            get
            {
                XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

                return this.Console.Adapter.QueryPackageExecutionState(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
            }
        }

        /// <summary>
        /// Gets the definition object backing this package object.
        /// This is a property and not a field so that it can be shimmed in the Unit Tests.
        /// </summary>
        internal XboxPackageDefinition Definition { get; private set; }

        /// <summary>
        /// Resumes execution of this package.
        /// </summary>
        public void Resume()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.ResumePackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Suspends execution of this package. Note: this may not work (or work incorrectly) with some system packages.
        /// </summary>
        public void Suspend()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.SuspendPackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Terminates execution of this package.
        /// </summary>
        public void Terminate()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.TerminatePackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Constrains execution of this package.
        /// </summary>
        public void Constrain()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.ConstrainPackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Unconstrains execution of this package.
        /// </summary>
        public void Unconstrain()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.UnconstrainPackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
        }

        /// <summary>
        /// Unsnaps the currently snapped application.
        /// </summary>
        public void Unsnap()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.UnsnapApplication(this.Console.SystemIpAddressAndSessionKeyCombined);
        }

        /// <summary>
        /// Uninstall this package.
        /// </summary>
        public void Uninstall()
        {
            XboxConsoleEventSource.Logger.MethodCalled(XboxConsoleEventSource.GetCurrentMethod());

            this.Console.Adapter.UninstallPackage(this.Console.SystemIpAddressAndSessionKeyCombined, this.Definition);
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

        private void ExecutionStatePollingAction(CancellationToken token)
        {
            // This task uses cancellation to end the polling when nobody's
            // listening. Therefore, this method just returns when a cancellation
            // is requested instead of throwing the cancellation exception.

            // Not checking the package for the initial state since it would be a
            // rather complex implementation to handle the case where the initial check
            // throws an exception and then gracefully allow certain types of exceptions
            // before finally starting the actual polling. It should be okay to 
            // have a event fired if the initial state is something other than
            // unknown.
            PackageExecutionState oldState = PackageExecutionState.Unknown;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Task.Delay(this.executionStatePollingTime).Wait();

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        var newState = this.ExecutionState;
                        if (oldState != newState)
                        {
                            if (this.executionStateChanged != null && !token.IsCancellationRequested)
                            {
                                this.executionStateChanged.Invoke(this, new PackageExecutionStateChangedEventArgs(this, oldState, newState));
                            }

                            oldState = newState;
                        }
                    }
                    catch (CannotConnectException ex)
                    {
                        // Unable to talk to the console right now.
                        // Will continue to poll in case communication is restored.

                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    catch (XboxConsoleException ex)
                    {
                        var innerException = ex.InnerException as COMException;
                        if (innerException != null)
                        {
                            switch (innerException.ErrorCode)
                            {
                                case unchecked((int)0x8007045B):
                                case unchecked((int)0x8C11040E):
                                    {
                                        // This error codes mean that system is in the process of shutting down.
                                        // Will continue to poll in case the console is started up again.
                                        System.Diagnostics.Debug.WriteLine(ex);
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch (XboxException ex)
            {
                // Something we can't handle has happened. The polling will be stopped
                // since we can't know what will happen if we continue to poll.

                System.Diagnostics.Debug.WriteLine(ex);

                if (this.executionStateChanged != null && !token.IsCancellationRequested)
                {
                    this.executionStateChanged.Invoke(this, new PackageExecutionStateChangedEventArgs(this, oldState, PackageExecutionState.Unknown, ex));
                }
            }
        }

        private void OnExecutionStateChangedSubscribeImpl()
        {
            this.executionStatePollingCancellationTokenSource = new CancellationTokenSource();
            var token = this.executionStatePollingCancellationTokenSource.Token;
            this.executionStatePollingTask = Task.Run(() => this.ExecutionStatePollingAction(token), token);
        }

        private void OnExecutionStateChangedUnsubscribeImpl()
        {
            try
            {
                this.executionStatePollingCancellationTokenSource.Cancel();
                this.executionStatePollingCancellationTokenSource = null;
                if (!this.executionStatePollingTask.Wait(this.executionStatePollingEndingTime))
                {
                    throw new TimeoutException("The Task to handle the ExecutionStateChanged event has become unresponsive");
                }
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
