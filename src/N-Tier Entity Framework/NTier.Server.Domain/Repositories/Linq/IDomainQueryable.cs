// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    public interface IDomainQueryable<TEntity> : IEnumerable<TEntity>
        where TEntity : Entity
    {
        IDomainQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        IOrderedDomainQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

        IOrderedDomainQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

        IDomainQueryable<TEntity> Skip(int count);

        IDomainQueryable<TEntity> Take(int count);

        TEntity FirstOrDefault();

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity First();

        TEntity First(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault();

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity Single();

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        int Count();

        int Count(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        IDomainQueryable<TEntity> AsQueryable();
    }
}