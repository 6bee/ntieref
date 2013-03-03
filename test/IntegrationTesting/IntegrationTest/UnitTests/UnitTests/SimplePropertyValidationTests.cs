using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NUnit.Framework;
using NTier.Common.Domain.Model;

namespace UnitTests
{
    [TestFixture]
    public class SimplePropertyValidationTests : AssertionHelper
    {
        private readonly string Name = Guid.NewGuid().ToString();
        private readonly Product Product = new Product();

        [SetUp]
        public void Init()
        {
            // note: tracking needs to be enabled in order to perform validation
            Product.StartTracking();
        }

        [Test]
        public void T1_SetProperty()
        {
            Product.ProductName = Name;
        }

        [Test]
        public void T2_CheckProperty()
        {
            Expect(Product.ProductName, Is.EqualTo(Name));
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void T3_TrySetPropertyToNull()
        {
            // note: as ProductName is marked as required it must not be possible to set it to null
            Product.ProductName = null;
        }

        [Test]
        public void T4_CheckPropertyAgain()
        {
            Expect(Product.ProductName, Is.EqualTo(Name));
        }
    }
}
