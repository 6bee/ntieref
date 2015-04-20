// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NTier.Client.Domain
{
    public abstract class ChangeSetFactory
    {
        protected sealed class EntityTuple<T>
        {
            public EntityTuple(T entity, T reducedEntity)
            {
                Entity = entity;
                ReducedEntity = reducedEntity;
            }

            public T Entity { get; set; }

            public T ReducedEntity { get; set; }
        }


        /// <summary>
        /// Returns modified entities
        /// </summary>
        protected virtual IList<TEntity> GetChangeSet<TEntity>(IEnumerable<TEntity> source, bool includeOnlyValid = true) where TEntity : Entity
        {
            return (source ?? Enumerable.Empty<TEntity>())
                 .Where(e => e.HasChanges && (e.IsValid || !includeOnlyValid || e.ChangeTracker.State == ObjectState.Deleted))
                 .ToList();
        }

        /// <summary>
        /// Copy changed values
        /// </summary>
        protected virtual IList<EntityTuple<TEntity>> ReduceToModifications<TEntity>(IList<TEntity> originalList) where TEntity : Entity
        {
            var entities = new List<EntityTuple<TEntity>>();

            foreach (var originalEntity in originalList)
            {
                var reducedEntity = ReduceToModifications(originalEntity);
                var tuple = new EntityTuple<TEntity>(originalEntity, reducedEntity);
                entities.Add(tuple);
            }

            return entities;
        }

        /// <summary>
        /// Copy changed values
        /// </summary>
        protected virtual TEntity ReduceToModifications<TEntity>(TEntity originalEntity) where TEntity : Entity
        {
            TEntity reducedEntity = (TEntity)Activator.CreateInstance(originalEntity.GetType());

            // copy key
            {
                var pkProperties = originalEntity.PropertyInfos
                    .Where(p => p.Attributes.Any(attribute => attribute is KeyAttribute))
                    .Select(p => p.PropertyInfo);

                foreach (var property in pkProperties)
                {
                    object value = property.GetValue(originalEntity, null);
                    property.SetValue(reducedEntity, value, null);
                }
            }

            // configure change tracker
            reducedEntity.ChangeTracker.State = originalEntity.ChangeTracker.State;
            reducedEntity.ChangeTracker.IsChangeTrackingEnabled = false;

            // copy modified properties
            switch (reducedEntity.ChangeTracker.State)
            {
                case ObjectState.Added:
                    {
                        // copy all properties (simple and complex properties only)
                        var properties = originalEntity.PropertyInfos
                            .Where(p => p.Attributes.Any(attribute => attribute is SimplePropertyAttribute || attribute is ComplexPropertyAttribute))
                            .Select(p => p.PropertyInfo);

                        foreach (var property in properties)
                        {
                            object value = property.GetValue(originalEntity, null);
                            property.SetValue(reducedEntity, value, null);
                        }
                    }
                    break;

                case ObjectState.Modified:
                case ObjectState.Deleted:
                    {
                        // copy changed properties (simple)
                        var simpleProperties = originalEntity.PropertyInfos
                            .Where(p => p.IsPhysical &&
                                        p.Attributes.Any(attribute => attribute is SimplePropertyAttribute) &&
                                        originalEntity.ChangeTracker.ModifiedProperties.Contains(p.Name) &&
                                        originalEntity.ChangeTracker.OriginalValues.ContainsKey(p.Name))
                            .Select(p => p.PropertyInfo);

                        foreach (var property in simpleProperties)
                        {
                            var value = property.GetValue(originalEntity, null);
                            property.SetValue(reducedEntity, value, null);

                            reducedEntity.ChangeTracker.ModifiedProperties.Add(property.Name);
                            reducedEntity.ChangeTracker.OriginalValues[property.Name] = originalEntity.ChangeTracker.OriginalValues[property.Name];
                        }

                        // copy changed properties (complex)
                        var complexProperties = originalEntity.PropertyInfos
                            .Where(p => p.IsPhysical &&
                                        p.Attributes.Any(attribute => attribute is ComplexPropertyAttribute) &&
                                        originalEntity.ChangeTracker.ModifiedProperties.Contains(p.Name))
                            .Select(p => p.PropertyInfo);

                        foreach (var property in complexProperties)
                        {
                            var value = property.GetValue(originalEntity, null);
                            property.SetValue(reducedEntity, value, null);

                            reducedEntity.ChangeTracker.ModifiedProperties.Add(property.Name);
                            foreach (var propertyName in originalEntity.ChangeTracker.OriginalValues.Keys.Where(x => x.StartsWith(string.Format("{0}.", property.Name))))
                            {
                                reducedEntity.ChangeTracker.OriginalValues[propertyName] = originalEntity.ChangeTracker.OriginalValues[propertyName];
                            }
                        }
                    }
                    break;

                case ObjectState.Unchanged:
                    break;

                default:
                    throw new InvalidOperationException();
            }

            // copy properties marked with IncludeOnUpdateAttribute and IncludeOnDeleteAttribute or ConcurrencyPropertyAttribute attributes
            if (reducedEntity.ChangeTracker.State == ObjectState.Modified || reducedEntity.ChangeTracker.State == ObjectState.Deleted)
            {
                using (reducedEntity.ChangeTrackingPrevention())
                {
                    var properties = originalEntity.PropertyInfos
                        .Where(p => p.IsPhysical &&
                                    ((originalEntity.ChangeTracker.State == ObjectState.Modified &&
                                     p.Attributes.Any(attribute => (attribute is IncludeOnUpdateAttribute || attribute is ConcurrencyPropertyAttribute) &&
                                                              !originalEntity.ChangeTracker.ModifiedProperties.Contains(p.Name))) ||
                                    (originalEntity.ChangeTracker.State == ObjectState.Deleted &&
                                     p.Attributes.Any(attribute => attribute is IncludeOnDeleteAttribute || attribute is ConcurrencyPropertyAttribute))))
                        .Select(p => p.PropertyInfo);

                    foreach (var property in properties)
                    {
                        // TODO: in case of delete --> copy original value
                        object value = property.GetValue(originalEntity, null);
                        property.SetValue(reducedEntity, value, null);
                    }
                }
            }

            return reducedEntity;
        }

        protected IEnumerable<EntityTuple<Entity>> CastToEntityTuple<TEntity>(IEnumerable<EntityTuple<TEntity>> tupleList) where TEntity : Entity
        {
            return tupleList.Select(e => new EntityTuple<Entity>(e.Entity, e.ReducedEntity));
        }

        /// <summary>
        /// Replaces related entities with their corresponding clones from the reduced entity set
        /// </summary>
        /// <param name="transmissionEntitySets"></param>
        protected virtual void FixupRelations(params IEnumerable<EntityTuple<Entity>>[] transmissionEntitySets)
        {
            var flatTransmissionSet = transmissionEntitySets.SelectMany(e => e).ToList();

            foreach (var entityTuple in flatTransmissionSet)
            {
                FixupRelations(entityTuple, flatTransmissionSet);
            }
        }

        /// <summary>
        /// Modifies the EntityTuple.ReducedEntity by replacing related entities with their corresponding clones from the reduced entity set
        /// </summary>
        /// <param name="entityTuple"></param>
        /// <param name="flatTransmissionSet"></param>
        protected virtual void FixupRelations(EntityTuple<Entity> entityTuple, IEnumerable<EntityTuple<Entity>> flatTransmissionSet)
        {
            Entity entity = entityTuple.Entity;
            Entity reducedEntity = entityTuple.ReducedEntity;

            switch (reducedEntity.ChangeTracker.State)
            {
                case ObjectState.Added:
                    {
                        // copy navigation properties
                        var navigationProperties = entity.PropertyInfos
                            .Where(p => p.IsPhysical && p.Attributes.Any(attribute => attribute is NavigationPropertyAttribute))
                            .Select(p => p.PropertyInfo);

                        foreach (var navigationProperty in navigationProperties)
                        {
                            var referencedInstance = navigationProperty.GetValue(entity, null);

                            var referencedEntityCollection = referencedInstance as ITrackableCollection;
                            if (!ReferenceEquals(null, referencedEntityCollection))
                            {
                                if (referencedEntityCollection.Count > 0)
                                {
                                    var collection = navigationProperty.GetValue(reducedEntity, null) as ITrackableCollection;

                                    foreach (Entity referencedEntity in referencedEntityCollection)
                                    {
                                        // lookup reduced version of current value (entity)
                                        Entity changeSetEntity;
                                        var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(referencedEntity));
                                        if (entry != null)
                                        {
                                            changeSetEntity = entry.ReducedEntity;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            changeSetEntity = ReduceToModifications(referencedEntity);
                                        }

                                        collection.Add(changeSetEntity);
                                    }
                                }
                            }
                            else if (referencedInstance is Entity)
                            {
                                // lookup reduced version of current value (entity)
                                var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(referencedInstance));
                                if (entry != null)
                                {
                                    navigationProperty.SetValue(reducedEntity, entry.ReducedEntity, null);
                                }
                            }
                        }
                    }
                    break;

                case ObjectState.Unchanged:
                    if (entity.HasChanges)
                    {
                        // there are changes in relations
                        goto case ObjectState.Modified;
                    }
                    break;

                case ObjectState.Modified:
                case ObjectState.Deleted:
                    {
                        // copy changed navigation properties (relations and foreign keys)

                        // single relation
                        var physicalProperties = entity.PropertyInfos.Where(p => p.IsPhysical && p.Attributes.Any(attribute => attribute is NavigationPropertyAttribute)).ToDictionary(p => p.Name);
                        foreach (var property in entity.ChangeTracker.OriginalValues.Where(p => physicalProperties.ContainsKey(p.Key)))
                        {
                            if (!reducedEntity.ChangeTracker.OriginalValues.ContainsKey(property.Key))
                            {
                                // lookup reduced version of current value (entity)
                                Entity currentChangeSetEntity = null;
                                {
                                    // get current value (entity)
                                    var currentEntity = (Entity)entity.GetProperty(property.Key, true);
                                    if (currentEntity != null)
                                    {
                                        var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(currentEntity));
                                        if (entry != null)
                                        {
                                            currentChangeSetEntity = entry.ReducedEntity;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            currentChangeSetEntity = ReduceToModifications(currentEntity);
                                        }
                                    }
                                }

                                Entity originalChangeSetEntity = null;
                                {
                                    // get current value (entity)
                                    var currentEntity = (Entity)entity.ChangeTracker.OriginalValues[property.Key];
                                    if (currentEntity != null)
                                    {
                                        var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(currentEntity));
                                        if (entry != null)
                                        {
                                            originalChangeSetEntity = entry.ReducedEntity;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            originalChangeSetEntity = ReduceToModifications(currentEntity);
                                        }
                                    }
                                }

                                reducedEntity.SetProperty(property.Key, currentChangeSetEntity, true);
                                reducedEntity.ChangeTracker.OriginalValues[property.Key] = originalChangeSetEntity;
                            }
                        }

                        // relation added to collection
                        foreach (var relation in entity.ChangeTracker.ObjectsAddedToCollectionProperties)
                        {
                            var navigationProperty = reducedEntity.GetType().GetProperty(relation.Key).GetValue(reducedEntity, null) as ITrackableCollection;
                            var objectList = new EntityList();
                            foreach (var referencedEntity in relation.Value)
                            {
                                // lookup reduced version of current value (entity)
                                Entity changeSetEntity;
                                var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(referencedEntity));
                                if (entry != null)
                                {
                                    changeSetEntity = entry.ReducedEntity;
                                }
                                else
                                {
                                    // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                    // the related entity might not yet be in the change set and has to be specifically created
                                    changeSetEntity = ReduceToModifications(referencedEntity);
                                }

                                if (changeSetEntity != null && !navigationProperty.Contains(changeSetEntity))
                                {
                                    navigationProperty.Add(changeSetEntity);
                                    objectList.Add(changeSetEntity);
                                }
                            }

                            if (reducedEntity.ChangeTracker.ObjectsAddedToCollectionProperties.ContainsKey(relation.Key))
                            {
                                continue;
                            }

                            reducedEntity.ChangeTracker.ObjectsAddedToCollectionProperties.Add(relation.Key, objectList);
                        }

                        // relation removed from collection
                        foreach (var relation in entity.ChangeTracker.ObjectsRemovedFromCollectionProperties)
                        {
                            var objectList = new EntityList();
                            foreach (var referencedEntity in relation.Value)
                            {
                                // lookup reduced version of current value (entity)
                                Entity changeSetEntity;
                                var entry = flatTransmissionSet.FirstOrDefault(e => e.Entity.Equals(referencedEntity));
                                if (entry != null)
                                {
                                    changeSetEntity = entry.ReducedEntity;
                                }
                                else
                                {
                                    // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has no relation back) 
                                    // the related entity might not yet be in the change set and has to be specifically created
                                    changeSetEntity = ReduceToModifications(referencedEntity);
                                }

                                if (changeSetEntity != null)
                                {
                                    objectList.Add(changeSetEntity);
                                }
                            }

                            reducedEntity.ChangeTracker.ObjectsRemovedFromCollectionProperties.Add(relation.Key, objectList);
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
