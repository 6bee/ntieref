// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Common.Domain.Model
{
    public interface IResultSet : IChangeSet
    {
        void AddConcurrencyConflicts(IEnumerable<Entity> entities);
        
        bool HasConcurrencyConflicts { get; }
    }
}
