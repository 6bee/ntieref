// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal sealed partial class OrderedDataServiceQueryable<TEntity> : DataServiceQueryable<TEntity>, IOrderedDataServiceQueryable<TEntity> where TEntity : Entity
    {
        #region Constructor

        internal OrderedDataServiceQueryable(DataServiceQueryable<TEntity> queryable)
            : base(queryable.EntitySet, queryable)
        {
        }

        #endregion Constructor

        #region Query properties

        internal override bool? QueryTotalCount
        {
            get { return Parent.QueryTotalCount; }
            set { Parent.QueryTotalCount = value; }
        }

        internal override bool? QueryData
        {
            get { return Parent.QueryData; }
            set { Parent.QueryData = value; }
        }

        internal override ISet<string> IncludeValues
        {
            get { return Parent.IncludeValues; }
        }

        internal override IList<Filter> Filters
        {
            get { return Parent.Filters; }
        }

        internal override IList<Sort> Sortings
        {
            get { return Parent.Sortings; }
        }

        internal override int? SkipValue
        {
            get { return Parent.SkipValue; }
            set { Parent.SkipValue = value; }
        }

        internal override int? TakeValue
        {
            get { return Parent.TakeValue; }
            set { Parent.TakeValue = value; }
        }

        #endregion Query properties

        #region Linq operations

        public IOrderedDataServiceQueryable<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return ThenBy((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity> ThenBy(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity>(this);
            queriable.Sortings.Add(new Sort(orderBy, RLinq.SortDirection.Ascending));
            return queriable;
        }

        public IOrderedDataServiceQueryable<TEntity> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return this.ThenByDescending((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity> ThenByDescending(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity>(this);
            queriable.Sortings.Add(new Sort(orderBy, RLinq.SortDirection.Descending));
            return queriable;
        }

        #endregion Linq operations
    }
}
