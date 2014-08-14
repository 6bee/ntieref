// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NTier.Client.Domain
{
    internal abstract partial class DataServiceQueryable<TEntity, TBase> : DataServiceQueryable, IDataServiceQueryable<TEntity, TBase>
        where TEntity : TBase
        where TBase : Entity
    {
        #region Private fields/properties

        private static readonly MethodInfo EnumerableContainsMethodInfo = typeof(System.Linq.Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
        public IEntitySet<TBase> EntitySet { get; private set; }

        #endregion Private fields/properties

        #region Constructor

        partial void Initialize();

        protected DataServiceQueryable(IEntitySet<TBase> entitySet, DataServiceQueryable parent)
            : base(parent)
        {
            this.EntitySet = entitySet;

            Initialize();
        }

        #endregion Constructor

        #region Query properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        public override ClientInfo ClientInfo
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
            internal set
            {
                _clientInfo = value;
                _isClientInfoSetOnQuery = true;
            }
        }
        private ClientInfo _clientInfo = null;
        private bool _isClientInfoSetOnQuery = false;

        #endregion Query properties

        #region Linq operations

        public IDataServiceQueryable<TEntity, TBase> Where(Expression<Func<TEntity, bool>> where)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.Filters.Add(new Filter(where));
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values)
        {
            return WhereIn(propertySelector, values, false);
        }

        public IDataServiceQueryable<TEntity, TBase> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return WhereIn(propertySelector, values, false);
        }

        public IDataServiceQueryable<TEntity, TBase> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, params TValue[] values)
        {
            return WhereIn(propertySelector, values, true);
        }

        public IDataServiceQueryable<TEntity, TBase> WhereNotIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return WhereIn(propertySelector, values, true);
        }

        private IDataServiceQueryable<TEntity, TBase> WhereIn<TValue>(Expression<Func<TEntity, TValue>> propertySelector, IEnumerable<TValue> values, bool doNegate)
        {
            //if (!values.Any())
            //{
            //    return Where(e => false);
            //}

            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);

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

        public IOrderedDataServiceQueryable<TEntity, TBase> OrderBy<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return OrderBy((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity, TBase> OrderBy(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity, TBase>(this);
            queriable.Sortings.Clear();
            queriable.Sortings.Add(new Sort(orderBy, Remote.Linq.Expressions.SortDirection.Ascending));
            return queriable;
        }

        public IOrderedDataServiceQueryable<TEntity, TBase> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return OrderByDescending((LambdaExpression)orderBy);
        }

        // implementation for IQueriable
        internal IOrderedDataServiceQueryable<TEntity, TBase> OrderByDescending(LambdaExpression orderBy)
        {
            var queriable = new OrderedDataServiceQueryable<TEntity, TBase>(this);
            queriable.Sortings.Clear();
            queriable.Sortings.Add(new Sort(orderBy, Remote.Linq.Expressions.SortDirection.Descending));
            return queriable;
        }

        public IDataServiceQueryable<T, TBase> OfType<T>() where T : TEntity
        {
            var queriable = new DataServiceQueryableImp<T, TBase>(this.EntitySet, this);
            queriable.OfTypeValue = new Remote.Linq.TypeSystem.TypeInfo(typeof(T));
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> Include(string include)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.IncludeValues.Add(include);
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> Skip(int number)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.SkipValue = number;
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> Take(int number)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.TakeValue = number;
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> SetClientInfo(ClientInfo clientInfo)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.ClientInfo = clientInfo;
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> IncludeTotalCount()
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.QueryTotalCount = true;
            return queriable;
        }

        public IDataServiceQueryable<TEntity, TBase> IncludeData(bool includeData = true)
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.QueryData = includeData;
            return queriable;
        }

        /// <summary>
        /// Prepares a query to get the total record count but no data
        /// </summary>
        public IDataServiceQueryable<TEntity, TBase> GetCount()
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.QueryData = false;
            queriable.QueryTotalCount = true;
            return queriable;
        }

        #endregion Linq operations
    }
}
