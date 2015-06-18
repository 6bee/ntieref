// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    // Note: class is currently not sealed to allow obsolete PrimitivePropertyAttribute to inherit from SimplePropertyAttribute
    [AttributeUsage(AttributeTargets.Property)]
    public /*sealed*/ class SimplePropertyAttribute : Attribute
    {
    }
}
