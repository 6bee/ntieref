// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public partial interface IOrderedDataServiceQueryable<TEntity> : IDataServiceQueryable<TEntity> where TEntity : Entity
    {
        #region Linq operations

        IOrderedDataServiceQueryable<TEntity> ThenBy<T2>(Expression<Func<TEntity, T2>> orderBy);

        IOrderedDataServiceQueryable<TEntity> ThenByDescending<T2>(Expression<Func<TEntity, T2>> orderBy);

        #endregion Linq operations
    }
}
