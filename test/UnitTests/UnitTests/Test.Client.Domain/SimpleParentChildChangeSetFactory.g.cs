﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator SimpleParentChildModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NTier.Client.Domain;
using Test.Common.Domain.Model.SimpleParentChild;

namespace Test.Client.Domain
{
    public partial class SimpleParentChildChangeSetFactory : ChangeSetFactory, ISimpleParentChildChangeSetFactory
    {
        public SimpleParentChildChangeSet CreateChangeSet(IEnumerable<Parent> parentSet, IEnumerable<Child> childSet)
        {
            // retrieve changes sets (modified entities)
            var parentChangeSet = GetChangeSet(parentSet);
            var childChangeSet = GetChangeSet(childSet);

            // reduce entities (copy changed values)
            var parentSetMap = ReduceToModifications(parentChangeSet);
            var childSetMap = ReduceToModifications(childChangeSet);

            // fixup relations (replaces related entities with reduced entites)
            FixupRelations(
                CastToEntityTuple(parentSetMap), 
                CastToEntityTuple(childSetMap)
            );

            var changeSet = new SimpleParentChildChangeSet();

            if (parentSetMap.Count > 0) changeSet.ParentSet = parentSetMap.Select(e => e.ReducedEntity).ToList();
            if (childSetMap.Count > 0) changeSet.ChildSet = childSetMap.Select(e => e.ReducedEntity).ToList();

            return changeSet;
        }
    }
}
