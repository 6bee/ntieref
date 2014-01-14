// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories
{   
    /// <summary>
    /// The exception that is thrown when an optimistic concurrency violation occurs.
    /// </summary>
    [Serializable]
    public sealed class OptimisticConcurrencyException : UpdateException
    {
        /// <summary>
        /// Initializes a new instance of System.Data.OptimisticConcurrencyException.
        /// </summary>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public OptimisticConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of System.Data.OptimisticConcurrencyException with a specialized error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public OptimisticConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of System.Data.OptimisticConcurrencyException that uses a specified error message and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public OptimisticConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of System.Data.UpdateException with serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private OptimisticConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of System.Data.OptimisticConcurrencyException that uses a specified error message, a reference to the inner exception, and an enumerable collection of entities.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="entities">The enumerable collection of entities.</param>
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public OptimisticConcurrencyException(string message, Exception innerException, IEnumerable<Entity> entities)
            : base(message, innerException, entities)
        {
        }
    }
}
