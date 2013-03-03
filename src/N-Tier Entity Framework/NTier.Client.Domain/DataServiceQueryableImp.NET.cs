// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryableImp<TEntity> : IEnumerable<TEntity>, IQueryable<TEntity>
    {
        internal DataServiceQueryableImp(DataServiceQueryable<TEntity> source, IEnumerable<TEntity> enumerable)
            : base(source.EntitySet, source)
        {
            this._enumerable = enumerable;
        }

        public override IEnumerator<TEntity> GetEnumerator()
        {
            if (_enumerable == null)
            {
                // framework implementation
                if (EntitySet is EntitySet<TEntity>)
                {
                    return ((EntitySet<TEntity>)EntitySet).Load(this).GetEnumerator();
                }
                else // client implementation e.g. mock-up
                {
                    return EntitySet.GetEnumerator();
                }
            }
            else // wrapped query
            {
                return _enumerable.GetEnumerator();
            }
        }
        private IEnumerable<TEntity> _enumerable = null;
    }
}