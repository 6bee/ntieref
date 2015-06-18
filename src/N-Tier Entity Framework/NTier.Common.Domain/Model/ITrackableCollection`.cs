// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Specialized;

namespace NTier.Common.Domain.Model
{
    public interface ITrackableCollection<T> : IList<T>, INotifyCollectionChanged
    {
    }
}
