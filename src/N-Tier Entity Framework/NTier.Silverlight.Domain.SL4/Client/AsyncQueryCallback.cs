// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    internal delegate void AsyncQueryCallback<TEntity>(IEnumerable<TEntity> entities, long? totalCount, Exception exception = null);
}
