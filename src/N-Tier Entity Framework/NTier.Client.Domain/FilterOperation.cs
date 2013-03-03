// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    /// <summary>
    /// Represents the valid operations to create a FilterInfo.
    /// </summary>
    public enum FilterOperation
    {
        /// <summary>
        /// Represents a contains operation
        /// </summary>
        Contains,

        /// <summary>
        /// Represents a starts with operation
        /// </summary>
        StartsWith,

        /// <summary>
        /// Represents an ends with operation
        /// </summary>
        EndsWith,

        /// <summary>
        /// Represents an equal operation
        /// </summary>
        Equal,

        /// <summary>
        /// Represents a not equal operation
        /// </summary>
        NotEqual,
        
        /// <summary>
        /// Represents a greater than operation
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Represents a greater than or equal operation
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Represents a less than operation
        /// </summary>
        LessThan,
        
        /// <summary>
        /// Represents a less than or equal operation
        /// </summary>
        LessThanOrEqual,

        ///// <summary>
        ///// Represents an is empty operation
        ///// </summary>
        //IsEmpty,

        ///// <summary>
        ///// Represents an is not empty operation
        ///// </summary>
        //IsNotEmpty,
    }
}
