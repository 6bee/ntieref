// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
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
            var entities = from entry in ex.StateEntries
                           where entry.Entity is NTier.Common.Domain.Model.Entity
                           select (NTier.Common.Domain.Model.Entity)entry.Entity;

            return new OptimisticConcurrencyException(ex.Message, ex, entities);
        }

        internal static UpdateException Map(this System.Data.UpdateException ex)
        {
            var entities = from entry in ex.StateEntries
                           where entry.Entity is Entity
                           select (Entity)entry.Entity;

            return new UpdateException(ex.Message, ex, entities);
        }
    }
}
