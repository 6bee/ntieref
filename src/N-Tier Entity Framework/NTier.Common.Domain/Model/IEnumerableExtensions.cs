// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Common.Domain.Model
{
    public static class IEnumerableExtensions
    {
        public static ISet<T> ToSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
        
#if !SILVERLIGHT
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source)
        {
            return new SortedSet<T>(source);
        }
#endif

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
