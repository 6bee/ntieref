﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator Model2.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Repositories;
using TableInheritance.Common.Domain.Model.TableInheritanceDemoDB;

namespace TableInheritance.Server.Domain.Repositories
{
    public partial interface ITableInheritanceDemoDBRepository : IRepository
    {
        #region EntitySets

        IEntitySet<Person> People { get; }

        IEntitySet<Address> Addresses { get; }

        IEntitySet<Demographic> Demographics { get; }

        IEntitySet<EmployeeRole> EmployeeRoles { get; }

        #endregion EntitySets
    }
}
