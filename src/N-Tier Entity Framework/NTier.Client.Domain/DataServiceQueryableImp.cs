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
    internal partial class DataServiceQueryableImp<TEntity> : DataServiceQueryable<TEntity> where TEntity : Entity<TEntity>
    {
        #region Constructor

        internal DataServiceQueryableImp(IEntitySet<TEntity> source)
            : base(source, null)
        {
        }

        internal DataServiceQueryableImp(DataServiceQueryable<TEntity> source)
            : base(source.EntitySet, source)
        {
        }

        #endregion Constructor

        #region Query properties

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Query Query
        {
            get
            {
                return new Query
                {
                    IncludeTotalCount = ParentQueryTotalCount,
                    IncludeData = ParentQueryData ?? true,
                    IncludeList = ParentIncludeValues.ToList(),
                    FilterExpressionList = ParentFilters.ToList(),
                    SortExpressionList = ParentSortings.ToList(),
                    Skip = ParentSkipValue,
                    Take = ParentTakeValue
                };
            }
        }


        internal override bool? QueryTotalCount { get; set; }

        internal override bool? QueryData { get; set; }

        private ISet<string> _includeValues;
        internal override ISet<string> IncludeValues
        {
            get
            {
                if (_includeValues == null)
                {
                    _includeValues = new HashSet<string>();
                }
                return _includeValues;
            }
        }

        internal override IList<Filter> Filters
        {
            get { return _filters ?? (_filters = new List<Filter>()); }
        }
        private IList<Filter> _filters;

        internal override IList<Sort> Sortings
        {
            get
            {
                if (_sortings == null)
                {
                    var sortings = new ObservableCollection<Sort>();
                    sortings.CollectionChanged += (sender, e) =>
                    {
                        if (e.Action == NotifyCollectionChangedAction.Reset)
                        {
                            IsSortingReset = true;
                        }
                    };
                    _sortings = sortings;
                }
                return _sortings;
            }
        }
        private IList<Sort> _sortings;


        private int? _skipValue = null;
        internal override int? SkipValue
        {
            get { return _skipValue; }
            set
            {
                _skipValue = value;

                // mimic behaviour of entity framework and linq-to-object when calling take before skip
                if (_skipValue.HasValue && Parent.TakeValue.HasValue)
                {
                    if (Parent.TakeValue.Value > _skipValue.Value)
                    {
                        TakeValue = Parent.TakeValue.Value - _skipValue.Value;
                    }
                    else
                    {
                        TakeValue = 0;
                        //_skipValue = null;
                    }
                }
            }
        }

        internal override int? TakeValue { get; set; }

        #endregion Query properties
    }
}
