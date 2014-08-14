// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;

namespace NTier.Client.Domain
{
    public interface IQueryResult<TEntity, TBase> 
        where TEntity : TBase
        where TBase : Entity
    {
        Exception Exception { get; }
        bool IsFaulted { get; }
        IEnumerable<TEntity> ResultSet { get; }
        IEntitySet<TBase> EntitySet { get; }
    }
}
