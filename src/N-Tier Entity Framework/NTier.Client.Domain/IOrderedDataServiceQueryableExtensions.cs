// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class IOrderedDataServiceQueryableExtensions
    {
        public static IOrderedDataServiceQueryable<TEntity> ThenBy<TEntity>(this IOrderedDataServiceQueryable<TEntity> queriable, RLinq.LambdaExpression expression) where TEntity : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((OrderedDataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression, RLinq.SortDirection.Ascending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedDataServiceQueryable<TEntity> queriable, RLinq.LambdaExpression expression) where TEntity : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((OrderedDataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression, RLinq.SortDirection.Descending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity> ThenBy<TEntity>(this IOrderedDataServiceQueryable<TEntity> queriable, RLinq.SortExpression expression) where TEntity : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((OrderedDataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression));
            return newQueriable;
        }
    }
}
