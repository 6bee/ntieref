// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryable<TEntity>
    {
        #region ExecuteAsync

        public abstract void ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null);

        #endregion ExecuteAsync
    }
}