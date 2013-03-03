// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NTier.Common.Domain.Model
{
    [DebuggerDisplay("{Name} (attributes: {_attributes == null ? 0 : _attributes.Count}, dynamic validators: {_validators == null ? 0 : _validators.Count})")]
    public sealed class PropertyInfos
    {
        internal PropertyInfos(string name, PropertyInfo propertyInfo, bool isPhysical, ReadOnlyCollection<Attribute> attributes)
        {
            Name = name;
            PropertyInfo = propertyInfo;
            IsPhysical = isPhysical;
            _attributes = attributes;
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
        public IEnumerable<Attribute> Attributes
        {
            get
            {
                if (_validators == null || _validators.Count == 0)
                {
                    return _attributes;
                }
                else
                {
                    return _attributes.Union(_validators.Cast<Attribute>());
                }
            }
        }
        private readonly ReadOnlyCollection<Attribute> _attributes;

        /// <summary>
        /// Gets a list of dynamically added ValidationAttribute attributes
        /// </summary>
        internal ICollection<ValidationAttribute> Validators
        {
            get
            {
                if (_validators == null)
                {
                    _validators = new List<ValidationAttribute>();
                }
                return _validators;
            }
        }
        private List<ValidationAttribute> _validators;

        /// <summary>
        /// Returns true if property if physically implemented, false otherwise.
        /// </summary>
        public readonly bool IsPhysical;
    }
}
