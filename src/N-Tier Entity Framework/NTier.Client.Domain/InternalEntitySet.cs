// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class InternalEntitySet
    {
        private sealed class ChangetrackingPrevention : IDisposable
        {
            public ChangetrackingPrevention()
            {
                SuppressChangeTracking = true;
            }

            public void Dispose()
            {
                SuppressChangeTracking = false;
            }
        }

        [ThreadStatic]
        protected static bool SuppressChangeTracking = false;

        public IDisposable PreventChangetracking()
        {
            return new ChangetrackingPrevention();
        }
    }

    [DebuggerDisplay("InternalEntitySet Count = {Count}, AllCount = {AllCount}, ServerCount = {DatasourceTotalCount}")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InternalEntitySet<TEntity> : InternalEntitySet, IInternalEntitySet<TEntity>, IEnumerable<TEntity>, INotifyCollectionChanged, INotifyPropertyChanged where TEntity : Entity
    {
        public readonly object SyncRoot = new object();

        private readonly IList<TEntity> _entitySet = new List<TEntity>();
        private readonly IList<TEntity> _added = new List<TEntity>();
        private readonly IList<TEntity> _removed = new List<TEntity>();
        private bool _isValidationEnabled = true;
        private bool _suppressCollectionChangedEvent = false;
        

        public InternalEntitySet()
        {
            this.Dispatcher = Deployment.Dispatcher;
        }

        public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

        //[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long? DatasourceTotalCount
        {
            get
            {
                return _datasourceTotalCount;
            }
            set
            {
                if (_datasourceTotalCount != value)
                {
                    _datasourceTotalCount = value;
                    OnPropertyChanged("TotalCount");
                }
            }
        }
        private long? _datasourceTotalCount = null;
        
        public int Count
        {
            get { return _entitySet.Count(e => e.ChangeTracker.State != ObjectState.Deleted); }
        }

        private int AllCount
        {
            get { return _entitySet.Count; }
        }
        

        public TEntity Add(TEntity entity)
        {
            TEntity existingEntity;
            lock (SyncRoot)
            {
                existingEntity = GetExistingByReference(entity);
                if (existingEntity == null)
                {
                    entity.MarkAsAdded();
                    // apply settings
                    if (entity.IsValidationEnabled != IsValidationEnabled)
                    {
                        entity.IsValidationEnabled = IsValidationEnabled;
                    }
                    RegisterForHasChangesEvent(entity);
                    _entitySet.Add(entity);
                    if (!SuppressChangeTracking)
                    {
                        if (!_removed.Remove(entity))
                        {
                            _added.Add(entity);
                        }
                    }

                    if (!_suppressCollectionChangedEvent && CollectionChanged != null)
                    {
                        var index = _entitySet.IndexOf(entity);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, entity, index));
                    }
                }
            }
            OnPropertyChanged("HasChanges");
            return existingEntity;
        }

        public TEntity Attach(TEntity entity)
        {
            TEntity existingEntity;
            lock (SyncRoot)
            {
                existingEntity = GetExisting(entity);
                if (existingEntity == null)
                {
                    entity.StartTracking();
                    // apply settings
                    if (entity.IsValidationEnabled != IsValidationEnabled)
                    {
                        entity.IsValidationEnabled = IsValidationEnabled;
                    }
                    RegisterForHasChangesEvent(entity);
                    _entitySet.Add(entity);
                    if (!SuppressChangeTracking)
                    {
                        if (!_removed.Remove(entity))
                        {
                            _added.Add(entity);
                        }
                    }

                    if (!_suppressCollectionChangedEvent && CollectionChanged != null)
                    {
                        var index = _entitySet.IndexOf(entity);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, entity, index));
                    }
                }
            }
            OnPropertyChanged("HasChanges");
            return existingEntity;
        }

        public TEntity Delete(TEntity entity)
        {
            TEntity existingEntity;
            lock (SyncRoot)
            {
                // new entity
                if (entity.ChangeTracker.State == ObjectState.Added)
                {
                    existingEntity = GetExistingByReference(entity);
                    if (existingEntity != null)
                    {
                        Detach(existingEntity);
                    }
                }
                else // existing entity
                {
                    _suppressDeletedEvent = true;

                    existingEntity = GetExisting(entity);
                    if (existingEntity != null)
                    {
                        existingEntity.MarkAsDeleted();
                        existingEntity.IsValidationEnabled = IsValidationEnabled;

                        if (!_suppressCollectionChangedEvent && CollectionChanged != null)
                        {
                            var index = _entitySet.IndexOf(existingEntity);
                            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, existingEntity, index));
                        }
                    }
                    else
                    {
                        entity.MarkAsDeleted();
                        entity.IsValidationEnabled = IsValidationEnabled;
                        RegisterForHasChangesEvent(entity);
                        _entitySet.Add(entity);
                        if (!SuppressChangeTracking)
                        {
                            if (!_removed.Remove(entity))
                            {
                                _added.Add(entity);
                            }
                        }
                    }

                    _suppressDeletedEvent = false;
                }
            }
            OnPropertyChanged("HasChanges");
            return existingEntity;
        }

        public void DeleteAll()
        {
            bool hasAny = false;
            lock (SyncRoot)
            {
                foreach (var entity in _entitySet.Where(e => e.ChangeTracker.State != ObjectState.Deleted).ToArray())
                {
                    hasAny = true;

                    if (entity.ChangeTracker.State == ObjectState.Added)
                    {
                        if (_entitySet.Remove(entity))
                        {
                            if (!SuppressChangeTracking)
                            {
                                if (!_added.Remove(entity))
                                {
                                    _removed.Add(entity);
                                }
                            }
                            UnRegisterFromHasChangesEvent(entity);
                        }
                    }
                    else
                    {
                        entity.MarkAsDeleted();
                        entity.IsValidationEnabled = IsValidationEnabled;
                    }
                }
            }
            if (!_suppressCollectionChangedEvent && hasAny && CollectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            OnPropertyChanged("HasChanges");
        }

        public void Detach(TEntity entity)
        {
            lock (SyncRoot)
            {
                var index = _entitySet.IndexOf(entity);
                var success = _entitySet.Remove(entity);
                if (success)
                {
                    if (!SuppressChangeTracking)
                    {
                        if (!_added.Remove(entity))
                        {
                            _removed.Add(entity);
                        }
                    }
                    UnRegisterFromHasChangesEvent(entity);
                    if (!_suppressCollectionChangedEvent && CollectionChanged != null)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, entity, index));
                    }
                }
            }
            OnPropertyChanged("HasChanges");
        }

        public void DetachAll()
        {
            lock (SyncRoot)
            {
                UnRegisterFromHasChangesEvent(_entitySet);
                foreach (var entity in _entitySet)
                {
                    if (!SuppressChangeTracking)
                    {
                        if (!_added.Remove(entity))
                        {
                            _removed.Add(entity);
                        }
                    }
                }
                _entitySet.Clear();
            }
            if (!_suppressCollectionChangedEvent && CollectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            OnPropertyChanged("HasChanges");
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public TEntity GetExisting(TEntity entity)
        {
            lock (SyncRoot)
            {
                var existingEntity = _entitySet.FirstOrDefault(e => e.Equals(entity));
                return existingEntity;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public TEntity GetExistingByReference(TEntity entity)
        {
            lock (SyncRoot)
            {
                var existingEntity = _entitySet.FirstOrDefault(e => object.ReferenceEquals(e, entity));
                return existingEntity;
            }
        }

        public IEnumerable<TEntity> GetAllEntities()
        {
            lock (SyncRoot)
            {
                return new List<TEntity>(_entitySet);
            }
        }

        System.Collections.IEnumerable IInternalEntitySet.GetAllEntities()
        {
            return GetAllEntities();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (var entity in _entitySet)
            {
                if (entity.ChangeTracker.State != ObjectState.Deleted)
                {
                    yield return entity;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ClearErrors()
        {
            if (_entitySet.Count > 0)
            {
                lock (SyncRoot)
                {
                    _suppressHasChangesEvent = true;
                    foreach (var e in _entitySet)
                    {
                        e.Errors.Clear();
                    }
                    _suppressHasChangesEvent = false;
                }
                OnPropertyChanged("HasChanges");
            }
        }

        public bool HasChanges
        {
            get
            {
                lock (SyncRoot)
                {
                    return _added.Count > 0
                        || _removed.Count > 0
                        || _entitySet.Any(e => e.HasChanges);
                }
            }
        }

        public bool IsValid
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.All(e => e.IsValid);
                }
            }
        }

        public void AcceptChanges(bool onlyForValidEntities = false)
        {
            lock (SyncRoot)
            {
                _suppressHasChangesEvent = true;
                foreach (var entity in _entitySet.Where(e => !onlyForValidEntities || e.IsValid).ToArray())
                {
                    if (entity.ChangeTracker.State == ObjectState.Deleted)
                    {
                        _entitySet.Remove(entity);
                        UnRegisterFromHasChangesEvent(entity);
                    }
                    else
                    {
                        entity.AcceptChanges();
                    }
                }
                if (onlyForValidEntities)
                {
                    foreach (var entity in _added.Where(e => e.IsValid).ToArray())
                    {
                        _added.Remove(entity);
                    }
                }
                else
                {
                    _added.Clear();
                }
                _removed.Clear();
                _suppressHasChangesEvent = false;
            }
            OnPropertyChanged("HasChanges");
        }

        public void RevertChanges()
        {
            lock (SyncRoot)
            {
                _suppressHasChangesEvent = true;
                _suppressCollectionChangedEvent = true;
                SuppressChangeTracking = true;

                try
                {
                    foreach (var entity in _added)
                    {
                        Detach(entity);
                    }
                    foreach (var entity in _removed)
                    {
                        Add(entity);
                    }
                    foreach (var entity in _entitySet.ToArray())
                    {
                        entity.RevertChanges();
                    }

                    _added.Clear();
                    _removed.Clear();
                }
                finally
                {
                    SuppressChangeTracking = false;
                    _suppressCollectionChangedEvent = false;
                    _suppressHasChangesEvent = false;
                }
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged("HasChanges");            
        }

        public bool IsValidationEnabled
        {
            get
            {
                return _isValidationEnabled;
            }
            set
            {
                lock (SyncRoot)
                {
                    _isValidationEnabled = value;
                    foreach (var e in _entitySet)
                    {
                        e.IsValidationEnabled = value;
                    }
                }
                OnPropertyChanged("IsValidationEnabled");
            }
        }

        #region INotifyCollectionChanged

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                var dispatcher = Dispatcher;
                if (dispatcher == null || dispatcher.CheckAccess())
                {
                    collectionChanged(this, e);
                }
                else
                {
                    dispatcher.BeginInvoke((Action)delegate { collectionChanged(this, e); });
                }
            }
        }
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        #endregion

        #region INotifyPropertyChanged
        
        private void RegisterForHasChangesEvent(TEntity entity)
        {
            entity.PropertyChanged += entity_PropertyChanged;
        }

        private void UnRegisterFromHasChangesEvent(TEntity entity)
        {
            entity.PropertyChanged -= entity_PropertyChanged;
        }

        private void RegisterForHasChangesEvent(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.PropertyChanged += entity_PropertyChanged;
            }
        }

        private void UnRegisterFromHasChangesEvent(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.PropertyChanged -= entity_PropertyChanged;
            }
        }

        private void entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HasChanges":
                    OnPropertyChanged("HasChanges");
                    break;

                case "ChangeTracker.State":
                    lock (SyncRoot)
                    {
                        if (!_suppressDeletedEvent)
                        {
                            var entity = sender as TEntity;
                            if (entity != null && entity.ChangeTracker.State == ObjectState.Deleted)
                            {
                                Delete(entity);
                            }
                        }
                    }
                    break;
            }
        }

        private bool _suppressHasChangesEvent = false;
        private bool _suppressDeletedEvent = false;

        private void OnPropertyChanged(string propertyName)
        {
            if (_suppressHasChangesEvent && propertyName == "HasChanges")
            {
                return;
            }

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                var dispatcher = Dispatcher;
                if (dispatcher == null || dispatcher.CheckAccess())
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    dispatcher.BeginInvoke((Action)delegate { propertyChanged(this, new PropertyChangedEventArgs(propertyName)); });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion
    }
}
