// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public class EditableEntityCollectionView<T> : EntityCollectionView<T>, IEditableCollectionView, ICollectionView, IEntityCollectionView, IFilteredCollectionView, IPagedCollectionView, INotifyPropertyChanged 
        where T : Entity
    {
        public EditableEntityCollectionView(DataLoader<T> dataLoader = null)
            : base(dataLoader)
        {
        }

        #region IEditableCollectionView

        private T _addingItem = null;
        private IEditable _editingItem = null;

        public Action<T> InitializeNewItem { get; set; }

        public object AddNew()
        {
            if (!CanAddNew) throw new Exception("Cannot add new");
            
            _addingItem = DataLoader.EntitySet.CreateNew(add: false);

            var initialize = InitializeNewItem;
            if (initialize != null)
            {
                initialize(_addingItem);
            }
            
            return _addingItem;
        }

        private bool _canAddNew = true;
        public bool CanAddNew
        {
            get { return _canAddNew && _addingItem == null; }
            set
            {
                if (_canAddNew != value)
                {
                    _canAddNew = value;
                    OnPropertyChanged("CanAddNew");
                }
            }
        }

        public bool CanCancelEdit
        {
            get { return _editingItem != null; }
        }

        public bool _canRemove = true;
        public bool CanRemove
        {
            get { return _canRemove && Count > 0; }
            set
            {
                if (_canRemove != value)
                {
                    _canRemove = value;
                    OnPropertyChanged("CanRemove");
                }
            }
        }

        public void CancelEdit()
        {
            if (_editingItem != null)
            {
                _editingItem.CancelEdit();
                _editingItem = null;
            }
        }

        public void CancelNew()
        {
            _addingItem = null;
        }

        public void CommitEdit()
        {
            if (_editingItem != null)
            {
                _editingItem.EndEdit();
                _editingItem = null;
            }
        }

        public void CommitNew()
        {
            if (_addingItem != null)
            {
                base.Add(_addingItem);

                var dataLoader = base.DataLoader;
                if (dataLoader != null)
                {
                    dataLoader.EntitySet.Add(_addingItem);
                }

                _addingItem = null;
            }
        }

        public object CurrentAddItem
        {
            get { return _addingItem; }
        }

        public object CurrentEditItem
        {
            get { return _editingItem; }
        }

        public void EditItem(object item)
        {
            _editingItem = item as T;
            if (_editingItem != null)
            {
                _editingItem.BeginEdit();
            }
        }

        public bool IsAddingNew
        {
            get { return _addingItem != null; }
        }

        public bool IsEditingItem
        {
            get { return _editingItem != null; }
        }

        private NewItemPlaceholderPosition _newItemPlaceholderPosition = NewItemPlaceholderPosition.None;
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get { return _newItemPlaceholderPosition; }
            set { _newItemPlaceholderPosition = value; }
        }

        public void Remove(object item)
        {
            if (item is T)
            {
                base.Remove(item as T);

                var dataLoader = base.DataLoader;
                if (dataLoader != null)
                {
                    dataLoader.EntitySet.Delete(item as T);
                }
            }
        }

        public new void RemoveAt(int index)
        {
            var item = this[index];
            this.Remove(item);
        }

        #endregion IEditableCollectionView
    }
}
