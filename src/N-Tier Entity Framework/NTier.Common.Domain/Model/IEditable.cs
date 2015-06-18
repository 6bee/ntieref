// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Common.Domain.Model
{
    /// <summary>
    /// Interface for the UNIT OF WORK pattern for editable objects
    /// </summary>
    public interface IEditable
    {
        /// <summary>
        /// Returns true if the object supports editing, false otherwise
        /// </summary>
        bool CanEdit { get; }

        /// <summary>
        /// Returns true if the object is in edit mode and editing may be canceled, false otherwise
        /// </summary>
        bool CanCancelEdit { get; }

        /// <summary>
        /// Returns true if the object is in edit mode, false otherwise
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// Starts edit mode and transactional tracking of changes
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Ends edit mode and commits transactional tracked changes
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Ends edit mode and rolls back transactional tracked changes
        /// </summary>
        void CancelEdit();
    }
}
