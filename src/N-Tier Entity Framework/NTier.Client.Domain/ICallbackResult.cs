// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public interface ICallbackResult<TEntity> where TEntity : Entity, IObjectWithChangeTracker
    {
        Exception Exception { get; }
        bool IsFaulted { get; }
        IEntitySet<TEntity> EntitySet { get; }
    }
}
