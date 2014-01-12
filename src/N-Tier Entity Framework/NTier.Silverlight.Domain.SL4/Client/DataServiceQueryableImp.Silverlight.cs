// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryableImp<TEntity>
    {
        #region ExecuteAsync

        public override void ExecuteAsync(Action<ICallbackResult<TEntity>> callback = null)
        {
            var dispatcher = Deployment.Dispatcher;

            // framework implementation
            if (EntitySet is EntitySet<TEntity>)
            {
                ((EntitySet<TEntity>)EntitySet).LoadAsync
                (
                    this,
                    delegate(IEnumerable<TEntity> data, long? totalCount, Exception exception)
                    {
                        if (callback != null)
                        {
                            var result = new CallbackResult(exception != null ? null : new QueryResultEntitySet(EntitySet, data, totalCount), exception);

                            // call back on calling thread
                            dispatcher.BeginInvoke(delegate
                            {
                                callback(result);
                            });
                        }
                    }
                );
            }
            else // client implementation e.g. mock-up
            {
                if (callback != null)
                {
                    var result = new CallbackResult(new QueryResultEntitySet(EntitySet, EntitySet, EntitySet.TotalCount), null);

                    // call back on calling thread
                    dispatcher.BeginInvoke(delegate
                    {
                        callback(result);
                    });
                }
            }
        }

        /// <summary>
        /// QueryResultEntitySet is used as a proxy to data context while providing only data from one specific query through its GetEnumerator() method.
        /// </summary>
        private sealed class QueryResultEntitySet : IEntitySet<TEntity>
        {
            private readonly IEntitySet<TEntity> _entitySet;
            private readonly ObservableCollection<TEntity> _data;
            private readonly long? _totalCount;

            public QueryResultEntitySet(IEntitySet<TEntity> entitySet, IEnumerable<TEntity> data, long? totalCount)
            {
                this._entitySet = entitySet;
                this._totalCount = totalCount;
                this._data = new ObservableCollection<TEntity>(data);
            }

            public object SyncRoot
            {
                get { return _entitySet.SyncRoot; }
            }

            void IEntitySet<TEntity>.Add(TEntity entity)
            {
                _entitySet.Add(entity);
                _data.Add(entity);
            }

            void IEntitySet<TEntity>.Delete(TEntity entity)
            {
                _entitySet.Delete(entity);
                _data.Remove(entity);
            }

            void IEntitySet<TEntity>.DeleteAll()
            {
                _entitySet.DeleteAll();
                _data.Clear();
            }

            void IEntitySet<TEntity>.Attach(TEntity entity)
            {
                _entitySet.Attach(entity);
                _data.Add(entity);
            }

            void IEntitySet<TEntity>.AttachAsModified(TEntity entity, TEntity original)
            {
                _entitySet.AttachAsModified(entity, original);
                _data.Add(entity);
            }

            void IEntitySet<TEntity>.Detach(TEntity entity)
            {
                _entitySet.Detach(entity);
                _data.Remove(entity);
            }

            void IEntitySet<TEntity>.DetachAll()
            {
                _entitySet.DetachAll();
                _data.Clear();
            }

            void IEntitySet<TEntity>.AcceptChanges(bool onlyForValidEntities)
            {
                _entitySet.AcceptChanges(onlyForValidEntities);
            }

            void IEntitySet<TEntity>.RevertChanges()
            {
                _entitySet.RevertChanges();
            }

            /// <summary>
            /// Returns all entites contained in the entity set, including entities marked as deleted.
            /// </summary>
            IEnumerable<TEntity> IEntitySet<TEntity>.GetAllEntities()
            {
                return _entitySet.GetAllEntities();
            }

            /// <summary>
            /// Creates a new instance of this entity sets entity type
            /// </summary>
            /// <param name="add">If true, adds the new entity to the entity set</param>
            /// <returns>A new instance of this entity sets entity type</returns>
            TEntity IEntitySet<TEntity>.CreateNew(bool add)
            {
                var entity = _entitySet.CreateNew(add);
                if (add)
                {
                    _data.Add(entity);
                }
                return entity;
            }

            IDataServiceQueryable<TEntity> IEntitySet<TEntity>.AsQueryable()
            {
                var queryable = new DataServiceQueryableImp<TEntity>(this);
                return queryable;
            }

            public long? TotalCount
            {
                get { return _totalCount; }
            }

            public IEnumerator<TEntity> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
            {
                add { _data.CollectionChanged += value; }
                remove { _data.CollectionChanged -= value; }
            }

            // note: this event is never going to fire since none of the properties of this class is ever going to change
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { throw new NotSupportedException("PropertyChanged event may not be registered on query result"); }
                remove { throw new NotSupportedException("PropertyChanged event may not be registered on query result"); }
            }

            //long? IEntitySet<TEntity>.TotalCount
            //{
            //    get { throw new NotImplementedException(); }
            //}

            int IEntitySet<TEntity>.Count
            {
                get { return _entitySet.Count; }
            }

            bool IEntitySet<TEntity>.HasChanges
            {
                get { return _entitySet.HasChanges; }
            }

            bool IEntitySet<TEntity>.IsValidationEnabled
            {
                get { return _entitySet.IsValidationEnabled; }
                set { _entitySet.IsValidationEnabled = value; }
            }

            bool IEntitySet<TEntity>.DetachEntitiesUponNewQueryResult
            {
                get { return _entitySet.DetachEntitiesUponNewQueryResult; }
                set { _entitySet.DetachEntitiesUponNewQueryResult = value; }
            }

            ClientInfo IEntitySet<TEntity>.ClientInfo
            {
                get { return _entitySet.ClientInfo; }
                set { _entitySet.ClientInfo = value; }
            }

            void IEntitySet<TEntity>.ClearErrors()
            {
                _entitySet.ClearErrors();
            }

            MergeOption IEntitySet<TEntity>.MergeOption
            {
                get { return _entitySet.MergeOption; }
                set { _entitySet.MergeOption = value; }
            }

            object IEntitySet<TEntity>.SyncRoot
            {
                get { return _entitySet.SyncRoot; }
            }
        }

        #endregion ExecuteAsync
    }
}