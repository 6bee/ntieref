// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using NTier.Common.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace NTier.Client.Domain
{
    partial class EntitySet<TEntity>
    {
        internal IEnumerable<TEntity> Load(ClientInfo clientInfo, Query query)
        {
            var result = _queryDelegate(clientInfo, query);

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
