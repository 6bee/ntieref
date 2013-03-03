// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial interface IDataServiceQueryable<TEntity> : IEntitySet<TEntity>, IEnumerable<TEntity>, IQueryable<TEntity>
    {
        #region Properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        new ClientInfo ClientInfo { get; set; }

        #endregion Properties

        #region Execute methods

        IEntitySet<TEntity> Execute();
        Task<IEntitySet<TEntity>> ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null, bool startImmediately = true, TaskScheduler taskScheduler = null, TaskCreationOptions taskCreationOptions = TaskCreationOptions.None);

        #endregion Execute methods

        #region Linq operations

        new IDataServiceQueryable<TEntity> AsQueryable();

        TEntity FirstOrDefault();

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where);

        TEntity First();

        TEntity First(Expression<Func<TEntity, bool>> where);

        TEntity SingleOrDefault();

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> where);

        TEntity Single();

        TEntity Single(Expression<Func<TEntity, bool>> where);

        new int Count();

        new int Count(Expression<Func<TEntity, bool>> where);

        long LongCount();

        long LongCount(Expression<Func<TEntity, bool>> where);

        #endregion Linq operations
    }
}