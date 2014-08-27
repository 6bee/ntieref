using IntegrationTest.Common.Domain.Model.Northwind;
using NTier.Common.Domain.Model;
using NTier.Server.Domain.Service;

namespace IntegrationTest.Server.Domain.Service
{
    partial class NorthwindDataService
    {
        // this code is for unit test  ServerValidationTests
        [NTier.Server.Domain.Service.ChangeInterceptor(typeof(Region))]
        public void OnChangeRegion(Region region, UpdateOperations operation)
        {
            // prevent regions from beeing modified
            region.Errors.Add(new Error("Region are not allowed to be modified"));
        }


        // this code is for unit test ClientInfoTests
        [NTier.Server.Domain.Service.ChangeInterceptor(typeof(Category))]
        public void OnChangeCategory(Category category, UpdateOperations operation, ClientInfo clientInfo)
        {
            // prevent category from beeing deleted
            if (operation == UpdateOperations.Delete)
            {
                if (clientInfo == null)
                {
                    category.Errors.Add(new Error("ClientInfo is null"));
                }
                else if (clientInfo == null || string.IsNullOrEmpty(clientInfo["TestString"] as string))
                {
                    category.Errors.Add(new Error("Test string could not be found in ClientInfo"));
                }
                else
                {
                    category.Errors.Add(new Error((string)clientInfo["TestString"]));
                }
            }
        }
    }
}
