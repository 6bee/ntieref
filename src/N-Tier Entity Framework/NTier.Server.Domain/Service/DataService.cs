// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using NTier.Server.Domain.Repositories.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Transactions;

namespace NTier.Server.Domain.Service
{
    /// <summary>
    /// Base class for data services
    /// </summary>
    /// <typeparam name="TRepository">The repository type of the specific data service</typeparam>
    public abstract class DataService<TRepository> where TRepository : IRepository
    {
        #region query

        /// <summary>
        /// Executes the query against the data store including query interceptors
        /// </summary>
        /// <returns>The result for the given query</returns>
        protected virtual QueryResult<TEntity> Get<TEntity>(IEntityQueryable<TEntity> entityQueryable, Query query, ClientInfo clientInfo) where TEntity : Entity
        {
            if (ReferenceEquals(null, query.OfType))
            {
                return Query<TEntity, TEntity>(entityQueryable, query, clientInfo);
            }
            else
            {
                var queryMethod = typeof(DataService<TRepository>)
                    .GetMethod("Query", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .MakeGenericMethod(query.OfType, typeof(TEntity));
                var result = queryMethod.Invoke(this, new object[] { entityQueryable, query, clientInfo });
                return (QueryResult<TEntity>)result;
            }
        }

        private QueryResult<TBase> Query<TEntity, TBase>(IEntityQueryable<TBase> entityQueryable, Query query, ClientInfo clientInfo)
            where TEntity : TBase
            where TBase : Entity
        {
            List<TEntity> result = null;
            long? totalCount = null;
            if ((query.IncludeData ?? false) || (query.IncludeTotalCount ?? false))
            {
                // get filters defined by interceptors
                var basefilters = GetQueryInterceptors<TBase>(clientInfo);
                var filters = GetQueryInterceptors<TEntity>(clientInfo);

                var baseEntityQueryable = entityQueryable
                    .ApplyInclude(query.IncludeList)
                    .ApplyFilters(basefilters);

                var queryable = typeof(TEntity) == typeof(TBase)
                    ? (IDomainQueryable<TEntity>)baseEntityQueryable
                    : baseEntityQueryable.OfType<TEntity>();

                queryable = queryable
                    .ApplyFilters(filters);

                // retrieve data
                if (query.IncludeData ?? false)
                {
                    var q = queryable
                        .ApplyFilters(query.FilterExpressionList)
                        .ApplySorting(query.SortExpressionList);

                    if (query.Skip.HasValue && query.Skip.Value > 0)
                    {
                        q = q.Skip(query.Skip.Value);
                    }

                    if (query.Take.HasValue && query.Take.Value > 0)
                    {
                        q = q.Take(query.Take.Value);
                    }

                    result = q.ToList();
                }

                // retrieve count
                if (query.IncludeTotalCount ?? false)
                {
                    var q = queryable.ApplyFilters(query.FilterExpressionList);
                    totalCount = q.LongCount();
                }
            }
            return new QueryResult<TBase> { Data = result, TotalCount = totalCount };
        }
        

        #endregion query

        #region update

        /// <summary>
        /// Executes change interceptors and applies changes to the objects set for each entity contained in the change set
        /// </summary>
        protected virtual void ApplyChanges<TEntity>(TRepository repository, IEntitySet<TEntity> entitySet, IChangeSet changeSet, IList<TEntity> entityChangeSet, ClientInfo clientInfo) where TEntity : Entity
        {
            // apply chnages to repository
            if (entityChangeSet != null && entityChangeSet.Count > 0)
            {
                foreach (var entity in entityChangeSet.ToArray())
                {
                    // run interceptors
                    ExecuteChangeInterceptors(repository, changeSet, entity, clientInfo);
                }
                foreach (var entity in entityChangeSet)
                {
                    // stop processing if transaction is not active
                    if (Transaction.Current.TransactionInformation.Status != TransactionStatus.Active)
                    {
                        return;
                    }

                    if (IsValid(entity))
                    {
                        // apply changes
                        entitySet.ApplyChanges(entity);
                    }
                }
            }
        }

        private static bool IsValid(Entity entity)
        {
            return !entity.Errors.Any(e => e.IsError);
        }

        /// <summary>
        /// Persists changes to the data store
        /// </summary>
        protected virtual void SaveChanges(TRepository repository, IChangeSet changeSet, IResultSet resultSet)
        {
            // don't save in case of errors
            if (changeSet.Any(entity => entity.Errors.Any(error => error.IsError)))
            {
                return;
            }


            var optimisticConcurrencyExceptionEntities = new List<Entity>();
            var updataeException = new List<Entity>();
            var allSaved = false;
            while (!allSaved)
            {
                try
                {
                    // save changes to database
                    repository.SaveChanges();
                    allSaved = true;
                }
                catch (UpdateException ex)
                {
                    var entities = ex.Entities;
                    // refresh changed entities from store
                    if (entities.Any(x => x.ChangeTracker.State != ObjectState.Added))
                    {
                        repository.Refresh(RefreshMode.StoreWins, entities.Where(x => x.ChangeTracker.State != ObjectState.Added));
                    }
                    // remove new entities from repository
                    if (entities.Any(x => x.ChangeTracker.State == ObjectState.Added))
                    {
                        foreach (var entity in entities.Where(x => x.ChangeTracker.State == ObjectState.Added))
                        {
                            DetachEntity(repository, entity);
                        }
                    }
                    AddExceptionMessageAsErrorEntry(ex, entities);
                    // collect faulted entities
                    if (ex is OptimisticConcurrencyException)
                    {
                        optimisticConcurrencyExceptionEntities.AddRange(entities);
                    }
                    else
                    {
                        updataeException.AddRange(entities);
                    }
                }
            }
            if (updataeException.Any())
            {
                var message = "Update, insert, or delete statement failed for one or more entities. Transaction was rolled back.";
                throw CreateUpdateFaultException(message, updataeException.Concat(optimisticConcurrencyExceptionEntities));
            }
            if (optimisticConcurrencyExceptionEntities.Any())
            {
                var message = "Update, insert, or delete statement failed for one or more entities. Transaction was rolled back.";
                throw CreateOptimisticConcurrencyFaultException(message, optimisticConcurrencyExceptionEntities);
            }
        }

        private static void DetachEntity(TRepository repository, Entity entity)
        {
            var entitySetType = entity.GetType();

            while (entitySetType.BaseType != typeof(Entity))
            {
                entitySetType = entitySetType.BaseType;
            }

            var detachMethod = typeof(DataService<>)
                .MakeGenericType(typeof(TRepository))
                .GetMethod("GenericDetachEntity", BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(entitySetType);
            detachMethod.Invoke(null, new object[] { repository, entity });
        }

        private static void GenericDetachEntity<T>(IRepository repository, T entity) where T : Entity
        {
            repository.GetEntitySet<T>().Detach(entity);
        }

        private static void AddExceptionMessageAsErrorEntry(Exception ex, IEnumerable<Entity> entities)
        {
            var message = GetInnerMostExceptionMessage(ex);
            var error = new Error(message);
            foreach (var entity in entities)
            {
                entity.Errors.Add(error);
            }
        }

        private static string GetInnerMostExceptionMessage(Exception ex)
        {
            if (!ReferenceEquals(null, ex.InnerException))
            {
                var message = GetInnerMostExceptionMessage(ex.InnerException);
                if (!string.IsNullOrEmpty(message))
                {
                    return message;
                }
            }
            return ex.Message;
        }

        protected abstract FaultException CreateOptimisticConcurrencyFaultException(string message, IEnumerable<Entity> entities);
        protected abstract FaultException CreateUpdateFaultException(string message, IEnumerable<Entity> entities);

        #endregion update

        #region transaction management

        protected virtual TransactionScope CreateSavingTransactionScope()
        {
            return new TransactionScope();
        }

        #endregion transaction management

        #region interceptors

        /// <summary>
        /// The private Interceptors class loads interceptors by reflection and caches these interceptors by data service and entity types
        /// </summary>
        private sealed class Interceptors
        {
            private sealed class InterceptorCollections
            {
                internal IEnumerable<MethodInfo> QueryInterceptors;
                internal IEnumerable<MethodInfo> ChangeInterceptors;
            }

            private static readonly object SingletonLock = new object();
            private readonly object QueryInterceptorsLock = new object();
            private readonly object ChangeInterceptorsLock = new object();

            private static Interceptors _sinleton = null;

            /// <summary>
            /// Gets the singleton instance
            /// </summary>
            internal static Interceptors Instance
            {
                get
                {
                    if (_sinleton == null)
                    {
                        lock (SingletonLock)
                        {
                            if (_sinleton == null)
                            {
                                _sinleton = new Interceptors();
                            }
                        }
                    }
                    return _sinleton;
                }
            }

            private Interceptors()
            {
            }

            private readonly Dictionary<Type, Dictionary<Type, InterceptorCollections>> _dict = new Dictionary<Type, Dictionary<Type, InterceptorCollections>>();

            private Dictionary<Type, InterceptorCollections> GetInterceptorsDictionary(Type dataServiceType)
            {
                if (!_dict.ContainsKey(dataServiceType))
                {
                    lock (_dict)
                    {
                        if (!_dict.ContainsKey(dataServiceType))
                        {
                            _dict[dataServiceType] = new Dictionary<Type, InterceptorCollections>();
                        }
                    }
                }
                return _dict[dataServiceType];
            }

            private InterceptorCollections GetInterceptorsDictionary<TEntity>(Type dataServiceType) where TEntity : Entity
            {
                var interceptors = GetInterceptorsDictionary(dataServiceType);

                var entityType = typeof(TEntity);
                if (!interceptors.ContainsKey(entityType))
                {
                    lock (interceptors)
                    {
                        if (!interceptors.ContainsKey(entityType))
                        {
                            interceptors[entityType] = new InterceptorCollections();
                        }
                    }
                }
                return interceptors[entityType];
            }

            /// <summary>
            /// Returns a collection of query interceptors for a given data service and entity type
            /// </summary>
            /// <typeparam name="TEntity">The type of the entity for which the interceptors have to be returned</typeparam>
            /// <param name="dataServiceType">The type of the data service for which the interceptors have to be returned</param>
            /// <returns>Returns a collection of query interceptors</returns>
            internal IEnumerable<MethodInfo> GetQueryInterceptors<TEntity>(Type dataServiceType) where TEntity : Entity
            {
                var interceptors = GetInterceptorsDictionary<TEntity>(dataServiceType);

                if (interceptors.QueryInterceptors == null)
                {
                    lock (QueryInterceptorsLock)
                    {
                        if (interceptors.QueryInterceptors == null)
                        {
                            interceptors.QueryInterceptors = dataServiceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(IsQueryInterceptorOf<TEntity>());
                        }
                    }
                }

                return interceptors.QueryInterceptors;
            }

            /// <summary>
            /// Returns a collection of change interceptors for a given data service and entity type
            /// </summary>
            /// <typeparam name="TEntity">The type of the entity for which the interceptors have to be returned</typeparam>
            /// <param name="dataServiceType">The type of the data service for which the interceptors have to be returned</param>
            /// <returns>Returns a collection of change interceptors</returns>
            internal IEnumerable<MethodInfo> GetChangeInterceptors<TEntity>(Type dataServiceType) where TEntity : Entity
            {
                var interceptors = GetInterceptorsDictionary<TEntity>(dataServiceType);

                if (interceptors.ChangeInterceptors == null)
                {
                    lock (ChangeInterceptorsLock)
                    {
                        if (interceptors.ChangeInterceptors == null)
                        {
                            interceptors.ChangeInterceptors = dataServiceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(IsChangeInterceptorOf<TEntity>());
                        }
                    }
                }

                return interceptors.ChangeInterceptors;
            }

            private static Func<MethodInfo, bool> IsQueryInterceptorOf<TEntity>() where TEntity : Entity
            {
                return m =>
                {
                    var attributes = m.GetCustomAttributes(typeof(QueryInterceptorAttribute), true);
                    return attributes.Length == 1 && ((QueryInterceptorAttribute)attributes[0]).Type == typeof(TEntity);
                };
            }

            private static Func<MethodInfo, bool> IsChangeInterceptorOf<TEntity>() where TEntity : Entity
            {
                return m =>
                {
                    var attributes = m.GetCustomAttributes(typeof(ChangeInterceptorAttribute), true);
                    return attributes.Length == 1 && ((ChangeInterceptorAttribute)attributes[0]).Type == typeof(TEntity);
                };
            }
        }

        /// <summary>
        /// Retrieves matching query interceptors using reflection
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to retrieve the query interceptors</typeparam>
        /// <param name="clientInfo">Client info parameter which is passed to query interceptors</param>
        /// <returns>An collection of query interceptors for the specified entity type</returns>
        protected virtual IEnumerable<Expression<Func<TEntity, bool>>> GetQueryInterceptors<TEntity>(ClientInfo clientInfo) where TEntity : Entity
        {
            var list = new List<Expression<Func<TEntity, bool>>>();

            foreach (var interceptor in Interceptors.Instance.GetQueryInterceptors<TEntity>(GetType()))
            {
                Expression<Func<TEntity, bool>> filter;
                if (interceptor.GetParameters().Length == 0)
                {
                    filter = (Expression<Func<TEntity, bool>>)interceptor.Invoke(this, null);
                }
                else
                {
                    filter = (Expression<Func<TEntity, bool>>)interceptor.Invoke(this, new object[] { clientInfo });
                }
                list.Add(filter);
            }

            return list;
        }

        /// <summary>
        /// Retrieves matching change interceptors using reflection and executes them for a given instance of an entity
        /// </summary>
        /// <typeparam name="TEntity">The entity type for which to execute the change interceptors</typeparam>
        /// <param name="entity">The entity for which to execute the change intercetors</param>
        /// <param name="clientInfo">Client info parameter which is passed to change interceptors</param>
        protected virtual void ExecuteChangeInterceptors<TEntity>(TRepository repository, IChangeSet changeSet, TEntity entity, ClientInfo clientInfo) where TEntity : Entity
        {
            foreach (var interceptor in Interceptors.Instance.GetChangeInterceptors<TEntity>(GetType()))
            {
                var parameters = interceptor.GetParameters();
                if (parameters.Length == 1)
                {
                    interceptor.Invoke(this, new object[] { new ChangeInterceptorArgs<TEntity>(repository, changeSet, entity, entity.ChangeTracker.State.ToUpdateOperation(), clientInfo) });
                }
                else if (parameters.Length == 2)
                {
                    interceptor.Invoke(this, new object[] { entity, entity.ChangeTracker.State.ToUpdateOperation() });
                }
                else
                {
                    interceptor.Invoke(this, new object[] { entity, entity.ChangeTracker.State.ToUpdateOperation(), clientInfo });
                }
            }
        }

        #endregion interceptors
    }
}