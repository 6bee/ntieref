// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Service
{
    internal static class ObjectStateExtensions
    {
        internal static UpdateOperations ToUpdateOperation(this ObjectState state)
        {
            switch (state)
            {
                case ObjectState.Added: 
                    return UpdateOperations.Add;
                case ObjectState.Deleted: 
                    return UpdateOperations.Delete;
                case ObjectState.Modified: 
                    return UpdateOperations.Change;
                default: 
                    return UpdateOperations.None;
            }
        }
    }
}
