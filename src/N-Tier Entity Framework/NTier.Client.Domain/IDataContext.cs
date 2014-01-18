// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public interface IDataContext : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets NTier.Common.Domain.Model.ClientInfo object to be passed to any server call
        /// </summary>
        ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// Gets or sets merge options to be used by default opon attaching entities retrieved from the server
        /// </summary>
        MergeOption MergeOption { get; set; }

        /// <summary>
        /// Gets or sets whether existing entities should be detached before attaching entites of a new query result
        /// </summary>
        bool DetachEntitiesUponNewQueryResult { get; set; }

        /// <summary>
        /// Gets whether any entity attached to this context has pending changes
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Gets or stes whether validation is enabled on all entities attached to this context. (Returns null if entites contain ambiguous settings.)
        /// </summary>
        bool? IsValidationEnabled { get; set; }

        /// <summary>
        /// Clears error entries an all entites attached to this context
        /// </summary>
        void ClearErrors();

        /// <summary>
        /// Accepts pending changes in all entities attached to this context
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        void AcceptChanges(bool onlyForValidEntities = false);

        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this context, re-attaches detached entities, and removes added entities
        /// </summary>
        void RevertChanges();
        
#if SILVERLIGHT
        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, ClientInfo clientInfo = null, Action<Exception> callback = null);
#else
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
#endif

        ///// <summary>
        ///// Applies any server generated values which are not yet applied to their corresponding entities and accepts all chnages of saved entites
        ///// </summary>
        //void AcceptSavedChanges();

        /// <summary>
        /// Discarts any server generated values which are not yet applied to their corresponding entities
        /// </summary>
        void DiscartServerGeneratedValues();

        /// <summary>
        /// Occurs upon completion of SaveChangesAsync method
        /// </summary>
        event EventHandler<AsyncCompletedEventArgs> SaveChangesCompleted;
    }
}
