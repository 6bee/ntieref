// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories.Linq
{
    internal static class IDomainQueryableExtensions
    {
        private static readonly MethodInfo _lambdaExpressionMethodInfo = typeof(System.Linq.Expressions.Expression)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "Lambda" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType == typeof(System.Linq.Expressions.ParameterExpression[]));

        private static TResult Call<TEntity, TQueriable, TResult>(this TQueriable queryable, LambdaExpression lambdaExpression, string methodName) where TEntity : Entity
        {
            var exp = lambdaExpression.Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TEntity), resultType);
            var lambdaExpressionMethodInfo = _lambdaExpressionMethodInfo.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = typeof(TQueriable).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(resultType);
            var result = method.Invoke(queryable, new object[] { funcExpression });

            return (TResult)result;
        }

        public static IOrderedDomainQueryable<TEntity> OrderBy<TEntity>(this IDomainQueryable<TEntity> queryable, LambdaExpression lambdaExpression) where TEntity : Entity
        {
            return queryable.Call<TEntity, IDomainQueryable<TEntity>, IOrderedDomainQueryable<TEntity>>(lambdaExpression, "OrderBy");
        }

        public static IOrderedDomainQueryable<TEntity> OrderByDescending<TEntity>(this IDomainQueryable<TEntity> queryable, LambdaExpression lambdaExpression) where TEntity : Entity
        {
            return queryable.Call<TEntity, IDomainQueryable<TEntity>, IOrderedDomainQueryable<TEntity>>(lambdaExpression, "OrderByDescending");
        }

        public static IOrderedDomainQueryable<TEntity> ThenBy<TEntity>(this IOrderedDomainQueryable<TEntity> queryable, LambdaExpression lambdaExpression) where TEntity : Entity
        {
            return queryable.Call<TEntity, IOrderedDomainQueryable<TEntity>, IOrderedDomainQueryable<TEntity>>(lambdaExpression, "ThenBy");
        }

        public static IOrderedDomainQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedDomainQueryable<TEntity> queryable, LambdaExpression lambdaExpression) where TEntity : Entity
        {
            return queryable.Call<TEntity, IOrderedDomainQueryable<TEntity>, IOrderedDomainQueryable<TEntity>>(lambdaExpression, "ThenByDescending");
        }
    }
}
