// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    internal class DomainQueryable<TEntity> : IDomainQueryable<TEntity> 
        where TEntity : Entity
    {
        #region Private fields

        private readonly IQueryable<TEntity> _queryable;
        private readonly Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> _expressionVisitor;

        #endregion Private fields

        #region Constructor

        public DomainQueryable(IQueryable<TEntity> queryable, Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> expressionVisitor)
        {
            _queryable = queryable;
            _expressionVisitor = expressionVisitor == null ? e => e : expressionVisitor;
        }

        #endregion Constructor

        protected Func<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> ExpressionVisitor
        {
            get { return _expressionVisitor; }
        }

        IDomainQueryable<TEntity> IDomainQueryable<TEntity>.Where(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var queryable = _queryable.Where(predicate);
            return new DomainQueryable<TEntity>(queryable, _expressionVisitor);
        }

        IOrderedDomainQueryable<TEntity> IDomainQueryable<TEntity>.OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            var queryable = _queryable.OrderBy(keySelector);
            return new OrderedDomainQueryable<TEntity>(queryable, _expressionVisitor);
        }

        IOrderedDomainQueryable<TEntity> IDomainQueryable<TEntity>.OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            var queryable = _queryable.OrderByDescending(keySelector);
            return new OrderedDomainQueryable<TEntity>(queryable, _expressionVisitor);
        }

        IDomainQueryable<TEntity> IDomainQueryable<TEntity>.Skip(int count)
        {
            var queryable = _queryable.Skip(count);
            return new DomainQueryable<TEntity>(queryable, _expressionVisitor);
        }

        IDomainQueryable<TEntity> IDomainQueryable<TEntity>.Take(int count)
        {
            var queryable = _queryable.Take(count);
            return new DomainQueryable<TEntity>(queryable, _expressionVisitor);
        }

        IDomainQueryable<TEntity> IDomainQueryable<TEntity>.AsQueryable()
        {
            //var queryable = _queryable.AsQueryable();
            //return new DomainQueryable<TEntity>(queryable);
            return this;
        }

        TEntity IDomainQueryable<TEntity>.FirstOrDefault()
        {
            var entity = _queryable.FirstOrDefault();
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var entity = _queryable.FirstOrDefault(predicate);
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.First()
        {
            var entity = _queryable.First();
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.First(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var entity = _queryable.First(predicate);
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.SingleOrDefault()
        {
            var entity = _queryable.SingleOrDefault();
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var entity = _queryable.SingleOrDefault(predicate);
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.Single()
        {
            var entity = _queryable.Single();
            return entity;
        }

        TEntity IDomainQueryable<TEntity>.Single(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var entity = _queryable.Single(predicate);
            return entity;
        }

        int IDomainQueryable<TEntity>.Count()
        {
            var count = _queryable.Count();
            return count;
        }

        int IDomainQueryable<TEntity>.Count(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var count = _queryable.Count(predicate);
            return count;
        }

        long IDomainQueryable<TEntity>.LongCount()
        {
            var count = _queryable.LongCount();
            return count;
        }

        long IDomainQueryable<TEntity>.LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            predicate = _expressionVisitor(predicate);
            var count = _queryable.LongCount(predicate);
            return count;
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            var enumerator = _queryable.GetEnumerator();
            return enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            var enumerator = ((System.Collections.IEnumerable)_queryable).GetEnumerator();
            return enumerator;
        }
    }
}
