// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    internal sealed class OrderedDomainQueryable<TEntity> : DomainQueryable<TEntity>, IOrderedDomainQueryable<TEntity> where TEntity : Entity
    {
        #region Private fields

        private readonly IOrderedQueryable<TEntity> _queryable;

        #endregion Private fields

        #region Constructor

        public OrderedDomainQueryable(IOrderedQueryable<TEntity> queryable, Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> expressionVisitor)
            : base(queryable, expressionVisitor)
        {
            _queryable = queryable;
        }

        #endregion Constructor

        IOrderedDomainQueryable<TEntity> IOrderedDomainQueryable<TEntity>.ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            var queryable = _queryable.ThenBy(keySelector);
            return new OrderedDomainQueryable<TEntity>(queryable, ExpressionVisitor);
        }

        IOrderedDomainQueryable<TEntity> IOrderedDomainQueryable<TEntity>.ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            var queryable = _queryable.ThenByDescending(keySelector);
            return new OrderedDomainQueryable<TEntity>(queryable, ExpressionVisitor);
        }
    }
}
