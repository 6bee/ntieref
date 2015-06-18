// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    [Flags]
    public enum ObjectState
    {
        Unchanged = 0x1,
        Added = 0x2,
        Modified = 0x4,
        Deleted = 0x8
    }
}
