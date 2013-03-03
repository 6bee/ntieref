// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public partial interface IDataServiceQueryable<TEntity>
        where TEntity : Entity
    {
        IEntitySet<TEntity> EntitySet { get; }

        #region Linq operations

        IDataServiceQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        IDataServiceQueryable<TEntity> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values);

        IDataServiceQueryable<TEntity> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values);

        IDataServiceQueryable<TEntity> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values);

        IDataServiceQueryable<TEntity> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values);

        IOrderedDataServiceQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> orderBy);

        IOrderedDataServiceQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy);

        IDataServiceQueryable<TEntity> Include(string include);

        IDataServiceQueryable<TEntity> Skip(int number);

        IDataServiceQueryable<TEntity> Take(int number);

        IDataServiceQueryable<TEntity> SetClientInfo(ClientInfo clientInfo);

        IDataServiceQueryable<TEntity> IncludeTotalCount();

        IDataServiceQueryable<TEntity> IncludeData(bool includeData = true);

        /// <summary>
        /// Prepares a query to get the total record count but no data
        /// </summary>
        IDataServiceQueryable<TEntity> GetCount();

        #endregion Linq operations
    }
}
