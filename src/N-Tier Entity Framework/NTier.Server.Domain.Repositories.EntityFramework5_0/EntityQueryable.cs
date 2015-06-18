// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories.Linq;

namespace NTier.Server.Domain.Repositories.EntityFramework
{
    internal sealed class EntityQueryable<TEntity> : DomainQueryable<TEntity>, IEntityQueryable<TEntity>
        where TEntity : Entity
    {
        #region Private fields

        private readonly ObjectQuery<TEntity> _queryable;
        private readonly bool _asNoTracking;

        #endregion Private fields

        #region Constructor

        internal EntityQueryable(ObjectQuery<TEntity> queryable, Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> expressionVisitor, bool asNoTracking)
            : base(asNoTracking ? CreateNoTrackingQuery(queryable) : queryable, expressionVisitor)
        {
            _queryable = queryable;
            _asNoTracking = asNoTracking;
        }

        #endregion Constructor

        IEntityQueryable<TEntity> IEntityQueryable<TEntity>.Include(string path)
        {
            var queryable = _queryable.Include(path);
            return new EntityQueryable<TEntity>(queryable, ExpressionVisitor, _asNoTracking);
        }

        private static ObjectQuery<TEntity> CreateNoTrackingQuery(ObjectQuery<TEntity> query)
        {
            IQueryable queryable = (IQueryable)query;
            ObjectQuery<TEntity> objectQuery = (ObjectQuery<TEntity>)queryable.Provider.CreateQuery(queryable.Expression);
            objectQuery.MergeOption = MergeOption.NoTracking;
            return objectQuery;
        }
    }
}
