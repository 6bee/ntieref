// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Threading;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial interface IDataContext
    {
        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        void SaveChanges(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, ClientInfo clientInfo = null);

        /// <summary>
        /// Saves pending changes asynchronously
        /// </summary>
        /// <remarks>Either use the task object returned or subscribe to the SaveChangesCompleted event to check for exeptions of the sync task.</remarks>
        /// <param name="acceptOption">Defines when changes should be accepted locally.</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages.</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server.</param>
        /// <param name="startImmediately">If set to true, async task is started automatically (has no effect if task sceduler is provided).</param>
        /// <param name="taskScheduler">If privided, async task is started automatically using task sceduler.</param>
        /// <param name="cancellationToken">Optional System.Threading.Tasks.Task.CancellationToken that the async task will observe.</param>
        /// <param name="taskCreationOptions">Optional System.Threading.Tasks.TaskCreationOptions used to customize the task's behavior.</param>
        /// <returns>The task being used for async execution</returns>
        System.Threading.Tasks.Task SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, ClientInfo clientInfo = null, bool startImmediately = true, System.Threading.Tasks.TaskScheduler taskScheduler = null, CancellationToken cancellationToken = default(CancellationToken), System.Threading.Tasks.TaskCreationOptions taskCreationOptions = System.Threading.Tasks.TaskCreationOptions.None);
    }
}
