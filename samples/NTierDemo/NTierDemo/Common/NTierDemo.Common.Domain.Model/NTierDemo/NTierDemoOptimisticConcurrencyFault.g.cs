﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator NTierDemoModel_NTierEntityGenerator.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NTier.Common.Domain.Model;

namespace NTierDemo.Common.Domain.Model.NTierDemo
{
    [Serializable]
    [DataContract]
    [KnownType("GetKnownTypes")]
    public partial class NTierDemoOptimisticConcurrencyFault : NTier.Common.Domain.OptimisticConcurrencyFault
    {
        public NTierDemoOptimisticConcurrencyFault(string message, IEnumerable<Entity> entities)
            : base(message, entities)
        {
        }

        private static Type[] GetKnownTypes()
        {
            var types = typeof(NTierDemoOptimisticConcurrencyFault).Assembly.GetTypes()
                .Where(x => x.Namespace == typeof(NTierDemoOptimisticConcurrencyFault).Namespace)
                .Where(x => typeof(Entity).IsAssignableFrom(x) )
                .Where(x => x.IsPublic)
                .Where(x => !x.IsAbstract)
                .ToArray();
            return types;
        }
    }
}
