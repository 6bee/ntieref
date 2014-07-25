// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTier.Client.Domain
{
    /// <summary>
    /// The exception that is thrown when an optimistic concurrency violation occurs in N-Tier Entity Framework.
    /// </summary>
    public partial class UpdateException : Exception
    {
        public UpdateException(Exception innerException, IEnumerable<StateEntry> stateEntries)
            : this("Store update, insert, or delete statement affected an unexpected number of rows. Transaction was rolled back. See StateEntries property for a list of affected entities.", innerException, stateEntries)
        {
        }

        protected UpdateException(string message, Exception innerException, IEnumerable<StateEntry> stateEntries)
            : base(message, innerException)
        {
            StateEntries = stateEntries.ToList().AsReadOnly();
        }

        public ReadOnlyCollection<StateEntry> StateEntries { get; private set; }
    }
}
