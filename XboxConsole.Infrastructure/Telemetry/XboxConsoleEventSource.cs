//------------------------------------------------------------------------------
// <copyright file="XboxConsoleEventSource.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Telemetry
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// EventSource for XboxConsole.
    /// </summary>
    [EventSource(Name = "XboxConsoleEventSource")]
    public class XboxConsoleEventSource : EventSource
    {
        private static readonly Lazy<XboxConsoleEventSource> Instance = new Lazy<XboxConsoleEventSource>(() => new XboxConsoleEventSource());
        private string sessionId;

        private XboxConsoleEventSource()
        {
            this.sessionId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the singleton instance of EventSource.
        /// </summary>
        public static XboxConsoleEventSource Logger
        {
            get { return Instance.Value; }
        }

        /// <summary>
        /// Analyzes the callstack to get details of the calling method.
        /// </summary>
        /// <returns>The signature of the calling method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "While this could be accomplished through a property, it would feel weird.")]
        [NonEvent]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace stack = new StackTrace();
            StackFrame frame = stack.GetFrames().Skip(1).FirstOrDefault(x => !x.GetMethod().Attributes.HasFlag(MethodAttributes.Private));

            if (frame != null)
            {
                string paramList = string.Join(",", frame.GetMethod().GetParameters().Select(x => x.Name));

                return string.Format(CultureInfo.CurrentCulture, "{0}.{1}({2})", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name, paramList);
            }

            return string.Empty;
        }

        /// <summary>
        /// Analyzes the callstack to get details of the calling constructor.
        /// </summary>
        /// <returns>The signature of the calling constructor.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "While this could be accomplished through a property, it would feel weird.")]
        [NonEvent]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentConstructor()
        {
            StackTrace stack = new StackTrace();
            StackFrame frame = stack.GetFrame(1);

            string paramList = string.Join(",", frame.GetMethod().GetParameters().Select(x => x.Name));

            return string.Format(CultureInfo.CurrentCulture, "{0}({1})", frame.GetMethod().DeclaringType.Name, paramList);
        }

        /// <summary>
        /// Logs a Module Loaded Event.
        /// </summary>
        /// <param name="parentApplication">The application which loaded the module.</param>
        /// <param name="userName">The user running the parent application.</param>
        /// <param name="machine">The machine on which the parent application is running.</param>
        /// <param name="version">The version of the XboxConsole assemblies.</param>
        [NonEvent]
        public void ModuleLoaded(string parentApplication, string userName, string machine, string version)
        {
            this.ModuleLoaded(parentApplication, userName, machine, version, this.sessionId);
        }

        /// <summary>
        /// Logs an Object Created Event.
        /// </summary>
        /// <param name="constructor">The name of the object that has been created.</param>
        [NonEvent]
        public void ObjectCreated(string constructor)
        {
            this.ObjectCreated(constructor, this.sessionId);
        }

        /// <summary>
        /// Logs a Method Called Event.
        /// </summary>
        /// <param name="method">The name of the method that has been called.</param>
        [NonEvent]
        public void MethodCalled(string method)
        {
            this.MethodCalled(method, this.sessionId);
        }

        [Event(10, Message = "User {1} loaded XboxConsole.", Level = EventLevel.Informational)]
        private void ModuleLoaded(string parentApplication, string userName, string machine, string version, string session)
        {
            this.WriteEvent(10, parentApplication, userName, machine, version, session);
        }

        [Event(11, Message = "Object created: {0}", Level = EventLevel.Informational)]
        private void ObjectCreated(string constructor, string session)
        {
            this.WriteEvent(11, constructor, session);
        }

        [Event(12, Message = "Method called: {0}.", Level = EventLevel.Informational)]
        private void MethodCalled(string method, string session)
        {
            this.WriteEvent(12, method, session);
        }
    }
}
