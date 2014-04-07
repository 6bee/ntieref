// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NTier.Common.Domain.Model
{
    // Helper class that captures most of the change tracking work that needs to be done
    // for self tracking entities.
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class ObjectChangeTracker : INotifyPropertyChanged
    {
        #region  Fields

        [NonSerialized]
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

        [field: NonSerialized]
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
        [NonSerialized]
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

        internal ObjectChangeTracker ShallowCopy()
        {
            return (ObjectChangeTracker)MemberwiseClone();
        }
    }
}
