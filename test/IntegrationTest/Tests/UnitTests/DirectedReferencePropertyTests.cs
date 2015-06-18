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
    public class DirectedReferencePropertyTests : AssertionHelper
    {
        private readonly int PROD_ID = 1; // Chai
        private readonly int CAT_OLD_ID = 1; // Beverages
        private readonly int CAT_NEW_ID = 3; // Confections

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T1_RetrieveModifyAndSaveData()
        {
            var ctx = Default.DataContext();

            var product = ctx.Products.AsQueryable().Include("Category").Single(p => p.ProductID == PROD_ID);
            var oldCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_OLD_ID);
            var newCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_NEW_ID);

            Assert.AreEqual(product.CategoryID, CAT_OLD_ID);
            Assert.AreSame(product.Category, oldCategory);

            product.Category = newCategory;

            Assert.AreEqual(product.CategoryID, CAT_NEW_ID);
            Assert.AreSame(product.Category, newCategory);

            ctx.SaveChanges();
        }

        [Test]
        public void T2_RetrieveAndResetModifyedData()
        {
            var ctx = Default.DataContext();

            var product = ctx.Products.AsQueryable().Include("Category").Single(p => p.ProductID == PROD_ID);
            var newCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_NEW_ID);

            Assert.AreEqual(product.CategoryID, CAT_NEW_ID);
            Assert.AreSame(product.Category, newCategory);

            product.CategoryID = CAT_OLD_ID;

            Assert.AreEqual(product.CategoryID, CAT_OLD_ID);
            //Assert.AreSame(product.Category, oldCategory);

            ctx.SaveChanges();
        }

        [Test]
        public void T3_RetrieveAndCheckModifyedData()
        {
            var ctx = Default.DataContext();

            var product = ctx.Products.AsQueryable().Include("Category").Single(p => p.ProductID == PROD_ID);
            var oldCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_OLD_ID);

            Assert.AreEqual(product.CategoryID, CAT_OLD_ID);
            Assert.AreSame(product.Category, oldCategory);
        }

        //[TearDown]
        //public void ResetData()
        //{
        //}
    }
}
