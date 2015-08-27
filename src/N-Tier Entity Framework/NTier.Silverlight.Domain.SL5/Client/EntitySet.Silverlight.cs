// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Client.Domain
{
    partial class EntitySet<TEntity>
    {
        internal void LoadAsync(ClientInfo clientInfo, Query query, AsyncQueryCallback<TEntity> callback)
        {
            //if (_queryDelegate == null)
            //{
            //    throw new Exception(string.Format("There is no query procedure for entity type {0}. Check whether this entity is not a aggregate root and needs to be loaded through its aggregate root.", typeof(TEntity)));
            //}

            _queryDelegate(clientInfo, query, delegate(QueryResult<TEntity> result, Exception exception)
            {
                try
                {
                    //if (DetachEntitiesUponNewQueryResult)
                    //{
                    //    if (MergeOption == MergeOption.PreserveChanges)
                    //    {
                    //        var unmodifiedEntities = this.Where(e => !e.HasChanges).ToArray();
                    //        foreach (var entity in unmodifiedEntities)
                    //        {
                    //            Detach(entity);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        DetachAll();
                    //    }
                    //}

                    IList<TEntity> resultList = null;
                    long? totalCount = null;

                    if (!ReferenceEquals(null, result))
                    //lock (_internalEntitySet.SyncRoot) // this would potentially lead to deadlocks
                    {
                        resultList = new List<TEntity>();
                        totalCount = result.TotalCount;

                        using (_internalEntitySet.PreventChangetracking())
                        {
                            if (result.TotalCount.HasValue)
                            {
                                _internalEntitySet.DatasourceTotalCount = result.TotalCount.Value;
                            }

                            if (result.Data != null)
                            {
                                if (DetachEntitiesUponNewQueryResult)
                                {
                                    if (MergeOption == MergeOption.PreserveChanges)
                                    {
                                        var unmodifiedEntities = this.Where(e => !e.HasChanges).ToArray();
                                        foreach (var entity in unmodifiedEntities)
                                        {
                                            Detach(entity);
                                        }
                                    }
                                    else
                                    {
                                        DetachAll();
                                    }
                                }

                                foreach (var entity in result.Data)
                                {
                                    TEntity existingEntity = null;
                                    if (MergeOption != MergeOption.NoTracking)
                                    {
                                        existingEntity = _attachDelegate(entity, DataContext.InsertMode.Attach, mergeOption: MergeOption);
                                    }
                                    if (existingEntity != null && existingEntity.ChangeTracker.State == ObjectState.Deleted)
                                    {
                                        continue;
                                    }
                                    resultList.Add(existingEntity != null ? existingEntity : entity);
                                }
                            }
                        }
                    }

                    callback(resultList, totalCount, exception);
                }
                catch (Exception e)
                {
                    callback(null, null, e);
                }
            });
        }
    }
}
