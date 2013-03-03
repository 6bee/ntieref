// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryable<TEntity> : IEntitySet<TEntity>
    {
        partial void Initialize()
        {
            this.Provider = new QueryProvider<TEntity>(this);
            this.Expression = System.Linq.Expressions.Expression.Constant(this);
        }

        #region SyncRoot

        public object SyncRoot
        {
            get { return EntitySet.SyncRoot; }
        }

        #endregion SyncRoot

        #region Execute / ExecuteAsync

        public IEntitySet<TEntity> Execute()
        {
            return new DataServiceQueryableImp<TEntity>(this, this.ToArray());
        }

        public Task<IEntitySet<TEntity>> ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null, bool startImmediately = true, TaskScheduler taskScheduler = null, TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
        {
            var task = new Task<IEntitySet<TEntity>>(
                () =>
                {
                    IEntitySet<TEntity> entitySet = null;
                    try
                    {
                        // load operation
                        entitySet = Execute();

                        // callback
                        if (callback != null)
                        {
                            callback(new CallbackResult(entitySet));
                        }
                    }
                    catch (Exception e)
                    {
                        // don't throw if callback is available
                        if (callback != null)
                        {
                            callback(new CallbackResult(null, e));
                        }
                        else
                        {
                            throw;
                        }
                    }

                    // return result
                    return entitySet;
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

        #region IEntitySet<TEntity>

        void IEntitySet<TEntity>.Add(TEntity entity)
        {
            EntitySet.Add(entity);
        }

        void IEntitySet<TEntity>.Delete(TEntity entity)
        {
            EntitySet.Delete(entity);
        }

        void IEntitySet<TEntity>.DeleteAll()
        {
            EntitySet.DeleteAll();
        }

        void IEntitySet<TEntity>.Attach(TEntity entity)
        {
            EntitySet.Attach(entity);
        }

        void IEntitySet<TEntity>.AttachAsModified(TEntity entity, TEntity original)
        {
            EntitySet.AttachAsModified(entity, original);
        }

        void IEntitySet<TEntity>.Detach(TEntity entity)
        {
            EntitySet.Detach(entity);
        }

        void IEntitySet<TEntity>.DetachAll()
        {
            EntitySet.DetachAll();
        }

        void IEntitySet<TEntity>.AcceptChanges(bool onlyForValidEntities)
        {
            EntitySet.AcceptChanges(onlyForValidEntities);
        }

        void IEntitySet<TEntity>.RevertChanges()
        {
            EntitySet.RevertChanges();
        }


        /// <summary>
        /// Returns all entites contained in the entity set, including entities marked as deleted.
        /// </summary>
        IEnumerable<TEntity> IEntitySet<TEntity>.GetAllEntities()
        {
            return EntitySet.GetAllEntities();
        }

        /// <summary>
        /// Creates a new instance of this entity sets entity type
        /// </summary>
        /// <param name="add">If true, adds the new entity to the entity set</param>
        /// <returns>A new instance of this entity sets entity type</returns>
        TEntity IEntitySet<TEntity>.CreateNew(bool add)
        {
            return EntitySet.CreateNew(add);
        }

        bool IEntitySet<TEntity>.IsValidationEnabled
        {
            get { return EntitySet.IsValidationEnabled; }
            set { EntitySet.IsValidationEnabled = value; }
        }

        bool IEntitySet<TEntity>.DetachEntitiesUponNewQueryResult
        {
            get { return EntitySet.DetachEntitiesUponNewQueryResult; }
            set { EntitySet.DetachEntitiesUponNewQueryResult = value; }
        }

        MergeOption IEntitySet<TEntity>.MergeOption
        {
            get { return EntitySet.MergeOption; }
            set { EntitySet.MergeOption = value; }
        }

        long? IEntitySet<TEntity>.TotalCount
        {
            get { return EntitySet.TotalCount; }
        }

        int IEntitySet<TEntity>.Count
        {
            get { return EntitySet.Count; }
        }

        bool IEntitySet<TEntity>.HasChanges
        {
            get { return EntitySet.HasChanges; }
        }

        void IEntitySet<TEntity>.ClearErrors()
        {
            EntitySet.ClearErrors();
        }

        //IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        //{
        //    return EntitySet.GetEnumerator();
        //}

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { EntitySet.CollectionChanged += value; }
            remove { EntitySet.CollectionChanged -= value; }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { EntitySet.PropertyChanged += value; }
            remove { EntitySet.PropertyChanged -= value; }
        }

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
            var queriable = new DataServiceQueryableImp<TEntity>(this);
            queriable.QueryData = false;
            queriable.QueryTotalCount = true;

            // execute
            var result = queriable.Execute();

            return result.TotalCount.Value;
        }

        public long LongCount(Expression<Func<TEntity, bool>> where)
        {
            return this.Where(where).LongCount();
        }

        #endregion Linq operations
    }
}