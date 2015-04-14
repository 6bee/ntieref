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
        /// <summary>
        /// Retursn modified entities
        /// </summary>
        protected virtual IList<TEntity> GetChangeSet<TEntity>(IEnumerable<TEntity> source, bool includeOnlyValid = true) where TEntity : Entity
        {
            return (source ?? new List<TEntity>())
                 .Where(e => e.HasChanges && (e.IsValid || !includeOnlyValid || e.ChangeTracker.State == ObjectState.Deleted))
                 .ToList();
        }

        /// <summary>
        /// Copy changed values
        /// </summary>
        protected virtual IList<Tuple<TEntity, TEntity>> ReduceToModifications<TEntity>(IList<TEntity> originalList) where TEntity : Entity
        {
            var entities = new List<Tuple<TEntity, TEntity>>();

            foreach (var originalEntity in originalList)
            {
                var reducedEntity = ReduceToModifications(originalEntity);
                var tuple = new Tuple<TEntity, TEntity>(originalEntity, reducedEntity);
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

        protected virtual IEnumerable<Entity> Union(params IEnumerable<Entity>[] entitySets)
        {
            return entitySets.SelectMany(e => e).Cast<Entity>();
        }

        protected virtual IEnumerable<Tuple<Entity, Entity>> Union(params IEnumerable<Tuple<Entity, Entity>>[] entitySets)
        {
            return entitySets.SelectMany(e => e);
        }

        protected virtual IEnumerable<Tuple<Entity, Entity>> CastToEntityTuple<TEntity>(IEnumerable<Tuple<TEntity, TEntity>> tupleList) where TEntity : Entity
        {
            return tupleList.Select(e => new Tuple<Entity, Entity>(e.Item1, e.Item2));
        }

        /// <summary>
        /// Replaces related entities with their corresponding clones from the reduced entity set
        /// </summary>
        /// <param name="reducedEntitySet"></param>
        /// <param name="originalChangeSet"></param>
        protected virtual void FixupRelations(IEnumerable<Tuple<Entity, Entity>> reducedEntitySet, IEnumerable<Entity> originalChangeSet)
        {
            foreach (var entity in reducedEntitySet)
            {
                FixupRelations(entity, reducedEntitySet, originalChangeSet);
            }
        }

        private void FixupRelations<TEntity>(Tuple<TEntity, TEntity> transmissionEntityTuple, IEnumerable<Tuple<TEntity, TEntity>> transmissionChangeSet, IEnumerable<TEntity> originalChangeSet) where TEntity : Entity
        {
            TEntity originalEntity = transmissionEntityTuple.Item1;
            TEntity reducedEntity = transmissionEntityTuple.Item2;

            switch (reducedEntity.ChangeTracker.State)
            {
                case ObjectState.Added:
                    {
                        // copy navigation properties
                        var navigationProperties = originalEntity.PropertyInfos
                            .Where(p => p.IsPhysical && p.Attributes.Any(attribute => attribute is NavigationPropertyAttribute))
                            .Select(p => p.PropertyInfo);

                        foreach (var navigationProperty in navigationProperties)
                        {
                            var property = navigationProperty.GetValue(originalEntity, null);

                            if (property is ITrackableCollection)
                            {
                                if (((ITrackableCollection)property).Count > 0)
                                {
                                    var collection = navigationProperty.GetValue(reducedEntity, null) as ITrackableCollection;

                                    foreach (Entity entity in (ITrackableCollection)property)
                                    {
                                        // lookup reduced version of current value (entity)
                                        Entity changeSetEntity;
                                        var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                        if (entry != null)
                                        {
                                            changeSetEntity = entry.Item2;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            changeSetEntity = ReduceToModifications(entity);
                                        }

                                        collection.Add(changeSetEntity);
                                    }
                                }
                            }
                            else if (property is Entity)
                            {
                                // lookup reduced version of current value (entity)
                                var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(property));
                                if (entry != null)
                                {
                                    navigationProperty.SetValue(reducedEntity, entry.Item2, null);
                                }
                            }
                        }
                    }
                    break;

                case ObjectState.Unchanged:
                    if (originalEntity.HasChanges)
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
                        var physicalProperties = originalEntity.PropertyInfos.Where(p => p.IsPhysical && p.Attributes.Any(attribute => attribute is NavigationPropertyAttribute)).ToDictionary(p => p.Name);
                        foreach (var property in originalEntity.ChangeTracker.OriginalValues.Where(p => physicalProperties.ContainsKey(p.Key)))
                        {
                            if (!reducedEntity.ChangeTracker.OriginalValues.ContainsKey(property.Key))
                            {
                                // lookup reduced version of current value (entity)
                                Entity currentChangeSetEntity = null;
                                {
                                    // get current value (entity)
                                    var entity = (Entity)originalEntity.GetProperty(property.Key, true);
                                    if (entity != null)
                                    {
                                        var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                        if (entry != null)
                                        {
                                            currentChangeSetEntity = entry.Item2;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            currentChangeSetEntity = ReduceToModifications(entity);
                                        }
                                    }
                                }

                                Entity originalChangeSetEntity = null;
                                {
                                    // get current value (entity)
                                    var entity = (Entity)originalEntity.ChangeTracker.OriginalValues[property.Key];
                                    if (entity != null)
                                    {
                                        var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                        if (entry != null)
                                        {
                                            originalChangeSetEntity = entry.Item2;
                                        }
                                        else
                                        {
                                            // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                            // the related entity might not yet be in the change set and has to be specifically created
                                            originalChangeSetEntity = ReduceToModifications(entity);
                                        }
                                    }
                                }

                                reducedEntity.SetProperty(property.Key, currentChangeSetEntity, true);
                                reducedEntity.ChangeTracker.OriginalValues[property.Key] = originalChangeSetEntity;
                            }
                        }

                        // relation added to collection
                        foreach (var relation in originalEntity.ChangeTracker.ObjectsAddedToCollectionProperties)
                        {
                            var navigationProperty = reducedEntity.GetType().GetProperty(relation.Key).GetValue(reducedEntity, null) as ITrackableCollection;
                            var objectList = new EntityList();
                            foreach (var entity in relation.Value)
                            {
                                // lookup reduced version of current value (entity)
                                Entity changeSetEntity;
                                var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                if (entry != null)
                                {
                                    changeSetEntity = entry.Item2;
                                }
                                else
                                {
                                    // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                    // the related entity might not yet be in the change set and has to be specifically created
                                    changeSetEntity = ReduceToModifications(entity);
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
                        foreach (var relation in originalEntity.ChangeTracker.ObjectsRemovedFromCollectionProperties)
                        {
                            var objectList = new EntityList();
                            foreach (var entity in relation.Value)
                            {
                                // lookup reduced version of current value (entity)
                                Entity changeSetEntity;
                                var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                if (entry != null)
                                {
                                    changeSetEntity = entry.Item2;
                                }
                                else
                                {
                                    // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has no relation back) 
                                    // the related entity might not yet be in the change set and has to be specifically created
                                    changeSetEntity = ReduceToModifications(entity);
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
