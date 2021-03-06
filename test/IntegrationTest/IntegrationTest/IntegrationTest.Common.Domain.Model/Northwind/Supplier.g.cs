﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator Northwind.tt.
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

namespace IntegrationTest.Common.Domain.Model.Northwind
{
    [Serializable]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Product))]
    public partial class Supplier : Entity, INotifyPropertyChanged, INotifyPropertyChanging, IDataErrorInfo
    {
        #region Constructor and Initialization

        // partial method for initialization
        partial void Initialize();

        public Supplier()
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
        public global::System.Int32 SupplierID
        {
            get { return _supplierID; }
            set
            {
                if (_supplierID != value)
                {
                    if (!IsDeserializing && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'SupplierID' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    SupplierIDChanging(value);
                    OnPropertyChanging("SupplierID", value);
                    var previousValue = _supplierID;
                    _supplierID = value;
                    OnPropertyChanged("SupplierID", previousValue, value);
                    SupplierIDChanged(previousValue);
                }
            }
        }
        private global::System.Int32 _supplierID;

        partial void SupplierIDChanging(global::System.Int32 newValue);
        partial void SupplierIDChanged(global::System.Int32 previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String CompanyName
        {
            get { return _companyName; }
            set
            {
                if (_companyName != value)
                {
                    CompanyNameChanging(value);
                    OnPropertyChanging("CompanyName", value);
                    var previousValue = _companyName;
                    _companyName = value;
                    OnPropertyChanged("CompanyName", previousValue, value);
                    CompanyNameChanged(previousValue);
                }
            }
        }
        private global::System.String _companyName;

        partial void CompanyNameChanging(global::System.String newValue);
        partial void CompanyNameChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String ContactName
        {
            get { return _contactName; }
            set
            {
                if (_contactName != value)
                {
                    ContactNameChanging(value);
                    OnPropertyChanging("ContactName", value);
                    var previousValue = _contactName;
                    _contactName = value;
                    OnPropertyChanged("ContactName", previousValue, value);
                    ContactNameChanged(previousValue);
                }
            }
        }
        private global::System.String _contactName;

        partial void ContactNameChanging(global::System.String newValue);
        partial void ContactNameChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String ContactTitle
        {
            get { return _contactTitle; }
            set
            {
                if (_contactTitle != value)
                {
                    ContactTitleChanging(value);
                    OnPropertyChanging("ContactTitle", value);
                    var previousValue = _contactTitle;
                    _contactTitle = value;
                    OnPropertyChanged("ContactTitle", previousValue, value);
                    ContactTitleChanged(previousValue);
                }
            }
        }
        private global::System.String _contactTitle;

        partial void ContactTitleChanging(global::System.String newValue);
        partial void ContactTitleChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    AddressChanging(value);
                    OnPropertyChanging("Address", value);
                    var previousValue = _address;
                    _address = value;
                    OnPropertyChanged("Address", previousValue, value);
                    AddressChanged(previousValue);
                }
            }
        }
        private global::System.String _address;

        partial void AddressChanging(global::System.String newValue);
        partial void AddressChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String City
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    CityChanging(value);
                    OnPropertyChanging("City", value);
                    var previousValue = _city;
                    _city = value;
                    OnPropertyChanged("City", previousValue, value);
                    CityChanged(previousValue);
                }
            }
        }
        private global::System.String _city;

        partial void CityChanging(global::System.String newValue);
        partial void CityChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Region
        {
            get { return _region; }
            set
            {
                if (_region != value)
                {
                    RegionChanging(value);
                    OnPropertyChanging("Region", value);
                    var previousValue = _region;
                    _region = value;
                    OnPropertyChanged("Region", previousValue, value);
                    RegionChanged(previousValue);
                }
            }
        }
        private global::System.String _region;

        partial void RegionChanging(global::System.String newValue);
        partial void RegionChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String PostalCode
        {
            get { return _postalCode; }
            set
            {
                if (_postalCode != value)
                {
                    PostalCodeChanging(value);
                    OnPropertyChanging("PostalCode", value);
                    var previousValue = _postalCode;
                    _postalCode = value;
                    OnPropertyChanged("PostalCode", previousValue, value);
                    PostalCodeChanged(previousValue);
                }
            }
        }
        private global::System.String _postalCode;

        partial void PostalCodeChanging(global::System.String newValue);
        partial void PostalCodeChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    CountryChanging(value);
                    OnPropertyChanging("Country", value);
                    var previousValue = _country;
                    _country = value;
                    OnPropertyChanged("Country", previousValue, value);
                    CountryChanged(previousValue);
                }
            }
        }
        private global::System.String _country;

        partial void CountryChanging(global::System.String newValue);
        partial void CountryChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    PhoneChanging(value);
                    OnPropertyChanging("Phone", value);
                    var previousValue = _phone;
                    _phone = value;
                    OnPropertyChanged("Phone", previousValue, value);
                    PhoneChanged(previousValue);
                }
            }
        }
        private global::System.String _phone;

        partial void PhoneChanging(global::System.String newValue);
        partial void PhoneChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Fax
        {
            get { return _fax; }
            set
            {
                if (_fax != value)
                {
                    FaxChanging(value);
                    OnPropertyChanging("Fax", value);
                    var previousValue = _fax;
                    _fax = value;
                    OnPropertyChanged("Fax", previousValue, value);
                    FaxChanged(previousValue);
                }
            }
        }
        private global::System.String _fax;

        partial void FaxChanging(global::System.String newValue);
        partial void FaxChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String HomePage
        {
            get { return _homePage; }
            set
            {
                if (_homePage != value)
                {
                    HomePageChanging(value);
                    OnPropertyChanging("HomePage", value);
                    var previousValue = _homePage;
                    _homePage = value;
                    OnPropertyChanged("HomePage", previousValue, value);
                    HomePageChanged(previousValue);
                }
            }
        }
        private global::System.String _homePage;

        partial void HomePageChanging(global::System.String newValue);
        partial void HomePageChanged(global::System.String previousValue);

        #endregion Simple Properties

        #region Complex Properties

        #endregion Complex Properties

        #region Navigation Properties

        [DataMember]
        [NavigationProperty]
        public TrackableCollection<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new TrackableCollection<Product>();
                    _products.CollectionChanged += FixupProducts;
                }
                return _products;
            }
            set
            {
                if (!ReferenceEquals(_products, value))
                {
                    if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }

                    if (_products != null)
                    {
                       _products.CollectionChanged -= FixupProducts;
                    }

                    _products = value;

                    if (_products != null)
                    {
                        _products.CollectionChanged += FixupProducts;
                    }

                    OnPropertyChanged("Products", trackInChangeTracker: false);
                }
            }
        }
        private TrackableCollection<Product> _products;

        #endregion Navigation Properties

        #region ChangeTracking

        protected override void ClearNavigationProperties()
        {
            Products.Clear();
        }

        #endregion ChangeTracking

        #region Association Fixup

        private void FixupProducts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (Product item in e.NewItems)
                {
                    item.Supplier = this;
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.IsChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        RecordAdditionToCollectionProperties("Products", item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Product item in e.OldItems)
                {
                    if (ReferenceEquals(item.Supplier, this))
                    {
                        item.Supplier = null;
                    }
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        RecordRemovalFromCollectionProperties("Products", item);
                    }
                }
            }
        }

        #endregion Association Fixup

        protected override bool IsKeyEqual(Entity other)
        {
            var entity = other as Supplier;
            if (ReferenceEquals(null, entity)) return false;
            return this.SupplierID == entity.SupplierID;
        }

        protected override int GetKeyHashCode()
        {
            return this.SupplierID.GetHashCode();
        }
    }
}
