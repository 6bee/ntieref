// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Linq.Expressions;

namespace NTier.Client.Domain
{
    public partial interface IOrderedDataServiceQueryable<TEntity, TBase> : IDataServiceQueryable<TEntity, TBase> 
        where TEntity : TBase
        where TBase : Entity
    {
        #region Linq operations

        IOrderedDataServiceQueryable<TEntity, TBase> ThenBy<T2>(Expression<Func<TEntity, T2>> orderBy);

        IOrderedDataServiceQueryable<TEntity, TBase> ThenByDescending<T2>(Expression<Func<TEntity, T2>> orderBy);

        #endregion Linq operations
    }
}
