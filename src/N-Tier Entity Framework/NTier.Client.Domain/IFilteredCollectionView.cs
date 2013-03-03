// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.ComponentModel;

namespace NTier.Client.Domain
{
    public interface IFilteredCollectionView : ICollectionView
    {
        FilterDescriptionCollection FilterDescriptions { get; }
    }
}
