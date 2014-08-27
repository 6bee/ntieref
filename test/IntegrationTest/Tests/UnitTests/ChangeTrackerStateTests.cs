using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using IntegrationTest.Client.Domain;
using NTier.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NTier.Common.Domain.Model;

namespace UnitTests
{
    [TestFixture]
    public class ChangeTrackerStateTests : AssertionHelper
    {
        private const int SUPPLIER1_ID = 1;
        private const int SUPPLIER2_ID = 2;
        private const int SUPPLIER3_ID = 3;
        private const int SUPPLIER4_ID = 4;

        //[SetUp]
        //public void Init()
        //{
        //}


        [Test]
        public void SetReferenceToAddedEntity()
        {
            var ctx = Default.DataContext();

            // load suppliers
            ctx.Suppliers.AsQueryable().WhereIn(s => s.SupplierID, SUPPLIER1_ID, SUPPLIER2_ID, SUPPLIER3_ID, SUPPLIER4_ID).Execute();

            var supplier1 = ctx.Suppliers.SingleOrDefault(s => s.SupplierID == SUPPLIER1_ID);
            var supplier2 = ctx.Suppliers.SingleOrDefault(s => s.SupplierID == SUPPLIER2_ID);
            var supplier3 = ctx.Suppliers.SingleOrDefault(s => s.SupplierID == SUPPLIER3_ID);
            var supplier4 = ctx.Suppliers.SingleOrDefault(s => s.SupplierID == SUPPLIER4_ID);

            Expect(supplier1, Is.Not.Null);
            Expect(supplier2, Is.Not.Null);
            Expect(supplier3, Is.Not.Null);
            Expect(supplier4, Is.Not.Null);


            // add new product
            var product = new Product();
            ctx.Products.Add(product);

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set supplier reference (existing supplier)
            product.Supplier = supplier1;

            Expect(product.HasChanges, Is.True); // product has changes (state is added)
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            Expect(supplier1.HasChanges, Is.True); // supplier1 has changes (state is unchanged)
            Expect(supplier1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(supplier2.HasChanges, Is.False); // supplier2 has no changes (state is unchanged)
            Expect(supplier2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set different supplier reference (existing supplier)
            product.Supplier = supplier2;

            Expect(product.HasChanges, Is.True); // product has changes (state is added)
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            Expect(supplier1.HasChanges, Is.False); // supplier1 has no changes anymore (state is unchanged)
            Expect(supplier1.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));

            Expect(supplier2.HasChanges, Is.True); // supplier2 has changes now (state is unchanged)
            Expect(supplier2.ChangeTracker.State, Is.EqualTo(ObjectState.Unchanged));


            // set different supplier reference (existing supplier)
            product.Supplier = supplier3;

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set different supplier reference (existing supplier)
            product.Supplier = supplier4;

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set supplier reference to null
            product.Supplier = null;

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set different supplier reference (existing supplier)
            product.Supplier = supplier4;

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // create and add new supplier to context (using factory method)
            var newSupplier1 = ctx.Suppliers.CreateNew();

            Expect(newSupplier1.HasChanges, Is.True);
            Expect(newSupplier1.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set different supplier reference (new supplier)
            product.Supplier = newSupplier1;

            Expect(product.HasChanges, Is.True);
            Expect(newSupplier1.HasChanges, Is.True); 
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));
            Expect(newSupplier1.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // create new supplier without adding it to context
            var newSupplier2 = new Supplier();

            Expect(newSupplier2.HasChanges, Is.True);
            Expect(newSupplier2.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // set different supplier reference (new supplier)
            product.Supplier = newSupplier2;

            Expect(product.HasChanges, Is.True);
            Expect(newSupplier2.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));
            Expect(newSupplier2.ChangeTracker.State, Is.EqualTo(ObjectState.Added));
        }


        [Test]
        public void DeleteAddedEntity()
        {
            var ctx = Default.DataContext();
            
            // create and add new product to context
            var product = new Product();
            ctx.Products.Add(product);

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            
            // check existance in entity set
            var element = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, product));
            Expect(element, Is.Not.Null);


            // delete product
            ctx.Products.Delete(product);

            Expect(product.HasChanges, Is.True);
            Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


            // check existance in entity set (including deleted entities)
            element = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, product));
            Expect(element, Is.Null);
        }


        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
