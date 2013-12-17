// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

namespace NTier.Server.Domain.Repositories
{
    /// <summary>
    /// Specifies whether property changes made to objects are kept or replaced with property values from the data source.
    /// </summary>
    public enum RefreshMode
    {
        /// <summary>
        /// Property changes made to objects in the repository are replaced with values from the data source.
        /// </summary> 
        StoreWins = 1,

        /// <summary>
        /// Property changes made to objects in the repository are not replaced with values from the data source. 
        /// On the next call to SaveChanges(), these changes are sent to the data source.
        /// </summary>
        ClientWins = 2,
    }
}
