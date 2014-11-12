﻿//------------------------------------------------------------------------------
// <auto-generated>
//   This file was generated by T4 code generator Model2.tt.
//   Any changes made to this file manually may cause incorrect behavior
//   and will be lost next time the file is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using NTier.Client.Domain;
using TableInheritance.Common.Domain.Model.TableInheritanceDemoDB;

namespace TableInheritance.Client.Domain
{
    public partial interface ITableInheritanceDemoDBDataContext : IDataContext
    {

        #region People

        IEntitySet<Person> People { get; }

        void Add(Person entity);
        void Delete(Person entity);
        void Attach(Person entity);
        void AttachAsModified(Person entity, Person original);
        void Detach(Person entity);

        #endregion People

        #region Addresses

        IEntitySet<Address> Addresses { get; }

        void Add(Address entity);
        void Delete(Address entity);
        void Attach(Address entity);
        void AttachAsModified(Address entity, Address original);
        void Detach(Address entity);

        #endregion Addresses

        #region Demographics

        IEntitySet<Demographic> Demographics { get; }

        void Add(Demographic entity);
        void Delete(Demographic entity);
        void Attach(Demographic entity);
        void AttachAsModified(Demographic entity, Demographic original);
        void Detach(Demographic entity);

        #endregion Demographics

        #region EmployeeRoles

        IEntitySet<EmployeeRole> EmployeeRoles { get; }

        void Add(EmployeeRole entity);
        void Delete(EmployeeRole entity);
        void Attach(EmployeeRole entity);
        void AttachAsModified(EmployeeRole entity, EmployeeRole original);
        void Detach(EmployeeRole entity);

        #endregion EmployeeRoles

    }
}