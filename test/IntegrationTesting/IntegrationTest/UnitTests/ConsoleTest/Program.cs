using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationTest.Client.Domain;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using UnitTests;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new NorthwindDataContext();
            var products = ctx.Products
                .AsQueryable()
                .Include("Category")
                .IncludeTotalCount()
                .OrderBy(p => p.ProductID)
                .Skip(20)
                .ToList();

            var localCount = ctx.Products.Count;
            var serverCount = ctx.Products.TotalCount;
        }
    }
}
