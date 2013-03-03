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
    public class DirectedNavigationCollectionTests : AssertionHelper
    {
        private const string TERRITORY1_ID = "06897"; // Wilton                                            
        private const string TERRITORY2_ID = "19713"; // Neward                                            

        private const int EMPLOYEE_ID = 1; // Davolio Nancy

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T1_CheckTerrotories()
        {
            var ctx = new NorthwindDataContext();

            ctx.Territories
                .AsQueryable()
                .WhereIn(t => t.TerritoryID, TERRITORY1_ID, TERRITORY2_ID)
                .Execute();

            Assert.AreEqual(ctx.Territories.Count, 2);
        }

        [Test]
        public void T2_CheckEmployeeTerrotories()
        {
            var ctx = new NorthwindDataContext();

            var employee = ctx.Employees
                .AsQueryable()
                .Include("Territories")
                .Single(e => e.EmployeeID == EMPLOYEE_ID);

            Assert.IsNotEmpty(employee.Territories.ToArray());
            Assert.AreEqual(employee.Territories.Count, 2);

            var territories = ctx.Territories
                .AsQueryable()
                .IncludeTotalCount()
                .WhereIn(t => t.TerritoryID, TERRITORY1_ID, TERRITORY2_ID)
                .ToArray();

            Assert.AreEqual(territories.Length, 2);

            foreach (var territory in territories)
            {
                Assert.Contains(territory, employee.Territories.ToArray());
            }
        }

        [Test]
        public void T3_RemoveTerriroryFromEmployee()
        {
            var ctx = new NorthwindDataContext();

            var employee = ctx.Employees
                .AsQueryable()
                .Include("Territories")
                .Single(e => e.EmployeeID == EMPLOYEE_ID);

            Assert.AreEqual(2, employee.Territories.Count);

            var territory = employee.Territories.FirstOrDefault(t => t.TerritoryID == TERRITORY1_ID);

            Assert.NotNull(territory);

            employee.Territories.Remove(territory);

            Assert.AreEqual(1, employee.Territories.Count);

            ctx.SaveChanges();
        }

        [Test]
        public void T4_CheckEmployeeTerrotory()
        {
            var ctx = new NorthwindDataContext();

            var employee = ctx.Employees
                .AsQueryable()
                .Include("Territories")
                .Single(e => e.EmployeeID == EMPLOYEE_ID);

            Assert.IsNotEmpty(employee.Territories.ToArray());
            Assert.AreEqual(1, employee.Territories.Count);

            var territory = ctx.Territories.AsQueryable().FirstOrDefault(t => t.TerritoryID == TERRITORY2_ID);

            Assert.NotNull(territory);
            
            Assert.Contains(territory, employee.Territories.ToArray());
        }

        [Test]
        public void T5_ReAddEmployeeTerrotory()
        {
            var ctx = new NorthwindDataContext();

            var employee = ctx.Employees
                .AsQueryable()
                .Include("Territories")
                .Single(e => e.EmployeeID == EMPLOYEE_ID);

            Assert.IsNotEmpty(employee.Territories.ToArray());
            Assert.AreEqual(1, employee.Territories.Count);

            var territories = ctx.Territories
                .AsQueryable()
                .IncludeTotalCount()
                .WhereIn(t => t.TerritoryID, TERRITORY1_ID, TERRITORY2_ID)
                .ToArray();

            Assert.AreEqual(2, territories.Length);


            var territory1 = ctx.Territories.AsQueryable().FirstOrDefault(t => t.TerritoryID == TERRITORY1_ID);
            var territory2 = ctx.Territories.AsQueryable().FirstOrDefault(t => t.TerritoryID == TERRITORY2_ID);

            Assert.NotNull(territory1);
            Assert.NotNull(territory2);

            Assert.Contains(territory2, employee.Territories.ToArray());

            employee.Territories.Add(territory1);

            Assert.AreEqual(2, employee.Territories.Count);
            Assert.Contains(territory1, employee.Territories.ToArray());

            ctx.SaveChanges();
        }


        [Test]
        public void T6_CheckEmployeeTerrotories()
        {
            T2_CheckEmployeeTerrotories();
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
