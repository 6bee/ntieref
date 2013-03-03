// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    partial class EntitySet<TEntity>
    {
        internal IEnumerable<TEntity> Load(DataServiceQueryableImp<TEntity> queryable)
        {
            //if (_queryDelegate == null)
            //{
            //    throw new Exception(string.Format("There is no query procedure for entity type {0}. Check whether this entity is not a aggregate root and needs to be loaded through its aggregate root.", typeof(TEntity)));
            //}

            var result = _queryDelegate(queryable.ClientInfo, queryable.Query);

            //lock (_internalEntitySet.SyncRoot) // this would potentially lead to deadlocks
            {
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
                            yield return existingEntity != null ? existingEntity : entity;
                        }
                    }
                }
            }
        }
    }
}
