﻿// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    // abstract base class Entity
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract partial class Entity : IObjectWithChangeTracker, IEditable, INotifyPropertyChanging, INotifyPropertyChanged, IDataErrorInfo
    {
        #region Metadata: property infos

        private static readonly Dictionary<Type, ReadOnlyCollection<PropertyInfos>> _propertyInfosRegistry = new Dictionary<Type, ReadOnlyCollection<PropertyInfos>>();

        private static ReadOnlyCollection<PropertyInfos> GetPropertyInfos(Type type)
        {
            ReadOnlyCollection<PropertyInfos> propertyInfos;
            lock (_propertyInfosRegistry)
            {
                if (!_propertyInfosRegistry.TryGetValue(type, out propertyInfos))
                {
                    Dictionary<string, Tuple<PropertyInfo, bool, List<Attribute>>> metadata = new Dictionary<string, Tuple<PropertyInfo, bool, List<Attribute>>>();

                    // define property list based on entity type and collect attributes defined on properties
                    var properties = type
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(p => p.DeclaringType.IsAssignableFrom(type))
                            .Where(p => p.DeclaringType != typeof(Entity));
                    AddMetadata(metadata, properties);

#if !SILVERLIGHT
                    // collect and append attributes defined on entity's metatdata types
                    foreach (var metadataTypeAttributes in type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>())
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
                    propertyInfos = metadata.Select(e => new PropertyInfos(e.Key, e.Value.Item1, e.Value.Item2, e.Value.Item3)).ToList().AsReadOnly();
                    _propertyInfosRegistry.Add(type, propertyInfos);
                }
            }
            return propertyInfos;
        }

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

        /// <summary>
        /// Registers a ValidationAttribute attribute to be included in validation
        /// </summary>
        /// <remarks>This method supports registration for dynamic properties.</remarks>
        /// <param name="entityType">Type of entity for validator to be registered</param>
        /// <param name="propertyName">The name of the property for which the validator has to be registred</param>
        /// <param name="validator">The validation attribute</param>
        /// <exception cref="System.ArgumentNullException">If any parameter is null or the property name is empty.</exception>
        /// <exception cref="System.ArgumentException">If entity type is not sub-type of Entity.</exception>
        public static void RegisterValidator(Type entityType, string propertyName, ValidationAttribute validator)
        {
            if (ReferenceEquals(null, entityType)) throw new ArgumentNullException("entity type must not be null", "entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType) || entityType == typeof(Entity)) throw new ArgumentException(string.Format("entity type expected to be sub-type of {0}", typeof(Entity).FullName), "entityType");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("property name must not be null or empty", "propertyName");
            if (ReferenceEquals(null, validator)) throw new ArgumentNullException("validation attribute must not be null", "validator");

            lock (_propertyInfosRegistry)
            {
                var propertyInfos = GetPropertyInfos(entityType);
                var propertyInfo = propertyInfos.FirstOrDefault(p => p.Name == propertyName);
                if (propertyInfo == null)
                {
                    propertyInfo = new PropertyInfos(propertyName, null, false, new Attribute[] { validator });
                    propertyInfos = propertyInfos.Concat(new[] { propertyInfo }).ToList().AsReadOnly();
                }
                else
                {
                    var attributes = propertyInfo.Attributes.Concat(new Attribute[] { validator });
                    var newPropertyInfo = new PropertyInfos(propertyInfo.Name, propertyInfo.PropertyInfo, propertyInfo.IsPhysical, attributes);
                    var newPropertyInfoList = propertyInfos.ToList();
                    newPropertyInfoList.Remove(propertyInfo);
                    newPropertyInfoList.Add(newPropertyInfo);
                    propertyInfos = newPropertyInfoList.AsReadOnly();
                }
                _propertyInfosRegistry[entityType] = propertyInfos;
            }
        }

        /// <summary>
        /// Unregisters a ValidationAttribute attribute
        /// </summary>
        /// <param name="entityType">Type of entity where validator was registered</param>
        /// <param name="propertyName">The name of the property on which the validator has been registred</param>
        /// <param name="validator">The validation attribute</param>
        /// <returns>Returns true if the validator was successfully unregistered for the property specified, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">If either parameter is null or the property name is empty.</exception>
        public static bool UnregisterValidator(Type entityType, string propertyName, ValidationAttribute validator)
        {
            if (ReferenceEquals(null, entityType)) throw new ArgumentNullException("entity type must not be null", "entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType) || entityType == typeof(Entity)) throw new ArgumentException(string.Format("entity type expected to be sub-type of {0}", typeof(Entity).FullName), "entityType");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("property name must not be null or empty", "propertyName");
            if (ReferenceEquals(null, validator)) throw new ArgumentNullException("validation attribute must not be null", "validator");

            lock (_propertyInfosRegistry)
            {
                var propertyInfos = GetPropertyInfos(entityType);
                var propertyInfo = propertyInfos.FirstOrDefault(p => p.Name == propertyName);
                if (propertyInfo != null)
                {
                    var attributes = propertyInfo.Attributes.ToList();
                    if (attributes.Remove(validator))
                    {
                        var newPropertyInfo = new PropertyInfos(propertyInfo.Name, propertyInfo.PropertyInfo, propertyInfo.IsPhysical, attributes);
                        var newPropertyInfoList = propertyInfos.ToList();
                        newPropertyInfoList.Remove(propertyInfo);
                        newPropertyInfoList.Add(newPropertyInfo);
                        propertyInfos = newPropertyInfoList.AsReadOnly();
                    }
                    _propertyInfosRegistry[entityType] = propertyInfos;
                    return true;
                }
            }

            return false;
        }

        protected internal IEnumerable<PropertyInfos> PropertyInfos { get { return GetPropertyInfos(GetType()); } }

        #endregion Metadata: property infos

        #region Indexer

        /// <summary>
        /// Gets or sets the value of a physical or dynamic property. Dynamic properties are supported only if the concrete entity type supports them.
        /// </summary>
        /// <param name="propertyName">The name of a physical or dynamic property</param>
        [Display(AutoGenerateField = false)]
        public virtual object this[string propertyName]
        {
            get
            {
                if (PropertyInfos.Any(p => p.IsPhysical && p.Name == propertyName))
                {
                    // physical property
                    return GetProperty(propertyName);
                }
                else
                {
                    // dynamic property
                    if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

                    return GetDynamicValue(propertyName);
                }
            }
            set
            {
                if (PropertyInfos.Any(p => p.IsPhysical && p.Name == propertyName))
                {
                    // physical property
                    SetProperty(propertyName, value);
                }
                else
                {
                    // dynamic property
                    if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

                    // get previous value
                    var previousValue = GetDynamicValue(propertyName);

                    // raise property changing
                    OnPropertyChanging(propertyName, value);

                    // set property
                    SetDynamicValue(propertyName, value);

                    // raise property chnaged
                    OnPropertyChanged(propertyName, previousValue, value);
                }
            }
        }

        internal object GetProperty(string propertyName, bool includePrivateProperties = false)
        {
            var separatorIndex = propertyName.IndexOf('.');
            if (separatorIndex > 0)
            {
                var complextPropertyName = propertyName.Substring(0, separatorIndex);
                var complexPropertyMemberName = propertyName.Substring(separatorIndex + 1, propertyName.Length - (separatorIndex + 1));
                var complexType = GetProperty(complextPropertyName, includePrivateProperties) as NTier.Common.Domain.Model.ValueObject;
                if (complexType == null) throw new Exception(string.Format("Expected property '{0}' to be of type '{1}'", complextPropertyName, typeof(NTier.Common.Domain.Model.ValueObject).FullName));
                return complexType[complexPropertyMemberName];
            }
            else if (includePrivateProperties)
            {
                return GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this, null);
            }
            else
            {
                return GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, this, null);
            }
        }

        internal void SetProperty(string propertyName, object value, bool includePrivateProperties = false)
        {
            try
            {
                // TOOD: to be tested
                var separatorIndex = propertyName.IndexOf('.');
                if (separatorIndex > 0)
                {
                    var complextPropertyName = propertyName.Substring(0, separatorIndex);
                    var complexPropertyMemberName = propertyName.Substring(separatorIndex + 1, propertyName.Length - (separatorIndex + 1));
                    var complexType = GetProperty(complextPropertyName, includePrivateProperties) as NTier.Common.Domain.Model.ValueObject;
                    if (complexType == null) throw new Exception(string.Format("Expected property '{0}' to be of type '{1}'", complextPropertyName, typeof(NTier.Common.Domain.Model.ValueObject).FullName));
                    complexType[complexPropertyMemberName] = value;
                }
                else if (includePrivateProperties)
                {
                    GetType().InvokeMember(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this, new[] { value });
                }
                else
                {
                    GetType().InvokeMember(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, this, new[] { value });
                }
            }
            catch (TargetInvocationException ex)
            {
                // unwrap validation exception
                var validationException = ex.InnerException as ValidationException;
                if (validationException != null) throw validationException;
                throw;
            }
        }

        protected virtual object GetDynamicValue(string propertyName)
        {
            throw new NotImplementedException("Dynamic properties are not supported by this entity.");
        }

        protected virtual void SetDynamicValue(string propertyName, object value)
        {
            throw new NotImplementedException("Dynamic properties are not supported by this entity.");
        }

        #endregion Indexer

        #region ChangeTracking

        #region IsDeserializing

        [Display(AutoGenerateField = false)]
        protected bool IsDeserializing
        {
            get { return _isDeserializing; }
        }
        [NonSerialized]
        private bool _isDeserializing;

        #endregion IsDeserializing

        #region HasChanges

        [Display(AutoGenerateField = false)]
        public virtual bool HasChanges
        {
            get
            {
                return ChangeTracker.State != ObjectState.Unchanged ||
                       ChangeTracker.ObjectsAddedToCollectionProperties.Any() ||
                       ChangeTracker.ObjectsRemovedFromCollectionProperties.Any();
            }
        }

        #endregion HasChanges

        #region ChangeTracker

        [DataMember]
        [Display(AutoGenerateField = false)]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.PropertyChanged += ChangeTracker_PropertyChanged;
                }
                return _changeTracker;
            }
            set
            {
                if (_changeTracker != null)
                {
                    _changeTracker.PropertyChanged -= ChangeTracker_PropertyChanged;
                }
                _changeTracker = value;
                if (_changeTracker != null)
                {
                    _changeTracker.PropertyChanged += ChangeTracker_PropertyChanged;
                }
                OnPropertyChanged("ChangeTracker", trackInChangeTracker: false);
            }
        }
        [NonSerialized]
        private ObjectChangeTracker _changeTracker;

        private void ChangeTracker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" && ChangeTracker.State == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(string.Format("ChangeTracker.{0}", e.PropertyName)));
                propertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
            }
        }

        protected virtual void ClearNavigationProperties()
        {
        }

        /// <summary>
        /// This event is used by entity types that are the dependent end in at least one association that performs cascade deletes.
        /// This event handler will process notifications that occur when the principal end is deleted.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void HandleCascadeDelete(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" &&
                sender is IObjectWithChangeTracker &&
                ((IObjectWithChangeTracker)sender).ChangeTracker.State == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
            }
        }

        #endregion ChangeTracker

        public virtual void AcceptChanges()
        {
            var properties = ChangeTracker.OriginalValues.Select(p => p.Key).ToArray();

            ChangeTracker.AcceptChanges();

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                foreach (var property in properties)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(property));
                }
                propertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
            }
        }

        /// <summary>
        /// Returns true while changes are beeing reverted, false otherwise.
        /// </summary>
        protected bool IsRevertingChanges { get { return _isRevertingChanges; } }
        private bool _isRevertingChanges = false;

        public virtual void RevertChanges(string propertyName = null)
        {
            _isRevertingChanges = true;
            var isValidationEnabled = _isValidationEnabled;
            try
            {
                _isValidationEnabled = false;
                if (propertyName == null)
                {
                    // supposing that dynamic properties are eventually stored within a physical property, dynamic properties are skipped
                    var physicalProperties = PropertyInfos.Where(p => p.IsPhysical).ToDictionary(p => p.Name);
                    foreach (var property in ChangeTracker.OriginalValues.Where(p => physicalProperties.ContainsKey(p.Key)).Reverse())
                    {
                        SetProperty(property.Key, property.Value, true);
                    }

                    // reset complex property members
                    // TODO: to be tested: create unit test (changed
                    foreach (var property in ChangeTracker.OriginalValues.Where(p => p.Key.Contains('.')).Reverse())
                    {
                        this[property.Key] = property.Value;
                    }

                    foreach (var relation in ChangeTracker.ObjectsAddedToCollectionProperties.ToArray().Reverse())
                    {
                        var navigationProperty = GetType().GetProperty(relation.Key).GetValue(this, null) as ITrackableCollection;
                        foreach (var entity in relation.Value.ToArray().Reverse())
                        {
                            navigationProperty.Remove(entity);
                        }
                    }

                    foreach (var relation in ChangeTracker.ObjectsRemovedFromCollectionProperties.ToArray().Reverse())
                    {
                        var navigationProperty = GetType().GetProperty(relation.Key).GetValue(this, null) as ITrackableCollection;
                        foreach (var entity in relation.Value.ToArray().Reverse())
                        {
                            navigationProperty.Add(entity);
                        }
                    }

                    AcceptChanges();
                }
                else if (ChangeTracker.OriginalValues.ContainsKey(propertyName))
                {
                    var originalValue = ChangeTracker.OriginalValues[propertyName];
                    this[propertyName] = originalValue;
                }
                else if (ChangeTracker.ObjectsAddedToCollectionProperties.ContainsKey(propertyName))
                {
                    var entityList = ChangeTracker.ObjectsAddedToCollectionProperties[propertyName];

                    var navigationProperty = GetType().GetProperty(propertyName).GetValue(this, null) as ITrackableCollection;
                    foreach (var entity in entityList.ToArray().Reverse())
                    {
                        navigationProperty.Remove(entity);
                    }

                    if (entityList.Count == 0)
                    {
                        ChangeTracker.ObjectsAddedToCollectionProperties.Remove(propertyName);
                    }
                }
                else if (ChangeTracker.ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                {
                    var entityList = ChangeTracker.ObjectsRemovedFromCollectionProperties[propertyName];

                    var navigationProperty = GetType().GetProperty(propertyName).GetValue(this, null) as ITrackableCollection;
                    foreach (var entity in entityList.ToArray().Reverse())
                    {
                        navigationProperty.Add(entity);
                    }

                    if (entityList.Count == 0)
                    {
                        ChangeTracker.ObjectsRemovedFromCollectionProperties.Remove(propertyName);
                    }
                }
            }
            finally
            {
                _isValidationEnabled = isValidationEnabled;
                _isRevertingChanges = false;
            }
        }

        #region OnDeserializing / OnDeserialized

        [OnDeserializing]
#if SILVERLIGHT
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public
#endif
        void OnDeserializingMethod(StreamingContext context)
        {
            _isDeserializing = true;
        }

        [OnDeserialized]
#if SILVERLIGHT
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public
#endif
        void OnDeserializedMethod(StreamingContext context)
        {
            _isDeserializing = false;
            ChangeTracker.IsChangeTrackingEnabled = true;
        }

        #endregion OnDeserializing / OnDeserialized

        #region ChangeTrackingPrevention

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public IDisposable ChangeTrackingPrevention()
        {
            return new ChangeTrackingPreventer(this);
        }

        private sealed class ChangeTrackingPreventer : IDisposable
        {
            private readonly Entity _entity;

            public ChangeTrackingPreventer(Entity entity)
            {
                _entity = entity;
                _entity._isDeserializing = true;
            }

            public void Dispose()
            {
                _entity._isDeserializing = false;
            }
        }

        [Display(AutoGenerateField = false)]
        public bool IsChangeTrackingPrevented
        {
            get { return _isDeserializing; }
        }

        #endregion ChangeTrackingPrevention

        protected void RecordOriginalValue(string propertyName, object oldValue, object newValue)
        {
            ChangeTracker.RecordOriginalValue(propertyName, oldValue, newValue);
            RecordOriginalValueInEditableEntityChangeTracker(propertyName, oldValue);
        }

        protected void RecordAdditionToCollectionProperties(string propertyName, Entity value)
        {
            ChangeTracker.RecordAdditionToCollectionProperties(propertyName, value);
            RecordAdditionToCollectionPropertiesInEditableEntityChangeTracker(propertyName, value);
        }

        protected void RecordRemovalFromCollectionProperties(string propertyName, Entity value)
        {
            ChangeTracker.RecordRemovalFromCollectionProperties(propertyName, value);
            RecordRemovalFromCollectionPropertiesInEditableEntityChangeTracker(propertyName, value);
        }

        #endregion ChangeTracking

        #region IEditable

        /// <summary>
        /// Holds a reference to a change tracker while editing within a unit of work
        /// </summary>
        [NonSerialized]
        private EditableEntityChangeTracker _editableEntityChangeTracker;

        /// <summary>
        /// Is set to true while in cancel 
        /// </summary>
        private bool _isInCancelEdit = false;

        bool IEditable.CanEdit
        {
            get { return true; }
        }

        bool IEditable.CanCancelEdit
        {
            get { return _editableEntityChangeTracker != null; }
        }

        bool IEditable.IsEditing
        {
            get { return _editableEntityChangeTracker != null; }
        }

        void IEditable.BeginEdit()
        {
            if (!((IEditable)this).CanEdit)
            {
                throw new Exception("This entity may not be edited.");
            }
            if (_editableEntityChangeTracker != null)
            {
                throw new Exception("BeginEdit() must not be called while entity is editing.");
            }

            _editableEntityChangeTracker = new EditableEntityChangeTracker();
        }

        void IEditable.EndEdit()
        {
            if (_editableEntityChangeTracker == null)
            {
                throw new Exception("EndEdit() may only be called while entity is editing.");
            }

            _editableEntityChangeTracker = null;
        }

        void IEditable.CancelEdit()
        {
            if (_editableEntityChangeTracker == null)
            {
                throw new Exception("CancelEdit() may only be called while entity is editing.");
            }

            var isValidationEnabled = _isValidationEnabled;
            try
            {
                _isValidationEnabled = false;
                _isInCancelEdit = true;

                // supposing that dynamic properties are eventually stored within a physical property, dynamic properties are skipped
                var physicalProperties = PropertyInfos.Where(p => p.IsPhysical).ToDictionary(p => p.Name);
                foreach (var property in _editableEntityChangeTracker.OriginalValues.Where(p => physicalProperties.ContainsKey(p.Key)).Reverse())
                {
                    SetProperty(property.Key, property.Value, true);
                }
                foreach (var relation in _editableEntityChangeTracker.ObjectsAddedToCollectionProperties.ToArray().Reverse())
                {
                    var navigationProperty = GetType().GetProperty(relation.Key).GetValue(this, null) as ITrackableCollection;
                    foreach (var entity in relation.Value.ToArray().Reverse())
                    {
                        navigationProperty.Remove(entity);
                    }
                }
                foreach (var relation in _editableEntityChangeTracker.ObjectsRemovedFromCollectionProperties.ToArray().Reverse())
                {
                    var navigationProperty = GetType().GetProperty(relation.Key).GetValue(this, null) as ITrackableCollection;
                    foreach (var entity in relation.Value.ToArray().Reverse())
                    {
                        navigationProperty.Add(entity);
                    }
                }
            }
            finally
            {
                _isInCancelEdit = false;
                _isValidationEnabled = isValidationEnabled;
            }

            // end edit
            _editableEntityChangeTracker = null;
        }

        private void RecordOriginalValueInEditableEntityChangeTracker(string propertyName, object oldValue)
        {
            // record in editable change tracker
            if (!_isInCancelEdit && _editableEntityChangeTracker != null)
            {
                var originalValues = _editableEntityChangeTracker.OriginalValues;
                if (!originalValues.ContainsKey(propertyName))
                {
                    originalValues[propertyName] = oldValue;
                }
            }
        }

        private void RecordAdditionToCollectionPropertiesInEditableEntityChangeTracker(string propertyName, Entity value)
        {
            // record in editable change tracker
            if (!_isInCancelEdit && _editableEntityChangeTracker != null)
            {
                // Adding the entity back after it has been deleted, we should do nothing here then
                var objectsRemovedFromCollectionProperties = _editableEntityChangeTracker.ObjectsRemovedFromCollectionProperties;
                if (objectsRemovedFromCollectionProperties.ContainsKey(propertyName) &&
                    objectsRemovedFromCollectionProperties[propertyName].Contains(value))
                {
                    objectsRemovedFromCollectionProperties[propertyName].Remove(value);
                    if (objectsRemovedFromCollectionProperties[propertyName].Count == 0)
                    {
                        objectsRemovedFromCollectionProperties.Remove(propertyName);
                    }
                }
                else
                {
                    var objectsAddedToCollectionProperties = _editableEntityChangeTracker.ObjectsAddedToCollectionProperties;
                    if (!objectsAddedToCollectionProperties.ContainsKey(propertyName))
                    {
                        objectsAddedToCollectionProperties[propertyName] = new EntityList();
                    }
                    objectsAddedToCollectionProperties[propertyName].Add(value);
                }
            }
        }

        private void RecordRemovalFromCollectionPropertiesInEditableEntityChangeTracker(string propertyName, Entity value)
        {
            // record in editable change tracker
            if (!_isInCancelEdit && _editableEntityChangeTracker != null)
            {
                // Deleteing the entity after it has been added, we should do nothing here then
                var objectsAddedToCollectionProperties = _editableEntityChangeTracker.ObjectsAddedToCollectionProperties;
                if (objectsAddedToCollectionProperties.ContainsKey(propertyName) &&
                    objectsAddedToCollectionProperties[propertyName].Contains(value))
                {
                    objectsAddedToCollectionProperties[propertyName].Remove(value);
                    if (objectsAddedToCollectionProperties[propertyName].Count == 0)
                    {
                        objectsAddedToCollectionProperties.Remove(propertyName);
                    }
                }
                else
                {
                    var objectsRemovedFromCollectionProperties = _editableEntityChangeTracker.ObjectsRemovedFromCollectionProperties;
                    if (!objectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                    {
                        objectsRemovedFromCollectionProperties[propertyName] = new EntityList();
                    }
                    objectsRemovedFromCollectionProperties[propertyName].Add(value);
                }
            }
        }

        /// <summary>
        /// Internal private class for change tracking
        /// </summary>
        private sealed class EditableEntityChangeTracker
        {
            /// <summary>
            /// Tracks initial values of properties
            /// </summary>
            public OriginalValuesDictionary OriginalValues
            {
                get
                {
                    return _originalValuesDictionary ?? (_originalValuesDictionary = new OriginalValuesDictionary());
                }
            }
            private OriginalValuesDictionary _originalValuesDictionary = null;

            /// <summary>
            /// Tracks added references of reference collections
            /// </summary>
            public EntitiesAddedToCollectionProperties ObjectsAddedToCollectionProperties
            {
                get
                {
                    return _entitiesAddedToCollectionProperties ?? (_entitiesAddedToCollectionProperties = new EntitiesAddedToCollectionProperties());
                }
            }
            private EntitiesAddedToCollectionProperties _entitiesAddedToCollectionProperties = null;

            /// <summary>
            /// Tracks removed references of reference collections
            /// </summary>
            public EntitiesRemovedFromCollectionProperties ObjectsRemovedFromCollectionProperties
            {
                get
                {
                    return _entitiesRemovedFromCollectionProperties ?? (_entitiesRemovedFromCollectionProperties = new EntitiesRemovedFromCollectionProperties());
                }
            }
            private EntitiesRemovedFromCollectionProperties _entitiesRemovedFromCollectionProperties = null;
        }

        #endregion IEditable

        #region Validation

        #region IsValidationEnabled

        [DataMember]
        [Display(AutoGenerateField = false)]
        public virtual bool IsValidationEnabled
        {
            get { return _isValidationEnabled ?? true; }
            set
            {
                if ((_isValidationEnabled ?? true) != value)
                {
                    _isValidationEnabled = value;
                    OnPropertyChanged("IsValidationEnabled", trackInChangeTracker: false);
                }
            }
        }
        [NonSerialized]
        private bool? _isValidationEnabled = null;

        #endregion IsValidationEnabled

        public virtual void ValidateProperty(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (!PropertyInfos.Any(p => p.Name == propertyName))
            {
                //throw new ArgumentException(string.Format("Property {1} does not exist on entity type {0}.", GetType().Name, propertyName));
                // just return, as virtual properties may be used via indexer
                return;
            }

            // lookup validation attributes
            var validationAttributes = PropertyInfos
                .Where(p => p.Name == propertyName && p.Attributes.Any(attribute => attribute is ValidationAttribute))
                .SelectMany(p => p.Attributes.Where(attribute => attribute is ValidationAttribute).Cast<ValidationAttribute>());

            if (validationAttributes.Any())
            {
                var validationContext = new ValidationContext(this, null, null) { MemberName = propertyName };

                // execute validation
                Validator.ValidateValue(value, validationContext, validationAttributes);
            }
        }

        private bool IsPropertyReadOnly(string propertyName)
        {
            return PropertyInfos.Any(p => p.Name == propertyName && p.Attributes.Any(a => a is EditableAttribute && !((EditableAttribute)a).AllowEdit));
        }

        #region IDataErrorInfo
        [Display(AutoGenerateField = false)]
        string IDataErrorInfo.Error
        {
            get
            {
                string errorMessage = null;

                var errors = PropertyInfos
                    .Where(p => p.Attributes.Any(a => a is SimplePropertyAttribute || a is ComplexPropertyAttribute || a is NavigationPropertyAttribute))
                    .Select(p => ((IDataErrorInfo)this)[p.Name])
                    .Where(err => !string.IsNullOrEmpty(err))
                    .ToArray();

                if (errors.Length > 0)
                {
                    errorMessage = string.Join("\n", errors);
                }

                return errorMessage ?? string.Empty;
            }
        }

        [Display(AutoGenerateField = false)]
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new ArgumentNullException("propertyName");
                }

                if (!PropertyInfos.Any(p => p.Name == propertyName))
                {
                    //throw new ArgumentException(string.Format("Property {1} does not exist on entity type {0}.", GetType().Name, propertyName));
                    return string.Empty;
                }

                // includes server side validation results
                var validationResult = Errors.Where(e => e.MemberNames.Contains(propertyName)).Select(e => (ValidationResult)e).ToList();

                // lookup validation attributes
                var validationAttributes = PropertyInfos
                    .Where(p => p.Name == propertyName && p.Attributes.Any(attribute => attribute is ValidationAttribute))
                    .SelectMany(p => p.Attributes.Where(attribute => attribute is ValidationAttribute).Cast<ValidationAttribute>());

                if (validationAttributes.Any())
                {
                    var value = GetProperty(propertyName, true);

                    var validationContext = new ValidationContext(this, null, null) { MemberName = propertyName };

                    // execute validation
                    Validator.TryValidateValue(value, validationContext, validationResult, validationAttributes);
                }

                // build validation summary
                if (validationResult.Count > 0)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var error in validationResult)
                    {
                        if (sb.Length > 0)
                        {
                            sb.AppendLine();
                        }

                        sb.Append(error.ErrorMessage);
                    }

                    return sb.ToString();
                }

                return string.Empty;
            }
        }
        #endregion

        [DataMember]
#if !SILVERLIGHT
        [System.ServiceModel.DomainServices.Server.Exclude] // exclude in WCF RIA Service
#endif
        [Display(AutoGenerateField = false)]
        public readonly List<Error> Errors = new List<Error>();

        [Display(AutoGenerateField = false)]
        public virtual bool IsValid
        {
            get
            {
                return !Errors.Any(e => e.IsError)
                    && string.IsNullOrEmpty(((IDataErrorInfo)this).Error)
                    && PropertyInfos.All(p => string.IsNullOrEmpty(((IDataErrorInfo)this)[p.Name]));
            }
        }

        #endregion Validation

        #region override Equals and GetHashCode

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is Entity && Equals((Entity)obj);
        }

        public virtual bool Equals(Entity entity)
        {
            if (ReferenceEquals(null, entity))
            {
                return false;
            }

            if (ReferenceEquals(this, entity))
            {
                return true;
            }

            if (GetType() != entity.GetType())
            {
                return false;
            }

            return ChangeTracker.State != ObjectState.Added
                && entity.ChangeTracker.State != ObjectState.Added
                && IsKeyEqual(entity);
        }

        protected abstract bool IsKeyEqual(Entity entity);

        public override int GetHashCode()
        {
            return ChangeTracker.State == ObjectState.Added ? base.GetHashCode() : GetKeyHashCode();
        }

        protected abstract int GetKeyHashCode();

        //public static bool operator ==(Entity entity1, Entity entity2)
        //{
        //    return ((object)entity1 == null && (object)entity2 == null) || ((object)entity1 != null && entity1.Equals(entity2));
        //}

        //public static bool operator !=(Entity entity1, Entity entity2)
        //{
        //    return !(entity1 == entity2);
        //}

        #endregion overridden object methods and operators

        #region Refresh

        public virtual void Refresh(Entity entity, bool trackChanges = true, bool preserveExistingChanges = false, ServerGenerationTypes type = ServerGenerationTypes.None)
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
                                if (a is ForeignKeyPropertyAttribute)
                                {
                                    return true;
                                }

                                var serverGenerationAttribute = a as ServerGenerationAttribute;
                                if (serverGenerationAttribute != null &&
                                    (
                                        (serverGenerationAttribute.Type & ServerGenerationTypes.Insert) == (type & ServerGenerationTypes.Insert) ||
                                        (serverGenerationAttribute.Type & ServerGenerationTypes.Update) == (type & ServerGenerationTypes.Update)
                                    ))
                                {
                                    return true;
                                }

                                return false;
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

        #region INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName, object oldValue = null, object newValue = null, bool trackInChangeTracker = true, bool isNavigationProperty = false)
        {
            if (_isDeserializing) return;

            if (trackInChangeTracker && ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.State == ObjectState.Unchanged)
                {
                    ChangeTracker.State = ObjectState.Modified;
                }
                if (!isNavigationProperty)
                {
                    ChangeTracker.OnPropertyChanged(propertyName);
                }
                ChangeTracker.RecordOriginalValue(propertyName, oldValue, newValue);
            }

            RecordOriginalValueInEditableEntityChangeTracker(propertyName, oldValue);

            RaisePropertyChanged(propertyName);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));

                if (propertyName != "HasChanges")
                {
                    propertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
                }
            }
        }

        protected void OnComplexPropertyMemberChanged(string complextPropertyName, string complextPropertyMemberName, object oldValue, object newValue)
        {
            if (ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.State == ObjectState.Unchanged)
                {
                    ChangeTracker.State = ObjectState.Modified;
                }

                var propertyName = string.Format("{0}.{1}", complextPropertyName, complextPropertyMemberName);
                ChangeTracker.OnPropertyChanged(complextPropertyName);
                ChangeTracker.RecordOriginalValue(propertyName, oldValue, newValue);
                RecordOriginalValueInEditableEntityChangeTracker(propertyName, oldValue);
            }
        }

        #endregion INotifyPropertyChanged

        #region INotifyPropertyChanging

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(string propertyName, object newValue)
        {
            if (_isDeserializing) return;

#if !SILVERLIGHT
            // note: due to a limitaion of EF ChangeTracker.IsChangeTrackingEnabled has to be true to perform validation
            // the limitation is that it is not possible to know when materializaion starts and _isDeserializing can not be set to true for object materialization
            if (ChangeTracker.IsChangeTrackingEnabled)
#endif
            {
                if (IsPropertyReadOnly(propertyName))
                {
                    throw new InvalidOperationException(string.Format("Property {1} on entity type {0} is read-only.", GetType().Name, propertyName));
                }
                if (IsValidationEnabled)
                {
                    ValidateProperty(propertyName, newValue);
                }
            }

            var propertyChanging = PropertyChanging;
            if (propertyChanging != null)
            {
                propertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected void OnComplexPropertyMemberChanging(string complextPropertyName, string complextPropertyMemberName, object oldValue, object newValue)
        {
#if !SILVERLIGHT
            // note: due to a limitaion of ef4 ChangeTracker.IsChangeTrackingEnabled has to be true to perform validation
            // the limitation is that it is not possible to know when materializaion starts and _isDeserializing can not be set to true for object materialization
            if (ChangeTracker.IsChangeTrackingEnabled)
#endif
            {
                var propertyName = string.Format("{0}.{1}", complextPropertyName, complextPropertyMemberName);
                if (IsPropertyReadOnly(propertyName))
                {
                    throw new InvalidOperationException(string.Format("Property {1} on entity type {0} is read-only.", GetType().Name, propertyName));
                }
                if (IsValidationEnabled)
                {
                    ValidateProperty(propertyName, newValue);
                }
            }
        }

        #endregion INotifyPropertyChanging
    }
}
