// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RLinq = Remote.Linq.Expressions;

namespace NTier.Common.Domain.Model
{
    [DataContract]
    public sealed class Query
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<bool> IncludeData { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<bool> IncludeTotalCount { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<string> IncludeList
        {
            get { return _include; }
            set
            {
                if (ReferenceEquals(value, null) || value.Count == 0)
                {
                    _include = null;
                }
                else
                {
                    _include = value;
                }
            }
        }
        private IList<string> _include = null;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<RLinq.Expression> FilterExpressionList
        {
            get { return _filterExpressionList; }
            set
            {
                if (ReferenceEquals(value, null) || value.Count == 0)
                {
                    _filterExpressionList = null;
                }
                else
                {
                    _filterExpressionList = value;
                }
            }
        }
        private IList<RLinq.Expression> _filterExpressionList;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<RLinq.SortExpression> SortExpressionList
        {
            get { return _sortExpressionList; }
            set
            {
                if (ReferenceEquals(value, null) || value.Count == 0)
                {
                    _sortExpressionList = null;
                }
                else
                {
                    _sortExpressionList = value;
                }
            }
        }
        private IList<RLinq.SortExpression> _sortExpressionList;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<int> Skip { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<int> Take { get; set; }
    }
}
