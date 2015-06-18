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
    public class RevertDirectedReferenceTest : AssertionHelper
    {
        private const int PROD_ID = 1; // Chai
        private const int CAT_OLD_ID = 1; // Beverages
        private const int CAT_NEW_ID = 3; // Confections


        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T1_RevertDirectedReference()
        {
            var ctx = Default.DataContext();

            // get data
            var product = ctx.Products
                .AsQueryable()
                .Include("Category")
                .Single(p => p.ProductID == PROD_ID);

            var oldCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_OLD_ID);
            var newCategory = ctx.Categories.AsQueryable().Single(c => c.CategoryID == CAT_NEW_ID);

            // check initial state
            Expect(product.CategoryID, Is.EqualTo(CAT_OLD_ID));
            Expect(product.Category, Is.EqualTo(oldCategory));

            Expect(product.HasChanges, Is.False);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set new
            product.Category = newCategory;

            Expect(product.CategoryID, Is.EqualTo(CAT_NEW_ID));
            Expect(product.Category, Is.EqualTo(newCategory));

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Modified));


            // revert to old
            product.RevertChanges();

            Expect(product.CategoryID, Is.EqualTo(CAT_OLD_ID));
            Expect(product.Category, Is.EqualTo(oldCategory));

            Expect(product.HasChanges, Is.False);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set null
            product.Category = null;

            Expect(product.CategoryID, Is.Null);
            Expect(product.Category, Is.Null);

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Modified));


            // revert to old
            product.RevertChanges();

            Expect(product.CategoryID, Is.EqualTo(CAT_OLD_ID));
            Expect(product.Category, Is.EqualTo(oldCategory));

            Expect(product.HasChanges, Is.False);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set null and accept
            product.Category = null;
            product.AcceptChanges();

            Expect(product.CategoryID, Is.Null);
            Expect(product.Category, Is.Null);

            Expect(product.HasChanges, Is.False);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set new
            product.Category = newCategory;

            Expect(product.CategoryID, Is.EqualTo(CAT_NEW_ID));
            Expect(product.Category, Is.EqualTo(newCategory));

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Modified));


            // revert to null
            product.RevertChanges();

            Expect(product.CategoryID, Is.Null);
            Expect(product.Category, Is.Null);

            Expect(product.HasChanges, Is.False);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));
        }

        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
