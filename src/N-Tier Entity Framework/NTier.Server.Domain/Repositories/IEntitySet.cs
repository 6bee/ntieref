// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories.Linq;

namespace NTier.Server.Domain.Repositories
{
    public interface IEntitySet<TEntity> : IEnumerable<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Attaches an entity to the entity set
        /// </summary>
        /// <param name="entity">The entity to be attached</param>
        void Attach(TEntity entity);


        /// <summary>
        /// Gets a data service queriable for the specific entity type
        /// </summary>
        IEntityQueryable<TEntity> AsQueryable();
    }
}
