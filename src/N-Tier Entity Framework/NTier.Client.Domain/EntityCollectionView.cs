// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public class EntityCollectionView<T> : ObservableCollection<T>, ICollectionView, IEntityCollectionView<T>, IFilteredCollectionView, IPagedCollectionView, INotifyPropertyChanged 
        where T : Entity
    {
        public EntityCollectionView(DataLoader<T> dataLoader = null, bool autoLoad = true)
        {
            this.DataLoader = dataLoader;
            this.AutoLoad = autoLoad;
            this.Dispatcher = Deployment.Dispatcher;
        }

        #region Properties

        public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

        public bool AutoLoad
        {
            get { return _autoLoad; }
            set
            {
                if (value && !_autoLoad)
                {
                    _autoLoad = true;
                    OnPropertyChanged("AutoLoad");
                    Refresh();
                }
            }
        }
        private bool _autoLoad = false;

        #endregion Properties

        #region DataLoader

        public virtual DataLoader<T> DataLoader
        {
            get { return _dataLoader; }
            set
            {
                if (_dataLoader != value)
                {
                    if (_dataLoader != null)
                    {
                        _dataLoader.DataLoading -= OnDataLoading;
                        _dataLoader.DataLoaded -= OnDataLoaded;
                        _dataLoader.EntitySet.PropertyChanged -= EntitySet_PropertyChanged;
                    }

                    _dataLoader = value;

                    if (_dataLoader != null)
                    {
                        _dataLoader.DataLoading += OnDataLoading;
                        _dataLoader.DataLoaded += OnDataLoaded;
                        _dataLoader.EntitySet.PropertyChanged += EntitySet_PropertyChanged;
                    }

                    OnPropertyChanged("DataLoader");
                }
            }
        }
        private DataLoader<T> _dataLoader = null;

        protected virtual void OnDataLoaded(object sender, DataLoadedEventArgs<T> e)
        {
            //if (e.Result != null)
            //{
            //    Clear();
            //    foreach (var item in e.Result)
            //    {
            //        var v = item["AddressID"];
            //        item["SupplierID"] = 0;
            //        Add(item);
            //    }

            //    TotalItemCount = (int)e.Result.TotalCount.Value;
            //}
            
            var dataLoaded = DataLoaded;
            if (dataLoaded != null)
            {
                dataLoaded(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDataLoading(object sender, EventArgs e)
        {
            var dataLoading = DataLoading;
            if (dataLoading != null)
            {
                dataLoading(this, EventArgs.Empty);
            }
        }

        private void EntitySet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName){
                case "HasChanges":
                    OnPropertyChanged("CanChangePage");
                    break;
            }
        }

        #endregion DataLoader

        #region ObservableCollection

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (index == 0 || this.CurrentItem == null)
            {
                CurrentItem = item;
                CurrentPosition = index;
            }
        }

        public virtual object GetItemAt(int index)
        {
            if ((index >= 0) && (index < this.Count))
            {
                return this[index];
            }
            return null;
        }

        #endregion ObservableCollection

        #region INotifyPropertyChanged

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (Dispatcher != null && !Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)delegate { OnPropertyChanged(e); });
                return;
            }

            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                case "Count":
                    OnPropertyChanged("IsEmpty");
                    OnPropertyChanged("IsCurrentInView");
                    OnPropertyChanged("IsCurrentAfterLast");
                    break;

                case "TotalItemCount":
                    OnPropertyChanged("ItemCount");
                    OnPropertyChanged("NumberOfPages");
                    break;

                case "ItemCount":
                    OnPropertyChanged("CanChangePage");
                    break;

                case "PageSize":
                    OnPropertyChanged("NumberOfPages");
                    OnPropertyChanged("CanChangePage");
                    break;

                case "CurrentPosition":
                    OnPropertyChanged("IsCurrentInView");
                    OnPropertyChanged("IsCurrentInSync");
                    OnPropertyChanged("IsCurrentBeforeFirst");
                    OnPropertyChanged("IsCurrentAfterLast");
                    break;

                case "CurrentItem":
                    OnPropertyChanged("IsCurrentInSync");
                    break;

                case "IsCurrentInView":
                    OnPropertyChanged("IsCurrentInSync");
                    break;

                case "IsEmpty":
                    OnPropertyChanged("IsCurrentBeforeFirst");
                    OnPropertyChanged("IsCurrentAfterLast");
                    break;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region ICollectionView

        public virtual bool CanFilter
        {
            get { return _canFilter; }
            set
            {
                if (_canFilter != value)
                {
                    _canFilter = value;
                    OnPropertyChanged("CanFilter");
                }
            }
        }
        private bool _canFilter = false;

        public virtual bool CanGroup
        {
            get { return _canGroup; }
            set
            {
                if (_canGroup != value)
                {
                    _canGroup = value;
                    OnPropertyChanged("CanGroup");
                }
            }
        }
        private bool _canGroup = false;

        public virtual bool CanSort
        {
            get { return _canSort; }
            set
            {
                if (_canSort != value)
                {
                    _canSort = value;
                    OnPropertyChanged("CanSort");
                }
            }
        }
        private bool _canSort = true;

        public bool Contains(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }

            return base.Contains((T)item);
        }

        private bool IsValidType(object item)
        {
            return item is T;
        }

        public virtual CultureInfo Culture
        {
            get { return this._culture; }
            set
            {
                if (_culture != value)
                {
                    _culture = value;
                    OnPropertyChanged("Culture");
                }
            }
        }
        private CultureInfo _culture;

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public object CurrentItem
        {
            get { return _currentItem; }
            protected set
            {
                if (_currentItem != value)
                {
                    _currentItem = value;
                    OnPropertyChanged("CurrentItem");
                }
            }
        }
        private object _currentItem = null;

        public int CurrentPosition
        {
            get { return _currentPosition; }
            protected set
            {
                if (_currentPosition != value)
                {
                    _currentPosition = value;
                    OnPropertyChanged("CurrentPosition");
                }
            }
        }
        private int _currentPosition = -1;

        public virtual IDisposable DeferRefresh()
        {
            return new DeferRefreshObject(this);
        }

        private sealed class DeferRefreshObject : IDisposable
        {
            private readonly ICollectionView CollectionView;

            public DeferRefreshObject(ICollectionView collectionView)
            {
                this.CollectionView = collectionView;
            }

            public void Dispose()
            {
                CollectionView.Refresh();
            }
        }

        /// <summary>
        /// The Filter property does not get applied to the remote query, consider using FilterDescriptions instead
        /// </summary>
        public virtual Predicate<object> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                OnPropertyChanged("Filter");
                //if (!MoveToFirstPage())
                //{
                //    Refresh();
                //}
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        private Predicate<object> _filter;

        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            //get { throw new NotImplementedException(); }
            //get { return null; }
            get { return _groupDescriptions; }
        }
        private ObservableCollection<GroupDescription> _groupDescriptions = new ObservableCollection<GroupDescription>();

        public virtual ReadOnlyObservableCollection<object> Groups
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        public bool IsCurrentAfterLast
        {
            get
            {
                if (!this.IsEmpty)
                {
                    return (this.CurrentPosition >= this.Count);
                }
                return true;
            }
        }

        public bool IsCurrentBeforeFirst
        {
            get
            {
                if (!this.IsEmpty)
                {
                    return (this.CurrentPosition < 0);
                }
                return true;
            }
        }

        protected bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return (this.GetItemAt(this.CurrentPosition) == this.CurrentItem);
                }
                return (this.CurrentItem == null);
            }
        }

        private bool IsCurrentInView
        {
            get
            {
                return ((0 <= this.CurrentPosition) && (this.CurrentPosition < this.Count));
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        public bool MoveCurrentTo(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }
            if (object.Equals(this.CurrentItem, item) && ((item != null) || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }
            int index = this.IndexOf((T)item);
            return this.MoveCurrentToPosition(index);
        }

        public bool MoveCurrentToFirst()
        {
            return this.MoveCurrentToPosition(0);
        }

        public bool MoveCurrentToLast()
        {
            return this.MoveCurrentToPosition(this.Count - 1);
        }

        public bool MoveCurrentToNext()
        {
            return ((this.CurrentPosition < this.Count) && this.MoveCurrentToPosition(this.CurrentPosition + 1));
        }

        public bool MoveCurrentToPrevious()
        {
            return ((this.CurrentPosition >= 0) && this.MoveCurrentToPosition(this.CurrentPosition - 1));
        }

        public bool MoveCurrentToPosition(int position)
        {
            if ((position < -1) || (position > this.Count))
            {
                throw new ArgumentOutOfRangeException("position");
            }
            if (((position != this.CurrentPosition) || !this.IsCurrentInSync) && this.OKToChangeCurrent())
            {
                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                ChangeCurrentToPosition(position);
                OnCurrentChanged();
                if (this.IsCurrentAfterLast != isCurrentAfterLast)
                {
                    this.OnPropertyChanged("IsCurrentAfterLast");
                }
                if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
                {
                    this.OnPropertyChanged("IsCurrentBeforeFirst");
                }
                this.OnPropertyChanged("CurrentPosition");
                this.OnPropertyChanged("CurrentItem");
            }
            return this.IsCurrentInView;
        }

        private void ChangeCurrentToPosition(int position)
        {
            if (position < 0)
            {
                this.CurrentItem = null;
                this.CurrentPosition = -1;
            }
            else if (position >= this.Count)
            {
                this.CurrentItem = null;
                this.CurrentPosition = this.Count;
            }
            else
            {
                this.CurrentItem = this[position];
                this.CurrentPosition = position;
            }
        }

        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            this.OnCurrentChanging(args);
            return !args.Cancel;
        }

        protected virtual void OnCurrentChanged()
        {
            if (this.CurrentChanged != null)
            {
                this.CurrentChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (this.CurrentChanging != null)
            {
                this.CurrentChanging(this, args);
            }
        }

        protected void OnCurrentChanging()
        {
            this.CurrentPosition = -1;
            this.OnCurrentChanging(new CurrentChangingEventArgs(false));
        }

        protected override void ClearItems()
        {
            OnCurrentChanging();
            base.ClearItems();
        }

        public event EventHandler<RefreshEventArgs> Refreshing;

        public virtual void Refresh()
        {
            var refreshing = Refreshing;
            if (refreshing != null)
            {
                refreshing(this, new RefreshEventArgs() { SortDescriptions = SortDescriptions, FilterDescriptions = FilterDescriptions });
            }

            //this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            var dataLoader = DataLoader;
            if (dataLoader != null)
            {
                dataLoader.Load(this);
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (_sortDescriptions == null)
                {
                    _sortDescriptions = new InnerSortDescriptionCollection();
                    _sortDescriptions.SortCollectionChanged += SortDescriptionsChanged;
                    //OnPropertyChanged("SortDescriptions");
                }
                return _sortDescriptions;
            }
        }
        private InnerSortDescriptionCollection _sortDescriptions;

        private class InnerSortDescriptionCollection : SortDescriptionCollection
        {
            public event NotifyCollectionChangedEventHandler SortCollectionChanged
            {
                add
                {
                    CollectionChanged += value;
                }
                remove
                {
                    CollectionChanged -= value;
                }
            }
        }

        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.NewStartingIndex == -1 && SortDescriptions.Count > 0)
            {
                return;
            }
            if (((e.Action != NotifyCollectionChangedAction.Reset) || (e.NewItems != null)) ||
                (((e.NewStartingIndex != -1) || (e.OldItems != null)) || (e.OldStartingIndex != -1)))
            {
                //if (MoveToFirstPage())
                //{
                //    return;
                //}
                Refresh();
            }
        }

        public System.Collections.IEnumerable SourceCollection
        {
            get
            {
                return this;
            }
        }

        #endregion ICollectionView

        #region IFilteredCollectionView

        public FilterDescriptionCollection FilterDescriptions
        {
            get
            {
                if (_filterDescriptions == null)
                {
                    _filterDescriptions = new FilterDescriptionCollection();
                    _filterDescriptions.CollectionChanged += FilterDescriptionsChanged;
                    //OnPropertyChanged("FilterDescriptions");
                }
                return _filterDescriptions;
            }
        }
        private FilterDescriptionCollection _filterDescriptions;

        protected bool SuppressFilterCollectionChangedEvent
        {
            get { return FilterDescriptions.SuppressCollectionChangedEvent; }
            set { FilterDescriptions.SuppressCollectionChangedEvent = value; }
        }

        private void FilterDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!MoveToFirstPage())
            {
                Refresh();
            }
        }

        #endregion IFilteredCollectionView

        #region IPagedCollectionView

        public bool CanChangePageWhenHasChanges
        {
            get { return _canChangePageWhenHasChanges; }
            set
            {
                if (_canChangePageWhenHasChanges != value)
                {
                    _canChangePageWhenHasChanges = value;
                    OnPropertyChanged("CanChangePageWhenHasChanges");
                    OnPropertyChanged("CanChangePage");
                }
            }
        }
        private bool _canChangePageWhenHasChanges;

        public bool CanChangePage
        {
            get { return PageSize > 0 && ItemCount > PageSize && (CanChangePageWhenHasChanges || (DataLoader != null && !DataLoader.EntitySet.HasChanges)); }
        }

        public bool IsPageChanging
        {
            get { return _isPageChanging; }
            private set
            {
                if (_isPageChanging != value)
                {
                    _isPageChanging = value;
                    OnPropertyChanged("IsPageChanging");
                }
            }
        }
        private bool _isPageChanging = false;

        public int ItemCount
        {
            //get { return Count; }
            get
            {
                if (TotalItemCount < 0) return 0;
                return TotalItemCount;
            }
        }

        public bool MoveToFirstPage()
        {
            return MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            return MoveToPage(NumberOfPages - 1);
        }

        public bool MoveToNextPage()
        {
            return MoveToPage(PageIndex + 1);
        }

        public bool MoveToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= NumberOfPages)
            {
                return false;
            }

            IsPageChanging = true;

            var pageChanging = PageChanging;
            if (pageChanging != null)
            {
                pageChanging(this, new PageChangingEventArgs(pageIndex));
            }

            if (PageIndex != pageIndex)
            {
                PageIndex = pageIndex;
                return true;
            }

            return false;
        }

        public bool MoveToPreviousPage()
        {
            return MoveToPage(PageIndex - 1);
        }

        public event EventHandler<EventArgs> PageChanged;

        public event EventHandler<PageChangingEventArgs> PageChanging;

        public int PageIndex
        {
            get { return _pageIndex; }
            private set
            {
                if (_pageIndex != value)
                {
                    _pageIndex = value;
                    OnPropertyChanged("PageIndex");
                    Refresh();
                }
            }
        }
        private int _pageIndex = 0;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value <= 0 ? -1 : value;
                    OnPropertyChanged("PageSize");
                }
            }
        }
        private int _pageSize = -1;

        public int TotalItemCount
        {
            get { return _totalItemCount; }
            set
            {
                if (_totalItemCount != value)
                {
                    _totalItemCount = value;
                    OnPropertyChanged("TotalItemCount");
                    OnPropertyChanged("ItemCount");
                }
            }
        }
        private int _totalItemCount = -1;

        private int NumberOfPages
        {
            get
            {
                return PageSize < 0 ? 1 : (int)((TotalItemCount / PageSize) + ((TotalItemCount % PageSize) == 0 ? 0 : 1));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (IsPageChanging)
            {
                var pageChanged = PageChanged;
                if (pageChanged != null)
                {
                    pageChanged(this, EventArgs.Empty);
                }
                IsPageChanging = false;
            }
        }
        
        #endregion IPagedCollectionView
        
        #region IEntityCollectionView
        
        public event EventHandler DataLoading;

        public event EventHandler DataLoaded;
        
        #endregion
    }
}
