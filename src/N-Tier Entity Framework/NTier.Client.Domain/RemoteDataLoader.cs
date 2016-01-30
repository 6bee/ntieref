// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using Remote.Linq;
using Remote.Linq.ExpressionVisitors;
using System;
using System.Linq;
using System.Windows.Threading;

namespace NTier.Client.Domain
{
    public class RemoteDataLoader<T> : DataLoader<T> where T : Entity
    {
        protected readonly System.Windows.Threading.Dispatcher Dispatcher;
        protected readonly IDataServiceQueryable<T, T> Queryable;
        private readonly object TimerLock = new object();
        private DispatcherTimer Timer = null;
        private long RequestId = long.MinValue;

        public RemoteDataLoader(IDataServiceQueryable<T, T> queryable)
            : base(queryable.EntitySet)
        {
            this.Dispatcher = Deployment.Dispatcher;
            this.Queryable = queryable;
        }

        private TimeSpan _requestDelay = TimeSpan.FromSeconds(0.1);
        public virtual TimeSpan RequestDelay
        {
            get { return _requestDelay; }
            set { _requestDelay = value; }
        }

        public override void Load(IEntityCollectionView<T> view)
        {
            lock (TimerLock)
            {
                if (Timer == null)
                {
                    Timer = new DispatcherTimer();
                    Timer.Tick += (s, arg) =>
                    {
                        lock (TimerLock)
                        {
                            if (Timer != null)
                            {
                                Timer.Stop();
                                Timer = null;

                                LoadData(view);
                            }
                        }
                    };
                    Timer.Interval = RequestDelay;
                    Timer.Start();
                }
            }
        }

        private void LoadData(IEntityCollectionView<T> view)
        {
            #region query

            var requestId = ++RequestId;

            var query = Queryable.IncludeTotalCount();

            #endregion query

            #region paging

            if (view.PageSize > 0)
            {
                query = query
                    .Skip(view.PageIndex * view.PageSize)
                    .Take(view.PageSize);
            }

            #endregion pagging

            #region filters

            foreach (var filterExpression in view.FilterExpressions)
            {
                var remoteExpression = DataServiceQueryable.ToRemoteExpression(filterExpression);
                query = query.Where(remoteExpression);
            }

            #endregion filters

            #region sorting

            // apply sort descriptors
            IOrderedDataServiceQueryable<T, T> sortedQuery = null;
            foreach (var sortDescription in view.SortDescriptions)
            {
                var sortExpression = sortDescription.ToExpression<T>();
                if (ReferenceEquals(sortedQuery, null))
                {
                    sortedQuery = query.OrderBy(sortExpression);
                }
                else
                {
                    sortedQuery = sortedQuery.ThenBy(sortExpression);
                }
            }
            if (!ReferenceEquals(sortedQuery, null))
            {
                query = sortedQuery;
            }

            #endregion sorting

            #region request

            // notify data loading
            OnDataLoading();

            query.ExecuteAsync(
                (result) =>
                {
                    if (requestId < RequestId)
                    {
                        // request outdated
                        return;
                    }

                    // synchronize with main thread
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        // check for exception
                        if (result.IsFaulted)
                        {
                            var queryException = QueryException;
                            if (queryException != null)
                            {
                                queryException(this, new ExceptionEventArgs(result.Exception));
                                OnDataLoaded(null);
                                return;
                            }
                            else
                            {
                                throw new Exception("Query failed", result.Exception);
                            }
                        }

                        // retrieve data
                        var entitySet = result.EntitySet;

                        // fill items
                        view.Clear();
                        foreach (var item in entitySet)
                        {
                            view.Add(item);
                        }

                        // sets total count
                        view.TotalItemCount = entitySet.TotalCount.HasValue ? (int)entitySet.TotalCount.Value : -1;

                        // notify data loaded
                        OnDataLoaded(entitySet);
                    });
                }
            );

            #endregion request
        }

        public event EventHandler<ExceptionEventArgs> QueryException;
    }
}
