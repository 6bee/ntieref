// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public abstract class DataLoader<T> : IDataLoader<T> where T : Entity
    {
        internal protected readonly IEntitySet<T> EntitySet;

        protected DataLoader(IEntitySet<T> entitySet)
        {
            if (entitySet == null) throw new ArgumentNullException("entitySet");
            this.EntitySet = entitySet;
        }

        public abstract void Load(IEntityCollectionView<T> view);

        protected virtual void OnDataLoading()
        {
            var dataLoading = DataLoading;
            if (dataLoading != null)
            {
                dataLoading(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDataLoaded(IEntitySet<T> result)
        {
            var dataLoaded = DataLoaded;
            if (dataLoaded != null)
            {
                dataLoaded(this, new DataLoadedEventArgs<T>(result));
            }
        }

        public event EventHandler DataLoading;
        public event EventHandler<DataLoadedEventArgs<T>> DataLoaded;
    }
}
