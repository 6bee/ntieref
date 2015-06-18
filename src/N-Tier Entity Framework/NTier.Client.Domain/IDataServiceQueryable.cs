// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NTier.Client.Domain
{
    public partial interface IDataServiceQueryable<TEntity, TBase>
        where TEntity : TBase
        where TBase : Entity
    {
        IEntitySet<TBase> EntitySet { get; }

        #region Linq operations

        IDataServiceQueryable<TEntity, TBase> Where(Expression<Func<TEntity, bool>> predicate);

        IDataServiceQueryable<TEntity, TBase> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values);

        IDataServiceQueryable<TEntity, TBase> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values);

        IDataServiceQueryable<TEntity, TBase> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values);

        IDataServiceQueryable<TEntity, TBase> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values);

        IOrderedDataServiceQueryable<TEntity, TBase> OrderBy<TKey>(Expression<Func<TEntity, TKey>> orderBy);

        IOrderedDataServiceQueryable<TEntity, TBase> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy);

        IDataServiceQueryable<T, TBase> OfType<T>() where T : TEntity;

        IDataServiceQueryable<TEntity, TBase> Include(string include);

        IDataServiceQueryable<TEntity, TBase> Skip(int number);

        IDataServiceQueryable<TEntity, TBase> Take(int number);

        IDataServiceQueryable<TEntity, TBase> SetClientInfo(ClientInfo clientInfo);

        IDataServiceQueryable<TEntity, TBase> IncludeTotalCount();

        IDataServiceQueryable<TEntity, TBase> IncludeData(bool includeData = true);

        /// <summary>
        /// Prepares a query to get the total record count but no data
        /// </summary>
        IDataServiceQueryable<TEntity, TBase> GetCount();

        #endregion Linq operations
    }
}
