// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
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
        public override void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = true, bool failOnValidationErrors = true, ClientInfo clientInfo = null, Action<Exception> callback = null)
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

            if (failOnValidationErrors && !IsValid)
            {
                throw new ValidationException("Validation failed for one or more entities.");
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
