// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    /// <summary>
    /// The exception that is thrown when an optimistic concurrency violation occurs in N-Tier Entity Framework.
    /// </summary>
    public sealed class OptimisticConcurrencyException : Exception
    {
        public readonly IEnumerable<StateEntry> StateEntries;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public OptimisticConcurrencyException(List<StateEntry> stateEntries)
            : base("Store update, insert, or delete statement affected an unexpected number of rows. Entities may have been modified or deleted since entities were loaded.")
        {
            this.StateEntries = stateEntries.AsReadOnly();
        }
    }
}
