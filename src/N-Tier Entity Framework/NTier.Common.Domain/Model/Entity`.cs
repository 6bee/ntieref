// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    // generic abstract base class Entity
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class Entity<T> : Entity where T : Entity<T>
    {
        #region Generic entity factory method

#if !SILVERLIGHT
        private delegate T ObjectActivator();

        private static readonly Lazy<ObjectActivator> _objectActivator = new Lazy<ObjectActivator>(() =>
        {
            var type = typeof(T);
            if (type.IsAbstract)
            {
                return () => null;
            }
            else
            {
                // get constructor by reflection
                var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);

                if (constructor == null)
                {
                    throw new Exception(string.Format("Entity type '{0}' does not implement a parameterless constructor and therefore may not be created using the generic entity factory method.", typeof(T).Name));
                }

                // create a NewExpression that calls the constructor
                var newExpression = System.Linq.Expressions.Expression.New(constructor);

                // create a lambda with the NewExpression as body
                var lambdaExpression = System.Linq.Expressions.Expression.Lambda(typeof(ObjectActivator), newExpression);

                // compile it 
                var compiledObjectActivator = (ObjectActivator)lambdaExpression.Compile();
                return compiledObjectActivator;
            }
        });
#endif

        /// <summary>
        /// Factory method: Creates a new instance of the specific entity type
        /// </summary>
        /// <remarks>This factory methos requires a parameterless constructor to exist on the specific entity type</remarks>
        /// <returns>New instance of the specific entity type</returns>
        public static T CreateNew()
        {
#if SILVERLIGHT
            return Activator.CreateInstance<T>();
#else
            return _objectActivator.Value.Invoke();
#endif
        }

        #endregion Generic entity factory method

        #region Metadata: property infos

        private static readonly Lazy<List<PropertyInfos>> _propertyInfos = new Lazy<List<PropertyInfos>>(() =>
        {
            Dictionary<string, Tuple<PropertyInfo, bool, List<Attribute>>> metadata = new Dictionary<string, Tuple<PropertyInfo, bool, List<Attribute>>>();

            // define property list based on entity type and collect attributes defined on properties
            var properties = typeof(T)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => p.DeclaringType == typeof(T));
            AddMetadata(metadata, properties);

#if !SILVERLIGHT
            // collect and append attributes defined on entity's metatdata types
            foreach (var metadataTypeAttributes in typeof(T).GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>())
            {
                foreach (var member in metadataTypeAttributes.MetadataClassType
                        .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property))
                {
                    var list = metadata.Where(e => e.Key == member.Name).Select(e => e.Value.Item3).SingleOrDefault();
                    if (list != null)
                    {
                        list.AddRange(member.GetCustomAttributes(true).Cast<Attribute>());
                    }
                }
            }
#endif

            return metadata.Select(e => new PropertyInfos(e.Key, e.Value.Item1, e.Value.Item2, e.Value.Item3.AsReadOnly())).ToList();
        });

        private static void AddMetadata(Dictionary<string, Tuple<PropertyInfo, bool, List<Attribute>>> metadata, IEnumerable<PropertyInfo> properties, string prefix = null)
        {
            foreach (var property in properties)
            {
                var propertyName = string.IsNullOrEmpty(prefix) ? property.Name : prefix + property.Name;
                List<Attribute> list;
                if (metadata.ContainsKey(propertyName))
                {
                    list = metadata[propertyName].Item3;
                }
                else
                {
                    list = new List<Attribute>();
                    metadata[propertyName] = new Tuple<PropertyInfo, bool, List<Attribute>>(property, true, list);
                }

                list.AddRange(property.GetCustomAttributes(true).Cast<Attribute>());

                // add attributes for members of complext properties (value types)
                if (property.GetCustomAttributes(typeof(ComplexPropertyAttribute), true).Any())
                {
                    var valueTypeProerties = property.PropertyType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(p => p.DeclaringType == property.PropertyType);

                    var complexPropertyPrefix = propertyName + ".";
                    AddMetadata(metadata, valueTypeProerties, complexPropertyPrefix);

#if !SILVERLIGHT
                    // collect and append attributes defined on complext properties (value types) metatdata types
                    foreach (var metadataTypeAttributes in property.PropertyType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>())
                    {
                        foreach (var member in metadataTypeAttributes.MetadataClassType
                                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property))
                        {
                            list = metadata.Where(e => e.Key == (complexPropertyPrefix + member.Name)).Select(e => e.Value.Item3).SingleOrDefault();
                            if (list != null)
                            {
                                list.AddRange(member.GetCustomAttributes(true).Cast<Attribute>());
                            }
                        }
                    }
#endif
                }
            }
        }

        protected internal override IEnumerable<PropertyInfos> PropertyInfos { get { return _propertyInfos.Value.AsReadOnly(); } }

        /// <summary>
        /// Registers a ValidationAttribute attribute to be included in validation
        /// </summary>
        /// <remarks>This method supports registration for dynamic properties.</remarks>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="validator">The ValidationAttribute attribute</param>
        /// <exception cref="System.ArgumentNullException">If either parameter is null or the property name is empty.</exception>
        public static void RegisterValidator(string propertyName, ValidationAttribute validator)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            if (validator == null) throw new ArgumentNullException("validator");

            // retrieve property info
            var propertyInfo = _propertyInfos.Value.FirstOrDefault(p => p.Name == propertyName);
            if (propertyInfo == null)
            {
                // add as dynamic property if not existing
                propertyInfo = new PropertyInfos(propertyName, null, false, new List<Attribute>(0).AsReadOnly());
                _propertyInfos.Value.Add(propertyInfo);
            }

            // add validator
            propertyInfo.Validators.Add(validator);
        }

        /// <summary>
        /// Unregisters a ValidationAttribute attribute
        /// </summary>
        /// <param name="propertyName">The name of the property for which the validator has been registred</param>
        /// <param name="validator">The ValidationAttribute attribute</param>
        /// <returns>Returns true if the validator was successfully unregistered for the property specified, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">If either parameter is null or the property name is empty.</exception>
        public static bool UnregisterValidator(string propertyName, ValidationAttribute validator)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            if (validator == null) throw new ArgumentNullException("validator");

            var propertyInfo = _propertyInfos.Value.FirstOrDefault(p => p.Name == propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.Validators.Remove(validator);
            }

            return false;
        }

        #endregion Metadata: property infos

        #region IsKeyEqual

        protected abstract bool IsKeyEqual(T entity);

        protected override bool IsKeyEqual(Entity entity)
        {
            return entity is T && IsKeyEqual((T)(object)entity);
        }

        #endregion IsKeyEqual

        #region Refresh

        public override void Refresh(Entity entity, bool trackChanges = true)
        {
            Refresh((Entity<T>)entity, trackChanges);
        }

        public virtual void Refresh(Entity<T> entity, bool trackChanges = true, bool preserveExistingChanges = false, ServerGenerationTypes type = ServerGenerationTypes.None)
        {
            bool isChangeTrackingEnabled = ChangeTracker.IsChangeTrackingEnabled;
            try
            {
                ChangeTracker.IsChangeTrackingEnabled = trackChanges;

                var properties = PropertyInfos.Where(p => p.IsPhysical && p.Attributes.Any(attribute => attribute is SimplePropertyAttribute || attribute is ComplexPropertyAttribute));

                if (type != ServerGenerationTypes.None)
                {
                    properties = properties
                        .Where(p => p.Attributes.Any(
                            a =>
                            {
                                var attribute = a as ServerGenerationAttribute;
                                return attribute != null &&
                                    (
                                        (attribute.Type & ServerGenerationTypes.Insert) == (type & ServerGenerationTypes.Insert) ||
                                        (attribute.Type & ServerGenerationTypes.Update) == (type & ServerGenerationTypes.Update)
                                    );
                            }
                        ));
                }

                // copy properties
                foreach (var property in properties.Select(e => e.PropertyInfo))
                {
                    if (preserveExistingChanges && ChangeTracker.ModifiedProperties.Contains(property.Name))
                    {
                        continue;
                    }
                    object newValue = property.GetValue(entity, null);
                    object oldValue = property.GetValue(this, null);
                    if (!object.Equals(newValue, oldValue))
                    {
                        property.SetValue(this, newValue, null);
                    }
                }

                // merge errors
                foreach (var error in entity.Errors)
                {
                    Errors.Add(error);
                    // notify ui
                    if (error.MemberNames.Any())
                    {
                        foreach (var property in error.MemberNames)
                        {
                            OnPropertyChanged(property, trackInChangeTracker: false);
                        }
                    }
                }
            }
            finally
            {
                ChangeTracker.IsChangeTrackingEnabled = isChangeTrackingEnabled;
            }
        }

        #endregion Refresh
    }
}
