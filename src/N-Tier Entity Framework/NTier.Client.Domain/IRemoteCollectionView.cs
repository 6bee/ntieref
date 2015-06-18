// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace NTier.Client.Domain
{
    public interface IRemoteCollectionView : ICollectionView, IFilteredCollectionView, IPagedCollectionView
    {
        TimeSpan RequestDelay { get; set; }
        event EventHandler RequestStarting;
        event EventHandler RequestCompleted;
    }
}
