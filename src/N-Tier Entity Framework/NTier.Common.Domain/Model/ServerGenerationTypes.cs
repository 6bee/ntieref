// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    [Flags]
    public enum ServerGenerationTypes
    {
        None = 0x0,
        Insert = 0x1,
        Update = 0x2
    }
}
