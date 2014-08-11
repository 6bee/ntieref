// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    [DebuggerDisplay("Count = {Count}, TotalCount = {TotalCount}")]
    internal sealed partial class EntitySet<TEntity> : IEntitySet<TEntity>, IEnumerable<TEntity>, INotifyCollectionChanged, INotifyPropertyChanged
        where TEntity : Entity
    {
        #region Private fields

        // data context
        private readonly IDataContext _dataContext;

        // linq to object
        private readonly InternalEntitySet<TEntity> _internalEntitySet;

        // attach delegate (registers entities into context)
        private readonly DataContext.AttachDelegate<TEntity> _attachDelegate;

        // query delegate (queries data from service)
        private readonly DataContext.QueryDelegate<TEntity> _queryDelegate;

        #endregion Private fields

        #region Constructor

        /// <summary>
        /// Constructor for a readonly entity set. <br />
        /// Calls to Add, Delete, Attach, AttachAsModified, and Detach will throw exceptions.
        /// </summary>
        public EntitySet(IDataContext dataContext, InternalEntitySet<TEntity> entitySet, DataContext.AttachDelegate<TEntity> attachDelegate, DataContext.QueryDelegate<TEntity> queryDelegate)
        {
            this._dataContext = dataContext;
            this._internalEntitySet = entitySet;
            this._attachDelegate = attachDelegate;
            this._queryDelegate = queryDelegate;
        }

        #endregion Constructor

        #region Public properties

        /// <summary>
        /// Client info for this entity set. By default client info of data context is used.
        /// </summary>
        /// <remarks>
        /// Setting client info at entity set level will override client info of data context for the specific entity set.
        /// </remarks>
        public ClientInfo ClientInfo
        {
            get
            {
                return _isClientInfoSetOnEntitySet ? _clientInfo : _dataContext.ClientInfo;
            }
            set
            {
                _clientInfo = value;
                _isClientInfoSetOnEntitySet = true;
            }
        }
        private ClientInfo _clientInfo = null;
        private bool _isClientInfoSetOnEntitySet = false;

        /// <summary>
        /// Sets client info on this entity set back to default. 
        /// </summary>
        public void ResetClientInfo()
        {
            _clientInfo = null;
            _isClientInfoSetOnEntitySet = false;
        }

        private MergeOption? _mergeOption = null;
        public MergeOption MergeOption
        {
            get
            {
                return _mergeOption.HasValue ? _mergeOption.Value : _dataContext.MergeOption;
            }
            set
            {
                _mergeOption = value;
            }
        }

        public int Count
        {
            get { return _internalEntitySet.Count; }
        }

        public long? TotalCount { get { return _internalEntitySet.DatasourceTotalCount; } }

        public object SyncRoot { get { return _internalEntitySet.SyncRoot; } }

        private bool? _detachEntitiesUponNewQueryResult;
        public bool DetachEntitiesUponNewQueryResult
        {
            get
            {
                return _detachEntitiesUponNewQueryResult.HasValue ? _detachEntitiesUponNewQueryResult.Value : _dataContext.DetachEntitiesUponNewQueryResult;
            }
            set
            {
                _detachEntitiesUponNewQueryResult = value;
            }
        }
        /// <summary>
        /// Returns true if one or more entities contained in this entity set have changes, false otherwise
        /// </summary>
        public bool HasChanges { get { return _internalEntitySet.HasChanges; } }

        /// <summary>
        /// Returns true if all entities contained in this entity set are valid, false otherwise
        /// </summary>
        public bool IsValid { get { return _internalEntitySet.IsValid; } }

        public bool IsValidationEnabled
        {
            set { _internalEntitySet.IsValidationEnabled = value; } 
            get { return _internalEntitySet.IsValidationEnabled; } 
        }

        #endregion Public properties

        #region Add, Delete, Attach, ...

        public void Add(TEntity entity)
        {
            _attachDelegate(entity, DataContext.InsertMode.Add);
        }

        public void Delete(TEntity entity)
        {
            _internalEntitySet.Delete(entity);
        }

        public void DeleteAll()
        {
            _internalEntitySet.DeleteAll();
        }

        public void Attach(TEntity entity)
        {
            _attachDelegate(entity, DataContext.InsertMode.Attach);
        }

        public void AttachAsModified(TEntity entity, TEntity original)
        {
            var existing = _attachDelegate(original, DataContext.InsertMode.Attach);
            if (existing != null)
            {
                original = existing;
            }
            original.Refresh(entity);
        }

        public void Detach(TEntity entity)
        {
            _internalEntitySet.Detach(entity);
        }

        public void DetachAll()
        {
            _internalEntitySet.DetachAll();
        }

        /// <summary>
        /// Creates a new instance of this entity sets entity type
        /// </summary>
        /// <param name="add">If true, adds the new entity to the entity set</param>
        /// <returns>A new instance of this entity sets entity type</returns>
        public TEntity CreateNew(bool add = true)
        {
            var entity = Activator.CreateInstance<TEntity>();
            if (add)
            {
                Add(entity);
            }
            return entity;
        }

        public void ClearErrors()
        {
            _internalEntitySet.ClearErrors();
        }

        public void AcceptChanges()
        {
            _internalEntitySet.AcceptChanges();
        }

        public static explicit operator DataServiceQueryable<TEntity>(EntitySet<TEntity> entitySet)
        {
            return (DataServiceQueryable<TEntity>)entitySet.AsQueryable();
        }

        public IDataServiceQueryable<TEntity> AsQueryable()
        {
            if (_queryDelegate == null)
            {
                throw new Exception(string.Format("There is no query procedure for entity type '{0}'. Check whether this entity is not a aggregate root and needs to be loaded through its aggregate root.", typeof(TEntity)));
            }

            var queryable = new DataServiceQueryableImp<TEntity>(this);
            return queryable;
        }

        /// <summary>
        /// Returns all entites contained in the entity set, including entities marked as deleted.
        /// </summary>
        public IEnumerable<TEntity> GetAllEntities()
        {
            return _internalEntitySet.GetAllEntities();
        }

        #endregion Add, Delete, Attach, ...

        #region AcceptChanges, RevertChanges

        /// <summary>
        /// Accepts pending changes in all entities containes in this entity set
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        public void AcceptChanges(bool onlyForValidEntities = false)
        {
            _internalEntitySet.AcceptChanges(onlyForValidEntities);
        }

        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this entity set, re-attaches detached entities, and removes added entities
        /// </summary>
        public void RevertChanges()
        {
            _internalEntitySet.RevertChanges();
        }

        #endregion AcceptChanges, RevertChanges

        #region IEnumerable<TEntity>

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _internalEntitySet
                .Where(e => e.ChangeTracker.State != ObjectState.Deleted)
                .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<TEntity>

        #region INotifyCollectionChanged, INotifyPropertyChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _internalEntitySet.CollectionChanged += value; }
            remove { _internalEntitySet.CollectionChanged -= value; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _internalEntitySet.PropertyChanged += value; }
            remove { _internalEntitySet.PropertyChanged -= value; }
        }

        #endregion INotifyCollectionChanged, INotifyPropertyChanged

        #region Static Helper Methods
        
        //private static void AddNotSupportedOperation<T>(T entity)
        //{
        //    throw new NotSupportedException(string.Format("Add operation is not supperted for entity type {0}", typeof(T).Name));
        //}
        
        //private static void AttachNotSupportedOperation<T>(T entity)
        //{
        //    throw new NotSupportedException(string.Format("Attach operation is not supperted for entity type {0}", typeof(T).Name));
        //}
        
        //private static void AttachAsModifiedNotSupportedOperation<T>(T entity, T original)
        //{
        //    throw new NotSupportedException(string.Format("AttachAsModifie operation is not supperted for entity type {0}", typeof(T).Name));
        //}
        
        #endregion
    }
}