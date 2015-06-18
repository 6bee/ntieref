// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NTier.Common.Domain.Model
{
    public sealed class PropertyChangingWithValuesEventArgs : PropertyChangingEventArgs
    {
        public PropertyChangingWithValuesEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
    }
}
