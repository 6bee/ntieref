using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model;
using ProductManager.Common.Domain.Model.ProductManager;

namespace ProductManager.Client.Domain.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new ProductManagerDataContext();
            var data = ctx.Products.AsQueryable().Take(5).ToList();
            data.First().ListPrice++;
            ctx.SaveChanges();

            //var category = new ProductCategory
            //{
            //    Name = "Test Category 1",
            //};

            //var product = new Product
            //{
            //    ProductID=1111,
            //    Name = "Test Product 1",
            //    ProductNumber = "Test Product 1",
            //    SellStartDate = DateTime.Now.Date,
            //};

            //product.ProductCategory = category;

            //ctx.Add(product);

            //ctx.SaveChanges();
        }
    }
}
