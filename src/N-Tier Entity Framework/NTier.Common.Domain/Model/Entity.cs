// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    // generic abstract base class Entity
    [DataContract(IsReference = true)]
    public abstract class Entity<T> : Entity where T : Entity<T>
    {
        #region Generic entity factory method
        
#if !SILVERLIGHT
        private delegate T ObjectActivator();

        private static readonly Lazy<ObjectActivator> _objectActivator = new Lazy<ObjectActivator>(() =>
        {
            // get constructor by reflection
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);

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

    // abstract base class Entity
    [DataContract(IsReference = true)]
    public abstract class Entity : IObjectWithChangeTracker, IEditable, INotifyPropertyChanging, INotifyPropertyChanged, IDataErrorInfo
    {
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
#if !SILVERLIGHT
        [NonSerialized]
#endif
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
                    //_changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                    _changeTracker.PropertyChanged += changeTracker_PropertyChanged;
                }
                return _changeTracker;
            }
            set
            {
                if (_changeTracker != null)
                {
                    //_changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                    _changeTracker.PropertyChanged -= changeTracker_PropertyChanged;
                }

                _changeTracker = value;

                if (_changeTracker != null)
                {
                    //_changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                    _changeTracker.PropertyChanged += changeTracker_PropertyChanged;
                }

                OnPropertyChanged("ChangeTracker", trackInChangeTracker: false);
            }
        }
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private ObjectChangeTracker _changeTracker;

        //private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        //{
        //    if (e.NewState == ObjectState.Deleted)
        //    {
        //        ClearNavigationProperties();
        //    }
        //}

        private void changeTracker_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        internal protected abstract IEnumerable<PropertyInfos> PropertyInfos { get; }

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
#if !SILVERLIGHT
        [NonSerialized]
#endif
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
                    .Where(err => err != null)
                    .ToArray();

                if (errors.Length > 0)
                {
                    errorMessage = string.Join("\n", errors);
                }

                return errorMessage;
            }
        }

        [Display(AutoGenerateField = false)]
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new ArgumentNullException("columnName");
                }

                if (!PropertyInfos.Any(p => p.Name == propertyName))
                {
                    //throw new ArgumentException(string.Format("Property {1} does not exist on entity type {0}.", GetType().Name, propertyName));
                    return null;
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

                return null;
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
                return !Errors.Any(e => e.IsError) && 
                    ((IDataErrorInfo)this).Error == null && 
                    PropertyInfos.All(p => ((IDataErrorInfo)this)[p.Name] == null);
            }
        }

        #endregion Validation

        #region override Equals and GetHashCode

        public override bool Equals(object obj)
        {
            return obj is Entity && Equals((Entity)obj);
        }

        public bool Equals(Entity entity)
        {
            return !object.ReferenceEquals(entity, null) 
                && (object.ReferenceEquals(this, entity) || (ChangeTracker.State != ObjectState.Added && entity.ChangeTracker.State != ObjectState.Added && IsKeyEqual(entity)));
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

        public abstract void Refresh(Entity entity, bool trackChanges = true);

        #region INotifyPropertyChanged

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName, object oldValue = null, object newValue = null, bool trackInChangeTracker = true, bool isNavigationProperty = false)
        {
            if (_isDeserializing) return;

            if (trackInChangeTracker && ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
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

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                propertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
            }
        }

        protected void OnComplexPropertyMemberChanged(string complextPropertyName, string complextPropertyMemberName, object oldValue, object newValue)
        {
            if (ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
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

#if !SILVERLIGHT
        [field: NonSerialized] 
#endif
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(string propertyName, object newValue)
        {
            if (_isDeserializing) return;

#if !SILVERLIGHT
            // note: due to a limitaion of ef4 ChangeTracker.IsChangeTrackingEnabled has to be true to perform validation
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

    [DataContract(IsReference = true)]
    public sealed class Error
    {
        /// <summary>
        ///  The error type of this error entry
        /// </summary>
        [DataMember]
        public ErrorType Type { get; private set; }

        /// <summary>
        ///  The error message of this error entry
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// A list of properties to which this error entry is associated with
        /// </summary>
        [DataMember]
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        ///  Returns true if this entry is of type warning, false otherwise
        /// </summary>
        public bool IsWarning
        {
            get { return Type == ErrorType.Warning; }
        }

        /// <summary>
        /// Returns true if this entry is of type error, false otherwise
        /// </summary>
        public bool IsError
        {
            get { return Type == ErrorType.Error; }
        }

        /// <summary>
        /// Creates a new error entry
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <param name="memberNames">An orbitrary list of properties to which this error entry is associated with</param>
        public Error(string errorMessage, params string[] memberNames)
            : this(ErrorType.Error, errorMessage, memberNames)
        {
        }

        /// <summary>
        /// Creates a new error entry
        /// </summary>
        /// <param name="type">The type of this entry</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="memberNames">An orbitrary list of properties to which this error entry is associated with</param>
        public Error(ErrorType type, string errorMessage, params string[] memberNames)
        {
            this.Type = type;
            this.Message = errorMessage;
            this.MemberNames = memberNames.ToList().AsReadOnly();
        }

        /// <summary>
        /// Casts an error entry into a validation result
        /// </summary>
        public static explicit operator ValidationResult(Error error)
        {
            return new ValidationResult(error.Message, error.MemberNames);
        }

        /// <summary>
        /// Casts a validation result into en error entry
        /// </summary>
        public static explicit operator Error(ValidationResult validationResult)
        {
            return new Error(validationResult.ErrorMessage, validationResult.MemberNames.ToArray());
        }
    }

    /// <summary>
    /// Type of error
    /// </summary>
    public enum ErrorType
    {
        Error = 0x0,
        Warning = 0x1
    }

    [Flags]
    public enum ServerGenerationTypes
    {
        None = 0x0,
        Insert = 0x1,
        Update = 0x2
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ServerGenerationAttribute : Attribute
    {
        public readonly ServerGenerationTypes Type;

        public ServerGenerationAttribute(ServerGenerationTypes type)
        {
            this.Type = type;
        }

        public override string ToString()
        {
            var serverGenerationTypesEnumName = Type.GetType().FullName;
            var serverGenerationTypes = Type.ToString().Split(',').Select(s => string.Format("global::{0}.{1}", serverGenerationTypesEnumName, s.Trim()));

            return string.Format("{0}({1})", base.ToString(), string.Join(" | ", serverGenerationTypes));
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ConcurrencyPropertyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IncludeOnUpdateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IncludeOnDeleteAttribute : Attribute
    {
    }

    // Helper class that captures most of the change tracking work that needs to be done
    // for self tracking entities.
    [DataContract(IsReference = true)]
    public sealed class ObjectChangeTracker : INotifyPropertyChanged
    {
        #region  Fields

#if !SILVERLIGHT
        [NonSerialized]
#endif
        private bool _isDeserializing;
        private ObjectState _objectState = ObjectState.Added;

        private OriginalValuesDictionary _originalValues;
        private ExtendedPropertiesDictionary _extendedProperties;
        private EntitiesAddedToCollectionProperties _objectsAddedToCollections;
        private EntitiesRemovedFromCollectionProperties _objectsRemovedFromCollections;
        private List<string> _modifiedProperties;

        #endregion

        #region Events

        //[field: NonSerialized]
        //public event EventHandler<ObjectStateChangingEventArgs> ObjectStateChanging;

        #region INotifyPropertyChanged

#if !SILVERLIGHT
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (_isDeserializing) return;

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

        #endregion

        //private void OnObjectStateChanging(ObjectState newState)
        //{
        //    var objectStateChanging = ObjectStateChanging;
        //    if (objectStateChanging != null)
        //    {
        //        objectStateChanging(this, new ObjectStateChangingEventArgs(newState));
        //    }
        //}

        internal void OnPropertyChanged(string propertyName)
        {
#if !SILVERLIGHT
            if (IsChangeTrackingEnabled)
#else
            if (IsChangeTrackingEnabled && _objectState != ObjectState.Added)
#endif
            {
                if (!ModifiedProperties.Contains(propertyName))
                {
                    ModifiedProperties.Add(propertyName);
                }
            }
        }

        [DataMember]
        public ObjectState State
        {
            get { return _objectState; }
            set
            {
                if (_isDeserializing || (_isChangeTrackingEnabled ?? true))
                {
                    //OnObjectStateChanging(value);
                    _objectState = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        [DataMember]
        public bool IsChangeTrackingEnabled
        {
            get
            {
#if !SILVERLIGHT
				return (_isChangeTrackingEnabled.HasValue && _isChangeTrackingEnabled.Value) || (!_isChangeTrackingEnabled.HasValue && _objectState != ObjectState.Added); 
#else
                return _isChangeTrackingEnabled ?? true;
#endif
            }
            set
            {
#if !SILVERLIGHT
                if (!_isChangeTrackingEnabled.HasValue || _isChangeTrackingEnabled.Value != value)
#else
                if ((_isChangeTrackingEnabled ?? true) != value)
#endif
                {
                    _isChangeTrackingEnabled = value;
                    RaisePropertyChanged("IsChangeTrackingEnabled");
                }
            }
        }
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private bool? _isChangeTrackingEnabled;

        [DataMember]
        public List<string> ModifiedProperties
        {
            get
            {
                if (_modifiedProperties == null)
                {
                    _modifiedProperties = new List<string>();
                }
                return _modifiedProperties;
            }
        }

        // Returns the removed objects to collection valued properties that were changed.
        [DataMember]
        public EntitiesRemovedFromCollectionProperties ObjectsRemovedFromCollectionProperties
        {
            get
            {
                if (_objectsRemovedFromCollections == null)
                {
                    _objectsRemovedFromCollections = new EntitiesRemovedFromCollectionProperties();
                }
                return _objectsRemovedFromCollections;
            }
        }

        // Returns the original values for properties that were changed.
        [DataMember]
        public OriginalValuesDictionary OriginalValues
        {
            get
            {
                if (_originalValues == null)
                {
                    _originalValues = new OriginalValuesDictionary();
                }
                return _originalValues;
            }
        }

        // Returns the extended property values.
        // This includes key values for independent associations that are needed for the
        // concurrency model in the Entity Framework
        [DataMember]
        public ExtendedPropertiesDictionary ExtendedProperties
        {
            get
            {
                if (_extendedProperties == null)
                {
                    _extendedProperties = new ExtendedPropertiesDictionary();
                }
                return _extendedProperties;
            }
        }

        // Returns the added objects to collection valued properties that were changed.
        [DataMember]
        public EntitiesAddedToCollectionProperties ObjectsAddedToCollectionProperties
        {
            get
            {
                if (_objectsAddedToCollections == null)
                {
                    _objectsAddedToCollections = new EntitiesAddedToCollectionProperties();
                }
                return _objectsAddedToCollections;
            }
        }

        #region MethodsForChangeTrackingOnClient

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
        }

        // Resets the ObjectChangeTracker to the Unchanged state and
        // clears the original values as well as the record of changes
        // to collection properties
        public void AcceptChanges()
        {
            //OnObjectStateChanging(ObjectState.Unchanged);
            ModifiedProperties.Clear();
            OriginalValues.Clear();
            ObjectsAddedToCollectionProperties.Clear();
            ObjectsRemovedFromCollectionProperties.Clear();
            IsChangeTrackingEnabled = true;
            State = ObjectState.Unchanged;
        }

        // Captures the original value for a property that is changing.
        internal void RecordOriginalValue(string propertyName, object oldValue, object newValue)
        {
            if (IsChangeTrackingEnabled && _objectState != ObjectState.Added)
            {
                if (OriginalValues.ContainsKey(propertyName))
                {
                    if (object.Equals(OriginalValues[propertyName], newValue))
                    {
                        OriginalValues.Remove(propertyName);
                        if (OriginalValues.Count == 0 && _objectState == ObjectState.Modified)
                        {
                            State = ObjectState.Unchanged;
                            ModifiedProperties.Clear();
                        }
                        else
                        {
                            ModifiedProperties.Remove(propertyName);
                        }
                    }
                }
                else
                {
                    OriginalValues[propertyName] = oldValue;
                }
            }
        }

        // Records an addition to collection valued properties on SelfTracking Entities.
        internal void RecordAdditionToCollectionProperties(string propertyName, Entity value)
        {
            if (IsChangeTrackingEnabled && _objectState != ObjectState.Added)
            {
                // Adding the entity back after it has been deleted, we should do nothing here then
                if (ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName) &&
                    ObjectsRemovedFromCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsRemovedFromCollectionProperties[propertyName].Remove(value);
                    if (ObjectsRemovedFromCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsRemovedFromCollectionProperties.Remove(propertyName);
                    }
                }
                else
                {
                    if (!ObjectsAddedToCollectionProperties.ContainsKey(propertyName))
                    {
                        ObjectsAddedToCollectionProperties[propertyName] = new EntityList();
                    }
                    ObjectsAddedToCollectionProperties[propertyName].Add(value);
                }
            }
        }

        // Records a removal to collection valued properties on SelfTracking Entities.
        internal void RecordRemovalFromCollectionProperties(string propertyName, Entity value)
        {
            if (IsChangeTrackingEnabled && _objectState != ObjectState.Added)
            {
                // Deleteing the entity after it has been added, we should do nothing here then
                if (ObjectsAddedToCollectionProperties.ContainsKey(propertyName) &&
                    ObjectsAddedToCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsAddedToCollectionProperties[propertyName].Remove(value);
                    if (ObjectsAddedToCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsAddedToCollectionProperties.Remove(propertyName);
                    }
                }
                else
                {
                    if (!ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                    {
                        ObjectsRemovedFromCollectionProperties[propertyName] = new EntityList();
                    }
                    ObjectsRemovedFromCollectionProperties[propertyName].Add(value);
                }
            }
        }

        #endregion
    }

    [Flags]
    public enum ObjectState
    {
        Unchanged = 0x1,
        Added = 0x2,
        Modified = 0x4,
        Deleted = 0x8
    }

    [CollectionDataContract(Name = "EntitiesAddedToCollectionProperties",
        ItemName = "AddedEntitiesForProperty", KeyName = "CollectionPropertyName", ValueName = "AddedEntities")]
    public class EntitiesAddedToCollectionProperties : Dictionary<string, EntityList> { }

    [CollectionDataContract(Name = "EntitiesRemovedFromCollectionProperties",
        ItemName = "DeletedEntitiesForProperty", KeyName = "CollectionPropertyName", ValueName = "DeletedEntities")]
    public class EntitiesRemovedFromCollectionProperties : Dictionary<string, EntityList> { }

    [CollectionDataContract(Name = "OriginalValuesDictionary",
        ItemName = "OriginalValues", KeyName = "Name", ValueName = "OriginalValue")]
    public class OriginalValuesDictionary : Dictionary<string, object> { }

    [CollectionDataContract(Name = "ExtendedPropertiesDictionary",
        ItemName = "ExtendedProperties", KeyName = "Name", ValueName = "ExtendedProperty")]
    public class ExtendedPropertiesDictionary : Dictionary<string, object> { }

    [CollectionDataContract(ItemName = "Entity")]
    public class EntityList : List<Entity> { }

    // The interface is implemented by the self tracking entities that EF will generate.
    // We will have an Adapter that converts this interface to the interface that the EF expects.
    // The Adapter will live on the server side.
    public interface IObjectWithChangeTracker
    {
        // Has all the change tracking information for the subgraph of a given object.
        ObjectChangeTracker ChangeTracker { get; set; }
    }

    public sealed class ObjectStateChangingEventArgs : EventArgs
    {
        public readonly ObjectState NewState;

        public ObjectStateChangingEventArgs(ObjectState newState)
        {
            this.NewState = newState;
        }
    }

    public static class ObjectWithChangeTrackerExtensions
    {
        public static T MarkAsDeleted<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Deleted;
            return trackingItem;
        }

        public static T MarkAsAdded<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Added;
            return trackingItem;
        }

        public static T MarkAsModified<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Modified;
            return trackingItem;
        }

        public static T MarkAsUnchanged<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Unchanged;
            return trackingItem;
        }

        public static void StartTracking(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = true;
        }

        public static void StopTracking(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.IsChangeTrackingEnabled = false;
        }

        public static void AcceptChanges(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }

            trackingItem.ChangeTracker.AcceptChanges();
        }
    }

    /// <summary>
    /// A collection that raises collection changed notifications and prevents adding duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TrackableCollection<T> : ITrackableCollection<T>, ITrackableCollection, INotifyCollectionChanged
    {
        private readonly IList<T> _data = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        private int IndexOf(T item)
        {
            return _data.IndexOf(item);
        }

        //public void Insert(int index, T item)
        //{
        //    if (Contains(item))
        //    {
        //        return;
        //    }

        //    _data.Insert(index, item);

        //    var collectionChanged = CollectionChanged;
        //    if (collectionChanged != null)
        //    {
        //        collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        //    }
        //}

        //public void RemoveAt(int index)
        //{
        //    var item = this[index];
        //    _data.RemoveAt(index);

        //    var collectionChanged = CollectionChanged;
        //    if (collectionChanged != null)
        //    {
        //        collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        //    }
        //}

        //public T this[int index]
        //{
        //    get
        //    {
        //        return _data[index];
        //    }
        //    set
        //    {
        //        if (Contains(value))
        //        {
        //            return;
        //        }

        //        if (index >= 0 && index < Count)
        //        {
        //            var oldItem = _data[index];
        //            _data[index] = value;

        //            var collectionChanged = CollectionChanged;
        //            if (collectionChanged != null)
        //            {
        //                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
        //            }
        //        }
        //        else
        //        {
        //            _data[index] = value;

        //            var collectionChanged = CollectionChanged;
        //            if (collectionChanged != null)
        //            {
        //                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        //            }
        //        }
        //    }
        //}

        public void Replace(T oldItem, T newItem)
        {
            if (object.ReferenceEquals(oldItem, newItem))
            {
                return;
            }

            //if (!Contains(oldItem))
            //{
            //    throw new Exception("Old item is not contained.");
            //}
            
            //if (Contains(newItem))
            //{
            //    throw new Exception("New item is already contained.");
            //}

            var index = IndexOf(oldItem);
            _data.Add(newItem);
            var successfullyRemoved = _data.Remove(oldItem);
            if (!successfullyRemoved)
            {
                _data.Remove(newItem);
                throw new Exception("Old item is not contained.");
            }

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            }
        }

        public void Add(T item)
        {
            if (Contains(item))
            {
                return;
            }

            _data.Add(item);

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                var index = IndexOf(item);
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }

#if !SILVERLIGHT
            System.Collections.IList list = null;

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                list = _data.ToList();
            }
			
            _data.Clear();
			
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
            }
#else
            _data.Clear();

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
#endif
        }

        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _data.Count; }
        }

        public bool IsReadOnly
        {
            get { return _data.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            if (!Contains(item))
            {
                return false;
            }

            var index = IndexOf(item);
            _data.Remove(item);

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ITrackableCollection.Add(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.GetType() != typeof(T))
            {
                throw new ArgumentException(string.Format("Argument expected to be of type {0} and got type {1}.", typeof(T).FullName, obj.GetType().FullName));
            }

            Add((T)obj);
        }

        bool ITrackableCollection.Remove(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.GetType() != typeof(T))
            {
                throw new ArgumentException(string.Format("Argument expected to be of type {0} and got type {1}.", typeof(T).FullName, obj.GetType().FullName));
            }

            return Remove((T)obj);
        }

        bool ITrackableCollection.Contains(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.GetType() != typeof(T))
            {
                throw new ArgumentException(string.Format("Argument expected to be of type {0} and got type {1}.", typeof(T).FullName, obj.GetType().FullName));
            }

            return Contains((T)obj);
        }
    }

    public interface ITrackableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }

    internal interface ITrackableCollection : System.Collections.IEnumerable
    {
        void Add(object obj);
        bool Contains(object obj);
        int Count { get; }
        void Clear();
        bool Remove(object item);
    }

    //// An interface that provides an event that fires when complex properties change.
    //// Changes can be the replacement of a complex property with a new complex type instance or
    //// a change to a scalar property within a complex type instance.
    //public interface INotifyComplexPropertyChanging
    //{
    //    event EventHandler ComplexPropertyChanging;
    //}

    public static class EqualityComparer
    {
        // Helper method to determine if two byte arrays are the same value even if they are different object references
        public static bool BinaryEquals(object binaryValue1, object binaryValue2)
        {
            if (object.ReferenceEquals(binaryValue1, binaryValue2))
            {
                return true;
            }

            byte[] array1 = binaryValue1 as byte[];
            byte[] array2 = binaryValue2 as byte[];

            if (array1 != null && array2 != null)
            {
                if (array1.Length != array2.Length)
                {
                    return false;
                }

                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
