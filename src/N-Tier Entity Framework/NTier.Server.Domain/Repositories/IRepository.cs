// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories
{
    public interface IRepository : IDisposable
    {
        int SaveChanges();

        void Refresh(RefreshMode refreshMode, IEnumerable<Entity> collection);
        void Refresh(RefreshMode refreshMode, Entity entity);

        /// <summary>
        /// Returns the entity set for the specified type
        /// </summary>
        IEntitySet<T> GetEntitySet<T>() where T : Entity;

        /// <summary>
        /// Returns the type used for retrieval of the entity set, based on a specific entity type.
        /// </summary>
        /// <returns>Returns the base type in case of a derived type, otherwise the actual type is returned.</returns>
        Type GetEntitySetType(Type entityType);
    }
}
