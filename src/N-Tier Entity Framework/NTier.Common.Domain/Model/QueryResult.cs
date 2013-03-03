// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    [DataContract(IsReference = true)]
    public class QueryResult<TEntity> where TEntity : Entity
    {
        [DataMember]
        public IList<TEntity> Data { get; set; }

        [DataMember]
        public Nullable<long> TotalCount { get; set; }
    }
}
