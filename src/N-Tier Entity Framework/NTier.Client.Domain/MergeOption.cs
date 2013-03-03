// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    /// <summary>
    /// Specifies how entities being loaded into the data context are merged with 
    /// entities already in the data context.
    /// </summary>
    public enum MergeOption
    {
        /// <summary>
        /// Entities that already exist in the data context are not getting replaced.
        /// This is the default behavior for queries.
        /// </summary>
        AppendOnly = 0,

        /// <summary>
        /// Any property changes made to entities in the data context are overwritten by server values.
        /// </summary>
        OverwriteChanges = 1,

        /// <summary>
        /// Unmodified properties of entities in the data context are overwritten with server values.
        /// </summary>
        PreserveChanges = 2,

        /// <summary>
        /// Entities are maintained in a Detached state and are not tracked in the data context.
        /// </summary>
        NoTracking = 3,
    }
}