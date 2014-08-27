using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using IntegrationTest.Client.Domain;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new NorthwindDataContext();
            var employee = ctx.Employees.AsQueryable().ToList();

            //var ctx = new NorthwindDataContext();
            //var products = ctx.Products
            //    .AsQueryable()
            //    .Include("Category")
            //    .IncludeTotalCount()
            //    .OrderBy(p => p.ProductID)
            //    .Skip(20)
            //    .ToList();

            //var localCount = ctx.Products.Count;
            //var serverCount = ctx.Products.TotalCount;


            //var customer = ctx.Customers.AsQueryable().Single(c => c.CustomerID == "ALFKI");

            //customer.Contact = new IntegrationTest.Common.Domain.Model.Northwind.Contact();
            //ctx.SaveChanges();

            //customer.CompanyName = "Alfreds Futterkiste";
            //customer.Contact.City = "Berlin";
            //customer.Contact.Country = "Germany";
            //customer.Contact.Name = "Maria Anders";
            //customer.Contact.Title = "Sales Representative";
            //ctx.SaveChanges();
        }
    }
}
