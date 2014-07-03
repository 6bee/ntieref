// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NTier.Common.Domain.Model;
using Remote.Linq;

namespace NTier.Client.Domain
{
    internal abstract partial class DataServiceQueryable<TEntity> : IDataServiceQueryable<TEntity> where TEntity : Entity<TEntity>
    {
        #region Inner classes

        internal sealed class Filter
        {
            private readonly System.Linq.Expressions.LambdaExpression _lambdaExpression;
            private readonly Remote.Linq.Expressions.LambdaExpression _queryExpression;

            public Filter(System.Linq.Expressions.LambdaExpression exp)
            {
                _lambdaExpression = exp;
                _queryExpression = null;
            }

            public Filter(Remote.Linq.Expressions.LambdaExpression exp)
            {
                _queryExpression = exp;
                _lambdaExpression = null;
            }

            public Remote.Linq.Expressions.LambdaExpression Expression
            {
                get { return _queryExpression ?? _lambdaExpression.ToRemoteLinqExpression(); }
            }
        }

        internal sealed class Sort
        {
            private readonly LambdaExpression _lambdaExpression;
            private readonly Remote.Linq.Expressions.SortDirection _orderingDirection;
            private readonly Remote.Linq.Expressions.LambdaExpression _queryExpression;

            public Sort(LambdaExpression exp, Remote.Linq.Expressions.SortDirection orderingDirection)
            {
                _lambdaExpression = exp;
                _orderingDirection = orderingDirection;
                _queryExpression = null;
            }

            public Sort(Remote.Linq.Expressions.LambdaExpression exp, Remote.Linq.Expressions.SortDirection orderingDirection)
            {
                _queryExpression = exp;
                _orderingDirection = orderingDirection;
                _lambdaExpression = null;
            }

            public Sort(Remote.Linq.Expressions.SortExpression exp)
            {
                _queryExpression = exp.Operand;
                _orderingDirection = exp.SortDirection;
                _lambdaExpression = null;
            }

            public Remote.Linq.Expressions.SortExpression Expression
            {
                get
                {
                    var expression = _queryExpression ?? _lambdaExpression.ToRemoteLinqExpression();
                    return Remote.Linq.Expressions.Expression.Sort(expression, _orderingDirection);
                }
            }
        }

        protected sealed class CallbackResult : ICallbackResult<TEntity>
        {
            private readonly IEntitySet<TEntity> _entitySet;
            private readonly Exception _exception;

            public CallbackResult(IEntitySet<TEntity> entitySet, Exception exception = null)
            {
                this._entitySet = entitySet;
                this._exception = exception;
            }

            public Exception Exception
            {
                get { return _exception; }
            }

            public bool IsFaulted
            {
                get { return _exception != null; }
            }

            public IEntitySet<TEntity> EntitySet
            {
                get { CheckException(); return _entitySet; }
            }

            private void CheckException()
            {
                if (_exception != null) throw new Exception("Query failed", _exception);
            }
        }

        #endregion  Inner classes
        
        #region Private fields/properties

        private static readonly MethodInfo EnumerableContainsMethodInfo = typeof(System.Linq.Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
        public IEntitySet<TEntity> EntitySet { get; private set; }
        protected readonly DataServiceQueryable<TEntity> Parent;

        #endregion Private fields/properties

        #region Constructor

        partial void Initialize();

        protected DataServiceQueryable(IEntitySet<TEntity> entitySet, DataServiceQueryable<TEntity> parent)
        {
            this.EntitySet = entitySet;
            this.Parent = parent;

            Initialize();
        }

        #endregion Constructor
        
        #region Query properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        public ClientInfo ClientInfo
        {
            get
            {
                // return client info if set
                if (_isClientInfoSetOnQuery)
                {
                    return _clientInfo;
                }

                // return parent client info
                if (Parent != null)
                {
                    return Parent.ClientInfo;
                }

                // ultimate parent returns client info of entity set
                return EntitySet.ClientInfo;
            }
            set
            {
                _clientInfo = value;
                _isClientInfoSetOnQuery = true;
            }
        }
        private ClientInfo _clientInfo = null;
        private bool _isClientInfoSetOnQuery = false;

        internal abstract bool? QueryTotalCount { get; set; }
        protected bool? ParentQueryTotalCount
        {
            get
            {
                var value = QueryTotalCount;
                if (Parent != null && !value.HasValue)
                {
                    value = Parent.ParentQueryTotalCount;
                }
                return value;
            }
        }

        internal abstract bool? QueryData { get; set; }
        protected bool? ParentQueryData
        {
            get
            {
                var value = QueryData;
                if (Parent != null && !value.HasValue)
                {
                    value = Parent.ParentQueryData;
                }
                return value;
            }
        }

        internal abstract ISet<string> IncludeValues { get; }
        protected ISet<string> ParentIncludeValues
        {
            get
            {
                return Parent == null ? IncludeValues : Parent.ParentIncludeValues.Union(IncludeValues).ToSet();
            }
        }

        internal abstract IList<Filter> Filters { get; }
        protected IEnumerable<Remote.Linq.Expressions.LambdaExpression> ParentFilters
        {
            get
            {
                var queryExpressions =
                    from filter in Filters
                    select filter.Expression;

                return Parent == null ? queryExpressions : Parent.ParentFilters.Concat(queryExpressions);
            }
        }

        internal abstract IList<Sort> Sortings { get; }
        protected IEnumerable<Remote.Linq.Expressions.SortExpression> ParentSortings
        {
            get
            {
                var queryExpressions =
                    from sorting in Sortings
                    select sorting.Expression;

                return IsSortingReset || Parent == null ? queryExpressions : Parent.ParentSortings.Concat(queryExpressions);
            }
        }

        protected bool IsSortingReset { set; get; }

        internal abstract int? SkipValue { get; set; }
        protected int? ParentSkipValue
        {
            get
            {
                var value = SkipValue;
                if (Parent != null && !value.HasValue)
                {
                    value = Parent.ParentSkipValue;
                }
                return value;
            }
        }

        internal abstract int? TakeValue { get; set; }
        protected int? ParentTakeValue
        {
            get
            {
                var value = TakeValue;
                if (Parent != null && !value.HasValue)
                {
                    value = Parent.ParentTakeValue;
                }
                return value;
            }
        }

        #endregion Query properties

        #region Linq operations

        public IDataServiceQueryable<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.Filters.Add(new Filter(where));
            return queriable;
        }

        public IDataServiceQueryable<TEntity> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values)
        {
            return WhereIn(propertySelector, values, false);
        }

        public IDataServiceQueryable<TEntity> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return WhereIn(propertySelector, values, false);
        }

        public IDataServiceQueryable<TEntity> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values)
        {
            return WhereIn(propertySelector, values, true);
        }

        public IDataServiceQueryable<TEntity> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return WhereIn(propertySelector, values, true);
        }

        private IDataServiceQueryable<TEntity> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values, bool doNegate)
        {
            //if (!values.Any())
            //{
            //    return Where(e => false);
            //}

            var queriable = new DataServiceQueryableImp<TEntity>(this);

            var collectionExpression = Expression.Constant(values);
            var compareExpression = Expression.Call(null, EnumerableContainsMethodInfo.MakeGenericMethod(typeof(TValue)), collectionExpression, propertySelector.Body) as Expression;
            if (doNegate)
            {
                compareExpression = Expression.MakeUnary(ExpressionType.Not, compareExpression, typeof(bool));
            }
            var parameter = propertySelector.Parameters[0];
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(compareExpression, parameter);
            queriable.Filters.Add(new Filter(lambdaExpression));

            return queriable;
        }

        public IOrderedDataServiceQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return OrderBy((LambdaExpression)orderBy);
        }
        
        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity> OrderBy(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity>(this);
            queriable.Sortings.Clear();
            queriable.Sortings.Add(new Sort(orderBy, Remote.Linq.Expressions.SortDirection.Ascending));
            return queriable;
        }

        public IOrderedDataServiceQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return OrderByDescending((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity> OrderByDescending(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity>(this);
            queriable.Sortings.Clear();
            queriable.Sortings.Add(new Sort(orderBy, Remote.Linq.Expressions.SortDirection.Descending));
            return queriable;
        }

        public IDataServiceQueryable<TEntity> Include(string include)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.IncludeValues.Add(include);
            return queriable;
        }

        public IDataServiceQueryable<TEntity> Skip(int number)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.SkipValue = number;
            return queriable;
        }

        public IDataServiceQueryable<TEntity> Take(int number)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.TakeValue = number;
            return queriable;
        }

        public IDataServiceQueryable<TEntity> SetClientInfo(ClientInfo clientInfo)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.ClientInfo = clientInfo;
            return queriable;
        }

        public IDataServiceQueryable<TEntity> IncludeTotalCount()
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.QueryTotalCount = true;
            return queriable;
        }

        public IDataServiceQueryable<TEntity> IncludeData(bool includeData = true)
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.QueryData = includeData;
            return queriable;
        }

        /// <summary>
        /// Prepares a query to get the total record count but no data
        /// </summary>
        public IDataServiceQueryable<TEntity> GetCount()
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.QueryData = false;
            queriable.QueryTotalCount = true;
            return queriable;
        }

        /// <summary>
        /// Gets a data service queriable for the specific entity type
        /// </summary>
        public IDataServiceQueryable<TEntity> AsQueryable()
        {
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            return queriable;
        }
  
        #endregion Linq operations
    }
}