﻿// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System.Linq;

namespace NTier.Server.Domain.Repositories.Linq
{
    internal sealed class EntityQueryable<TEntity> : DomainQueryable<TEntity>, IEntityQueryable<TEntity> where TEntity : Entity
    {
        public EntityQueryable(IQueryable<TEntity> queryable)
            : base(queryable)
        {
        }

        public IEntityQueryable<TEntity> Include(string path)
        {
            //throw new NotImplementedException();
            return this;
        }
    }
}
