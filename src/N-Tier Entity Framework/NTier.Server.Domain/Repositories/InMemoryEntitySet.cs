// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Server.Domain.Repositories
{
    public interface IInMemoryEntitySet
    {
        int AcceptChanges();
    }

    public interface IInMemoryEntitySet<TEntity> : IEntitySet<TEntity>, IInMemoryEntitySet where TEntity : Entity
    {
    }

    public class InMemoryEntitySet<TEntity> : IInMemoryEntitySet<TEntity> where TEntity : Entity
    {
        private readonly InMemoryRepository _repository;
        private readonly ISet<TEntity> _source;

        public InMemoryEntitySet(InMemoryRepository repository, ISet<TEntity> source)
        {
            _repository = repository;
            _source = source;
        }

        public IEntityQueryable<TEntity> AsQueryable()
        {
            var query = from item in _source
                        where item.ChangeTracker.State == ObjectState.Unchanged
                        select item;
            return new EntityQueryable<TEntity>(query.AsQueryable());
        }

        public IEntityQueryable<TEntity> AsNoTrackingQueryable()
        {
            return AsQueryable();
        }

        int IInMemoryEntitySet.AcceptChanges()
        {
            lock (_source)
            {
                var modified = _source.Where(i => i.ChangeTracker.State == ObjectState.Added || i.ChangeTracker.State == ObjectState.Modified).ToList();
                var deleted = _source.Where(i => i.ChangeTracker.State == ObjectState.Deleted).ToList();

                if (modified.Count > 0) foreach (var item in modified) item.AcceptChanges();
                if (deleted.Count > 0) foreach (var item in deleted) _source.Remove(item);

                return modified.Count + deleted.Count;
            }
        }

        public void ApplyChanges(TEntity entity)
        {
            Attach(entity);
        }

        public void Attach(TEntity entity)
        {
            lock (_source)
            {
                switch (entity.ChangeTracker.State)
                {
                    case ObjectState.Added:
                        Add(entity);
                        break;

                    case ObjectState.Modified:
                        Update(entity);
                        break;

                    case ObjectState.Deleted:
                        Remove(entity);
                        break;
                }
            }
        }

        public void Detach(TEntity entity)
        {
        }

        public void Add(TEntity entity)
        {
            var copy = (TEntity)entity.ShallowCopy();
            _repository.OnInsert(copy);
            ApplyChangedProperties(copy, entity);
            copy.AcceptChanges();
            _source.Add(copy);
        }

        public void Remove(TEntity entity)
        {
            var existing = _source.SingleOrDefault(i => Equals(i, entity));
            if (!ReferenceEquals(existing, null))
            {
                var copy = (TEntity)entity.ShallowCopy();
                _repository.OnDelete(copy);
                _source.Remove(existing);
            }
        }

        private void Update(TEntity entity)
        {
            var existing = _source.SingleOrDefault(i => Equals(i, entity));
            if (!ReferenceEquals(existing, null))
            {
                var copy = (TEntity)entity.ShallowCopy();
                _repository.OnUpdate(copy);
                ApplyChangedProperties(copy, existing);
                existing.AcceptChanges();
            }
        }

        private static void ApplyChangedProperties(TEntity source, TEntity target)
        {
            var properties =
                from p in source.PropertyInfos
                where p.IsPhysical &&
                      p.Attributes.Any(attribute => attribute is SimplePropertyAttribute || attribute is ComplexPropertyAttribute) &&
                      source.ChangeTracker.ModifiedProperties.Contains(p.Name)
                select p.PropertyInfo;

            foreach (var property in properties)
            {
                var newValue = property.GetValue(source, null);
                var oldValue = property.GetValue(target, null);
                if (!object.Equals(newValue, oldValue))
                {
                    property.SetValue(target, newValue, null);
                }
            }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
