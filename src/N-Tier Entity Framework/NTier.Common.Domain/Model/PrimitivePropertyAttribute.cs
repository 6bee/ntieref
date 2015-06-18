// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    [Obsolete("Use NTier.Common.Domain.Model.SimplePropertyAttribute instead.", false)]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimitivePropertyAttribute : SimplePropertyAttribute
    {
    }
}
