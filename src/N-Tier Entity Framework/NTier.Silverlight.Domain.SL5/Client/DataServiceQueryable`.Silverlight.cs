// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryable<TEntity, TBase>
    {
        #region ExecuteAsync

        public abstract void ExecuteAsync(Action<IQueryResult<TEntity, TBase>> callback = null);

        #endregion ExecuteAsync

        /// <summary>
        /// Gets a data service queriable for the specific entity type
        /// </summary>
        public IDataServiceQueryable<TEntity, TBase> AsQueryable()
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            return queriable;
        }
    }
}