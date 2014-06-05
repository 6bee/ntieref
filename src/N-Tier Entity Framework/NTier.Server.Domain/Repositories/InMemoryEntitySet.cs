// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories.Linq;

namespace NTier.Server.Domain.Repositories
{
    internal interface IInMemoryEntitySet
    {
        int AcceptChanges();
    }

    public class InMemoryEntitySet<TEntity> : IEntitySet<TEntity>, IInMemoryEntitySet where TEntity : Entity
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

        public void Attach(TEntity entity)
        {
            lock (_source)
            {
                switch (entity.ChangeTracker.State)
                {
                    case ObjectState.Added:
                        _repository.OnInsert(entity);
                        _source.Add((TEntity)entity.ShallowCopy());
                        break;

                    case ObjectState.Modified:
                    case ObjectState.Deleted:
                        var existing = _source.SingleOrDefault(i => Equals(i, entity));
                        if (!ReferenceEquals(existing, null) && !ReferenceEquals(existing, entity))
                        {
                            if (entity.ChangeTracker.State == ObjectState.Modified)
                            {
                                _repository.OnUpdate(entity);
                                var properties =
                                    from p in entity.PropertyInfos
                                    where p.IsPhysical &&
                                          p.Attributes.Any(attribute => attribute is SimplePropertyAttribute || attribute is ComplexPropertyAttribute) &&
                                          entity.ChangeTracker.ModifiedProperties.Contains(p.Name)
                                    select p.PropertyInfo;

                                foreach (var property in properties)
                                {
                                    var newValue = property.GetValue(entity, null);
                                    var oldValue = property.GetValue(existing, null);
                                    if (!object.Equals(newValue, oldValue))
                                    {
                                        property.SetValue(existing, newValue, null);
                                    }
                                }
                            }
                            else
                            {
                                _repository.OnDelete(entity);
                            }
                            existing.ChangeTracker.State = entity.ChangeTracker.State;
                        }
                        break;
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
