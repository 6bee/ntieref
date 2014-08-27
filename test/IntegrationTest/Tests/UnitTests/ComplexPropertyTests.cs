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
    public class ComplexPropertyTests : AssertionHelper
    {
        private static void SetOriginalData(Customer customer)
        {
            customer.CompanyName = "Alfreds Futterkiste";
            customer.Contact.City = "Berlin";
            customer.Contact.Country = "Germany";
            customer.Contact.Name = "Maria Anders";
            customer.Contact.Title = "Sales Representative";
        }

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T0_InitializeData()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");
            SetOriginalData(customer);

            ctx.SaveChanges();
        }

        [Test]
        public void T1_SetNewValueObject()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            customer.Contact = new Contact();

            ctx.SaveChanges();
        }

        [Test]
        public void T2_CheckEmptyValues()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            Expect(customer.Contact.City, Is.Null);
        }

        [Test]
        public void T3_SetOriginalValues()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            SetOriginalData(customer);

            ctx.SaveChanges();
        }


        [Test]
        public void T4_CheckCorrectValues()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            Expect(customer.Contact.City, Is.EqualTo("Berlin"));
        }


        [Test]
        public void T5_SetNullAndCheck()
        {
            var ctx = Default.DataContext();

            var customer = ctx.Customers.CreateNew();

            Expect(customer.Contact, Is.Not.Null);

            customer.Contact = null;

            Expect(customer.Contact, Is.Null);
        }
    }
}
