// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
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
}
