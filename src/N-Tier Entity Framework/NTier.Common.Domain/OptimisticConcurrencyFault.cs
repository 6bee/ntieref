// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain
{
    /// <summary>
    /// The exception that is thrown when an optimistic concurrency violation occurs.
    /// </summary>
    [Serializable]
    [DataContract]
    public class OptimisticConcurrencyFault : UpdateFault
    {
        /// <summary>
        /// Initializes a new instance of OptimisticConcurrencyException that uses a specified error message, a reference to the inner exception, and an enumerable collection of entities.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="entities">The enumerable collection of entities.</param>
        public OptimisticConcurrencyFault(string message, IEnumerable<Entity> entities)
            : base(message, entities)
        {
        }
    }
}
