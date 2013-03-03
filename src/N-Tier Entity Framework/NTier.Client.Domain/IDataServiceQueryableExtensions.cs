// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class IDataServiceQueryableExtensions
    {
        public static IDataServiceQueryable<TEntity> Where<TEntity>(this IDataServiceQueryable<TEntity> queriable, RLinq.Expression expression) where TEntity : Entity<TEntity>
        {
            var newQueriable = new DataServiceQueryableImp<TEntity>((DataServiceQueryable<TEntity>)queriable);
            newQueriable.Filters.Add(new DataServiceQueryable<TEntity>.Filter(expression));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity> OrderBy<TEntity>(this IDataServiceQueryable<TEntity> queriable, RLinq.Expression expression) where TEntity : Entity<TEntity>
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((DataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression, RLinq.SortDirection.Ascending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity> OrderByDescending<TEntity>(this IDataServiceQueryable<TEntity> queriable, RLinq.Expression expression) where TEntity : Entity<TEntity>
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((DataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression, RLinq.SortDirection.Descending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity> OrderBy<TEntity>(this IDataServiceQueryable<TEntity> queriable, RLinq.SortExpression expression) where TEntity : Entity<TEntity>
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity>((DataServiceQueryable<TEntity>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity>.Sort(expression));
            return newQueriable;
        }
    }
}
