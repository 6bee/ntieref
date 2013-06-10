// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using NTier.Common.Domain.Model;

namespace NTier.Server.Domain.Repositories
{
    [System.Obsolete("This class was moved to a different namespace: NTier.Server.Domain.Repositories.EntityFramework", true)]
    public abstract class Repository : NTier.Server.Domain.Repositories.EntityFramework.Repository
    {
        protected Repository(string connectionString, string containerName)
            : base(connectionString, containerName)
        {
        }

        protected Repository(EntityConnection connection, string containerName)
            : base(connection, containerName)
        {
        }
    }
}

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

        #region Refresh

        public void Refresh(RefreshMode refreshMode, Entity entity)
        {
            base.Refresh(refreshMode, entity);
        }

        public void Refresh(RefreshMode refreshMode, IEnumerable<Entity> collection)
        {
            base.Refresh(refreshMode, collection);
        }

        #endregion Refresh

        #region EntitySet factory method

        protected virtual IEntitySet<T> CreateEntitySet<T>(string entitySetName) where T : Entity
        {
            var objectSet = CreateObjectSet<T>(entitySetName);
            return new EntitySet<T>(objectSet);
        }

        #endregion EntitySet factory method
    }
}
