// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    public sealed class FilterDescription// : IFilterDescription
    {
        /// <summary>
        /// The name of the property where the filter has to be applied. If PropertyName is null the filter is applied as global filter.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The value to be filtered
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The filter operation
        /// </summary>
        public FilterOperation FilterOperation { get; set; }
    }
}
