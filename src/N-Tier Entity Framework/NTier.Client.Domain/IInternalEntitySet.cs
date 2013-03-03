// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInternalEntitySet<TEntity> : IInternalEntitySet, IEnumerable<TEntity>, INotifyCollectionChanged, INotifyPropertyChanged where TEntity : Entity
    {
        /// <summary>
        /// Adds an entity to the entity set in case it is not contained yet.
        /// Marks the entity as new and starts entity's change tracking.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns existing entity if it is already contained in this entity set, null otherwise</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Adds an entity to the entity set in case it is not contained yet and starts entity's change tracking.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns existing entity if it is already contained in this entity set, null otherwise</returns>
        TEntity Attach(TEntity entity);

        /// <summary>
        /// Removes an entity from the entity set.
        /// </summary>
        /// <remarks>
        /// Entities with status added (new) are deleted from the entity set.
        /// Other entities are only marked as deleted and captured internally until changes are saved and the entity was removed sucessfully from server.
        /// </remarks>
        /// <param name="entity"></param>
        /// <returns>Returns existing entity if it is contained in this entity set, null otherwise</returns>
        TEntity Delete(TEntity entity);

        /// <summary>
        /// Removes all entities from the entity set.
        /// </summary>
        /// <remarks>
        /// Entities with status added (new) are deleted from the entity set.
        /// Other entities are only marked as deleted and captured internally until changes are saved and the entity was removed sucessfully from server.
        /// </remarks>
        void DeleteAll();

        /// <summary>
        /// Removes an entity from the entity set without modifying the entity's state.
        /// </summary>
        /// <param name="entity"></param>
        void Detach(TEntity entity);

        /// <summary>
        /// Removes all entities from the entity set without modifying the entity's states.
        /// </summary>
        void DetachAll();

        /// <summary>
        /// Returns server's total entity count.
        /// </summary>
        /// <remarks>
        /// Server's total entity count is set if <code>IncludeTotalCount</code> was set to true in at least one data query.
        /// </remarks>
        long? DatasourceTotalCount { get; }

        /// <summary>
        /// Returns all entites contained in the entity set, including entities marked as deleted.
        /// </summary>
        IEnumerable<TEntity> GetAllEntities();
        
        /// <summary>
        /// Gets a contained entity which is equal to the entity passed as parameter.
        /// </summary>
        /// <returns> The contained entity which is equal to the entity passed as parameter, or null if such an entity does not exists.</returns>
        TEntity GetExisting(TEntity entity);

        //TEntity GetExistingByReference(TEntity entity);
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInternalEntitySet : System.Collections.IEnumerable, INotifyPropertyChanged
    {
        IDisposable PreventChangetracking();

        /// <summary>
        /// Clears errors on all entities contained in this entity set
        /// </summary>
        void ClearErrors();

        /// <summary>
        /// Returns true if at least one entity contained in this entity set has changes, false otherwise
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Calls <code>AcceptChanges</code> on all entities contained in this entity set
        /// </summary>
        /// <param name="onlyForValidEntities">If set to true, only changes of valid entity are accepted.</param>
        void AcceptChanges(bool onlyForValidEntities = false);

        /// <summary>
        /// Calls <code>RevertChanges</code> on all entities contained in this entity set, re-adds detached entities, and removes added entities
        /// </summary>
        void RevertChanges();

        /// <summary>
        /// Gets or sets <code>IsValidationEnabled</code> flag on this entity set. 
        /// Setting also applies the value on all entities corrently contained in this entity set
        /// </summary>
        bool IsValidationEnabled { get; set; }
    }
}
