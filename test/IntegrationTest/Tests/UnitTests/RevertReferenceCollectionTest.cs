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
    public class RevertReferenceCollectionTest : AssertionHelper
    {
        // TODO

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
        public void T0_SetupTestData()
        {
            DeleteAllCustomerDemographics();

            var ctx = Default.DataContext();

            ctx.DemographicGroups.Add(new DemographicGroup { CustomerTypeID = GROUP1_ID, CustomerDesc = GROUP1_DESC });
            ctx.DemographicGroups.Add(new DemographicGroup { CustomerTypeID = GROUP2_ID, CustomerDesc = GROUP2_DESC });

            ctx.SaveChanges();
        }

        private void DeleteAllCustomerDemographics()
        {
            var ctx = Default.DataContext();

            // load existing customer groups
            ctx.DemographicGroups
                .AsQueryable()
                .WhereIn(g => g.CustomerTypeID, GROUP1_ID, GROUP2_ID)
                .Execute();

            // delete all existing
            if (ctx.DemographicGroups.Any())
            {
                ctx.DemographicGroups.DeleteAll();
                ctx.SaveChanges();
            }
        }


        [Test]
        public void T1_RevertReferenceCollection()
        {
            var ctx = Default.DataContext();

            // get data
            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == CUSTOMER_ID);

            var customerGroup1 = ctx.DemographicGroups.AsQueryable().Single(g => g.CustomerTypeID == GROUP1_ID);
            var customerGroup2 = ctx.DemographicGroups.AsQueryable().Single(g => g.CustomerTypeID == GROUP2_ID);

            Expect(customer, Is.Not.Null);
            Expect(customerGroup1, Is.Not.Null);
            Expect(customerGroup2, Is.Not.Null);

            Expect(customer.CustomerDemographics, Is.Empty);
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(0));

            Expect(customer.HasChanges, Is.False);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.False);
            Expect(customerGroup2.HasChanges, Is.False);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(customerGroup2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // add one group
            customer.CustomerDemographics.Add(customerGroup1);

            Expect(customer.CustomerDemographics, Is.Not.Empty);
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(1));

            Expect(customerGroup1.Customers, Is.Not.Empty);
            Expect(customerGroup1.Customers.Count, Is.EqualTo(1));

            Expect(customer.HasChanges, Is.True);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.True);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Assert.Contains(customerGroup1, customer.CustomerDemographics.ToArray());
            Assert.Contains(customer, customerGroup1.Customers.ToArray());


            // revert on customer
            customer.RevertChanges();

            Expect(customer.CustomerDemographics, Is.Empty);
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(0));

            Expect(customerGroup1.Customers, Is.Empty);
            Expect(customerGroup1.Customers.Count, Is.EqualTo(0));


            // add two groups
            customer.CustomerDemographics.Add(customerGroup1);
            customer.CustomerDemographics.Add(customerGroup2);

            Expect(customer.CustomerDemographics, Is.Not.Empty);
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(2));

            Expect(customerGroup1.Customers, Is.Not.Empty);
            Expect(customerGroup1.Customers.Count, Is.EqualTo(1)); ;

            Expect(customerGroup2.Customers, Is.Not.Empty);
            Expect(customerGroup2.Customers.Count, Is.EqualTo(1));

            Expect(customer.HasChanges, Is.True);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.True);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup2.HasChanges, Is.True);
            Expect(customerGroup2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Assert.Contains(customerGroup1, customer.CustomerDemographics.ToArray());
            Assert.Contains(customerGroup2, customer.CustomerDemographics.ToArray());
            Assert.Contains(customer, customerGroup1.Customers.ToArray());
            Assert.Contains(customer, customerGroup2.Customers.ToArray());


            // revert on one group
            customerGroup1.RevertChanges();

            Expect(customer.CustomerDemographics, Is.Not.Empty);
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(1));

            Expect(customerGroup1.Customers, Is.Empty);
            Expect(customerGroup1.Customers.Count, Is.EqualTo(0)); ;

            Expect(customerGroup2.Customers, Is.Not.Empty);
            Expect(customerGroup2.Customers.Count, Is.EqualTo(1));

            Expect(customer.HasChanges, Is.True);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.False);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup2.HasChanges, Is.True);
            Expect(customerGroup2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Assert.Contains(customerGroup2, customer.CustomerDemographics.ToArray());
            Assert.Contains(customer, customerGroup2.Customers.ToArray());


            // accept changes on customer
            customer.AcceptChanges();

            Expect(customer.HasChanges, Is.False);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.False);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup2.HasChanges, Is.True); // note: changes on customer group have not yet been accepted
            Expect(customerGroup2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // revert on group
            customerGroup2.RevertChanges();

            Expect(customer.HasChanges, Is.True); // note: since chage has been accepted, this revert is now a change
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup1.HasChanges, Is.False);
            Expect(customerGroup1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(customerGroup2.HasChanges, Is.False); 
            Expect(customerGroup2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
        }

        [Test]
        public void T2_RemoveTestData()
        {
            DeleteAllCustomerDemographics();
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
