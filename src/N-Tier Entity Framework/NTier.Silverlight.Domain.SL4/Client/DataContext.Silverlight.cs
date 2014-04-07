// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    abstract partial class DataContext
    {
        internal protected delegate void QueryDelegate<TEntity>(ClientInfo clientInfo, Query query, Action<QueryResult<TEntity>, Exception> callback) where TEntity : Entity;

        #region Save

        public abstract void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, ClientInfo clientInfo = null, Action<Exception> callback = null);

        #endregion Save

        protected void Invoke(Action action)
        {
            var dispatcher = Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }
}
