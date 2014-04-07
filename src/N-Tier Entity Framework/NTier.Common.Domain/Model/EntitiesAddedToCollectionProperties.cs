// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [Serializable]
    [CollectionDataContract(Name = "EntitiesAddedToCollectionProperties",
        ItemName = "AddedEntitiesForProperty", KeyName = "CollectionPropertyName", ValueName = "AddedEntities")]
    public class EntitiesAddedToCollectionProperties : Dictionary<string, EntityList> { }
}
