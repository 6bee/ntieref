using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformaceTests.Framework;
using IntegrationTest.Common.Domain.Model.Northwind;

namespace PerformaceTests.Tests
{
    class InternalEntitySetTests
    {
        const int NUMBER_OF_ENTITIES = 16000;
        readonly Product[] _products;

        public InternalEntitySetTests()
        {
            var products = new List<Product>();

            for (int i = 0; i < NUMBER_OF_ENTITIES; i++)
            {
                var product = new Product { ProductID = i, ProductName = "Prod #" + i };
                product.AcceptChanges();

                products.Add(product);
            }

            _products = products.ToArray();
        }

        readonly NTier.Client.Domain.InternalEntitySet<Product> set = new NTier.Client.Domain.InternalEntitySet<Product>();
        int c = 0;


        [Test(NumberOfRepetes = NUMBER_OF_ENTITIES)]
        void AttachOne()
        {
            set.Attach(_products[c]);
            c++;
        }


        [Test(NumberOfRepetes = 3)]
        void Clear()
        {
            set.DetachAll();
        }


        //[Test(NumberOfRepetes = 3)]
        //void AttachAll()
        //{
        //    foreach (var product in _products)
        //    {
        //        set.Attach(product);
        //    }
        //}


        //[Test(NumberOfRepetes = 3)]
        //void AttachRemoveReAttach()
        //{
        //    int count = _products.Count();

        //    for (int i = 0; i < count; i++)
        //    {
        //        set.Attach(_products[i]);
        //    }

        //    for (int i = 0; i < count; i += 2)
        //    {
        //        set.Detach(_products[i]);
        //    }

        //    for (int i = 0; i < count; i += 2)
        //    {
        //        set.Attach(_products[i]);
        //    }
        //}


        //[Test(NumberOfRepetes = 3)]
        //void AttachRemoveReAttach2()
        //{
        //    int count = _products.Count();

        //    for (int i = 0; i < count; i++)
        //    {
        //        set.Attach(_products[i]);
        //    }

        //    for (int i = 0; i < count; i += 2)
        //    {
        //        set.Detach(_products[i]);
        //    }

        //    for (int i = 0; i < count; i++)
        //    {
        //        set.Attach(_products[i]);
        //    }
        //}
    }
}
