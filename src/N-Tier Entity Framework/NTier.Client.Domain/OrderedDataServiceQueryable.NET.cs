// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;

namespace NTier.Client.Domain
{
    partial class OrderedDataServiceQueryable<TEntity, TBase>
    {
        public override IEnumerator<TEntity> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
    }
}