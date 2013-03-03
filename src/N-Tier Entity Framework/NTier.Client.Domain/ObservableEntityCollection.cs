// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableEntityCollection<T> : ObservableCollection<T> where T : Entity
    {
        private readonly IEntitySet<T> EntitySet;
        private bool suppressUpdatesFromEntitySet = false;
        private bool suppressUpdatesToEntitySet = false;

        public ObservableEntityCollection(IEntitySet<T> entitySet, bool reflectChangesOfEntitySet = false)
            : base((IEnumerable<T>)entitySet ?? new T[0])
        {
            if (entitySet == null) { throw new ArgumentNullException("entitySet"); }

            this.EntitySet = entitySet;

            if (reflectChangesOfEntitySet)
            {
                this.EntitySet.CollectionChanged += (s, e) =>
                {
                    lock (EntitySet.SyncRoot)
                    {
                        if (!suppressUpdatesFromEntitySet)
                        {
                            suppressUpdatesToEntitySet = true;

                            switch (e.Action)
                            {
                                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                                    Clear();
                                    break;

                                default:
                                    if (e.OldItems != null)
                                    {
                                        foreach (T item in e.OldItems)
                                        {
                                            if (Contains(item))
                                            {
                                                Remove(item);
                                            }
                                        }
                                    }
                                    if (e.NewItems != null)
                                    {
                                        foreach (T item in e.NewItems)
                                        {
                                            if (!Contains(item))
                                            {
                                                Add(item);
                                            }
                                        }
                                    }
                                    break;
                            }


                            suppressUpdatesToEntitySet = false;
                        }
                    }
                };
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            lock (EntitySet.SyncRoot)
            {
                if (!suppressUpdatesToEntitySet)
                {
                    suppressUpdatesFromEntitySet = true;
                    EntitySet.Add(item);
                    suppressUpdatesFromEntitySet = false;
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            T itemToDelete = this[index];
            base.RemoveItem(index);

            lock (EntitySet.SyncRoot)
            {
                if (!suppressUpdatesToEntitySet)
                {
                    suppressUpdatesFromEntitySet = true;
                    EntitySet.Delete(itemToDelete);
                    suppressUpdatesFromEntitySet = false;
                }
            }
        }

        protected override void ClearItems()
        {
            T[] itemsToDelete = this.ToArray<T>();
            base.ClearItems();

            lock (EntitySet.SyncRoot)
            {
                if (!suppressUpdatesToEntitySet)
                {
                    suppressUpdatesFromEntitySet = true;
                    foreach (T item in itemsToDelete)
                    {
                        EntitySet.Delete(item);
                    }
                    suppressUpdatesFromEntitySet = false;
                }
            }
        }

        protected override void SetItem(int index, T item)
        {
            T itemToReplace = this[index];
            base.SetItem(index, item);

            lock (EntitySet.SyncRoot)
            {
                if (!suppressUpdatesToEntitySet)
                {
                    suppressUpdatesFromEntitySet = true;
                    EntitySet.Delete(itemToReplace);
                    EntitySet.Add(item);
                    suppressUpdatesFromEntitySet = false;
                }
            }
        }
    }
}
