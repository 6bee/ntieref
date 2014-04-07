// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [Serializable]
    [CollectionDataContract(Name = "ExtendedPropertiesDictionary",
        ItemName = "ExtendedProperties", KeyName = "Name", ValueName = "ExtendedProperty")]
    public class ExtendedPropertiesDictionary : Dictionary<string, object> { }
}
