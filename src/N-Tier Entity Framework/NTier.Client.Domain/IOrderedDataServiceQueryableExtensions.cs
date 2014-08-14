// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class IOrderedDataServiceQueryableExtensions
    {
        public static IOrderedDataServiceQueryable<TEntity, TBase> ThenBy<TEntity, TBase>(this IOrderedDataServiceQueryable<TEntity, TBase> queriable, RLinq.LambdaExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((OrderedDataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression, RLinq.SortDirection.Ascending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity, TBase> ThenByDescending<TEntity, TBase>(this IOrderedDataServiceQueryable<TEntity, TBase> queriable, RLinq.LambdaExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((OrderedDataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression, RLinq.SortDirection.Descending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity, TBase> ThenBy<TEntity, TBase>(this IOrderedDataServiceQueryable<TEntity, TBase> queriable, RLinq.SortExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((OrderedDataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression));
            return newQueriable;
        }
    }
}
