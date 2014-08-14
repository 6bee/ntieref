// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NTier.Client.Domain
{
    partial interface IDataServiceQueryable<TEntity, TBase> : IQueryable<TEntity>
    {
        #region Properties

        /// <summary>
        /// Query specific client info. By default client info of entity set is used.
        /// </summary>
        ClientInfo ClientInfo { get; }

        #endregion Properties

        #region Execute methods

        IQueryResult<TEntity, TBase> Execute();
        Task<IQueryResult<TEntity, TBase>> ExecuteAsync(Action<IQueryResult<TEntity, TBase>> callback = null, bool startImmediately = true, TaskScheduler taskScheduler = null, TaskCreationOptions taskCreationOptions = TaskCreationOptions.None);

        #endregion Execute methods

        #region Linq operations

        IEnumerable<TEntity> AsEnumerable();

        IDataServiceQueryable<TEntity, TBase> AsQueryable();

        TEntity FirstOrDefault();

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where);

        TEntity First();

        TEntity First(Expression<Func<TEntity, bool>> where);

        TEntity SingleOrDefault();

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> where);

        TEntity Single();

        TEntity Single(Expression<Func<TEntity, bool>> where);

        int Count();

        int Count(Expression<Func<TEntity, bool>> where);

        long LongCount();

        long LongCount(Expression<Func<TEntity, bool>> where);

        #endregion Linq operations
    }
}