﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator DemoModel.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest;
using NTier.Client.Domain;

namespace ConcurrencyDemo.Client.Domain
{
    public partial interface IConcurrencyTestChangeSetFactory
    {
        ConcurrencyTestChangeSet CreateChangeSet(IEnumerable<ARecord> aRecords, IEnumerable<BRecord> bRecords, IEnumerable<CRecord> cRecords);
    }
}
