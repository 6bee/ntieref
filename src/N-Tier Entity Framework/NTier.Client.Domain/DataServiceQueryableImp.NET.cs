// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryableImp<TEntity, TBase> : IEnumerable<TEntity>, IQueryable<TEntity>
    {
        internal DataServiceQueryableImp(DataServiceQueryable<TEntity, TBase> source, IEnumerable<TBase> enumerable)
            : base(source.EntitySet, source)
        {
            this._enumerable = enumerable;
        }

        public override IEnumerator<TEntity> GetEnumerator()
        {
            IEnumerable<TBase> enumerable;
            if (_enumerable == null)
            {
                // framework implementation
                if (EntitySet is EntitySet<TBase>)
                {
                    enumerable = ((EntitySet<TBase>)EntitySet).Load(ClientInfo, Query);
                }
                else // client implementation e.g. mock-up
                {
                    enumerable = EntitySet;
                }
            }
            else // wrapped query
            {
                enumerable = _enumerable;
            }

            return enumerable.OfType<TEntity>().GetEnumerator();
        }
        private IEnumerable<TBase> _enumerable = null;
    }
}