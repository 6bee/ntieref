// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories.Linq;
using Remote.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NTier.Server.Domain.Repositories
{
    public static class IEntitySetExtensions
    {
        internal static IEntityQueryable<TEntity> ApplyInclude<TEntity>(this IEntityQueryable<TEntity> queriable, IEnumerable<string> includeList) where TEntity : Entity
        {
            if (!ReferenceEquals(includeList, null))
            {
                foreach (var include in includeList)
                {
                    queriable = queriable.Include(include);
                }
            }
            return queriable;
        }

        internal static IDomainQueryable<TEntity> ApplyFilters<TEntity>(this IDomainQueryable<TEntity> queriable, IEnumerable<Remote.Linq.Expressions.LambdaExpression> filterList) where TEntity : Entity
        {
            if (!ReferenceEquals(filterList, null))
            {
                var filters =
                    from f in filterList
                    select f.ToLinqExpression<TEntity, bool>();
                queriable = queriable.ApplyFilters(filters);
            }
            return queriable;
        }

        internal static IDomainQueryable<TEntity> ApplyFilters<TEntity>(this IDomainQueryable<TEntity> queriable, IEnumerable<Expression<Func<TEntity, bool>>> filters) where TEntity : Entity
        {
            if (!ReferenceEquals(filters, null))
            {
                foreach (var filter in filters)
                {
                    queriable = queriable.Where(filter);
                }
            }
            return queriable;
        }

        internal static IDomainQueryable<TEntity> ApplySorting<TEntity>(this IDomainQueryable<TEntity> queriable, IEnumerable<Remote.Linq.Expressions.SortExpression> sortList) where TEntity : Entity
        {
            IOrderedDomainQueryable<TEntity> orderedQueriable = null;

            if (!ReferenceEquals(sortList, null))
            {
                foreach (var sort in sortList)
                {
                    var exp = sort.Operand.ToLinqExpression();
                    if (ReferenceEquals(orderedQueriable, null))
                    {
                        switch (sort.SortDirection)
                        {
                            case Remote.Linq.Expressions.SortDirection.Ascending:
                                orderedQueriable = queriable.OrderBy(exp);
                                break;
                            case Remote.Linq.Expressions.SortDirection.Descending:
                                orderedQueriable = queriable.OrderByDescending(exp);
                                break;
                        }
                    }
                    else
                    {
                        switch (sort.SortDirection)
                        {
                            case Remote.Linq.Expressions.SortDirection.Ascending:
                                orderedQueriable = orderedQueriable.ThenBy(exp);
                                break;
                            case Remote.Linq.Expressions.SortDirection.Descending:
                                orderedQueriable = orderedQueriable.ThenByDescending(exp);
                                break;
                        }
                    }
                }
            }
            return orderedQueriable ?? queriable;
        }
    }
}
