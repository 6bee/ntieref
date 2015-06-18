// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using Remote.Linq.Expressions;
using System;
using System.ComponentModel;
using System.Reflection;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class SortDescriptionExtensions
    {
        public static RLinq.SortExpression ToExpression<T>(this SortDescription sortDescription) where T : Entity
        {
            if (ReferenceEquals(null, sortDescription)) throw new ArgumentNullException("sortDescription");
            var propertyName = sortDescription.PropertyName;
            if (string.IsNullOrEmpty(propertyName)) throw new Exception("property name must not be null or empty");

            var type = typeof(T);
            var propertyNames = propertyName.Split('.');
            var parameterExpression = RLinq.Expression.Parameter("i", type);

            Expression expression = parameterExpression;
            foreach (var property in propertyNames)
            {
                var propertyInfo = type.GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                expression = RLinq.Expression.MakeMemberAccess(expression, propertyInfo);
                type = propertyInfo.PropertyType;
            }

            var lambdaExpression = RLinq.Expression.Lambda(expression, parameterExpression);
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
