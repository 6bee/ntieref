using System;
using ProductManager.Common.Domain.Model.ProductManager;
using NTier.Server.Domain.Service;

namespace ProductManager.Server.Domain.Service
{
    partial class ProductManagerDataService
    {
        [NTier.Server.Domain.Service.ChangeInterceptor(typeof(Product))]
        public void OnChangeProducts(Product product, UpdateOperations operation)
        {
            //// only admins may change data
            // this functionality is already provided by PrincipalPermissionAttribute on service operation
            //var principalPermission = new PrincipalPermission(null, "Administrators");
            //principalPermission.Demand();

            // set row guid
            if (operation == UpdateOperations.Add)
            {
                if (product.rowguid == Guid.Empty)
                {
                    product.rowguid = Guid.NewGuid();
                }
            }

            // set mofification time stamp
            if (operation == UpdateOperations.Add || operation == UpdateOperations.Change)
            {
                product.ModifiedDate = DateTime.Now;
            }
        }

        [NTier.Server.Domain.Service.ChangeInterceptor(typeof(ProductCategory))]
        public void OnChangeProductCategory(ProductCategory category, UpdateOperations operation)
        {
            //// only admins may change data
            // this functionality is already provided by PrincipalPermissionAttribute on service operation
            //var principalPermission = new PrincipalPermission(null, "Administrators");
            //principalPermission.Demand();

            // set row guid
            if (operation == UpdateOperations.Add)
            {
                if (category.rowguid == Guid.Empty)
                {
                    category.rowguid = Guid.NewGuid();
                }
            }

            // set mofification time stamp
            if (operation == UpdateOperations.Add || operation == UpdateOperations.Change)
            {
                category.ModifiedDate = DateTime.Now;
            }
        }
    }
}
