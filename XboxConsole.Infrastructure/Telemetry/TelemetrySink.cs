//------------------------------------------------------------------------------
// <copyright file="TelemetrySink.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Telemetry
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Threading;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;

    /// <summary>
    /// Provides an in-process SQL Sink for XboxConsole Telemetry.
    /// </summary>
    public static class TelemetrySink
    {
        private const string LoggingConnectionString = "";   

        private static ObservableEventListener processEventListener;
        private static SinkSubscription<SqlDatabaseSink> sinkSubscription;

        private static object listenerMutex = new object();

        /// <summary>
        /// Starts the in-process SQL telemetry sink.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to make absolutely sure this does not impact the parent process in any way.")]
        public static void StartTelemetry()
        {
            lock (listenerMutex)
            {
                if (processEventListener == null)
                {
                    processEventListener = new ObservableEventListener();

                    try
                    {
                        sinkSubscription = processEventListener.LogToSqlDatabase("XboxConsoleTelemetry", LoggingConnectionString);
                        processEventListener.EnableEvents(XboxConsoleEventSource.Logger, EventLevel.LogAlways, Keywords.All);
                    }
                    catch (Exception)
                    {
                        processEventListener.DisableEvents(XboxConsoleEventSource.Logger);
                    }
                }
            }
        }

        /// <summary>
        /// Stops the in-process SQL telemetry sink.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to make absolutely sure this does not impact the parent process in any way.")]
        public static void StopTelemetry()
        {
            lock (listenerMutex)
            {
                if (processEventListener != null)
                {
                    try
                    {
                        // Disable all events
                        processEventListener.DisableEvents(XboxConsoleEventSource.Logger);

                        // Flush all remaining events before shutting down
                        if (sinkSubscription != null)
                        {
                            sinkSubscription.Sink.FlushAsync().Wait();
                            sinkSubscription.Dispose();
                        }

                        processEventListener.Dispose();
                    }
                    catch (Exception)
                    {
                        // Prevent Logging problems from having any user impact.
                    }
                }
            }
        }
    }
}
