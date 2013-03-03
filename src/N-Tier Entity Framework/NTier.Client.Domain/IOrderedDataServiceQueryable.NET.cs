// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Client.Domain
{
    partial interface IOrderedDataServiceQueryable<TEntity> : IEnumerable<TEntity>, IOrderedQueryable<TEntity> 
    {
    }
}
