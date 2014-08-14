// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryable<TEntity, TBase> : IEnumerable<TEntity>
    {
        partial void Initialize()
        {
            this.Provider = new QueryProvider<TEntity, TBase>(this);
            this.Expression = System.Linq.Expressions.Expression.Constant(this);
        }

        #region SyncRoot

        public object SyncRoot
        {
            get { return EntitySet.SyncRoot; }
        }

        #endregion SyncRoot

        #region Execute / ExecuteAsync

        public IQueryResult<TEntity, TBase> Execute()
        {
            IEnumerable<TEntity> data = this.ToArray();
            return new QueryResult(EntitySet, data, QueryTotalCount.GetValueOrDefault(false) ? EntitySet.TotalCount : default(long?));
        }

        public Task<IQueryResult<TEntity, TBase>> ExecuteAsync(Action<IQueryResult<TEntity, TBase>> callback = null, bool startImmediately = true, TaskScheduler taskScheduler = null, TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
        {
            var task = new Task<IQueryResult<TEntity, TBase>>(
                () =>
                {
                    IQueryResult<TEntity, TBase> result = null;
                    try
                    {
                        // load operation
                        result = Execute();

                        // callback
                        if (callback != null)
                        {
                            callback(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        // don't throw if callback is available
                        if (callback != null)
                        {
                            callback(new QueryResult(ex));
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return result;
                },
                taskCreationOptions
            );

            if (startImmediately || taskScheduler != null)
            {
                if (taskScheduler != null)
                {
                    task.Start(taskScheduler);
                }
                else
                {
                    task.Start();
                }
            }

            return task;
        }

        #endregion

        #region IEnumerator<T>

        public abstract IEnumerator<TEntity> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerator<T>

        #region IQueryable

        Type IQueryable.ElementType { get { return typeof(TEntity); } }

        System.Linq.Expressions.Expression IQueryable.Expression { get { return Expression; } }
        internal System.Linq.Expressions.Expression Expression { get; set; }

        IQueryProvider IQueryable.Provider { get { return Provider; } }
        private IQueryProvider Provider;

        #endregion

        #region Linq operations

        public TEntity FirstOrDefault()
        {
            return this
                .Take(1)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where)
        {
            return this
                .Where(where)
                .FirstOrDefault();
        }

        public TEntity First()
        {
            return this
                .Take(1)
                .AsEnumerable()
                .First();
        }

        public TEntity First(Expression<Func<TEntity, bool>> where)
        {
            return this
                .Where(where)
                .First();
        }

        public TEntity SingleOrDefault()
        {
            return this
                .Take(2)
                .AsEnumerable()
                .SingleOrDefault();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> where)
        {
            return this
                .Where(where)
                .SingleOrDefault();
        }

        public TEntity Single()
        {
            return this
                .Take(2)
                .AsEnumerable()
                .Single();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> where)
        {
            return this
                .Where(where)
                .Single();
        }

        public int Count()
        {
            return (int)this.LongCount();
        }

        public int Count(Expression<Func<TEntity, bool>> where)
        {
            return this.Where(where).Count();
        }

        public long LongCount()
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            queriable.QueryData = false;
            queriable.QueryTotalCount = true;

            var result = queriable.Execute();
            return result.EntitySet.TotalCount.Value;
        }

        public long LongCount(Expression<Func<TEntity, bool>> where)
        {
            return this.Where(where).LongCount();
        }

        public IEnumerable<TEntity> AsEnumerable()
        {
            return this;
        }

        /// <summary>
        /// Gets a data service queriable for the specific entity type
        /// </summary>
        public IDataServiceQueryable<TEntity, TBase> AsQueryable()
        {
            var queriable = new DataServiceQueryableImp<TEntity, TBase>(this);
            return queriable;
        }
  
        #endregion Linq operations
    }
}