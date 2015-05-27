// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public abstract partial class DataContext : IDisposable, IDataContext
    {
        #region Delegates and Enums

        internal protected delegate TEntity AttachDelegate<TEntity>(TEntity entity, InsertMode insertMode /*= InsertMode.Attach*/, MergeOption mergeOption = MergeOption.AppendOnly, List<object> referenceTrackingList = null) where TEntity : Entity;

        internal protected delegate void DetachDelegate<TEntity>(TEntity entity) where TEntity : Entity;

        internal protected enum InsertMode
        {
            Add,
            Attach
        }

        #endregion Delegates and Enums

        #region Private Members

        private readonly ICollection<IInternalEntitySet> _entitySets = new List<IInternalEntitySet>();
        protected IEnumerable<IInternalEntitySet> EntitySets { get { return _entitySets; } }

        protected bool SuppressHasChanges = false; 
        private bool _disposed = false;

        #endregion

        #region Contructor

        protected DataContext()
        {
            this.Dispatcher = Deployment.Dispatcher;
        }

        #endregion

        #region Public Properties

        protected bool Disposed { get { return _disposed; } }

        /// <summary>
        ///  Gets or sets whether server validation exceptions are suppressed
        /// </summary>
        public bool IsServerValidationExceptionSuppressed
        {
            get
            {
                return _isServerValidationExceptionSuppressed;
            }
            set
            {
                if (_isServerValidationExceptionSuppressed != value)
                {
                    _isServerValidationExceptionSuppressed = value;
                    OnPropertyChanged("IsServerValidationExceptionSuppressed");
                }
            }
        }
        private bool _isServerValidationExceptionSuppressed = false;

        /// <summary>
        /// Gets or sets NTier.Common.Domain.Model.ClientInfo object to be passed to any server call
        /// </summary>
        public ClientInfo ClientInfo
        {
            get
            {
                return _clientInfo;
            }
            set
            {
                _clientInfo = value;
                OnPropertyChanged("ClientInfo");
            }
        }
        private ClientInfo _clientInfo = null;

        /// <summary>
        /// Gets or sets merge options to be used by default opon attaching entities retrieved from the server
        /// </summary>
        public MergeOption MergeOption
        {
            get
            {
                return _mergeOption;
            }
            set
            {
                _mergeOption = value;
                OnPropertyChanged("MergeOption");
            }
        }
        private MergeOption _mergeOption = MergeOption.AppendOnly;

        /// <summary>
        /// Gets or sets whether existing entities should be detached before attaching entites of a new query result
        /// </summary>
        public bool DetachEntitiesUponNewQueryResult
        {
            get
            {
                return _detachEntitiesUponNewQueryResult;
            }
            set
            {
                if (_detachEntitiesUponNewQueryResult != value)
                {
                    _detachEntitiesUponNewQueryResult = value;
                    OnPropertyChanged("DetachEntitiesUponNewQueryResult");
                }
            }
        }
        private bool _detachEntitiesUponNewQueryResult = false;

        /// <summary>
        /// Gets or stes whether validation is enabled on all entities attached to this context. (Returns null if entites contain ambiguous settings.)
        /// </summary>
        public bool? IsValidationEnabled
        {
            set
            {
                foreach (var entitySet in EntitySets)
                {
                    entitySet.IsValidationEnabled = value ?? true;
                }
                OnPropertyChanged("IsValidationEnabled");
            }
            get
            {
                // enabled in all entity sets
                var isValidationEnabled = EntitySets.All(entitySet => entitySet.IsValidationEnabled);
                if (isValidationEnabled) return true;

                // disabled in all entity sets
                var isValidationDisabled = EntitySets.All(entitySet => !entitySet.IsValidationEnabled);
                if (isValidationDisabled) return false;

                // ambiguous settings
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the System.Windows.Threading.Dispatcher to be used to synchronize with the main thread from async workers. By default the dispatcher is set to the thread creating the data context.
        /// </summary>
        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
            set
            {
                _dispatcher = value;
                OnPropertyChanged("Dispatcher");
            }
        }
        private System.Windows.Threading.Dispatcher _dispatcher = null;

        /// <summary>
        /// Returns true if one or more entities attached to this context have pending changes, false otherwise
        /// </summary>
        public virtual bool HasChanges { get { return EntitySets.Any(set => set.HasChanges); } }

        /// <summary>
        /// Returns true if all entities attached to this data contect are valid, false otherwise
        /// </summary>
        public virtual bool IsValid { get { return EntitySets.All(set => set.IsValid); } }

        #endregion

        #region Save / Accept / Discart / ClearErrors

        /// <summary>
        /// Occurs upon completion of SaveChangesAsync method
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> SaveChangesCompleted;

        protected virtual void OnSaveChangesCompleted(Exception error = null)
        {
            var saveChangesCompleted = SaveChangesCompleted;
            if (saveChangesCompleted != null)
            {
                // raise event on calling thread
                Invoke(
                    delegate
                    {
                        saveChangesCompleted(this, new AsyncCompletedEventArgs(error, false, null)); 
                    }  
#if !SILVERLIGHT
                    , invokeAsync: true
#endif
                ); 
            }
            else
            {
                if (error != null)
                {
                    // if an error occurred and no callback handler is registered, exception is re-thrown and ...
                    // NET: ...and is required to be handled using the task instance (for .NET you have the option to register the SaveChangesCompleted event or handle the Task instance)
                    // SL:  ...and may not be handled anymore (for Silverlight it is mandatory to register the SaveChangesCompleted event or use a callback handler)
                    throw new Exception("Unhandled Exception", error);
                }
            }
        }

        protected bool HasSaveChangesCompletedHandler { get { return SaveChangesCompleted != null; } }


        /// <summary>
        /// Clears error entries an all entites attached to this context
        /// </summary>
        public virtual void ClearErrors()
        {
            SuppressHasChanges = true;
            try
            {
                foreach (var entitySet in EntitySets)
                {
                    entitySet.ClearErrors();
                }
            }
            finally
            {
                SuppressHasChanges = false;
                OnPropertyChanged("HasChanges");
            }
        }

        /// <summary>
        /// Accepts pending changes in all entities attached to this context
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        public abstract void AcceptChanges(bool onlyForValidEntities = false);


        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this context, re-attaches detached entities, and removes added entities
        /// </summary>
        public abstract void RevertChanges();

        public abstract void DiscartServerGeneratedValues();

        #endregion Save / Accept / Discart / ClearErrors

        #region Helper Methods

        #region EntitySet factory method

        protected virtual IEntitySet<TEntity> CreateEntitySet<TEntity>(InternalEntitySet<TEntity> entitySet, AttachDelegate<TEntity> attachDelegate, DetachDelegate<TEntity> detachDelegate, QueryDelegate<TEntity> queryDelegate) where TEntity : Entity
        {
            return new EntitySet<TEntity>(this, entitySet, attachDelegate, detachDelegate, queryDelegate);
        }

        protected InternalEntitySet<TEntity> CreateAndRegisterInternalEntitySet<TEntity>() where TEntity:Entity
        {
            var entitySet = new InternalEntitySet<TEntity>();
            
            _entitySets.Add(entitySet);

            entitySet.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "HasChanges":
                        if (SuppressHasChanges) return;
                        OnPropertyChanged("HasChanges");
                        break;
                }
            };

            return entitySet;
        }

        #endregion EntitySet factory method

        protected void Refresh<TEntity>(IInternalEntitySet<TEntity> internalEntitySet, IEnumerable<TEntity> changeSet) where TEntity : Entity
        {
            if (changeSet != null && changeSet.Any())
            {
                var changeSetHasErrors = changeSet.Any(entity => entity.Errors.Any(error => error.IsError));

                #region new entities

                var addedEntities = changeSet.Where(p => p.ChangeTracker.State == ObjectState.Added).ToArray();
                var localAddedEntities = internalEntitySet.Where(p => p.ChangeTracker.State == ObjectState.Added && p.IsValid).ToArray();
                // the following check is required since we rely on number and order of added entities 
                // because we may not compare primare key as servergenerated keys are not present in local entities yet
                var addedEntityList = new List<TEntity>();
                if (addedEntities.Length < localAddedEntities.Length)
                {
                    throw new Exception(string.Format("Number of added items does not correspond with number of context items in added state for type {0}", typeof(TEntity).Name));
                }
                if (localAddedEntities.Length > 0)
                {
                    for (int i = 0; i < localAddedEntities.Length; i++)
                    {
                        var localEntity = localAddedEntities[i];
                        var entity = addedEntities[i];
                        addedEntityList.Add(entity);
                        Invoke(delegate
                        {
                            localEntity.Refresh(entity, type: ServerGenerationTypes.Insert);
                            if (!changeSetHasErrors && localEntity.IsValid)
                            {
                                localEntity.AcceptChanges();
                            }
                        });
                    }
                }

                #endregion

                #region modified entities

                foreach (var entity in changeSet)
                {
                    if (entity.ChangeTracker.State == ObjectState.Added)
                    {
                        if (addedEntityList.Contains(entity))
                        {
                            // already handled
                        }
                        else
                        {
                            // add additionally added entity to local data context
                            Invoke(delegate
                            {
                                if (!changeSetHasErrors && entity.IsValid)
                                {
                                    entity.AcceptChanges();
                                }
                                internalEntitySet.Attach(entity);
                            });
                        }
                    }
                    else if (entity.ChangeTracker.State == ObjectState.Deleted && entity.Errors.Count == 0)
                    {
                        internalEntitySet.Detach(entity);
                    }
                    else
                    {
                        var localEntity = internalEntitySet.GetAllEntities().FirstOrDefault(e => e.Equals(entity));
                        if (localEntity == null)
                        {
                            throw new Exception(string.Format("Modified entity of type {0} does not exist in local context.", typeof(TEntity).Name));
                        }
                        Invoke(delegate
                        {
                            localEntity.Refresh(entity, type: ServerGenerationTypes.Update);
                            if (!changeSetHasErrors && localEntity.IsValid)
                            {
                                localEntity.AcceptChanges();
                            }
                        });
                    }
                }

                #endregion
            }
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #region IDisposable

        ~DataContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                }

                // dispose unmanaged resources

                _disposed = true;
            }
        }

        #endregion IDisposable
    }
}
