// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace NTier.Client.Domain
{
    public interface IEntityCollectionView<T> : IEntityCollectionView
    {
        new int TotalItemCount { get; set; }
        void Add(T item);
        void Clear();
    }

    public interface IEntityCollectionView : ICollectionView, IFilteredCollectionView, IPagedCollectionView
    {
        event EventHandler DataLoading;
        event EventHandler DataLoaded;
    }
}
