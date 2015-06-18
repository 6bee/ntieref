using System;
using System.Linq.Expressions;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;

namespace ProductManager.Silverlight
{
    internal static class QueryHelper
    {
        public static IOrderedDataServiceQueryable<T> Sort<T, T2>(this IDataServiceQueryable<T> query, Expression<Func<T, T2>> member, C1.Silverlight.DataGrid.DataGridSortDirection direction, ref bool isFirst) where T : Entity, new()
        {
            if (direction == C1.Silverlight.DataGrid.DataGridSortDirection.None)
            {
                direction = C1.Silverlight.DataGrid.DataGridSortDirection.Ascending;
            }

            if (isFirst)
            {
                isFirst = false;

                if (direction == C1.Silverlight.DataGrid.DataGridSortDirection.Ascending)
                {
                    return query.OrderBy(member);
                }
                else
                {
                    return query.OrderByDescending(member);
                }
            }
            else
            {
                if (direction == C1.Silverlight.DataGrid.DataGridSortDirection.Ascending)
                {
                    return ((IOrderedDataServiceQueryable<T>)query).ThenBy(member);
                }
                else
                {
                    return ((IOrderedDataServiceQueryable<T>)query).ThenByDescending(member);
                }
            }
        }
    }
}
