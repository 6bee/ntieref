using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Model;

namespace IntegrationTest.Client.Domain.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new NorthwindDataContext();
            var products = ctx.Products.AsQueryable().ToList();
        }
    }
}
