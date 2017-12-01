//------------------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Static class to hold extension methods for the Task class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the task until either it finishes or a timeout is reached.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the task.</typeparam>
        /// <param name="task">The task instance to add a timeout to.</param>
        /// <param name="timeout">The length of time to wait for the task to finish.</param>
        /// <returns>A new task containing the result of the task parameter.</returns>
        /// <exception cref="System.TimeoutException">Throws a TimeoutException the task has taken longer than timeout.</exception>
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            // This pattern is described in the Task.Delay section here:
            // http://msdn.microsoft.com/en-us/library/hh873173(v=vs.110).aspx
            // Basically it allows for an awaitable version of the Task.Wait
            // without having to worry about exception being wrapped in
            // an AggregateException.
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task;
            }
            else
            {
                throw new TimeoutException("Task timed out");
            }
        }

        /// <summary>
        /// Awaits the task until either it finishes or a timeout is reached.
        /// </summary>
        /// <param name="task">The task instance to add a timeout to.</param>
        /// <param name="timeout">The length of time to wait for the task to finish.</param>
        /// <returns>A new task to wait on until the timeout is reached.</returns>
        /// <exception cref="System.TimeoutException">Throws a TimeoutException the task has taken longer than timeout.</exception>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            // This pattern is described in the Task.Delay section here:
            // http://msdn.microsoft.com/en-us/library/hh873173(v=vs.110).aspx
            // Basically it allows for an awaitable version of the Task.Wait
            // without having to worry about exception being wrapped in
            // an AggregateException.
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                await task;
            }
            else
            {
                throw new TimeoutException("Task timed out");
            }
        }
    }
}
