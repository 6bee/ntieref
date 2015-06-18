// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;

namespace NTier.Server.Domain.Repositories
{
    /// <summary>
    /// The exception that is thrown when modifications to object instances cannot be persisted to the data source.
    /// </summary>
    [Serializable]
    public class UpdateException : DataException
    {
        /// <summary>
        /// Initializes a new instance of UpdateException.
        /// </summary>
        public UpdateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of UpdateException with a specialized error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UpdateException(string message)
            : base(message)
        {
        }
   
        /// <summary>
        /// Initializes a new instance of UpdateException with serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected UpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Entities = (ReadOnlyCollection<Entity>)info.GetValue("Entities", typeof(ReadOnlyCollection<Entity>));
        }

        /// <summary>
        /// Initializes a new instance of the UpdateException class that uses a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public UpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UpdateException class that uses a specified error message, a reference to the inner exception, and an enumerable collection of Entity objects.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="entities">The collection of Entity objects.</param>
        public UpdateException(string message, Exception innerException, IEnumerable<Entity> entities)
            : base(message, innerException)
        {
            Entities = ReferenceEquals(null, entities) ? null : entities.ToList().AsReadOnly();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Entities", Entities);
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
