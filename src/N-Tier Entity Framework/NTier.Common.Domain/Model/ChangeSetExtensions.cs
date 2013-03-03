// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace NTier.Common.Domain.Model
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public static class ChangeSetExtensions
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IList<TEntity> GetChangeSet<TEntity>(this IEnumerable<TEntity> source, bool includeOnlyValid = true) where TEntity : Entity
        {
            return (source ?? new List<TEntity>())
                 .Where(e => e.HasChanges && (e.IsValid || !includeOnlyValid || e.ChangeTracker.State == ObjectState.Deleted))
                 .ToList();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IList<Tuple<TEntity,TEntity>> ReduceToModifications<TEntity>(this IList<TEntity> originalList) where TEntity : Entity<TEntity>
        {
            var entities = new List<Tuple<TEntity, TEntity>>();
            foreach (var entity in originalList)
            {
                entities.Add(entity.ReduceToModifications());
            }
            return entities;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<TEntity, TEntity> ReduceToModifications<TEntity>(this TEntity originalEntity) where TEntity : Entity<TEntity>
        {
            TEntity reducedEntity = Entity<TEntity>.CreateNew();
            return new Tuple<TEntity, TEntity>(originalEntity, (TEntity)ReduceToModifications(originalEntity, reducedEntity));
        }

        private static Entity ReduceToModifications(Entity originalEntity, Entity reducedEntity)
        {
            if (reducedEntity == null)
            {
                reducedEntity = (Entity)Activator.CreateInstance(originalEntity.GetType());
            }

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
                        // copy all properties (primitive and complex properties only)
                        var properties = originalEntity.PropertyInfos
                            .Where(p => p.Attributes.Any(attribute => attribute is PrimitivePropertyAttribute || attribute is ComplexPropertyAttribute))
                            .Select(p => p.PropertyInfo);

                        foreach (var property in properties)
                        {
                            object value = property.GetValue(originalEntity, null);
                            property.SetValue(reducedEntity, value, null);
                        }
                    }
                    break;

                case ObjectState.Modified:
                    {
                        reducedEntity.ChangeTracker.IsChangeTrackingEnabled = true;

                        // copy changed properties (primitive and complex properties only)
                        var properties = originalEntity.PropertyInfos
                            .Where(p => p.IsPhysical && 
                                        p.Attributes.Any(attribute => attribute is PrimitivePropertyAttribute || attribute is ComplexPropertyAttribute) &&
                                        originalEntity.ChangeTracker.ModifiedProperties.Contains(p.Name))
                            .Select(p => p.PropertyInfo);

                        foreach (var property in properties)
                        {
                            var value = property.GetValue(originalEntity, null);
                            property.SetValue(reducedEntity, value, null);

                            // in case of default value, ModifiedProperties list need to be filled manualy
                            if (!reducedEntity.ChangeTracker.ModifiedProperties.Contains(property.Name))
                            {
                                reducedEntity.ChangeTracker.ModifiedProperties.Add(property.Name);
                            }
                        }
                    }
                    break;

                case ObjectState.Deleted:
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<Entity> Union(this IChangeSet source, params IEnumerable<Entity>[] entitySets)
        {
            return entitySets.SelectMany(e => e).Cast<Entity>();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<Tuple<Entity, Entity>> Union(this IChangeSet source, params IEnumerable<Tuple<Entity, Entity>>[] entitySets)
        {
            return entitySets.SelectMany(e => e);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<Tuple<Entity, Entity>> CastToEntityTuple<TEntity>(this IEnumerable<Tuple<TEntity, TEntity>> tupleList) where TEntity : Entity
        {
            return tupleList.Select(e => new Tuple<Entity, Entity>(e.Item1, e.Item2));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static void FixupRelations(this IChangeSet source, IEnumerable<Tuple<Entity, Entity>> reducedEntitySet, IEnumerable<Entity> originalChangeSet)
        {
            foreach (var entity in reducedEntitySet)
            {
                entity.FixupRelations(reducedEntitySet, originalChangeSet);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private static void FixupRelations<TEntity>(this Tuple<TEntity, TEntity> transmissionEntityTuple, IEnumerable<Tuple<TEntity, TEntity>> transmissionChangeSet, IEnumerable<TEntity> originalChangeSet) where TEntity : Entity
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
                                            changeSetEntity = ReduceToModifications(entity, null);
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
                    {
                        // copy changed navigation properties (relations and foreign keys)

                        // single relation
                        var physicalProperties = originalEntity.PropertyInfos.Where(p => p.IsPhysical && !p.Name.Contains('.')).ToDictionary(p => p.Name);
                        foreach (var property in originalEntity.ChangeTracker.OriginalValues.Where(p => physicalProperties.ContainsKey(p.Key)))
                        {
                            if (!reducedEntity.ChangeTracker.OriginalValues.ContainsKey(property.Key))
                            {
                                // lookup reduced version of current value (entity)
                                Entity changeSetEntity = null;
                                // get current value (entity)
                                var entity = (Entity)originalEntity.GetProperty(property.Key, true);
                                if (entity != null)
                                {
                                    var entry = transmissionChangeSet.FirstOrDefault(e => e.Item1.Equals(entity));
                                    if (entry != null)
                                    {
                                        changeSetEntity = entry.Item2;
                                    }
                                    else
                                    {
                                        // in case of directed relation (i.e. only one entity has a relation to the other and the other entity has not relation back) 
                                        // the related entity might not yet be in the change set and has to be specifically created
                                        changeSetEntity = ReduceToModifications(entity, null);
                                    }
                                }
                                reducedEntity.SetProperty(property.Key, changeSetEntity, true);
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
                                    changeSetEntity = ReduceToModifications(entity, null);
                                }
                                if (changeSetEntity != null && !navigationProperty.Contains(changeSetEntity))
                                {
                                    navigationProperty.Add(changeSetEntity);
                                    objectList.Add(changeSetEntity);
                                }
                            }
                            if (reducedEntity.ChangeTracker.ObjectsAddedToCollectionProperties.ContainsKey(relation.Key)) continue;
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
                                    changeSetEntity = ReduceToModifications(entity, null);
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

                case ObjectState.Deleted:
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}