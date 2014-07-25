// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NTier.Client.Domain
{
    /// <summary>
    /// The exception that is thrown when an optimistic concurrency violation occurs in N-Tier Entity Framework.
    /// </summary>
    public sealed partial class OptimisticConcurrencyException : UpdateException
    {
        public OptimisticConcurrencyException(Exception innerException, IEnumerable<StateEntry> stateEntries)
            : base("Store update, insert, or delete statement affected an unexpected number of rows. Entities may have been modified or deleted since entities were loaded. Transaction was rolled back. See StateEntries property for a list of affected entities.", innerException, stateEntries)
        {
        }
    }
}
