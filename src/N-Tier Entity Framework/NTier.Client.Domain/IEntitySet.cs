// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public interface IEntitySet<TEntity> : IEnumerable<TEntity>, INotifyCollectionChanged, INotifyPropertyChanged where TEntity : Entity, IObjectWithChangeTracker
    {
        /// <summary>
        /// Adds an entity to the entity set in case it is not contained yet.
        /// Marks the entity as new and starts entity's change tracking.
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// Removes an entity from the entity set.
        /// </summary>
        /// <remarks>
        /// Entities with status added (new) are removed from the entity set.
        /// Other entities are marked as deleted and captured internally until changes are saved and the entity was removed sucessfully from server.
        /// </remarks>
        void Delete(TEntity entity);

        /// <summary>
        /// Removes all entities from the entity set.
        /// </summary>
        /// <remarks>
        /// Entities with status added (new) are removed from the entity set.
        /// Other entities are marked as deleted and captured internally until changes are saved and the entity was removed sucessfully from server.
        /// </remarks>
        void DeleteAll();

        /// <summary>
        /// Adds an entity to the entity set in case it is not contained yet and starts entity's change tracking.
        /// </summary>
        void Attach(TEntity entity);

        /// <summary>
        /// Adds an entity to the entity set in case it is not contained yet and starts entity's change tracking.
        /// </summary>
        /// <remarks>
        /// This method is designed for situations where state of the modified entity could not be tracked properly 
        /// and therefor allowes to specify the entity with its original state as a second property which allowes making a diff between the two. 
        /// </remarks>
        void AttachAsModified(TEntity entity, TEntity original);

        /// <summary>
        /// Removes an entity from the entity set without modifying the entity's state.
        /// </summary>
        void Detach(TEntity entity);

        /// <summary>
        /// Removes all entities from the entity set without modifying the entity's states.
        /// </summary>
        void DetachAll();

        /// <summary>
        /// Returns all entites contained in the entity set, including entities marked as deleted.
        /// </summary>
        IEnumerable<TEntity> GetAllEntities();

        /// <summary>
        /// Accepts pending changes in all entities containes in this entity set
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        void AcceptChanges(bool onlyForValidEntities = false);

        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this entity set, re-attaches detached entities, and removes added entities
        /// </summary>
        void RevertChanges();

        /// <summary>
        /// Creates a new instance of this entity sets entity type
        /// </summary>
        /// <param name="add">If true, adds the new entity to the entity set</param>
        /// <returns>A new instance of this entity sets entity type</returns>
        TEntity CreateNew(bool add = true);

        /// <summary>
        /// Get the record count within the data store
        /// </summary>
        long? TotalCount { get; }

        /// <summary>
        /// Get the record count of the local entity set
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns true if one or more entities contained in this entity set have changes, false otherwise
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Returns true if all entities contained in this entity set are valid, false otherwise
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Get or set whether validation is enabled for all entities attached to this entity set
        /// </summary>
        bool IsValidationEnabled { get; set; }

        /// <summary>
        /// Clears all errors in all entities attached to this entity set
        /// </summary>
        void ClearErrors();

        /// <summary>
        ///  Get or set merge options to be considered when subsequently loading entites retrieved from server
        /// </summary>
        MergeOption MergeOption { get; set; }

        /// <summary>
        ///  Get or set an optional client info instance which has to be included upon each data query for the specific entity typei
        /// </summary>
        ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// Get or set true if entities contained in this data set should be detached before attaching entities retrieved from server in a subsequent data request, false otherwise
        /// </summary>
        bool DetachEntitiesUponNewQueryResult { get; set; }

        /// <summary>
        /// Gets the sync root of this entity set
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets a data service queriable for the specific entity type
        /// </summary>
        IDataServiceQueryable<TEntity> AsQueryable();
    }
}
