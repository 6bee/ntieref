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
    public class EditableEntityTests : AssertionHelper
    {
        //[SetUp]
        //public void Init()
        //{
        //}


        [Test]
        public void T0_ModifyProperties()
        {
            var price1 = 12.345M;
            var name1 = "Name1";
            var name2 = "Name2";

            var ctx = new NorthwindDataContext();

            var product = ctx.Products.CreateNew();
            product.UnitPrice = price1;
            product.ProductName = name1;
            Expect(product.HasChanges, Is.True);

            product.AcceptChanges();

            Expect(product.HasChanges, Is.False);
            Expect(product.UnitPrice, Is.EqualTo(price1));
            Expect(product.ProductName, Is.EqualTo(name1));


            product.RevertChanges();

            Expect(product.HasChanges, Is.False);
            Expect(product.UnitPrice, Is.EqualTo(price1));
            Expect(product.ProductName, Is.EqualTo(name1));

            ((IEditable)product).BeginEdit();
            product.ProductName = name2;

            Expect(product.HasChanges, Is.True);
            Expect(product.ProductName, Is.EqualTo(name2));

            ((IEditable)product).CancelEdit();
            Expect(product.HasChanges, Is.False);
            Expect(product.ProductName, Is.EqualTo(name1));


            ((IEditable)product).BeginEdit();
            product.ProductName = name2;

            Expect(product.HasChanges, Is.True);
            Expect(product.ProductName, Is.EqualTo(name2));

            ((IEditable)product).EndEdit();
            Expect(product.HasChanges, Is.True);
            Expect(product.ProductName, Is.EqualTo(name2));


            product.RevertChanges();

            Expect(product.HasChanges, Is.False);
            Expect(product.ProductName, Is.EqualTo(name1));
        }

        [Test]
        public void T1_AddReferences()
        {
            var ctx = new NorthwindDataContext();

            // create supplier
            var supplier1 = ctx.Suppliers.CreateNew();
            supplier1.AcceptChanges();


            // create products
            var product1 = ctx.Products.CreateNew();
            var product2 = ctx.Products.CreateNew();
            var product3 = ctx.Products.CreateNew();


            // set supplier reference (existing supplier)
            product1.Supplier = supplier1;

            Expect(product1.HasChanges, Is.True); // product has changes 
            Expect(supplier1.HasChanges, Is.True); // supplier has changes 
            Expect(supplier1.Products.Count, Is.EqualTo(1));


            // begin edit of suplier
            var editableSuplier = supplier1 as IEditable;

            Expect(editableSuplier, Is.Not.Null);
            Expect(editableSuplier.CanEdit, Is.True);
            Expect(editableSuplier.CanCancelEdit, Is.False);
            Expect(editableSuplier.IsEditing, Is.False);

            editableSuplier.BeginEdit();

            Expect(editableSuplier.CanEdit, Is.True);
            Expect(editableSuplier.CanCancelEdit, Is.True);
            Expect(editableSuplier.IsEditing, Is.True);



            // set different supplier 
            product2.Supplier = supplier1;
            Expect(product2.Supplier, Is.EqualTo(supplier1));
            Expect(supplier1.Products.Count, Is.EqualTo(2));


            // cancel edit of suplier
            editableSuplier.CancelEdit();
            Expect(product2.Supplier, Is.Null);
            Expect(supplier1.Products.Count, Is.EqualTo(1));



            // re-begin edit of suplier
            editableSuplier.BeginEdit();
            Expect(supplier1.HasChanges, Is.True); // supplier still has changes 



            // set different supplier 
            product3.Supplier = supplier1;
            Expect(product3.Supplier, Is.EqualTo(supplier1));
            Expect(supplier1.Products.Count, Is.EqualTo(2));



            // end edit of suplier
            editableSuplier.EndEdit();
            Expect(product3.Supplier, Is.EqualTo(supplier1));
            Expect(supplier1.Products.Count, Is.EqualTo(2));
        }


        //[Test]
        //public void DeleteAddedEntity()
        //{
        //    var ctx = new NorthwindDataContext();
            
        //    // create and add new product to context
        //    var product = new Product();
        //    ctx.Products.Add(product);

        //    Expect(product.HasChanges, Is.True);
        //    Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            
        //    // check existance in entity set
        //    var element = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, product));
        //    Expect(element, Is.Not.Null);


        //    // delete product
        //    ctx.Products.Delete(product);

        //    Expect(product.HasChanges, Is.True);
        //    Expect(product.ChangeTracker.State, Is.EqualTo(ObjectState.Added));


        //    // check existance in entity set (including deleted entities)
        //    element = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, product));
        //    Expect(element, Is.Null);
        //}


        //[TearDown]
        //public void CleanUp()
        //{
        //}
    }
}
