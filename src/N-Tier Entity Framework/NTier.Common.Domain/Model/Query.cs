// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using Aqua.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<string> IncludeList
        {
            get { return _include; }
            set
            {
                if (ReferenceEquals(value, null) || !value.Any())
                {
                    _include = null;
                }
                else
                {
                    _include = value;
                }
            }
        }
        private IEnumerable<string> _include = null;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<RLinq.LambdaExpression> FilterExpressionList
        {
            get { return _filterExpressionList; }
            set
            {
                if (ReferenceEquals(value, null) || !value.Any())
                {
                    _filterExpressionList = null;
                }
                else
                {
                    _filterExpressionList = value;
                }
            }
        }
        private IEnumerable<RLinq.LambdaExpression> _filterExpressionList;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<RLinq.SortExpression> SortExpressionList
        {
            get { return _sortExpressionList; }
            set
            {
                if (ReferenceEquals(value, null) || !value.Any())
                {
                    _sortExpressionList = null;
                }
                else
                {
                    _sortExpressionList = value;
                }
            }
        }
        private IEnumerable<RLinq.SortExpression> _sortExpressionList;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo OfType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<int> Skip { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Nullable<int> Take { get; set; }
    }
}
