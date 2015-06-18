// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;

namespace NTier.Client.Domain
{
    partial class DataServiceQueryable<TEntity, TBase>
        where TEntity : TBase
        where TBase : Entity
    {
        protected sealed class QueryResult : IQueryResult<TEntity, TBase>
        {
            private readonly IEntitySet<TBase> _entitySet;
            private readonly IEnumerable<TEntity> _resultSet;
            private readonly long? _totalCount;
            private readonly Exception _exception;

            public QueryResult(IEntitySet<TBase> entitySet, IEnumerable<TEntity> resultSet, long? totalCount)
                : this(entitySet, resultSet, totalCount, null)
            {
            }

            public QueryResult(Exception exception)
                : this(null, null, null, exception)
            {
            }

            private QueryResult(IEntitySet<TBase> entitySet, IEnumerable<TEntity> resultSet, long? totalCount, Exception exception)
            {
                this._entitySet = entitySet;
                this._resultSet = resultSet;
                this._totalCount = totalCount;
                this._exception = exception;
            }

            public Exception Exception
            {
                get { return _exception; }
            }

            public bool IsFaulted
            {
                get { return _exception != null; }
            }

            public IEntitySet<TBase> EntitySet
            {
                get { CheckException(); return _entitySet; }
            }

            public IEnumerable<TEntity> ResultSet
            {
                get { CheckException(); return _resultSet; }
            }

            public long? TotalCount
            {
                get { CheckException(); return _totalCount; }
            }

            private void CheckException()
            {
                if (_exception != null) throw new Exception("Query failed", _exception);
            }
        }
    }
}
