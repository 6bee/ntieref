// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NTier.Server.Domain.Repositories.EntityFramework
{
    internal static class ExpressionVisitor
    {
        public static Expression<Func<TEntity, bool>> Visit<TEntity>(this Expression<Func<TEntity, bool>> expression)
        {
            var newBody = SqlFunctions.Translate(expression.Body);

            if (ReferenceEquals(newBody, expression.Body))
            {
                return expression;
            }
            else
            {
                var newExpression = Expression.Lambda<Func<TEntity, bool>>(newBody, expression.Parameters);
                return newExpression;
            }
        }

        private sealed class SqlFunctions : NTier.Server.Domain.Repositories.Linq.Expressions.ExpressionVisitor
        {
            private static readonly SqlFunctions _instance = new SqlFunctions();

            private static readonly MethodInfo StringConvertDecimal = typeof(System.Data.Entity.SqlServer.SqlFunctions).GetMethod("StringConvert", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(decimal?), typeof(int?), typeof(int?) }, null);
            private static readonly MethodInfo StringConvertDouble = typeof(System.Data.Entity.SqlServer.SqlFunctions).GetMethod("StringConvert", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(double?), typeof(int?), typeof(int?) }, null);


            private SqlFunctions()
            {
            }

            public static Expression Translate(Expression exp)
            {
                return _instance.Visit(exp);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                m = (MethodCallExpression)base.VisitMethodCall(m);

                switch (m.Method.Name)
                {
                    case "ToString":
                        {
                            if (m.Object != null && m.Arguments.Count == 0)
                            {
                                var instance = m.Object;
                                var lengthExpression = Expression.Constant(38, typeof(int?));
                                var decimalArgExpression = Expression.Constant(16, typeof(int?));
                                if (instance.Type == typeof(decimal?))
                                {
                                    return Expression.Call(StringConvertDecimal, instance, lengthExpression, decimalArgExpression);
                                }
                                if (instance.Type == typeof(double?))
                                {
                                    return Expression.Call(StringConvertDouble, instance, lengthExpression, decimalArgExpression);
                                }
                                if (instance.Type.IsConvertibleToNullableDecimal())
                                {
                                    var body = Expression.Convert(instance, typeof(decimal?));
                                    return Expression.Call(StringConvertDecimal, body, lengthExpression, decimalArgExpression);
                                }
                                if (instance.Type.IsConvertibleToNullableDouble())
                                {
                                    var body = Expression.Convert(instance, typeof(double?));
                                    return Expression.Call(StringConvertDouble, body, lengthExpression, decimalArgExpression);
                                }
                            }
                        }
                        break;
                }

                return m;
            }
        }

        private static bool IsConvertibleToNullableDouble(this Type type)
        {
            return type == typeof(int)
                || type == typeof(int?)
                || type == typeof(uint)
                || type == typeof(uint?)
                || type == typeof(short)
                || type == typeof(short?)
                || type == typeof(ushort)
                || type == typeof(ushort?)
                || type == typeof(byte)
                || type == typeof(byte?)
                || type == typeof(sbyte)
                || type == typeof(sbyte?)
                || type == typeof(long)
                || type == typeof(long?)
                || type == typeof(ulong)
                || type == typeof(ulong?)
                || type == typeof(float)
                || type == typeof(float?)
                || type == typeof(double)
                || type == typeof(double?);
        }

        private static bool IsConvertibleToNullableDecimal(this Type type)
        {
            return type == typeof(decimal)
                || type == typeof(decimal?);
        }
    }
}
