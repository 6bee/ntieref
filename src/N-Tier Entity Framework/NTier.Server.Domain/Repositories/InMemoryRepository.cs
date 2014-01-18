// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories
{
    public abstract class InMemoryRepository : IRepository
    {
        #region fields

        private static readonly object StaticSyncRoot = new object();
        private readonly IDictionary<Type, object> _entitySets = new Dictionary<Type, object>();
        private bool disposed = false;

        #endregion fields

        #region methods

        protected internal virtual void OnInsert(Entity entity)
        {
        }
        protected internal virtual void OnUpdate(Entity entity)
        {
        }
        protected internal virtual void OnDelete(Entity entity)
        {
        }

        protected virtual IEntitySet<T> GetEntitySet<T>(ISet<T> source) where T : Entity
        {
            lock (StaticSyncRoot)
            {
                object entitySet;
                if (!_entitySets.TryGetValue(typeof(T), out entitySet))
                {
                    entitySet = new InMemoryEntitySet<T>(this, source);
                    _entitySets.Add(typeof(T), entitySet);
                }
                return (InMemoryEntitySet<T>)entitySet;
            }
        }

        public virtual void Refresh(RefreshMode refreshMode, Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Refresh(RefreshMode refreshMode, IEnumerable<Entity> collection)
        {
            foreach (var item in collection)
            {
                Refresh(refreshMode, item);
            }
        }

        public virtual int SaveChanges()
        {
            lock (StaticSyncRoot)
            {
                var count = 0;
                foreach (IInMemoryEntitySet set in _entitySets.Values)
                {
                    count += set.AcceptChanges();
                }
                return count;
            }
        }

        #region dispose

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _entitySets.Clear();
            }
        }

        ~InMemoryRepository()
        {
            Dispose(false);
        }

        #endregion dispose

        protected static ISet<T> Initialize<T>(params T[] items) where T : Entity
        {
            return SetUnchanged(items).ToSet();
        }

        private static IEnumerable<T> SetUnchanged<T>(IEnumerable<T> items) where T : Entity
        {
            foreach (var item in items)
            {
                item.ChangeTracker.State = ObjectState.Unchanged;
            }
            return items;
        }

        protected static T DeepCopy<T>(T obj) where T : Entity
        {
            return (T)obj.DeepCopy();
        }

        protected static T ShallowCopy<T>(T obj) where T : Entity
        {
            return (T)obj.ShallowCopy();
        }

        #endregion methods
    }
}
