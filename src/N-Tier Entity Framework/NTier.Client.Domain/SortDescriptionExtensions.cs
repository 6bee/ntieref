// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;
using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class SortDescriptionExtensions
    {
        public static RLinq.SortExpression ToExpression<T>(this SortDescription sortDescription) where T : Entity<T>
        {
            var propertyName = sortDescription.PropertyName;
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            var parameterExpression = RLinq.Expression.Parameter("i", typeof(T));
            var propertyExpression = RLinq.Expression.PropertyAccess(parameterExpression, propertyInfo);
            var lambdaExpression = RLinq.Expression.Lambda(propertyExpression, parameterExpression);
            var sortDirection = sortDescription.Direction.Translate();
            var sortExpression = RLinq.Expression.Sort(lambdaExpression, sortDirection);
            return sortExpression;
        }

        private static RLinq.SortDirection Translate(this ListSortDirection direction)
        {
            switch (direction){
                case ListSortDirection.Ascending:
                    return RLinq.SortDirection.Ascending;
                case ListSortDirection.Descending:
                    return RLinq.SortDirection.Descending;
                default:
                    throw new Exception(string.Format("Unmapped sort direction: {0}", direction));
            }
        }
 
    }
}
