using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProductManager.Client.Domain;
using ProductManager.Common.Domain.Model.ProductManager;
using NTier.Client.Domain;

namespace ProductManager.WPF.Applications.Services
{
    public interface IEntityService
    {
        IEntitySet<ProductCategory> Categories { get; }
        IEntitySet<Product> Products { get; }
    }
}
