// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal sealed partial class OrderedDataServiceQueryable<TEntity, TBase> : DataServiceQueryable<TEntity, TBase>, IOrderedDataServiceQueryable<TEntity, TBase> 
        where TEntity : TBase
        where TBase : Entity
    {
        private readonly DataServiceQueryable<TEntity, TBase> _queryable;

        #region Constructor

        internal OrderedDataServiceQueryable(DataServiceQueryable<TEntity, TBase> queryable)
            : base(queryable.EntitySet, queryable)
        {
            _queryable = queryable;
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
        
        internal override TypeInfo OfTypeValue
        {
            get { return Parent.OfTypeValue; }
            set { Parent.OfTypeValue = value; }
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

        public IOrderedDataServiceQueryable<TEntity, TBase> ThenBy<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return ThenBy((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity, TBase> ThenBy(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity, TBase>(this);
            queriable.Sortings.Add(new Sort(orderBy, RLinq.SortDirection.Ascending));
            return queriable;
        }

        public IOrderedDataServiceQueryable<TEntity, TBase> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return this.ThenByDescending((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity, TBase> ThenByDescending(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity, TBase>(this);
            queriable.Sortings.Add(new Sort(orderBy, RLinq.SortDirection.Descending));
            return queriable;
        }

        #endregion Linq operations
    }
}
