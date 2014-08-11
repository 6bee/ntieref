// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace NTier.Common.Domain.Model
{
    [DebuggerDisplay("{Name} attributes: {Attributes.Count}")]
    public sealed class PropertyInfos
    {
        internal PropertyInfos(string name, PropertyInfo propertyInfo, bool isPhysical, IEnumerable<Attribute> attributes)
        {
            Name = name;
            PropertyInfo = propertyInfo;
            IsPhysical = isPhysical;
            Attributes = attributes.ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns the name of the property
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Returns the PropertyInfo for physical properties, null for virtual properties
        /// </summary>
        public readonly PropertyInfo PropertyInfo;

        /// <summary>
        /// Returns a list of attributes including dynamically added ValidationAttribute attributes
        /// </summary>
        public readonly ReadOnlyCollection<Attribute> Attributes;

        /// <summary>
        /// Returns true if property if physically implemented, false otherwise.
        /// </summary>
        public readonly bool IsPhysical;
    }
}
