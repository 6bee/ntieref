// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public sealed class FilterDescriptionCollection : ObservableCollection<FilterDescription>
    {
        internal bool SuppressCollectionChangedEvent
        {
            get { return _suppressCollectionChangedEvent; }
            set
            {
                if (_suppressCollectionChangedEvent != value)
                {
                    _suppressCollectionChangedEvent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SuppressCollectionChangedEvent"));
                }
            }
        }
        private bool _suppressCollectionChangedEvent = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!SuppressCollectionChangedEvent)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
