// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Server.Domain.Service
{
    /// <summary>
    /// An enumeration used to specify the update operations that were performed on an entity.
    /// </summary>
    [Flags]
    public enum UpdateOperations
    {
        /// <summary>
        /// No operations were performed on the resource.
        /// </summary>
        None = 0,
        /// <summary>
        /// The entity was added.
        /// </summary>
        Add = 1,
        /// <summary>
        /// The entity was modified.
        /// </summary>
        Change = 2,
        /// <summary>
        /// The entity was deleted.
        /// </summary>
        Delete = 4,
    }
}
