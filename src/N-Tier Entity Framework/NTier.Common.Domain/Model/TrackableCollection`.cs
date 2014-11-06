// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NTier.Common.Domain.Model
{
    /// <summary>
    /// A collection that raises collection changed notifications and prevents adding duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TrackableCollection<T> : ITrackableCollection<T>, ITrackableCollection, INotifyCollectionChanged
    {
        private readonly object SyncRoot = new object();
        private readonly IList<T> _data = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        private int IndexOf(T item)
        {
            return _data.IndexOf(item);
        }

        int IList<T>.IndexOf(T item)
        {
            return IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            if (Contains(item))
            {
                return;
            }

            _data.Insert(index, item);

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        void IList<T>.RemoveAt(int index)
        {
            var item = this[index];
            _data.RemoveAt(index);

            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }

        private T this[int index]
        {
            get
            {
                return _data[index];
            }
            set
            {
                if (Contains(value))
                {
                    return;
                }

                if (index >= 0 && index < Count)
                {
                    var oldItem = _data[index];
                    _data[index] = value;

                    var collectionChanged = CollectionChanged;
                    if (collectionChanged != null)
                    {
                        collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
                    }
                }
                else
                {
                    _data[index] = value;

                    var collectionChanged = CollectionChanged;
                    if (collectionChanged != null)
                    {
                        collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
                    }
                }
            }
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = value; }
        }

        public void Replace(T oldItem, T newItem)
        {
            if (object.ReferenceEquals(oldItem, newItem))
            {
                return;
            }

            if (!Contains(oldItem))
            {
                throw new Exception("Old item is not contained.");
            }

            var index = IndexOf(oldItem);
            this[index] = newItem;

            //if (Contains(newItem))
            //{
            //    throw new Exception("New item is already contained.");
            //}

            //var index = IndexOf(oldItem);
            //_data.Add(newItem);
            //var successfullyRemoved = _data.Remove(oldItem);
            //if (!successfullyRemoved)
            //{
            //    _data.Remove(newItem);
            //    throw new Exception("Old item is not contained.");
            //}

            //var collectionChanged = CollectionChanged;
            //if (collectionChanged != null)
            //{
            //    collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            //}
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

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _data.Count; }
        }

        bool ICollection<T>.IsReadOnly
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

        int System.Collections.IList.Add(object obj)
        {
            var item = Cast(obj);
            Add(item);
            return IndexOf(item) + 1;
        }

        //void System.Collections.IList.Clear()
        //{
        //    throw new NotImplementedException();
        //}

        bool System.Collections.IList.Contains(object obj)
        {
            return Contains(Cast(obj));
        }

        int System.Collections.IList.IndexOf(object obj)
        {
            return IndexOf(Cast(obj));
        }

        void System.Collections.IList.Insert(int index, object obj)
        {
            ((IList<T>)this).Insert(index, Cast(obj));
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return false; }
        }

        void System.Collections.IList.Remove(object obj)
        {
            Remove(Cast(obj));
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            ((IList<T>)this).RemoveAt(index);
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = Cast(value);
            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            ((System.Collections.IList)_data).CopyTo(array, index);
        }

        //int System.Collections.ICollection.Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return SyncRoot; }
        }

        private static T Cast(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException("obj");
            }
            if (!(obj is T))
            {
                throw new ArgumentException(string.Format("Argument expected to be of type {0} and got type {1}.", typeof(T).FullName, obj.GetType().FullName));
            }
            return (T)obj;
        }
    }
}
