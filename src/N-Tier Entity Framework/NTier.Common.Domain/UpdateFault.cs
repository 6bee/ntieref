// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace NTier.Common.Domain
{
    /// <summary>
    /// The exception that is thrown when modifications to object instances cannot be persisted to the data source.
    /// </summary>
    [Serializable]
    [DataContract]
    public class UpdateFault
    {
        /// <summary>
        /// Initializes a new instance of the UpdateException class that uses a specified error message, a reference to the inner exception, and an enumerable collection of Entity objects.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="entities">The collection of Entity objects.</param>
        public UpdateFault(string message, IEnumerable<Entity> entities)
        {
            Message = message;
            Entities = ReferenceEquals(null, entities) ? null : entities.ToList().AsReadOnly();
        }

        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the entities for this UpdateException.
        /// </summary>
        [DataMember]
        public ReadOnlyCollection<Entity> Entities { get; private set; }
    }
}
