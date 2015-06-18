using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NUnit.Framework;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;

namespace UnitTests
{
    [TestFixture]
    public class RevertDirectedReferenceCollectionTest : AssertionHelper
    {
        private const int EMPLOYEE_ID = 1; // Davolio Nancy

        private const string TERRITORY1_ID = "06897"; // Wilton
        private const string TERRITORY2_ID = "19713"; // Neward
        private const string TERRITORY3_ID = "01833"; // Georgetow


        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T1_RevertDirectedReferenceCollection()
        {
            var ctx = Default.DataContext();

            // get data
            var employee = ctx.Employees
                .AsQueryable()
                .Include("Territories")
                .Single(e => e.EmployeeID == EMPLOYEE_ID);

            var territory1 = ctx.Territories.AsQueryable().Single(t => t.TerritoryID == TERRITORY1_ID);
            var territory2 = ctx.Territories.AsQueryable().Single(t => t.TerritoryID == TERRITORY2_ID);
            var territory3 = ctx.Territories.AsQueryable().Single(t => t.TerritoryID == TERRITORY3_ID);

            // check initial state
            Expect(employee, Is.Not.Null);
            Expect(territory1, Is.Not.Null);
            Expect(territory2, Is.Not.Null);
            Expect(territory3, Is.Not.Null);

            Expect(employee.Territories, Is.Not.Empty);
            Expect(employee.Territories.Count, Is.EqualTo(2));

            Assert.Contains(territory1, employee.Territories.ToArray());
            Assert.Contains(territory2, employee.Territories.ToArray());

            Expect(employee.HasChanges, Is.False);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // add one new
            employee.Territories.Add(territory3);

            Expect(employee.Territories.Count, Is.EqualTo(3));
            Assert.Contains(territory3, employee.Territories.ToArray());

            Expect(employee.HasChanges, Is.True);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(territory3.HasChanges, Is.False);
            Expect(territory3.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // revert
            employee.RevertChanges();

            Expect(employee.Territories.Count, Is.EqualTo(2));

            Expect(employee.HasChanges, Is.False);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(territory3.HasChanges, Is.False);
            Expect(territory3.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // remove one existing
            employee.Territories.Remove(territory2);

            Expect(employee.Territories.Count, Is.EqualTo(1));

            Expect(employee.HasChanges, Is.True);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(territory2.HasChanges, Is.False);
            Expect(territory2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // revert
            employee.RevertChanges();

            Expect(employee.Territories.Count, Is.EqualTo(2));

            Expect(employee.HasChanges, Is.False);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(territory2.HasChanges, Is.False);
            Expect(territory2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // remove all existing, add one new, and re-add one of the old
            employee.Territories.Clear();

            Expect(employee.Territories.Count, Is.EqualTo(0));
            Expect(employee.Territories, Is.Empty);

            Expect(employee.HasChanges, Is.True);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            employee.Territories.Add(territory3);
            employee.Territories.Add(territory2);

            Expect(territory1.HasChanges, Is.False);
            Expect(territory2.HasChanges, Is.False);
            Expect(territory3.HasChanges, Is.False);
            Expect(territory1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(territory2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(territory3.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // revert
            employee.RevertChanges();

            Expect(employee.Territories.Count, Is.EqualTo(2));

            Expect(employee.HasChanges, Is.False);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(territory1.HasChanges, Is.False);
            Expect(territory2.HasChanges, Is.False);
            Expect(territory3.HasChanges, Is.False);
            Expect(territory1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(territory2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(territory3.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // add one new, remove one existing, and accept changes
            Assert.Contains(territory1, employee.Territories.ToArray());
            Assert.Contains(territory2, employee.Territories.ToArray());

            employee.Territories.Add(territory3);
            employee.Territories.Remove(territory2);

            Expect(employee.Territories.Count, Is.EqualTo(2));
            Expect(employee.Territories, Is.Not.Empty);

            Expect(employee.HasChanges, Is.True);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Assert.Contains(territory1, employee.Territories.ToArray());
            Assert.Contains(territory3, employee.Territories.ToArray());

            employee.AcceptChanges();

            Expect(employee.HasChanges, Is.False);
            Expect(employee.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // revert
            employee.RevertChanges();

            Expect(employee.Territories.Count, Is.EqualTo(2));
            Assert.Contains(territory1, employee.Territories.ToArray());
            Assert.Contains(territory3, employee.Territories.ToArray());
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
