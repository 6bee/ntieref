// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [Serializable]
    [CollectionDataContract(Name = "OriginalValuesDictionary",
        ItemName = "OriginalValues", KeyName = "Name", ValueName = "OriginalValue")]
    public class OriginalValuesDictionary : Dictionary<string, object>
    {
    }
}
