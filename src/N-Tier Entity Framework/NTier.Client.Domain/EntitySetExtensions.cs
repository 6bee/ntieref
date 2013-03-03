// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public static class EntitySetExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEntitySet<T> entitySet, bool reflectChangesOfEntitySet = false) where T : Entity<T>
        {
            var observableCollection = new ObservableEntityCollection<T>(entitySet, reflectChangesOfEntitySet);
            return observableCollection;
        }

        public static EntityCollectionView<T> ToRemoteCollectionView<T>(this IEntitySet<T> entitySet, TimeSpan requestDelay = default(TimeSpan)) where T : Entity<T>
        {
            var queryable = entitySet.AsQueryable();

            var loader = new RemoteDataLoader<T>(queryable);
            if (requestDelay != default(TimeSpan))
            {
                loader.RequestDelay = requestDelay;
            }

            var collectionView = new EntityCollectionView<T>(loader);
            return collectionView;
        }
    }
}
