// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Data.Objects;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    internal sealed class EntityQueryable<TEntity> : DomainQueryable<TEntity>, IEntityQueryable<TEntity>
        where TEntity : Entity
    {
        #region Private fields

        private readonly ObjectQuery<TEntity> _queryable;

        #endregion Private fields

        #region Constructor

        public EntityQueryable(ObjectQuery<TEntity> queryable, Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> expressionVisitor)
            : base(queryable, expressionVisitor)
        {
            _queryable = queryable;
        }

        #endregion Constructor

        IEntityQueryable<TEntity> IEntityQueryable<TEntity>.Include(string path)
        {
            var queryable = _queryable.Include(path);
            return new EntityQueryable<TEntity>(queryable, ExpressionVisitor);
        }
    }
}
