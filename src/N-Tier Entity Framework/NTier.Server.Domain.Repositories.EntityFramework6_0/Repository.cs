// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;

namespace NTier.Server.Domain.Repositories.EntityFramework
{
    public abstract class Repository : ObjectContext, IRepository
    {
        #region Constructors

        protected Repository(string connectionString, string containerName)
            : base(connectionString, containerName)
        {
            Initialize();
        }

        protected Repository(EntityConnection connection, string containerName)
            : base(connection, containerName)
        {
            Initialize();
        }

        #endregion

        #region Initialize

        protected virtual void Initialize()
        {
            ContextOptions.LazyLoadingEnabled = false;
            // Creating proxies requires the use of the ProxyDataContractResolver and
            // may allow lazy loading which can expand the loaded graph during serialization.
            ContextOptions.ProxyCreationEnabled = false;
            ObjectMaterialized += new ObjectMaterializedEventHandler(HandleObjectMaterialized);
        }

        private void HandleObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as Entity;
            if (entity != null)
            {
                entity.IsValidationEnabled = true;
                //bool changeTrackingEnabled = entity.ChangeTracker.IsChangeTrackingEnabled;
                //try
                //{
                //    entity.MarkAsUnchanged();
                //}
                //finally
                //{
                //    entity.ChangeTracker.IsChangeTrackingEnabled = changeTrackingEnabled;
                //}
                entity.ChangeTracker.State = ObjectState.Unchanged;
                entity.ChangeTracker.IsChangeTrackingEnabled = false;
                this.StoreReferenceKeyValues(entity);
            }
        }

        #endregion Initialize

        #region SaveChanges

        public override int SaveChanges(SaveOptions options)
        {
            try
            {
                return base.SaveChanges(options);
            }
            catch (System.Data.Entity.Core.OptimisticConcurrencyException ex)
            {
                throw ex.Map();
            }
            catch (System.Data.Entity.Core.UpdateException ex)
            {
                throw ex.Map();
            }
        }

        #endregion SaveChanges

        #region Refresh

        public void Refresh(RefreshMode refreshMode, Entity entity)
        {
            base.Refresh(refreshMode.Map(), entity);
        }

        public void Refresh(RefreshMode refreshMode, IEnumerable<Entity> collection)
        {
            base.Refresh(refreshMode.Map(), collection);
        }

        #endregion Refresh

        #region EntitySet factory method

        protected virtual IEntitySet<T> CreateEntitySet<T>(string entitySetName) where T : Entity
        {
            var objectSet = ReferenceEquals(null, entitySetName) ? CreateObjectSet<T>() : CreateObjectSet<T>(entitySetName);
            return new EntitySet<T>(objectSet);
        }

        public IEntitySet<T> GetEntitySet<T>() where T : Entity
        {
            var entitySet = CreateEntitySet<T>(null);
            return entitySet;
        }

        public virtual Type GetEntitySetType(Type entityType)
        {
            if (ReferenceEquals(null, entityType))
            {
                throw new ArgumentNullException("entityType");
            }

            if (!typeof(Entity).IsAssignableFrom(entityType))
            {
                throw new ArgumentException(string.Format("Type '{0}' is not a subtype of '{1}'.", entityType.FullName, typeof(Entity).FullName), "entityType");
            }

            var entitySetType = entityType;

            while (entitySetType.BaseType != typeof(Entity))
            {
                entitySetType = entitySetType.BaseType;
            }

            return entitySetType;
        }

        #endregion EntitySet factory method
    }
}
