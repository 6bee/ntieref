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
    public class ReferenceCollectionTests : AssertionHelper
    {
        private readonly string GROUP1_ID = "F";
        private readonly string GROUP1_DESC = "Female";
     
        private readonly string GROUP2_ID = "M";
        private readonly string GROUP2_DESC = "Male";

        private readonly string CUSTOMER_ID = "ALFKI";

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T0_DeleteAllCustomerDemographics()
        {
            DeleteAllCustomerDemographics();
        }

        private void DeleteAllCustomerDemographics()
        {
            var ctx = new NorthwindDataContext();

            // load existing customer groups
            ctx.CustomerDemographics
                .AsQueryable()
                .WhereIn(g => g.ID, GROUP1_ID, GROUP2_ID)
                .Execute();

            // delete all existing
            if (ctx.CustomerDemographics.Any())
            {
                ctx.CustomerDemographics.DeleteAll();
                ctx.SaveChanges();
            }
        }


        [Test]
        public void T1_CreateCustomerGroups()
        {
            var ctx = new NorthwindDataContext();

            ctx.CustomerDemographics.Add(new DemographicGroup { ID = GROUP1_ID, CustomerDesc = GROUP1_DESC });
            ctx.CustomerDemographics.Add(new DemographicGroup { ID = GROUP2_ID, CustomerDesc = GROUP2_DESC });

            ctx.SaveChanges();
        }


        [Test]
        public void T2_CheckCustomerGroups()
        {
            var ctx = new NorthwindDataContext();

            ctx.CustomerDemographics
                .AsQueryable()
                .IncludeTotalCount()
                .WhereIn(g => g.ID, GROUP1_ID, GROUP2_ID)
                .Execute();

            Assert.AreEqual(ctx.CustomerDemographics.Count, 2);
            Assert.AreEqual(ctx.CustomerDemographics.TotalCount, 2);
        }

        [Test]
        public void T3_AddCustomerGroupsToCustomer()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == CUSTOMER_ID);

            Assert.IsEmpty(customer.CustomerDemographics.ToArray());
            Assert.AreEqual(customer.CustomerDemographics.Count, 0);

            var customerGroup = ctx.CustomerDemographics
                .AsQueryable()
                .IncludeTotalCount()
                .Single(g => g.ID == GROUP2_ID);

            customer.CustomerDemographics.Add(customerGroup);

            Assert.IsNotEmpty(customer.CustomerDemographics.ToArray());
            Assert.AreEqual(customer.CustomerDemographics.Count, 1);
            Assert.AreSame(customer.CustomerDemographics.First(), customerGroup);
            Assert.Contains(customerGroup, customer.CustomerDemographics.ToArray());

            ctx.SaveChanges();
        }

        [Test]
        public void T4_CheckCustomerGroupsOfCustomer()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == CUSTOMER_ID);

            Assert.IsNotEmpty(customer.CustomerDemographics.ToArray());
            Assert.AreEqual(customer.CustomerDemographics.Count, 1);

            var customerGroup = ctx.CustomerDemographics
                .AsQueryable()
                .IncludeTotalCount()
                .Single(g => g.ID== GROUP2_ID);

            Assert.AreSame(customer.CustomerDemographics.First(), customerGroup);
            Assert.Contains(customerGroup, customer.CustomerDemographics.ToArray());
        }

        [Test]
        public void T5_RemoveCustomerGroupsFromCustomer()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == CUSTOMER_ID);

            Assert.IsNotEmpty(customer.CustomerDemographics.ToArray());
            Assert.IsNotEmpty(ctx.CustomerDemographics.ToArray());

            customer.CustomerDemographics.Clear();

            Assert.IsEmpty(customer.CustomerDemographics.ToArray());
            Assert.IsNotEmpty(ctx.CustomerDemographics.ToArray());

            ctx.SaveChanges();
        }

        [Test]
        public void T6_CheckCustomerGroupsOfCustomer()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == CUSTOMER_ID);

            Assert.IsEmpty(customer.CustomerDemographics.ToArray());
            Assert.IsEmpty(ctx.CustomerDemographics.ToArray());
        }

        [Test]
        public void T7_DeleteAllCustomerDemographics()
        {
            DeleteAllCustomerDemographics();
        }

        [Test]
        public void T8_CheckCustomerDemographics()
        {
            var ctx = new NorthwindDataContext();

            // load existing customer groups
            var result = ctx.CustomerDemographics
                .AsQueryable()
                .WhereIn(g => g.ID, GROUP1_ID, GROUP2_ID)
                .ToArray();

            Assert.IsEmpty(result);
            Assert.IsEmpty(ctx.CustomerDemographics.ToArray());
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
