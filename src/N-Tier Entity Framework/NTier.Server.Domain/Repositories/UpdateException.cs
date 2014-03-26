// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories
{
    /// <summary>
    /// The exception that is thrown when modifications to object instances cannot be persisted to the data source.
    /// </summary>
    [Serializable]
    public class UpdateException : DataException
    {
        /// <summary>
        /// Initializes a new instance of System.Data.UpdateException.
        /// </summary>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public UpdateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of System.Data.UpdateException with a specialized error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public UpdateException(string message)
            : base(message)
        {
        }
   
        /// <summary>
        /// Initializes a new instance of System.Data.UpdateException with serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected UpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.UpdateException class that uses a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public UpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.UpdateException class that uses a specified error message, a reference to the inner exception, and an enumerable collection of System.Data.Objects.ObjectStateEntry objects.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="entities">The collection of System.Data.Objects.ObjectStateEntry objects.</param>
        public UpdateException(string message, Exception innerException, IEnumerable<Entity> entities)
            : base(message, innerException)
        {
            Entities = ReferenceEquals(null, entities) ? null : entities.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the entities for this UpdateException.
        /// </summary>
        /// <value>A collection of entities comprised of either a single entity and 0 or more relationships, or 0 entities and 1 or more relationships.</value>
        public ReadOnlyCollection<Entity> Entities
        {
            get;
            private set;
        }
    }
}
