// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain;
using NTier.Common.Domain.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace NTier.Client.Domain
{
    partial class DataContext<TResultSet>
    {
        #region Save
        
        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="failOnValidationErrors">If set to true, exception is thrown if IsValid=false, otherwise invalid entities are skipped from saving silently</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        public override void SaveChanges(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, bool failOnValidationErrors = true, ClientInfo clientInfo = null)
        {
            if (acceptOption == AcceptOption.Default)
            {
                acceptOption = System.Transactions.Transaction.Current != null ? AcceptOption.AcceptChangesOnTransactionCompleted : AcceptOption.AcceptChangesAfterSave;
            }

            if (clearErrors)
            {
                ClearErrors();
            }

            if (failOnValidationErrors && !IsValid)
            {
                throw new ValidationException("Validation failed for one or more entities.");
            }

            // submit data
            TResultSet resultSet;
            try
            {
                resultSet = SubmitChanges(clientInfo ?? ClientInfo);
            }
            catch (FaultException ex)
            {
                var faultExceptionType = ex.GetType();
                if (faultExceptionType.IsGenericType)
                {
                    var detailType = faultExceptionType.GetGenericArguments()[0];
                    var detailProperty = typeof(FaultException<>).MakeGenericType(detailType).GetProperty("Detail");
                    var detail = detailProperty.GetValue(ex, null);
                    if (detail is OptimisticConcurrencyFault)
                    {
                        var stateEntries = GetStateEntries(((OptimisticConcurrencyFault)detail).Entities);
                        throw new OptimisticConcurrencyException(null, stateEntries);
                    }
                    if (detail is UpdateFault)
                    {
                        var stateEntries = GetStateEntries(((UpdateFault)detail).Entities);
                        throw new UpdateException(null, stateEntries);
                    }
                }
                throw;
            }

            // accept changes
            switch (acceptOption)
            {
                case AcceptOption.AcceptChangesAfterSave:
                    {
                        // refresh local data
                        var hasValidationErrors = IsServerValidationExceptionSuppressed ? false : resultSet.Any(entity => entity.Errors.Any(e => e.IsError));                        
                        Refresh(resultSet);
                        if (hasValidationErrors) throw new ServerValidationException();
                        AcceptChanges(true);
                    }
                    break;
                case AcceptOption.None:
                    {
                        // store result in private list
                        // allowes user to apply result (accept changes) asynchronousely
                        ResultSets.Add(resultSet);

                        // remove result from private list in case of a transaction rollback
                        var transaction = System.Transactions.Transaction.Current;
                        if (transaction != null)
                        {
                            transaction.TransactionCompleted += (sender, e) =>
                            {
                                if (e.Transaction.TransactionInformation.Status != TransactionStatus.Committed)
                                {
                                    ResultSets.Remove(resultSet);
                                }
                            };
                        }
                    }
                    break;
                case AcceptOption.AcceptChangesOnTransactionCompleted:
                    {
                        var transaction = System.Transactions.Transaction.Current;
                        if (transaction == null)
                        {
                            throw new Exception(string.Format("{0}.{1} requires an active transaction scope.", acceptOption.GetType().Name, acceptOption));
                        }

                        // accept changes upon successfull completion of transaction
                        transaction.TransactionCompleted += (sender, e) =>
                        {
                            if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                            {
                                // refresh local data
                                var hasValidationErrors = IsServerValidationExceptionSuppressed ? false : resultSet.Any(entity => entity.Errors.Any(error => error.IsError));
                                Refresh(resultSet);
                                if (hasValidationErrors) throw new ServerValidationException();
                            }
                        };
                    }
                    break;
                default:
                    throw new Exception(string.Format("This {0} is not implemented: {1}", acceptOption.GetType().Name, acceptOption));
            }

            //if (resultSet.HasConcurrencyConflicts)
            //{
            //    HandleConcurrencyConflicts(resultSet);
            //}
        }

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
        public override Task SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, bool failOnValidationErrors = true, ClientInfo clientInfo = null, bool startImmediately = true, TaskScheduler taskScheduler = null, CancellationToken cancellationToken = default(CancellationToken), TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
        {
            var task = new Task(
                () =>
                {
                    // save operation
                    Exception error = null;
                    try
                    {
                        SaveChanges(acceptOption, clearErrors, failOnValidationErrors, clientInfo);
                    }
                    catch (Exception e)
                    {
                        if (!HasSaveChangesCompletedHandler && !cancellationToken.IsCancellationRequested)
                        {
                            // if error occurred and no completed event handler registered, exception is re-thrown and is required to be handled using the task instance
                            throw;
                        }

                        error = e;
                    }

                    // return upon cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    OnSaveChangesCompleted(error);
                },
                cancellationToken == default(CancellationToken) ? CancellationToken.None : cancellationToken,
                taskCreationOptions
            );

            if (taskScheduler != null)
            {
                task.Start(taskScheduler);
            }
            else if (startImmediately)
            {
                task.Start();
            }

            return task;
        }

        protected abstract TResultSet SubmitChanges(ClientInfo clientInfo = null);

        #endregion Save
    }
}
