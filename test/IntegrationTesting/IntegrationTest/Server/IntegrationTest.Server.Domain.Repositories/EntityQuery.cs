using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Server.Domain.Repositories;

namespace IntegrationTest.Server.Domain.Repositories
{
    public class EntityQuery<TEntity> : IEntityQuery<TEntity> where TEntity : global::NTier.Common.Domain.Model.Entity
    {
        private readonly IEnumerable<TEntity> _entitySet;

        public EntityQuery(IEnumerable<TEntity> entitySet)
        {
            _entitySet = entitySet;
        }

        public long LongCount()
        {
            return _entitySet.LongCount();
        }

        public IEntityQuery<TEntity> Where(System.Linq.Expressions.Expression<Func<TEntity, bool>> where)
        {
            var predicate = where.Compile();
            return new EntityQuery<TEntity>(_entitySet.Where(predicate));
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entitySet.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entitySet.GetEnumerator();
        }
    }
}
