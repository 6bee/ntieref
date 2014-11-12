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
    [KnownType(typeof(Employee))]
    [KnownType(typeof(Order))]
    [KnownType(typeof(Territory))]
    public partial class Employee : Entity, INotifyPropertyChanged, INotifyPropertyChanging, IDataErrorInfo
    {
        #region Constructor and Initialization

        // partial method for initialization
        partial void Initialize();

        public Employee()
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
        public global::System.Int32 EmployeeID
        {
            get { return _employeeID; }
            set
            {
                if (_employeeID != value)
                {
                    if (!IsDeserializing && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'EmployeeID' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    EmployeeIDChanging(value);
                    OnPropertyChanging("EmployeeID", value);
                    var previousValue = _employeeID;
                    _employeeID = value;
                    OnPropertyChanged("EmployeeID", previousValue, value);
                    EmployeeIDChanged(previousValue);
                }
            }
        }
        private global::System.Int32 _employeeID;

        partial void EmployeeIDChanging(global::System.Int32 newValue);
        partial void EmployeeIDChanged(global::System.Int32 previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    LastNameChanging(value);
                    OnPropertyChanging("LastName", value);
                    var previousValue = _lastName;
                    _lastName = value;
                    OnPropertyChanged("LastName", previousValue, value);
                    LastNameChanged(previousValue);
                }
            }
        }
        private global::System.String _lastName;

        partial void LastNameChanging(global::System.String newValue);
        partial void LastNameChanged(global::System.String previousValue);

        [DataMember]
        [Required]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    FirstNameChanging(value);
                    OnPropertyChanging("FirstName", value);
                    var previousValue = _firstName;
                    _firstName = value;
                    OnPropertyChanged("FirstName", previousValue, value);
                    FirstNameChanged(previousValue);
                }
            }
        }
        private global::System.String _firstName;

        partial void FirstNameChanging(global::System.String newValue);
        partial void FirstNameChanged(global::System.String previousValue);

        [DataMember]
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
        public global::System.String TitleOfCourtesy
        {
            get { return _titleOfCourtesy; }
            set
            {
                if (_titleOfCourtesy != value)
                {
                    TitleOfCourtesyChanging(value);
                    OnPropertyChanging("TitleOfCourtesy", value);
                    var previousValue = _titleOfCourtesy;
                    _titleOfCourtesy = value;
                    OnPropertyChanged("TitleOfCourtesy", previousValue, value);
                    TitleOfCourtesyChanged(previousValue);
                }
            }
        }
        private global::System.String _titleOfCourtesy;

        partial void TitleOfCourtesyChanging(global::System.String newValue);
        partial void TitleOfCourtesyChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public Nullable<global::System.DateTime> BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    BirthDateChanging(value);
                    OnPropertyChanging("BirthDate", value);
                    var previousValue = _birthDate;
                    _birthDate = value;
                    OnPropertyChanged("BirthDate", previousValue, value);
                    BirthDateChanged(previousValue);
                }
            }
        }
        private Nullable<global::System.DateTime> _birthDate;

        partial void BirthDateChanging(Nullable<global::System.DateTime> newValue);
        partial void BirthDateChanged(Nullable<global::System.DateTime> previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public Nullable<global::System.DateTime> HireDate
        {
            get { return _hireDate; }
            set
            {
                if (_hireDate != value)
                {
                    HireDateChanging(value);
                    OnPropertyChanging("HireDate", value);
                    var previousValue = _hireDate;
                    _hireDate = value;
                    OnPropertyChanged("HireDate", previousValue, value);
                    HireDateChanged(previousValue);
                }
            }
        }
        private Nullable<global::System.DateTime> _hireDate;

        partial void HireDateChanging(Nullable<global::System.DateTime> newValue);
        partial void HireDateChanged(Nullable<global::System.DateTime> previousValue);

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
        public global::System.String HomePhone
        {
            get { return _homePhone; }
            set
            {
                if (_homePhone != value)
                {
                    HomePhoneChanging(value);
                    OnPropertyChanging("HomePhone", value);
                    var previousValue = _homePhone;
                    _homePhone = value;
                    OnPropertyChanged("HomePhone", previousValue, value);
                    HomePhoneChanged(previousValue);
                }
            }
        }
        private global::System.String _homePhone;

        partial void HomePhoneChanging(global::System.String newValue);
        partial void HomePhoneChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    ExtensionChanging(value);
                    OnPropertyChanging("Extension", value);
                    var previousValue = _extension;
                    _extension = value;
                    OnPropertyChanged("Extension", previousValue, value);
                    ExtensionChanged(previousValue);
                }
            }
        }
        private global::System.String _extension;

        partial void ExtensionChanging(global::System.String newValue);
        partial void ExtensionChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.Byte[] Photo
        {
            get { return _photo; }
            set
            {
                if (_photo != value)
                {
                    PhotoChanging(value);
                    OnPropertyChanging("Photo", value);
                    var previousValue = _photo;
                    _photo = value;
                    OnPropertyChanged("Photo", previousValue, value);
                    PhotoChanged(previousValue);
                }
            }
        }
        private global::System.Byte[] _photo;

        partial void PhotoChanging(global::System.Byte[] newValue);
        partial void PhotoChanged(global::System.Byte[] previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    NotesChanging(value);
                    OnPropertyChanging("Notes", value);
                    var previousValue = _notes;
                    _notes = value;
                    OnPropertyChanged("Notes", previousValue, value);
                    NotesChanged(previousValue);
                }
            }
        }
        private global::System.String _notes;

        partial void NotesChanging(global::System.String newValue);
        partial void NotesChanged(global::System.String previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public Nullable<global::System.Int32> ReportsTo
        {
            get { return _reportsTo; }
            set
            {
                if (_reportsTo != value)
                {
                    ReportsToChanging(value);
                    OnPropertyChanging("ReportsTo", value);
                    if (!IsDeserializing)
                    {
                        if (Employee1 != null && Employee1.EmployeeID != value)
                        {
                            Employee1 = null;
                        }
                    }
                    var previousValue = _reportsTo;
                    _reportsTo = value;
                    OnPropertyChanged("ReportsTo", previousValue, value);
                    ReportsToChanged(previousValue);
                }
            }
        }
        private Nullable<global::System.Int32> _reportsTo;

        partial void ReportsToChanging(Nullable<global::System.Int32> newValue);
        partial void ReportsToChanged(Nullable<global::System.Int32> previousValue);

        [DataMember]
#if !CLIENT_PROFILE
        [RoundtripOriginal]
#endif
        [SimpleProperty]
        public global::System.String PhotoPath
        {
            get { return _photoPath; }
            set
            {
                if (_photoPath != value)
                {
                    PhotoPathChanging(value);
                    OnPropertyChanging("PhotoPath", value);
                    var previousValue = _photoPath;
                    _photoPath = value;
                    OnPropertyChanged("PhotoPath", previousValue, value);
                    PhotoPathChanged(previousValue);
                }
            }
        }
        private global::System.String _photoPath;

        partial void PhotoPathChanging(global::System.String newValue);
        partial void PhotoPathChanged(global::System.String previousValue);

        #endregion Simple Properties

        #region Complex Properties

        #endregion Complex Properties

        #region Navigation Properties

        [DataMember]
        [NavigationProperty]
        public TrackableCollection<Employee> Employees1
        {
            get
            {
                if (_employees1 == null)
                {
                    _employees1 = new TrackableCollection<Employee>();
                    _employees1.CollectionChanged += FixupEmployees1;
                }
                return _employees1;
            }
            set
            {
                if (!ReferenceEquals(_employees1, value))
                {
                    if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }

                    if (_employees1 != null)
                    {
                       _employees1.CollectionChanged -= FixupEmployees1;
                    }

                    _employees1 = value;

                    if (_employees1 != null)
                    {
                        _employees1.CollectionChanged += FixupEmployees1;
                    }

                    OnPropertyChanged("Employees1", trackInChangeTracker: false);
                }
            }
        }
        private TrackableCollection<Employee> _employees1;

        [DataMember]
        [NavigationProperty]
        public Employee Employee1
        {
            get { return _employee1; }
            set
            {
                if (!ReferenceEquals(_employee1, value))
                {
                    Employee1Changing(value);
                    OnPropertyChanging("Employee1", value);
                    var previousValue = _employee1;
                    _employee1 = value;
                    FixupEmployee1(previousValue);
                    OnPropertyChanged("Employee1", previousValue, value, isNavigationProperty: true);
                    Employee1Changed(previousValue);
                }
            }
        }
        private Employee _employee1;

        partial void Employee1Changing(Employee newValue);
        partial void Employee1Changed(Employee previousValue);

        [DataMember]
        [NavigationProperty]
        public TrackableCollection<Order> Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new TrackableCollection<Order>();
                    _orders.CollectionChanged += FixupOrders;
                }
                return _orders;
            }
            set
            {
                if (!ReferenceEquals(_orders, value))
                {
                    if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }

                    if (_orders != null)
                    {
                       _orders.CollectionChanged -= FixupOrders;
                    }

                    _orders = value;

                    if (_orders != null)
                    {
                        _orders.CollectionChanged += FixupOrders;
                    }

                    OnPropertyChanged("Orders", trackInChangeTracker: false);
                }
            }
        }
        private TrackableCollection<Order> _orders;

        [DataMember]
        [NavigationProperty]
        public TrackableCollection<Territory> Territories
        {
            get
            {
                if (_territories == null)
                {
                    _territories = new TrackableCollection<Territory>();
                    _territories.CollectionChanged += FixupTerritories;
                }
                return _territories;
            }
            set
            {
                if (!ReferenceEquals(_territories, value))
                {
                    if (!IsDeserializing && ChangeTracker.IsChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }

                    if (_territories != null)
                    {
                       _territories.CollectionChanged -= FixupTerritories;
                    }

                    _territories = value;

                    if (_territories != null)
                    {
                        _territories.CollectionChanged += FixupTerritories;
                    }

                    OnPropertyChanged("Territories", trackInChangeTracker: false);
                }
            }
        }
        private TrackableCollection<Territory> _territories;

        #endregion Navigation Properties

        #region ChangeTracking

        protected override void ClearNavigationProperties()
        {
            Employees1.Clear();
            Employee1 = null;
            Orders.Clear();
            Territories.Clear();
        }

        #endregion ChangeTracking

        #region Association Fixup

        private void FixupEmployee1(Employee previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (previousValue != null && previousValue.Employees1.Contains(this))
            {
                previousValue.Employees1.Remove(this);
            }

            if (Employee1 != null)
            {
                if (!Employee1.Employees1.Contains(this))
                {
                    Employee1.Employees1.Add(this);
                }

                ReportsTo = Employee1.EmployeeID;
            }
            else if (!skipKeys)
            {
                ReportsTo = null;
            }

            if (ChangeTracker.IsChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Employee1")
                    && ReferenceEquals(ChangeTracker.OriginalValues["Employee1"], Employee1))
                {
                    //ChangeTracker.OriginalValues.Remove("Employee1");
                }
                else
                {
                    //RecordOriginalValue("Employee1", previousValue);
                }
                if (Employee1 != null && !Employee1.ChangeTracker.IsChangeTrackingEnabled)
                {
                    Employee1.StartTracking();
                }
            }
        }

        private void FixupEmployees1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (Employee item in e.NewItems)
                {
                    item.Employee1 = this;
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.IsChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        RecordAdditionToCollectionProperties("Employees1", item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Employee item in e.OldItems)
                {
                    if (ReferenceEquals(item.Employee1, this))
                    {
                        item.Employee1 = null;
                    }
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        RecordRemovalFromCollectionProperties("Employees1", item);
                    }
                }
            }
        }

        private void FixupOrders(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (Order item in e.NewItems)
                {
                    item.Employee = this;
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.IsChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        RecordAdditionToCollectionProperties("Orders", item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Order item in e.OldItems)
                {
                    if (ReferenceEquals(item.Employee, this))
                    {
                        item.Employee = null;
                    }
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        RecordRemovalFromCollectionProperties("Orders", item);
                    }
                }
            }
        }

        private void FixupTerritories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (Territory item in e.NewItems)
                {
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.IsChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        RecordAdditionToCollectionProperties("Territories", item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Territory item in e.OldItems)
                {
                    if (ChangeTracker.IsChangeTrackingEnabled)
                    {
                        RecordRemovalFromCollectionProperties("Territories", item);
                    }
                }
            }
        }

        #endregion Association Fixup

        protected override bool IsKeyEqual(Entity other)
        {
            var entity = other as Employee;
            if (ReferenceEquals(null, entity)) return false;
            return this.EmployeeID == entity.EmployeeID;
        }

        protected override int GetKeyHashCode()
        {
            return this.EmployeeID.GetHashCode();
        }
    }
}