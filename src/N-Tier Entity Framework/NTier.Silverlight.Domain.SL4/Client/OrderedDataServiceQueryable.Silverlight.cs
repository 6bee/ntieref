// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    partial class OrderedDataServiceQueryable<TEntity>
    {
        public override void ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null)
        {
            Parent.ExecuteAsync(callback);
        }
    }
}