// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    abstract partial class DataContext
    {
        internal protected delegate QueryResult<TEntity> QueryDelegate<TEntity>(ClientInfo clientInfo, Query query) where TEntity : Entity;

        #region Save

        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="failOnValidationErrors">If set to true, exception is thrown if IsValid=false, otherwise invalid entities are skipped from saving silently</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        public abstract void SaveChanges(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, bool failOnValidationErrors = true, ClientInfo clientInfo = null);

        /// <summary>
        /// Saves pending changes asynchronously
        /// </summary>
        /// <remarks>Either use the task object returned or subscribe to the SaveChangesCompleted event to check for exeptions of the sync task.</remarks>
        /// <param name="acceptOption">Defines when changes should be accepted locally.</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages.</param>
        /// <param name="failOnValidationErrors">If set to true, exception is thrown if IsValid=false, otherwise invalid entities are skipped from saving silently</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server.</param>
        /// <param name="startImmediately">If set to true, async task is started automatically (has no effect if task sceduler is provided).</param>
        /// <param name="taskScheduler">If privided, async task is started automatically using task sceduler.</param>
        /// <param name="cancellationToken">Optional System.Threading.Tasks.Task.CancellationToken that the async task will observe.</param>
        /// <param name="taskCreationOptions">Optional System.Threading.Tasks.TaskCreationOptions used to customize the task's behavior.</param>
        /// <returns>The task being used for async execution</returns>
        public abstract Task SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, bool failOnValidationErrors = true, ClientInfo clientInfo = null, bool startImmediately = true, TaskScheduler taskScheduler = null, CancellationToken cancellationToken = default(CancellationToken), TaskCreationOptions taskCreationOptions = TaskCreationOptions.None);

        #endregion Save

        protected void Invoke(Action action, bool invokeAsync = false)
        {
            var dispatcher = Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
            }
            else if (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
            {
                // dispatcher must no longer be used
            }
            else if (invokeAsync)
            {
                dispatcher.BeginInvoke(action);
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }
    }
}
