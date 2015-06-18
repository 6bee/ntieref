// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace NTier.Client.Domain
{
    [Serializable]
    partial class OptimisticConcurrencyException 
    {
        private OptimisticConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
