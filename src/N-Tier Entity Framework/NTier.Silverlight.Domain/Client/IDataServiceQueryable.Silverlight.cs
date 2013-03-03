// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial interface IDataServiceQueryable<TEntity>
    {
        #region Properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        ClientInfo ClientInfo { get; set; }

        #endregion Properties

        #region Execute methods

        void ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null);

        #endregion Execute methods

        #region Linq operations

        IDataServiceQueryable<TEntity> AsQueryable();

        #endregion Linq operations
    }
}