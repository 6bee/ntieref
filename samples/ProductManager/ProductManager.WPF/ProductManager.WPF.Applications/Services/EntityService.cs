using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using NTier.Client.Domain;

namespace ProductManager.WPF.Applications.Services
{
    [Export(typeof(IEntityService)), Export]
    public class EntityService : IEntityService
    {
        private ProductManagerDataContext context;

        public ProductManagerDataContext Context
        {
            get { return context; }
            set { context = value; }
        }

        public IEntitySet<Product> Products
        {
            get
            {
                return context.Products;
            }
        }


        public IEntitySet<ProductCategory> Categories
        {
            get
            {
                return context.ProductCategories;
            }
        }
    }
}
