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
    public class DeleteOrderTests : AssertionHelper
    {
        [Test]
        public void DeleteExecutionOrderForRelatedEntities()
        {
            var ctx = Default.DataContext();
            
            var prodcount = ctx.Products.AsQueryable().Count();

            //create new category and new supplier for a new product and save together   
            var newCategory = ctx.Categories.CreateNew(false);
            var newSupplier = ctx.Suppliers.CreateNew(false);
            var newProduct = ctx.Products.CreateNew(false);

            newCategory.CategoryName = "NewCategory";
            newCategory.Description = "NewCategory for Test Description";

            Expect(newCategory.HasChanges, Is.True);
            Expect(newCategory.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            newSupplier.CompanyName = "TestCompanyName";
            newSupplier.Region = "TestRegion";
            newSupplier.PostalCode = "007007";
            newSupplier.Phone = "1234567890";
            newSupplier.ContactName = "TestContact";
            newSupplier.ContactTitle = "TestTitle";
            newSupplier.City = "TestCity";
            newSupplier.Address = "TestAdress";
            newSupplier.Country = "TestCountry";

            Expect(newSupplier.HasChanges, Is.True);
            Expect(newSupplier.ChangeTracker.State, Is.EqualTo(ObjectState.Added));

            newProduct.ProductName = "TestName";
            newProduct.QuantityPerUnit = "2";
            newProduct.UnitPrice = 33;
            newProduct.Discontinued = false;
            newProduct.ReorderLevel = 0;
            newProduct.UnitsInStock = 3;
            newProduct.UnitsOnOrder = 0;
            newProduct.Category = newCategory;
            newProduct.Supplier = newSupplier;

            ctx.Add(newCategory);
            ctx.Add(newSupplier);
            ctx.Add(newProduct);

            //save all in one call   
            ctx.SaveChanges();

            // check existance in entity set   
            var catElement = ctx.Categories.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newCategory));
            Expect(catElement, Is.Not.Null);
            var suppElement = ctx.Suppliers.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newSupplier));
            Expect(suppElement, Is.Not.Null);
            var prodElement = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newProduct));
            Expect(prodElement, Is.Not.Null);

            //delete created entities   
            ctx.Delete(newProduct);
            ctx.Delete(newSupplier);
            ctx.Delete(newCategory);

            //save deletes in one call (execution order must match for delete to be successful)   
            //have to supply a secret to make the delete pass thru clientinfo test logic in service    
            var clientInfo = new ClientInfo();
            clientInfo["ForceDelete"] = "yes please";


            ServerValidationException serverValidationException = null;
            try
            {
                ctx.SaveChanges(clientInfo: clientInfo);
            }
            catch (ServerValidationException ex)
            {
                serverValidationException = ex;
                throw;
            }
            catch (UpdateException ex)
            {
                foreach (var se in ex.StateEntries)
                {
                    string error = string.Empty;
                    if (se.Entity != null)
                    {
                        foreach (var er in se.Entity.Errors)
                        {
                            error += er.Message;
                        }
                    }
                    if (se.StoreValue != null)
                    {
                        foreach (var er in se.StoreValue.Errors)
                        {
                            error += er.Message;
                        }
                    }
                    Expect(error.Length == 0, String.Format("Concatenated Errors from StateEntities: {0}", error));
                }

                throw;
            }

            // check existance in entity set   
            catElement = ctx.Categories.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newCategory));
            Expect(catElement, Is.Null);
            suppElement = ctx.Suppliers.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newSupplier));
            Expect(suppElement, Is.Null);
            prodElement = ctx.Products.GetAllEntities().FirstOrDefault(p => object.ReferenceEquals(p, newProduct));
            Expect(prodElement, Is.Null);
        }
    }
}
