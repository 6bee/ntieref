using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Server.Domain.Repositories;
using NTier.Common.Domain.Model;

namespace IntegrationTest.Server.Domain.Repositories
{
    public class EntitySet<TEntity> : IEntitySet<TEntity> where TEntity : global::NTier.Common.Domain.Model.Entity
    {
        private readonly ICollection<TEntity> _records = new HashSet<TEntity>();

        public EntitySet(params TEntity[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (var entity in args)
                {
                    _records.Add(entity.MarkAsUnchanged());
                }
            }
        }
        
        public void Attach(TEntity entity)
        {
            _records.Add(entity);
        }

        public IEntityQuery<TEntity> CreateCountQuery(NTier.Common.Domain.Model.Query query)
        {
            var entityQuery = new EntityQuery<TEntity>(_records);
            return entityQuery;
        }

        public IEntityQuery<TEntity> CreateEntityQuery(NTier.Common.Domain.Model.Query query)
        {
            var entityQuery = new EntityQuery<TEntity>(_records);
            return entityQuery;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }
    }
}
