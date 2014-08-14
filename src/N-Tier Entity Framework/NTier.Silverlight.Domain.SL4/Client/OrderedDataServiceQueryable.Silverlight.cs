// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    partial class OrderedDataServiceQueryable<TEntity, TBase>
    {
        public override void ExecuteAsync(Action<IQueryResult<TEntity, TBase>> callback = null)
        {
            _queryable.ExecuteAsync(callback);
        }
    }
}