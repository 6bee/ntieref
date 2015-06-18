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
        /// Applies all changes of an entity or entity graph.
        /// </summary>
        /// <param name="entity">The entity to be applied</param>
        void ApplyChanges(TEntity entity);

        /// <summary>
        /// Attaches an entity or entity graph to the current entity set.
        /// </summary>
        /// <param name="entity">The entity to be attached</param>
        void Attach(TEntity entity);

        /// <summary>
        /// Detaches the given entity from the current entity set.
        /// </summary>
        /// <param name="entity">The entity to be detached</param>
        void Detach(TEntity entity);

        /// <summary>
        /// Marks the given entity as new and adds it to the current entity set.
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        void Add(TEntity entity);

        /// <summary>
        /// Marks the given entity for deletion and ensures it's beeing attached to the current entity set.
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        void Remove(TEntity entity);


        /// <summary>
        /// Gets an entity queriable for the specific entity type
        /// </summary>
        IEntityQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Gets an entity queriable where the entities returned will not be cached in the repository
        /// </summary>
        IEntityQueryable<TEntity> AsNoTrackingQueryable();
    }
}
