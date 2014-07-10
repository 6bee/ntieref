// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial interface IDataContext
    {
        /// <summary>
        /// Saves pending changes
        /// </summary>
        /// <param name="acceptOption">Defines when changes should be accepted locally</param>
        /// <param name="clearErrors">If set to true, all error entries are cleared before saving chnages</param>
        /// <param name="clientInfo">Optional client info object, to be submitted to the server</param>
        void SaveChangesAsync(AcceptOption acceptOption = AcceptOption.Default, bool clearErrors = false, bool failOnValidationErrors = true, ClientInfo clientInfo = null, Action<Exception> callback = null);
    }
}
