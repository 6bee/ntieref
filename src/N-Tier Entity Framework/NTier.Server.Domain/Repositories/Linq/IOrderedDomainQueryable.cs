// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    public interface IOrderedDomainQueryable<TEntity> : IDomainQueryable<TEntity> 
        where TEntity : Entity
    {
        IOrderedDomainQueryable<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

        IOrderedDomainQueryable<TEntity> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);
    }
}
