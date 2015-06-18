// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTier.Client.Domain
{
    internal sealed class QueryProvider<TEntity, TBase> : IQueryProvider
        where TEntity : TBase
        where TBase : Entity
    {
        private DataServiceQueryable<TEntity, TBase> _queriable;
        private long? _totalCountInt64 = null;
        private int? _totalCountInt32 = null;

        internal QueryProvider(DataServiceQueryable<TEntity, TBase> queriable)
        {
            _queriable = queriable;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(TEntity))
            {
                return new QueryTypeMapper<TElement>(this, expression);
            }

            ParseExpression(expression);

            return (IQueryable<TElement>)_queriable;
        }

        private void ParseExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    bool isSupported = ParseExpression((MethodCallExpression)expression);
                    if (!isSupported)
                    {
                        goto default;
                    }
                    break;

                default:
                    // save as local expression
                    _queriable.Expression = expression;
                    break;
                //throw new Exception("Linq operation not supported: " + expression.ToString() + "\nConsider applying AsEnumerable() before this call.");
            }
        }

        private bool ParseExpression(MethodCallExpression expression)
        {
            switch (expression.Method.Name)
            {
                case "Where":
                    {
                        var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                        _queriable = (DataServiceQueryable<TEntity, TBase>)_queriable.Where(exp);
                    }
                    break;

                case "First":
                    {
                        if (expression.Arguments.Count > 1)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.First(exp) });
                        }
                        else
                        {
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.First() });
                        }
                    }
                    break;

                case "FirstOrDefault":
                    {
                        if (expression.Arguments.Count > 1)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.FirstOrDefault(exp) });
                        }
                        else
                        {
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.FirstOrDefault() });
                        }
                    }
                    break;

                case "Single":
                    {
                        if (expression.Arguments.Count > 1)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.Single(exp) });
                        }
                        else
                        {
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.Single() });
                        }
                    }
                    break;

                case "SingleOrDefault":
                    {
                        if (expression.Arguments.Count > 1)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.SingleOrDefault(exp) });
                        }
                        else
                        {
                            _queriable = new DataServiceQueryableImp<TEntity, TBase>(_queriable, new[] { _queriable.SingleOrDefault() });
                        }
                    }
                    break;

                case "OrderBy":
                    {
                        var exp = (UnaryExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)_queriable.OrderBy((LambdaExpression)exp.Operand);
                    }
                    break;

                case "OrderByDescending":
                    {
                        var exp = (UnaryExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)_queriable.OrderByDescending((LambdaExpression)exp.Operand);
                    }
                    break;

                case "ThenBy":
                    {
                        var exp = (UnaryExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)((OrderedDataServiceQueryable<TEntity, TBase>)_queriable).ThenBy((LambdaExpression)exp.Operand);
                    }
                    break;

                case "ThenByDescending":
                    {
                        var exp = (UnaryExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)((OrderedDataServiceQueryable<TEntity, TBase>)_queriable).ThenByDescending((LambdaExpression)exp.Operand);
                    }
                    break;

                case "Skip":
                    {
                        var exp = (ConstantExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)_queriable.Skip((int)exp.Value);
                    }
                    break;

                case "Take":
                    {
                        var exp = (ConstantExpression)expression.Arguments[1];
                        _queriable = (DataServiceQueryable<TEntity, TBase>)_queriable.Take((int)exp.Value);
                    }
                    break;

                case "Count":
                    {
                        if (expression.Arguments.Count == 2)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _totalCountInt32 = _queriable.Count(exp);
                        }
                        else
                        {
                            _totalCountInt32 = _queriable.Count();
                        }
                    }
                    break;

                case "LongCount":
                    {
                        if (expression.Arguments.Count == 2)
                        {
                            var exp = (Expression<Func<TEntity, bool>>)((UnaryExpression)expression.Arguments[1]).Operand;
                            _totalCountInt64 = _queriable.LongCount(exp);
                        }
                        else
                        {
                            _totalCountInt64 = _queriable.LongCount();
                        }
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression, typeof(TResult));
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return CreateQuery<TEntity>(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression, expression.Type);
        }

        private object Execute(Expression expression, Type returnType)
        {
            ParseExpression(expression);

            if (returnType == typeof(IEnumerable<TEntity>))
            {
                return (IEnumerable<TEntity>)_queriable;
            }
            if (returnType == typeof(TEntity))
            {
                return (object)_queriable.AsEnumerable().FirstOrDefault();
            }
            if (returnType == typeof(int) && _totalCountInt32.HasValue)
            {
                return (object)_totalCountInt32;
            }
            if (returnType == typeof(long) && _totalCountInt64.HasValue)
            {
                return (object)_totalCountInt64;
            }
            return null;
        }

        private sealed class QueryTypeMapper<TElement> : IQueryable<TElement>
        {
            private readonly QueryProvider<TEntity, TBase> OuterProvider;
            private readonly IList<Tuple<System.Reflection.PropertyInfo, bool>> Properties;

            public QueryTypeMapper(QueryProvider<TEntity, TBase> provider, Expression expression)
            {
                this.OuterProvider = provider;
                this.Provider = new QueryProvider(this);
                this.Expression = System.Linq.Expressions.Expression.Constant(this);

                this.Properties = new List<Tuple<System.Reflection.PropertyInfo, bool>>();
                foreach (var exp in ((MethodCallExpression)expression).Arguments.Skip(1).Cast<UnaryExpression>().Select(e => (LambdaExpression)e.Operand))
                {
                    if (exp.Body is MemberExpression)
                    {
                        AddPropertyAccess((MemberExpression)exp.Body);
                    }
                    //else if (exp.Body is ParameterExpression)
                    //{
                    //}
                }

                string includeString = string.Join(".", this.Properties.Where(p => p.Item2 /* isEntity */).Select(p => p.Item1.Name /* property name */));
                if (!string.IsNullOrEmpty(includeString))
                {
                    this.OuterProvider._queriable = (DataServiceQueryable<TEntity, TBase>)this.OuterProvider._queriable.Include(includeString);
                }
            }

            private void AddPropertyAccess(MemberExpression expression)
            {
                if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    var exp2 = (MemberExpression)expression.Expression;
                    AddPropertyAccess(exp2);
                }
                System.Reflection.PropertyInfo property = expression.Member as System.Reflection.PropertyInfo;
                Properties.Add(new Tuple<System.Reflection.PropertyInfo, bool>(property, typeof(Entity).IsAssignableFrom(property.PropertyType) || (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && !typeof(string).IsAssignableFrom(property.PropertyType))));
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                var result = new List<TElement>();
                foreach (var e in this.OuterProvider._queriable)
                {
                    //yield return (TElement)Property.GetValue(e, null);
                    object element = e;
                    foreach (var property in Properties)
                    {
                        if (element is System.Collections.IEnumerable)
                        {
                            var elements = new List<object>();
                            foreach (var item in (System.Collections.IEnumerable)element)
                            {
                                if (item != null)
                                {
                                    elements.Add(property.Item1.GetValue(item, null));
                                }
                            }
                            element = elements;
                        }
                        else
                        {
                            element = property.Item1.GetValue(element, null);
                        }
                    }

                    if (element is System.Collections.IEnumerable)
                    {
                        foreach (var item in (System.Collections.IEnumerable)element)
                        {
                            if (item != null)
                            {
                                result.Add((TElement)item);
                            }
                        }
                    }
                    else
                    {
                        result.Add((TElement)element);
                    }
                }
                return result.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Type ElementType
            {
                get { return typeof(TElement); }
            }

            public Expression Expression
            {
                get;
                private set;
            }

            public IQueryProvider Provider
            {
                get;
                private set;
            }

            private sealed class QueryProvider : IQueryProvider
            {
                private readonly QueryTypeMapper<TElement> QueryTypeMapper;

                public QueryProvider(QueryTypeMapper<TElement> queryTypeMapper)
                {
                    this.QueryTypeMapper = queryTypeMapper;
                }

                public IQueryable<TQueryElement> CreateQuery<TQueryElement>(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public IQueryable CreateQuery(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public TResult Execute<TResult>(Expression expression)
                {
                    QueryTypeMapper.OuterProvider.ParseExpression(expression);


                    if (typeof(TResult) == typeof(IEnumerable<TElement>))
                    {
                        return (TResult)(IEnumerable<TElement>)QueryTypeMapper;
                    }
                    if (typeof(TResult) == typeof(TElement))
                    {
                        return (TResult)(object)QueryTypeMapper.AsEnumerable().FirstOrDefault();
                    }
                    //if (typeof(TResult) == typeof(int) && _totalCountInt32.HasValue)
                    //{
                    //    return (TResult)(object)_totalCountInt32;
                    //}
                    //if (typeof(TResult) == typeof(long) && _totalCountInt64.HasValue)
                    //{
                    //    return (TResult)(object)_totalCountInt64;
                    //}
                    return default(TResult); // null

                }

                public object Execute(Expression expression)
                {
                    return Execute<TElement>(expression);
                }
            }
        }
    }
}
