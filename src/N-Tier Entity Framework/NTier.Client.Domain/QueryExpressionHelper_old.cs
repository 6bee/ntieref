using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    internal static class QueryExpressionHelper
    {
        #region Consolidation
        
        public static QueryExpression Or(this IEnumerable<QueryExpression> expressions)
        {
            return Consolidate(expressions, " OR ");
        }

        public static QueryExpression And(this IEnumerable<QueryExpression> expressions)
        {
            return Consolidate(expressions, " AND ");
        }

        internal static QueryExpression Consolidate(this IEnumerable<QueryExpression> queries, string separator, bool distinct = true)
        {
            if (queries == null || !queries.Any())
            {
                return null;
            }

            if (queries.Count() == 1)
            {
                return queries.First();
            }

            var queryValue = new StringBuilder();
            var parameters = new List<Parameter>();

            if (distinct)
            {
                queries = queries.Distinct(QueryExpressionComparerInstance);
            }

            foreach (var query in queries)
            {
                if (queryValue.Length > 0)
                {
                    queryValue.Append(separator);
                }

                if (string.IsNullOrEmpty(query.Value))
                {
                    throw new ArgumentException("Query expression must not be null or empty.", "queries");
                }

                queryValue.Append(query.Value);

                if (query.Parameters != null)
                {
                    parameters.AddRange(query.Parameters);
                }
            }

            return new QueryExpression(queryValue.ToString(), parameters.Distinct());
        }

        #endregion Consolidation

        #region QueryExpressionComparer

        private static readonly QueryExpressionComparer QueryExpressionComparerInstance = new QueryExpressionComparer();

        private sealed class QueryExpressionComparer : IEqualityComparer<QueryExpression>
        {
            public bool Equals(QueryExpression x, QueryExpression y)
            {
                if (x.Value != y.Value) return false;
                if (x.Parameters == null && y.Parameters == null) return true;
                if (x.Parameters == null || y.Parameters == null) return false;

                var p1 = x.Parameters.ToArray();
                var p2 = y.Parameters.ToArray();

                if (p1.Length != p2.Length) return false;

                for (int i = 0; i < p1.Length; i++)
                {
                    if (p1[i] != p2[i]) return false;
                }

                return true;
            }

            public int GetHashCode(QueryExpression obj)
            {
                var hashCode = 0;
                if (obj.Value != null)
                {
                    hashCode = obj.Value.GetHashCode();
                }
                if (obj.Parameters != null)
                {
                    var primes = new PrimeNumbers();
                    foreach (var p in obj.Parameters)
                    {
                        unchecked
                        {
                            hashCode = (int)(hashCode + (p.GetHashCode() * primes.Next));
                        }
                    }
                }
                return hashCode;
            }
        }

        #endregion QueryExpressionComparer

        #region Sort

        public static QueryExpression ToExpression(this IEnumerable<SortDescription> sortDescriptions)
        {
            var expresison = string.Join(", ", 
                sortDescriptions.Select(s => string.Format("{0} {1}", 
                    ExpressionTranslator.FormatPropertyName(s.PropertyName), 
                    s.Direction.GetString())));
            return new QueryExpression(expresison);
        }

        private static string GetString(this ListSortDirection direction)
        {
            return direction == ListSortDirection.Ascending ? "ASC" : "DESC";
        }

        #endregion Sort

        #region Filter

        public static IEnumerable<QueryExpression> ToExpressions<T>(this IEnumerable<FilterDescription> filterDescriptions)
        {
            var filterExpressions = new List<QueryExpression>();
            ISet<Parameter> parameters = new HashSet<Parameter>();

            foreach (var filter in filterDescriptions)
            {
                var expression = ToExpression<T>(filter, parameters);
                filterExpressions.Add(expression);
            }

            return filterExpressions;
        }

        public static QueryExpression ToExpression<T>(this FilterDescription filter)
        {
            var expression = ToExpression<T>(filter, null);
            return expression;
        }

        private static QueryExpression ToExpression<T>(FilterDescription filter, ISet<Parameter> parameters)
        {
            QueryExpression expression;

            switch (filter.FilterOperation)
            {
                case FilterOperation.Contains:
                    expression = QueryExpressionHelper.GetLikeFilter<T>(filter, parameters, pattern: "%{0}%");
                    break;

                case FilterOperation.StartsWith:
                    expression = QueryExpressionHelper.GetLikeFilter<T>(filter, parameters, pattern: "{0}%");
                    break;

                case FilterOperation.EndsWith:
                    expression = QueryExpressionHelper.GetLikeFilter<T>(filter, parameters, pattern: "%{0}");
                    break;

                case FilterOperation.Equal:
                    //expression = QueryExpressionHelper.GetLikeFilter<T>(filter, parameters, pattern: "{0}");
                    expression = GetFilter(filter, parameters, "=");
                    break;

                case FilterOperation.NotEqual:
                    //expression = QueryExpressionHelper.GetLikeFilter<T>(filter, parameters, pattern: "{0}", negate: true);
                    expression = GetFilter(filter, parameters, "<>");
                    break;

                case FilterOperation.GreaterThan:
                    expression = GetFilter(filter, parameters, ">");
                    break;

                case FilterOperation.GreaterThanOrEqual:
                    expression = GetFilter(filter, parameters, ">=");
                    break;

                case FilterOperation.LessThan:
                    expression = GetFilter(filter, parameters, "<");
                    break;

                case FilterOperation.LessThanOrEqual:
                    expression = GetFilter(filter, parameters, "<=");
                    break;

                default:
                    throw new NotSupportedException(string.Format("Not supported filter operation '{0}'", filter.FilterOperation));
            }

            return expression;
        }

        private static QueryExpression GetFilter(FilterDescription filter, ISet<Parameter> parameters, string filterOperator)
        {
            var parameter = GetParameter(filter.Value, parameters);
            return new QueryExpression(
                string.Format("({1} {0} {2})",
                    filterOperator,
                    ExpressionTranslator.FormatPropertyName(filter.PropertyName),
                    ExpressionTranslator.FormatParameterName(parameter.Name)),
                parameter);
        }

        private static QueryExpression GetLikeFilter<T>(FilterDescription filter, ISet<Parameter> parameters = null, string pattern = "%{0}%", bool negate = false)
        {
            //if (parameters == null)
            //{
            //    //parameters = new HashSet<Parameter>(ParameterComparerInstance);
            //    parameters = new HashSet<Parameter>();
            //}

            var expression = PropertyLike<T>(filter.PropertyName, filter.Value == null ? string.Empty : filter.Value.ToString(), pattern, negate, parameters);

            return new QueryExpression(expression, parameters);
        }

        private static string PropertyLike<T>(string propertyName, string value, string pattern, bool negate, ISet<Parameter> parameters)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            var property = typeof(T).GetProperty(propertyName);
            
            string expression = null;
            if (property.PropertyType == typeof(System.String) || property.PropertyType == typeof(System.Char))
            {
                expression = BuildStringLikeExpression(propertyName, value, parameters, pattern, negate);
            }
            else if (NumericTypes.Contains(property.PropertyType))
            {
                expression = BuildNumberAsStringLikeExpression(propertyName, value, parameters, pattern, negate);
            }
            else if (property.PropertyType == typeof(System.DateTime))
            {
                expression = BuildDateTimeExpression(propertyName, value, parameters, negate);
            }
            else if (property.PropertyType == typeof(System.Boolean))
            {
                expression = BuildBooleanExpression(propertyName, value, parameters, negate);
            }

            if (expression == null)
            {
                //expression = string.Format("({0} LIKE {1})", formattedPropertyName, formattedParameterName);
                expression = "(1 = 2)";
            }

            return expression;
        }

        private static string BuildStringLikeExpression(string propertyName, string value, ISet<Parameter> parameters, string pattern = null, bool negate = false)
        {
            if (pattern == null)
            {
                pattern = "%{0}%";
            }
            value = string.Format(pattern, value.Replace("%", "[%]"));
            var parameter = GetParameter(value, parameters);
            var expression = string.Format("({0} {2}LIKE {1})", 
                ExpressionTranslator.FormatPropertyName(propertyName), 
                ExpressionTranslator.FormatParameterName(parameter.Name),
                negate ? "NOT " : null);
            return expression;
        }

        private static string BuildNumberAsStringLikeExpression(string propertyName, string value, ISet<Parameter> parameters, string pattern = null, bool negate = false)
        {
            if (pattern == null)
            {
                pattern = "%{0}%";
            }
            value = string.Format(pattern, value.Replace("%", "[%]"));
            var parameter = GetParameter(value, parameters);
            var expression = string.Format("(CAST({0} AS System.String) {2}LIKE {1})", 
                ExpressionTranslator.FormatPropertyName(propertyName),
                ExpressionTranslator.FormatParameterName(parameter.Name),
                negate ? "NOT " : null);
            return expression;
        }

        private static string BuildBooleanExpression(string propertyName, string value, ISet<Parameter> parameters, bool negate = false)
        {
            string expression;
            bool booleanValue;
            if (bool.TryParse(value, out booleanValue))
            {
                var parameter = GetParameter(value, parameters);
                expression = string.Format("({2}{0} = {1})", 
                    ExpressionTranslator.FormatPropertyName(propertyName),
                    ExpressionTranslator.FormatParameterName(parameter.Name),
                    negate ? "NOT " : null);
            }
            else
            {
                expression = "(1 = 0)";
            }
            return expression;
        }

        private static string BuildDateTimeExpression(string propertyName, string value, ISet<Parameter> parameters, bool negate = false)
        {
            string expression = null;

            // "dd.MM.yyyy HH:mm"
            // "Year($.column) == Year(CAST(@param as System.DateTime))"

            DateTime dateValue = DateTime.MinValue;
            if (DateTime.TryParseExact(value, new string[] { "yyyy", "yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Year({0}) = Year({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName),
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }
            else if (DateTime.TryParseExact(value, new string[] { "M.yyyy", "M.yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Year({0}) = Year({1}) AND Month({0}) = Month({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName), 
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }
            else if (DateTime.TryParseExact(value, new string[] { "d.M.yyyy", "d.M.yy" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Year({0}) = Year({1}) AND Month({0}) = Month({1}) AND Day({0}) = Day({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName), 
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }
            else if (DateTime.TryParseExact(value, "d.M.", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Month({0}) = Month({1}) AND Day({0}) = Day({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName), 
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }
            else if (DateTime.TryParseExact(value, "H:m", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Hour({0}) = Hour({1}) AND Minute({0}) = Minute({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName), 
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }
            else if (DateTime.TryParseExact(value, new string[] { "d.M.yy H:m", "d.M.yyyy H:m" }, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateValue))
            {
                var parameter = GetParameter(dateValue, parameters);
                expression = string.Format("(Year({0}) = Year({1}) AND Month({0}) = Month({1}) AND Day({0}) = Day({1}) AND Hour({0}) = Hour({1}) AND Minute({0}) = Minute({1}))", 
                    ExpressionTranslator.FormatPropertyName(propertyName), 
                    ExpressionTranslator.FormatParameterName(parameter.Name));
            }

            if (expression == null)
            {
                expression = "(1 = 0)";
            }
            else if (negate)
            {
                expression = string.Format("(NOT {0})", expression);
            }

            return expression;
        }

        private static Parameter GetParameter<T>(T value, ISet<Parameter> parameters = null)
        {
            Parameter parameter = null;

            if (parameters != null)
            {
                parameter = parameters.FirstOrDefault(p => value.Equals(p.Value));
            }

            if (parameter == null)
            {
                parameter = new Parameter(ExpressionTranslator.NextParameterName, value);
                parameters.Add(parameter);
            }

            return parameter;
        }

        #region NumericTypes
        
        private static Type[] NumericTypes = new Type[]
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
            typeof(System.Decimal?)
        };

        #endregion

        //public static QueryExpression ValueInStringProperties(IEnumerable<string> propertyNames, string value, bool negate = false)
        //{
        //    StringBuilder csvList = new StringBuilder();
        //    foreach (var property in propertyNames)
        //    {
        //        if (csvList.Length > 0)
        //        {
        //            csvList.Append(",");
        //        }
        //        csvList.Append("$.");
        //        csvList.Append(property);
        //    }

        //    var parameterName = ExpressionTranslator.NextParameterName;
        //    var formattedParameterName = ExpressionTranslator.FormatParameterName(parameterName);
        //    var parameter = new Parameter(parameterName, value);

        //    var queryString = string.Format("({0} {1}IN {{{2}}})", formattedParameterName, negate ? "NOT " : "", csvList.ToString());

        //    return new QueryExpression(queryString, parameter);
        //}

        //public static QueryExpression StringPropertiesContainValue(IEnumerable<string> propertyNames, string value)
        //{
        //    var parameterName = ExpressionTranslator.NextParameterName;
        //    var formattedParameterName = ExpressionTranslator.FormatParameterName(parameterName);
        //    var parameter = new Parameter(parameterName, string.Format("%{0}%", value));

        //    StringBuilder list = new StringBuilder();
        //    list.Append("(");
        //    foreach (var property in propertyNames)
        //    {
        //        if (list.Length > 1)
        //        {
        //            list.Append(" OR ");
        //        }
        //        list.Append("($.");
        //        list.Append(property);
        //        list.Append(" LIKE ");
        //        list.Append(formattedParameterName);
        //        list.Append(")");
        //    }
        //    list.Append(")");

        //    return new QueryExpression(list.ToString(), parameter);
        //}

        #endregion Filter
    }
}
