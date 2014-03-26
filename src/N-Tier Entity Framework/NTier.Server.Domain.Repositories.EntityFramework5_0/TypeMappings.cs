// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Server.Domain.Repositories.EntityFramework
{
    internal static class TypeMappings
    {
        internal static System.Data.Objects.RefreshMode Map(this RefreshMode refreshMode)
        {
            switch (refreshMode)
            {
                case RefreshMode.ClientWins: 
                    return System.Data.Objects.RefreshMode.ClientWins;

                case RefreshMode.StoreWins: 
                    return System.Data.Objects.RefreshMode.StoreWins;

                default: 
                    throw new ArgumentException(string.Format("Unknown refresh mode: '{0}'", refreshMode), "refreshMode");
            }
        }

        internal static OptimisticConcurrencyException Map(this System.Data.OptimisticConcurrencyException ex)
        {
            var entities = ex.StateEntries.Map();
            return new OptimisticConcurrencyException(ex.Message, ex, entities);
        }

        internal static UpdateException Map(this System.Data.UpdateException ex)
        {
            var entities = ex.StateEntries.Map();
            return new UpdateException(ex.Message, ex, entities);
        }

        private static System.Collections.Generic.IEnumerable<Entity> Map(this IEnumerable<System.Data.Objects.ObjectStateEntry> stateEntries)
        {
            return ReferenceEquals(null, stateEntries) ? null : stateEntries.OfType<Entity>();
        }
    }
}
