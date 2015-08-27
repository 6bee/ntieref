// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial interface IDataServiceQueryable<TEntity, TBase>
    {
        #region Execute methods

        void ExecuteAsync(Action<IQueryResult<TEntity, TBase>> callback = null);

        #endregion Execute methods

        #region Linq operations

        IDataServiceQueryable<TEntity, TBase> AsQueryable();

        #endregion Linq operations
    }
}