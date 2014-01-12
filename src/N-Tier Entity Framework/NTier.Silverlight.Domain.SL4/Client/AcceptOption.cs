// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    public enum AcceptOption
    {
        /// <summary>
        /// Default uses <see cref="AcceptChangesAfterSave"/>. (Default) 
        /// </summary>
        /// <remarks>
        /// Changes are accepted upon transaction completion by default in case of an existing transaction. 
        /// If no transaction exists, changes are accepted automatically. 
        /// </remarks>
        Default = 0,

        /// <summary>
        /// Changes are not accepted automatically, user may call AcceptChanges subsequently to accept changes.
        /// </summary>
        None = 1,

        /// <summary>
        /// Changes are accepted automatically.
        /// </summary>
        AcceptChangesAfterSave = 2,
    }
}
