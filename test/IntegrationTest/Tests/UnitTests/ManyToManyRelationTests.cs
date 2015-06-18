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
    public class ManyToManyRelationTests : AssertionHelper
    {
        private static readonly object[] TestCases;

        static ManyToManyRelationTests()
        {
            var male = new DemographicGroup { CustomerTypeID = "DemoGroup5", CustomerDesc = "Male" };
            var female = new DemographicGroup { CustomerTypeID = "DemoGroup6", CustomerDesc = "Female" };

            var smoker = new DemographicGroup { CustomerTypeID = "DemoGroup7", CustomerDesc = "Smoker" };
            var noSmoker = new DemographicGroup { CustomerTypeID = "DemoGroup8", CustomerDesc = "NoSmoker" };

            var undefined = new DemographicGroup { CustomerTypeID = "DemoGroup9", CustomerDesc = "Undefined" };

            var customerDemographics = new DemographicGroup[] { male, female, smoker, noSmoker, undefined };

            var customer5 = new Customer { CustomerID = "CUST5", CompanyName = "Test Customer 5" };
            var customer6 = new Customer { CustomerID = "CUST6", CompanyName = "Test Customer 6" };
            var customer7 = new Customer { CustomerID = "CUST7", CompanyName = "Test Customer 7" };
            var customer8 = new Customer { CustomerID = "CUST8", CompanyName = "Test Customer 8" };

            customer5.CustomerDemographics.Add(male);
            customer5.CustomerDemographics.Add(smoker);

            customer6.CustomerDemographics.Add(male);
            customer6.CustomerDemographics.Add(noSmoker);

            customer7.CustomerDemographics.Add(female);

            TestCases = new object[]
            {
                new object[] { 
                    new Customer[] { customer5 }, 
                    new DemographicGroup[0]
                },
                new object[] { 
                    new Customer[] { customer6, customer7 }, 
                    customerDemographics
                },
                new object[] { 
                    new Customer[] { customer7, customer8 }, 
                    customerDemographics
                }
            };
        }

        [Test, TestCase("CUST0", "Test Customer 0", "DemoGroup0", "Test Demographic Group 0")]
        public void T1_CreateSingleRelation(string customerId, string customerName, string groupId, string groupDesc)
        {
            var ctx = Default.DataContext();

            var group = new DemographicGroup { CustomerTypeID = groupId, CustomerDesc = groupDesc };
            Expect(group.IsValid);

            var customer = new Customer { CustomerID = customerId, CompanyName = customerName };
            Expect(customer.IsValid);

            customer.CustomerDemographics.Add(group);

            Expect(customer.CustomerDemographics.Contains(group));
            Expect(group.Customers.Contains(customer));

            Expect(group.IsValid);
            Expect(customer.IsValid);

            ctx.Add(customer);

            ctx.SaveChanges();

            Expect(group.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
        }

        [Test]
        public void T2_DeleteSingleRelation()
        {
            var ctx = Default.DataContext();

            // re-query customer including demographics
            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == "CUST0");

            // check returned customer
            Expect(customer, Is.Not.Null);
            Expect(customer.CustomerID, Is.EqualTo("CUST0"));
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(1));
            Expect(customer.CustomerDemographics.First().CustomerTypeID, Is.EqualTo("DemoGroup0"));

            // check demographic group
            var group = customer.CustomerDemographics.First();
            Expect(group.Customers.Count, Is.EqualTo(1));
            Expect(group.Customers.First().CustomerID, Is.EqualTo("CUST0"));

            // check context (demographics)
            Expect(ctx.DemographicGroups.Count(), Is.EqualTo(1));
            Expect(ctx.DemographicGroups.Single().CustomerTypeID, Is.EqualTo("DemoGroup0"));
            Expect(object.ReferenceEquals(ctx.DemographicGroups.Single(), group));

            // check context (customers)
            Expect(ctx.Customers.Count(), Is.EqualTo(1));
            Expect(ctx.Customers.Single().CustomerID, Is.EqualTo("CUST0"));
            Expect(object.ReferenceEquals(ctx.Customers.Single(), customer));

            // remove relation
            customer.CustomerDemographics.Remove(group);

            // check relation was removed
            Expect(customer.CustomerDemographics.Count, Is.EqualTo(0));
            Expect(group.Customers.Count, Is.EqualTo(0));


            Expect(group.HasChanges, Is.True);
            Expect(customer.HasChanges, Is.True);
            Expect(group.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(group.IsValid);
            Expect(customer.IsValid);

            ctx.SaveChanges();

            Expect(group.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
        }

        [Test]
        public void T3_CascadeDeleteSingleRelation()
        {
            // create test data
            T1_CreateSingleRelation("CUST1", "Test Customer 1", "DemoGroup1", "Test Demographic Group 1");

            // retrieve and delete customer
            var ctx = Default.DataContext();
            // re-query customer including demographics
            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == "CUST1");

            var group = customer.CustomerDemographics.First();

            // check state before delete
            Expect(customer.HasChanges, Is.False);
            Expect(group.HasChanges, Is.False);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
            Expect(group.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            // retrieve and delete customer
            ctx.Delete(customer);

            // check cascade delete of relation
            Expect(group.Customers.Count, Is.EqualTo(0));

            // check state before delete
            Expect(customer.HasChanges, Is.True);
            Expect(group.HasChanges, Is.True);
            Expect(customer.ChangeTracker.State, Is.EqualTo(ObjectState.Deleted));
            Expect(group.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            // performe deletion on backend
            ctx.SaveChanges();

            // retieve demographic group
            ctx = Default.DataContext();
            group = ctx.DemographicGroups
                .AsQueryable()
                .Single(g => g.CustomerTypeID == "DemoGroup1");

            // check cascade delete of relation in backend
            Expect(group.Customers.Count, Is.EqualTo(0));
        }

        [Test, TestCaseSource("TestCases")]
        public void T4_CreateEntitiesWithMultipleRelations(IEnumerable<Customer> customers, IEnumerable<DemographicGroup> groups)
        {
            var ctx = Default.DataContext();

            foreach (var customer in customers)
            {
                ctx.Attach(customer);
            }

            foreach (var group in groups)
            {
                ctx.Attach(group);
            }

            ctx.SaveChanges();
        }

        [Test]
        [TestCase("CUST5", new string[] { "DemoGroup5", "DemoGroup7" })]
        [TestCase("CUST6", new string[] { "DemoGroup5", "DemoGroup8" })]
        [TestCase("CUST7", new string[] { "DemoGroup6" })]
        [TestCase("CUST8", new string[] { })]
        public void T5_ValidateEntitiesWithMultipleRelations(string customerId, string[] groupIds)
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers
                .AsQueryable()
                .Include("CustomerDemographics")
                .Single(c => c.CustomerID == customerId);

            Expect(customer.CustomerDemographics.Count, Is.EqualTo(groupIds.Length));

            foreach (var group in customer.CustomerDemographics)
            {
                Expect(groupIds, Contains(group.CustomerTypeID));
            }
        }

        [Test]
        [TestCase("DemoGroup5", new string[] { "CUST5", "CUST6" })]
        [TestCase("DemoGroup6", new string[] { "CUST7" })]
        [TestCase("DemoGroup7", new string[] { "CUST5" })]
        [TestCase("DemoGroup8", new string[] { "CUST6" })]
        [TestCase("DemoGroup9", new string[] { })]
        public void T6_ValidateEntitiesWithMultipleRelations(string groupId, string[] customerIds)
        {
            var ctx = Default.DataContext();

            var group = ctx.DemographicGroups
                .AsQueryable()
                .Include("Customers")
                .Single(c => c.CustomerTypeID == groupId);

            Expect(group.Customers.Count, Is.EqualTo(customerIds.Length));

            foreach (var customer in group.Customers)
            {
                Expect(customerIds, Contains(customer.CustomerID));
            }
        }

        [Test]
        public void T7_CheckCustomerExistance([Values("CUST5", "CUST6", "CUST7", "CUST8")] string id)
        {
            var ctx = Default.DataContext();

            var count = ctx.Customers
                .AsQueryable()
                .Count(c => c.CustomerID == id);

            Expect(count, Is.EqualTo(1));
        }

        [Test]
        public void T7_CheckDemographicsExistance([Values("DemoGroup5", "DemoGroup6", "DemoGroup7", "DemoGroup8", "DemoGroup9")] string id)
        {
            var ctx = Default.DataContext();

            var count = ctx.DemographicGroups
                .AsQueryable()
                .Count(g => g.CustomerTypeID == id);

            Expect(count, Is.EqualTo(1));
        }

        [Test]
        public void T8_DeleteAll()
        {
            var ctx = Default.DataContext();

            // load customers
            ctx.Customers
                .AsQueryable()
                .Where(c => c.CustomerID.StartsWith("CUST"))
                .Execute();

            // load demographics
            ctx.DemographicGroups
                .AsQueryable()
                .Where(g => g.CustomerTypeID.StartsWith("DemoGroup"))
                .Execute();

            // delete all data
            ctx.DemographicGroups.DeleteAll();
            ctx.Customers.DeleteAll();

            // save
            ctx.SaveChanges();
        }

        [Test]
        public void T9_CheckCustomerExistance([Values("CUST0", "CUST1", "CUST5", "CUST6", "CUST7", "CUST8")] string id)
        {
            var ctx = Default.DataContext();

            var count = ctx.Customers
                .AsQueryable()
                .Count(c => c.CustomerID == id);

            Expect(count, Is.EqualTo(0));
        }

        [Test]
        public void T9_CheckDemographicsExistance([Values("DemoGroup0", "DemoGroup1", "DemoGroup5", "DemoGroup6", "DemoGroup7", "DemoGroup8", "DemoGroup9")] string id)
        {
            var ctx = Default.DataContext();

            var count = ctx.DemographicGroups
                .AsQueryable()
                .Count(g => g.CustomerTypeID == id);

            Expect(count, Is.EqualTo(0));
        }
    }
}
