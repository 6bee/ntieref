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
    public class ComplexPropertyValidationTests : AssertionHelper
    {
        private static readonly StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(10) { MinimumLength = 8 };

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
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");
            SetOriginalData(customer);

            ctx.SaveChanges();
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void T1_SetToLongString()
        {
            // add dynamic validator
            Customer.RegisterValidator("Contact.City", StringLengthAttribute);


            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            //customer.CompanyName = null;
            customer.Contact.City = "12345678901";
            //customer.Contact.Country = null;
            //customer.Contact.Name = null;
            //customer.Contact.Title = null;
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void T2_SetToShortString()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            customer.Contact.City = "1234567";
        }


        [Test]
        public void T3_SetValidString()
        {
            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            customer.Contact.City = null;
            customer.Contact.City = "12345678";
            customer.Contact.City = "123456789";
            customer.Contact.City = "1234567890";
        }


        [Test]
        public void T4_RemoveValidator()
        {
            // remove dynamic validator
            Customer.UnregisterValidator("Contact.City", StringLengthAttribute);

            var ctx = new NorthwindDataContext();

            var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            customer.Contact.City = null;
            customer.Contact.City = "";
            customer.Contact.City = "1";
            customer.Contact.City = "1234567";
            customer.Contact.City = "12345678";
            customer.Contact.City = "123456789";
            customer.Contact.City = "1234567890";
            customer.Contact.City = "12345678901";
            customer.Contact.City = "1234567890123456789012345678901234567890";
        }
    }
}
