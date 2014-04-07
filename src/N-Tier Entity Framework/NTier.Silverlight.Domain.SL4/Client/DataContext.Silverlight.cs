// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    abstract partial class DataContext
    {
        internal protected delegate void QueryDelegate<TEntity>(ClientInfo clientInfo, Query query, Action<QueryResult<TEntity>, Exception> callback) where TEntity : Entity;

        #region Save

        public abstract void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, ClientInfo clientInfo = null, Action<Exception> callback = null);

        #endregion Save

        protected void Invoke(Action action)
        {
            var dispatcher = Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }

    partial class DataContext<TResultSet>
    {
        #region Save
        
        protected delegate void SaveChangesCallback(TResultSet resultSet, Exception exception = null);

        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        public override void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, ClientInfo clientInfo = null, Action<Exception> callback = null)
        {
            if (acceptOption == AcceptOption.Default)
            {
                //acceptOption = Transaction.Current != null ? AcceptOption.AcceptChangesOnTransactionCompleted : AcceptOption.AcceptChangesAfterSave;
                acceptOption = AcceptOption.AcceptChangesAfterSave;
            }

            if (clearErrors)
            {
                ClearErrors();
            }

            SubmitChangesAsync(
                clientInfo ?? ClientInfo,
                (resultSet, exception) =>
                {
                    var error = exception;
                    if (error == null)
                    {
                        try
                        {
                            // accept changes
                            switch (acceptOption)
                            {
                                case AcceptOption.AcceptChangesAfterSave:
                                    // refresh local data
                                    var hasValidationErrors = IsServerValidationExceptionSuppressed ? false : resultSet.Any(entity => entity.Errors.Any(e => e.IsError));
                                    Refresh(resultSet);
                                    if (hasValidationErrors) throw new ServerValidationException();
                                    AcceptChanges(true);
                                    break;
                                
                                case AcceptOption.None:
                                    // store result in private list
                                    // allowes user to apply result (accept changes) asynchronousely
                                    ResultSets.Add(resultSet);
                                    break;
                                
                                default:
                                    throw new Exception(string.Format("This {0} is not implemented: {1}", acceptOption.GetType().Name, acceptOption));
                            }

                            if (resultSet.HasConcurrencyConflicts)
                            {
                                // handle conflicts on calling thread (I'm not sure if this makes sense)
                                HandleConcurrencyConflicts(resultSet);
                            }
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }
                    }

                    if (callback != null)
                    {
                        Invoke(delegate
                        {
                            callback(error);
                        });
                    }
                    OnSaveChangesCompleted(error);
                }
            );
        }

        protected abstract void SubmitChangesAsync(ClientInfo clientInfo, SaveChangesCallback callback);

        #endregion Save
    }
}
