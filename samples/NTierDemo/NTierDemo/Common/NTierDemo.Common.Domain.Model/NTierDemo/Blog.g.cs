﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator NTierDemoModel_NTierEntityGenerator.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace NTierDemo.Common.Domain.Model.NTierDemo
{
    [Serializable]
    [DataContract(IsReference = true)]
    [KnownType(typeof(User))]
    [KnownType(typeof(Post))]
    public partial class Blog : Entity, INotifyPropertyChanged, INotifyPropertyChanging, IDataErrorInfo
    {
        #region Constructor and Initialization

        // partial method for initialization
        partial void Initialize();

        public Blog()
        {
            Initialize();
        }

        #endregion Constructor and Initialization

        #region Simple Properties

        [DataMember]
        [Key]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [ServerGeneration(ServerGenerationTypes.Insert)]
        [SimpleProperty]
        public global::System.Int64 Id
        {
            get { return _id; }
            internal set
            {
                if (_id != value)
                {
                    if (!IsDeserializing && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    IdChanging(value);
                    OnPropertyChanging("Id", value);
                    var previousValue = _id;
                    _id = value;
                    OnPropertyChanged("Id", previousValue, value);
                    IdChanged(previousValue);
                }
            }
        }
        private global::System.Int64 _id;

        partial void IdChanging(global::System.Int64 newValue);
        partial void IdChanged(global::System.Int64 previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [ForeignKeyProperty]
        [SimpleProperty]
        public global::System.Int64 OwnerId
        {
            get { return _ownerId; }
            set
            {
                if (_ownerId != value)
                {
                    OwnerIdChanging(value);
                    OnPropertyChanging("OwnerId", value);
                    if (!IsDeserializing)
                    {
                        if (Owner != null && Owner.Id != value)
                        {
                            Owner = null;
                        }
                    }
                    var previousValue = _ownerId;
                    _ownerId = value;
                    OnPropertyChanged("OwnerId", previousValue, value);
                    OwnerIdChanged(previousValue);
                }
            }
        }
        private global::System.Int64 _ownerId;

        partial void OwnerIdChanging(global::System.Int64 newValue);
        partial void OwnerIdChanged(global::System.Int64 previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    TitleChanging(value);
                    OnPropertyChanging("Title", value);
                    var previousValue = _title;
                    _title = value;
                    OnPropertyChanged("Title", previousValue, value);
                    TitleChanged(previousValue);
                }
            }
        }
        private global::System.String _title;

        partial void TitleChanging(global::System.String newValue);
        partial void TitleChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    DescriptionChanging(value);
                    OnPropertyChanging("Description", value);
                    var previousValue = _description;
                    _description = value;
                    OnPropertyChanged("Description", previousValue, value);
                    DescriptionChanged(previousValue);
                }
            }
        }
        private global::System.String _description;

        partial void DescriptionChanging(global::System.String newValue);
        partial void DescriptionChanged(global::System.String previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.DateTime CreatedDate
        {
            get { return _createdDate; }
            internal set
            {
                if (_createdDate != value)
                {
                    CreatedDateChanging(value);
                    OnPropertyChanging("CreatedDate", value);
                    var previousValue = _createdDate;
                    _createdDate = value;
                    OnPropertyChanged("CreatedDate", previousValue, value);
                    CreatedDateChanged(previousValue);
                }
            }
        }
        private global::System.DateTime _createdDate;

        partial void CreatedDateChanging(global::System.DateTime newValue);
        partial void CreatedDateChanged(global::System.DateTime previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.DateTime ModifiedDate
        {
            get { return _modifiedDate; }
            internal set
            {
                if (_modifiedDate != value)
                {
                    ModifiedDateChanging(value);
                    OnPropertyChanging("ModifiedDate", value);
                    var previousValue = _modifiedDate;
                    _modifiedDate = value;
                    OnPropertyChanged("ModifiedDate", previousValue, value);
                    ModifiedDateChanged(previousValue);
                }
            }
        }
        private global::System.DateTime _modifiedDate;

        partial void ModifiedDateChanging(global::System.DateTime newValue);
        partial void ModifiedDateChanged(global::System.DateTime previousValue);

        #endregion Simple Properties

        #region Complex Properties

        #endregion Complex Properties

        #region Navigation Properties

        [DataMember]
        [NavigationProperty]
        public User Owner
        {
            get { return _owner; }
            set
            {
                if (!ReferenceEquals(_owner, value))
                {
                    OwnerChanging(value);
                    OnPropertyChanging("Owner", value);
                    var previousValue = _owner;
                    _owner = value;
                    FixupOwner(previousValue);
                    OnPropertyChanged("Owner", previousValue, value, isNavigationProperty: true);
                    OwnerChanged(previousValue);
                }
            }
        }
        private User _owner;

        partial void OwnerChanging(User newValue);
        partial void OwnerChanged(User previousValue);

        [DataMember]
        [NavigationProperty]
        public TrackableCollection<Post> Posts
        {
            get
            {
                if (_posts == null)
                {
                    _posts = new TrackableCollection<Post>();
                    _posts.CollectionChanged += FixupPosts;
                }
                return _posts;
            }
            set
            {
                if (!ReferenceEquals(_posts, value))
                {
                    if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }

                    if (_posts != null)
                    {
                       _posts.CollectionChanged -= FixupPosts;
                    }

                    _posts = value;

                    if (_posts != null)
                    {
                        _posts.CollectionChanged += FixupPosts;
                    }

                    OnPropertyChanged("Posts", trackInChangeTracker: false);
                }
            }
        }
        private TrackableCollection<Post> _posts;

        #endregion Navigation Properties

        #region ChangeTracking

        protected override void ClearNavigationProperties()
        {
            Owner = null;
            Posts.Clear();
        }

        #endregion ChangeTracking

        #region Association Fixup

        private void FixupOwner(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (previousValue != null && previousValue.Blogs.Contains(this))
            {
                previousValue.Blogs.Remove(this);
            }

            if (Owner != null)
            {
                if (!Owner.Blogs.Contains(this))
                {
                    Owner.Blogs.Add(this);
                }

                OwnerId = Owner.Id;
            }
            if (ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Owner")
                    && ReferenceEquals(ChangeTracker.OriginalValues["Owner"], Owner))
                {
                    //ChangeTracker.OriginalValues.Remove("Owner");
                }
                else
                {
                    //RecordOriginalValue("Owner", previousValue);
                }
                if (Owner != null && !Owner.ChangeTracker.IsChangeTrackingEnabled)
                {
                    Owner.StartTracking();
                }
            }
        }

        private void FixupPosts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (Post item in e.NewItems)
                {
                    item.BlogId = Id;
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.IsChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        RecordAdditionToCollectionProperties("Posts", item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Post item in e.OldItems)
                {
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        RecordRemovalFromCollectionProperties("Posts", item);
                    }
                }
            }
        }

        #endregion Association Fixup

        protected override bool IsKeyEqual(Entity other)
        {
            var entity = other as Blog;
            if (ReferenceEquals(null, entity)) return false;
            return this.Id == entity.Id;
        }

        protected override int GetKeyHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
