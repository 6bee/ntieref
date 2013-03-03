// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Service
{
    public sealed class ChangeInterceptorArgs<TEntity>
    {
        public readonly object Repository;
        public readonly IChangeSet ChangeSet;
        public readonly TEntity Entity;
        public readonly UpdateOperations UpdateOperation;
        public readonly ClientInfo ClientInfo;

        public ChangeInterceptorArgs(object repository, IChangeSet changeSet, TEntity entity, UpdateOperations updateOperation, ClientInfo clientInfo)
        {
            this.Repository = repository;
            this.ChangeSet = changeSet;
            this.Entity = entity;
            this.UpdateOperation = updateOperation;
            this.ClientInfo = clientInfo;
        }
    }
}
