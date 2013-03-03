// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;
using System.Collections.Generic;
using System.Data.Objects;

namespace NTier.Server.Domain.Repositories
{
    public interface IRepository : IDisposable
    {
        int SaveChanges();

        void Refresh(RefreshMode refreshMode, IEnumerable<Entity> collection);
        void Refresh(RefreshMode refreshMode, Entity entity);
    }
}
