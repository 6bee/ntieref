// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class Error
    {
        /// <summary>
        ///  The error type of this error entry
        /// </summary>
        [DataMember]
        public ErrorType Type { get; private set; }

        /// <summary>
        ///  The error message of this error entry
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// A list of properties to which this error entry is associated with
        /// </summary>
        [DataMember]
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        ///  Returns true if this entry is of type warning, false otherwise
        /// </summary>
        public bool IsWarning
        {
            get { return Type == ErrorType.Warning; }
        }

        /// <summary>
        /// Returns true if this entry is of type error, false otherwise
        /// </summary>
        public bool IsError
        {
            get { return Type == ErrorType.Error; }
        }

        /// <summary>
        /// Creates a new error entry
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <param name="memberNames">An orbitrary list of properties to which this error entry is associated with</param>
        public Error(string errorMessage, params string[] memberNames)
            : this(ErrorType.Error, errorMessage, memberNames)
        {
        }

        /// <summary>
        /// Creates a new error entry
        /// </summary>
        /// <param name="type">The type of this entry</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="memberNames">An orbitrary list of properties to which this error entry is associated with</param>
        public Error(ErrorType type, string errorMessage, params string[] memberNames)
        {
            this.Type = type;
            this.Message = errorMessage;
            this.MemberNames = memberNames.ToList().AsReadOnly();
        }

        /// <summary>
        /// Casts an error entry into a validation result
        /// </summary>
        public static explicit operator ValidationResult(Error error)
        {
            return new ValidationResult(error.Message, error.MemberNames);
        }

        /// <summary>
        /// Casts a validation result into en error entry
        /// </summary>
        public static explicit operator Error(ValidationResult validationResult)
        {
            return new Error(validationResult.ErrorMessage, validationResult.MemberNames.ToArray());
        }
    }
}
