// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace NTier.Client.Domain
{
    public sealed class FilterExpressionCollection : ObservableCollection<System.Linq.Expressions.LambdaExpression>
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
