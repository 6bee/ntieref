// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace NTier.Client.Domain
{
    public class RefreshEventArgs : EventArgs
    {
        public SortDescriptionCollection SortDescriptions { get; internal set; }
        public FilterExpressionCollection FilterExpressions { get; internal set; }
    }
}
