// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class IDataServiceQueryableExtensions
    {
        public static IDataServiceQueryable<TEntity, TBase> Where<TEntity, TBase>(this IDataServiceQueryable<TEntity, TBase> queriable, RLinq.LambdaExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new DataServiceQueryableImp<TEntity, TBase>((DataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Filters.Add(new DataServiceQueryable<TEntity, TBase>.Filter(expression));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity, TBase> OrderBy<TEntity, TBase>(this IDataServiceQueryable<TEntity, TBase> queriable, RLinq.LambdaExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((DataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression, RLinq.SortDirection.Ascending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity, TBase> OrderByDescending<TEntity, TBase>(this IDataServiceQueryable<TEntity, TBase> queriable, RLinq.LambdaExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((DataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression, RLinq.SortDirection.Descending));
            return newQueriable;
        }

        public static IOrderedDataServiceQueryable<TEntity, TBase> OrderBy<TEntity, TBase>(this IDataServiceQueryable<TEntity, TBase> queriable, RLinq.SortExpression expression) where TEntity : TBase where TBase : Entity
        {
            var newQueriable = new OrderedDataServiceQueryable<TEntity, TBase>((DataServiceQueryable<TEntity, TBase>)queriable);
            newQueriable.Sortings.Clear();
            newQueriable.Sortings.Add(new DataServiceQueryable<TEntity, TBase>.Sort(expression));
            return newQueriable;
        }
    }
}
