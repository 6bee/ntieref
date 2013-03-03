// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NTier.Common.Domain.Model;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal static class FilterDescriptionExtensions
    {
        private static readonly Type[] _numericTypes = new Type[]
        {
            typeof(System.SByte),
            typeof(System.SByte?),
            typeof(System.Byte),
            typeof(System.Byte?),
            typeof(System.Int16),
            typeof(System.Int16?),
            typeof(System.UInt16),
            typeof(System.UInt16?),
            typeof(System.Int32),
            typeof(System.Int32?),
            typeof(System.UInt32),
            typeof(System.UInt32?),
            typeof(System.Int64),
            typeof(System.Int64?),
            typeof(System.UInt64),
            typeof(System.UInt64?),
            typeof(System.Single),
            typeof(System.Single?),
            typeof(System.Double),
            typeof(System.Double?),
            typeof(System.Decimal),
            typeof(System.Decimal?),
            //typeof(System.DateTime),
            //typeof(System.DateTime?),
        };

        private static bool IsNumericType(this Type type)
        {
            return _numericTypes.Contains(type);
        }

        public static RLinq.Expression ToExpression<T>(this FilterDescription filterDescription) where T : Entity<T>
        {
            var propertyName = filterDescription.PropertyName;
            var entityType = typeof(T);
            var filterExpression = CreateFilterExpression(filterDescription, propertyName, entityType);
            return filterExpression ?? RLinq.Expression.ConstantValue(false);
        }

        public static RLinq.Expression ToGlobalExpression<T>(this FilterDescription filterDescription, IEnumerable<string> propertyNames) where T : Entity<T>
        {
            var entityType = typeof(T);
            RLinq.Expression expression = null;

            foreach (var propertyName in propertyNames)
            {
                var filterExpression = CreateFilterExpression(filterDescription, propertyName, entityType);
                if (ReferenceEquals(filterExpression, null)) continue;
                if (ReferenceEquals(expression, null))
                {
                    expression = filterExpression;
                }
                else
                {
                    expression = RLinq.Expression.Binary(expression, filterExpression, RLinq.BinaryOperator.Or);
                }
            }
            return expression ?? RLinq.Expression.ConstantValue(false);
        }

        private static RLinq.Expression CreateFilterExpression(FilterDescription filterDescription, string propertyName, Type entityType)
        {
            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            
            var parameterExpression = RLinq.Expression.Parameter("i", entityType);
            var propertyExpression = RLinq.Expression.PropertyAccess(parameterExpression, propertyInfo);
            var expression = propertyExpression as RLinq.Expression;

            var value = TranslateValue(propertyInfo.PropertyType, filterDescription.Value);

            var filterOperation=filterDescription.FilterOperation;
            switch (filterOperation)
            {
                case FilterOperation.StartsWith:
                case FilterOperation.EndsWith:
                case FilterOperation.Contains:
                    if (propertyInfo.PropertyType != typeof(string))
                    {
                        if (propertyInfo.PropertyType.IsNumericType())
                        {
                            expression = RLinq.Expression.MethodCall(expression, "ToString", typeof(object), BindingFlags.Public | BindingFlags.Instance, new Type[0], new RLinq.Expression[0]);
                            value = value == null ? null : value.ToString();
                        }
                        else
                        {
                            filterOperation = FilterOperation.Equal;
                            var genericTypeParameter = Nullable.GetUnderlyingType(propertyExpression.PropertyInfo.PropertyType);
                            if (genericTypeParameter != null)
                            {
                                expression = RLinq.Expression.Conversion(expression, genericTypeParameter);
                            }
                        }
                    }
                    break;
                case FilterOperation.GreaterThan:
                case FilterOperation.GreaterThanOrEqual:
                case FilterOperation.LessThan:
                case FilterOperation.LessThanOrEqual:
                    if (!propertyInfo.PropertyType.IsNumericType())
                    {
                        filterOperation = FilterOperation.Equal;
                    }
                    break;
            }

            if (ReferenceEquals(value, null)) return null;

            var binaryOperator = filterOperation.ToBinaryOperator();

            var valueExpression = RLinq.Expression.ConstantValue(value);

            var filterExpression = RLinq.Expression.Binary(expression, valueExpression, binaryOperator);
            return filterExpression;
        }

        // todo: remove this method and improve silverlight sample to buil-up global filter based on data types of relevant columns
        private static object TranslateValue(Type type, object value)
        {
            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }
            if (value is string)
            {
                if (type == typeof(int) || type == typeof(int?))
                {
                    int v;
                    if (int.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(long) || type == typeof(long?))
                {
                    long v;
                    if (long.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    decimal v;
                    if (decimal.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(double) || type == typeof(double?))
                {
                    double v;
                    if (double.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(float) || type == typeof(float?))
                {
                    float v;
                    if (float.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(byte) || type == typeof(byte?))
                {
                    byte v;
                    if (byte.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(sbyte) || type == typeof(sbyte?))
                {
                    sbyte v;
                    if (sbyte.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(short) || type == typeof(short?))
                {
                    short v;
                    if (short.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(ushort) || type == typeof(ushort?))
                {
                    ushort v;
                    if (ushort.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(ulong) || type == typeof(ulong?))
                {
                    ulong v;
                    if (ulong.TryParse((string)value, out v))
                    {
                        return v;
                    }
                }
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    DateTime dateValue;
                    if (DateTime.TryParseExact((string)value, new string[] { "yyyy", "yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                    if (DateTime.TryParseExact((string)value, new string[] { "M.yyyy", "M.yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                    if (DateTime.TryParseExact((string)value, new string[] { "d.M.yyyy", "d.M.yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                    if (DateTime.TryParseExact((string)value, "d.M.", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                    if (DateTime.TryParseExact((string)value, "H:m", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                    if (DateTime.TryParseExact((string)value, new string[] { "d.M.yy H:m", "d.M.yyyy H:m" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
                    {
                        return dateValue;
                    }
                }
            }
            return null;
        }

        private static RLinq.BinaryOperator ToBinaryOperator(this FilterOperation op)
        {
            switch (op)
            {
                case FilterOperation.Contains:
                    return RLinq.BinaryOperator.Contains;
                case FilterOperation.EndsWith:
                    return RLinq.BinaryOperator.EndsWith;
                case FilterOperation.Equal:
                    return RLinq.BinaryOperator.Equal;
                case FilterOperation.GreaterThan:
                    return RLinq.BinaryOperator.GreaterThan;
                case FilterOperation.GreaterThanOrEqual:
                    return RLinq.BinaryOperator.GreaterThanOrEqual;
                case FilterOperation.LessThan:
                    return RLinq.BinaryOperator.LessThan;
                case FilterOperation.LessThanOrEqual:
                    return RLinq.BinaryOperator.LessThanOrEqual;
                case FilterOperation.NotEqual:
                    return RLinq.BinaryOperator.NotEqual;
                case FilterOperation.StartsWith:
                    return RLinq.BinaryOperator.StartsWith;
                default:
                    throw new Exception(string.Format("Unmapped filter operator: {0}", op));
            }
        }
    }
}
