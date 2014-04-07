// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [CollectionDataContract(ItemName = "Entity")]
    public class EntityList : List<Entity> { }
}
