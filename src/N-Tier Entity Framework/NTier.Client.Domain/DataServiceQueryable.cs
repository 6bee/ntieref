// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using Remote.Linq;
using Remote.Linq.ExpressionVisitors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTier.Client.Domain
{
    internal abstract partial class DataServiceQueryable
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
                get { return GetOrCreateRemoteExpression(_queryExpression, _lambdaExpression); }
            }
        }

        internal sealed class Sort
        {
            private readonly System.Linq.Expressions.LambdaExpression _lambdaExpression;
            private readonly Remote.Linq.Expressions.SortDirection _orderingDirection;
            private readonly Remote.Linq.Expressions.LambdaExpression _queryExpression;

            public Sort(System.Linq.Expressions.LambdaExpression exp, Remote.Linq.Expressions.SortDirection orderingDirection)
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
                    var expression = GetOrCreateRemoteExpression(_queryExpression, _lambdaExpression);
                    return Remote.Linq.Expressions.Expression.Sort(expression, _orderingDirection);
                }
            }
        }

        #endregion  Inner classes

        #region Private fields/properties

        private static readonly MethodInfo EnumerableContainsMethodInfo = typeof(System.Linq.Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
        protected readonly DataServiceQueryable Parent;

        #endregion Private fields/properties

        #region Constructor

        partial void Initialize();

        protected DataServiceQueryable(DataServiceQueryable parent)
        {
            this.Parent = parent;
            Initialize();
        }

        #endregion Constructor

        #region Query properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        public abstract ClientInfo ClientInfo { get; internal set; }

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

        internal abstract Aqua.TypeSystem.TypeInfo OfTypeValue { get; set; }
        internal Aqua.TypeSystem.TypeInfo ParentOfTypeValue
        {
            get
            {
                var value = OfTypeValue;
                if (!ReferenceEquals(null, Parent) && ReferenceEquals(null, value))
                {
                    value = Parent.ParentOfTypeValue;
                }
                return value;
            }
        }

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

        private static Remote.Linq.Expressions.LambdaExpression GetOrCreateRemoteExpression(Remote.Linq.Expressions.LambdaExpression remoteExpression, System.Linq.Expressions.LambdaExpression systemExpression)
        {
            return remoteExpression ?? ToRemoteExpression(systemExpression);
        }

        internal static Remote.Linq.Expressions.LambdaExpression ToRemoteExpression(System.Linq.Expressions.LambdaExpression systemExpression)
        {
            return systemExpression
                .ResolveDynamicPropertySelectors(throwOnInvalidProperty: true)
                .ToRemoteLinqExpression()
                .ReplaceGenericQueryArgumentsByNonGenericArguments();
        }
    }
}
