// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public abstract partial class DataContext<TResultSet> : DataContext, IDataContext where TResultSet : IResultSet
    {
        #region Private Members

        private IList<TResultSet> ResultSets
        {
            get
            {
                if (_resultSets == null)
                {
                    _resultSets = new List<TResultSet>();
                }
                return _resultSets;
            }
        }
        private IList<TResultSet> _resultSets = null;

        #endregion Private Members

        #region Save / Accept / Revert

        protected abstract void Refresh(TResultSet exchangeSet);
        protected abstract void HandleConcurrencyConflicts(TResultSet exchangeSet);


        ///// <summary>
        ///// Applies any server generated values which are not yet applied to their corresponding entities and accepts all chnages of saved entites
        ///// </summary>
        //public override void AcceptSavedChanges()
        //{
        //    AcceptChanges(true);
        //}

        /// <summary>
        /// Discarts any server generated values which are not yet applied to their corresponding entities
        /// </summary>
        public override void DiscartServerGeneratedValues()
        {
            ResultSets.Clear();
        }

        /// <summary>
        /// Accepts pending changes in all entities attached to this context
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        public override void AcceptChanges(bool onlyForValidEntities = false)
        {
            SuppressHasChanges = true;
            try
            {
                var hasValidationErrors = IsServerValidationExceptionSuppressed ? false : ResultSets.Any(resultSet => resultSet.Any(entity => entity.Errors.Any(error => error.IsError)));

                foreach (var submitChangesResult in ResultSets.ToArray())
                {
                    // refresh local data
                    Refresh(submitChangesResult);

                    // remove result from private list
                    ResultSets.Remove(submitChangesResult);
                }

                if (hasValidationErrors) throw new ServerValidationException();

                Invoke(delegate
                {
                    foreach (var entitySet in EntitySets)
                    {
                        entitySet.AcceptChanges(onlyForValidEntities);
                    }
                });
            }
            finally
            {
                SuppressHasChanges = false;
                OnPropertyChanged("HasChanges");
            }
        }

        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this context, re-attaches detached entities, and removes added entities
        /// </summary>
        public override void RevertChanges()
        {
            SuppressHasChanges = true;
            try
            {
                foreach (var entitySet in EntitySets)
                {
                    entitySet.RevertChanges();
                }
            }
            finally
            {
                SuppressHasChanges = false;
                OnPropertyChanged("HasChanges");
            }
        }

        #endregion Save / Accept / Revert
    }
}
