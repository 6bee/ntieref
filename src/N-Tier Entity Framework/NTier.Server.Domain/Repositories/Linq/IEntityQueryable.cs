// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    public interface IEntityQueryable<TEntity> : IDomainQueryable<TEntity>
        where TEntity : Entity
    {
        IEntityQueryable<TEntity> Include(string path);
    }
}
