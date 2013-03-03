using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductManager.WebServiceClient.ProductManagerDataService;

namespace ProductManager.WebServiceClient
{
    /// <summary>
    /// This demo code shows how to access an n-tier entity framework data service without using client components.
    /// It is strongly sugested using the framework's client components which provide rich client side functionality!
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new ProductManagerDataServiceClient("WSHttpBinding_IProductManagerDataService"))
            {
                // Compose query (p => p.ProductID == 680)
                var parameterExpression = new ParameterExpression
                {
                    ParameterName = "p",
                    ParameterType = "ProductManager.Common.Domain.Model.ProductManager.Product",
                };
                var propertyExpression = new PropertyAccessExpression
                {
                    DeclaringType = "ProductManager.Common.Domain.Model.ProductManager.Product",
                    Instance = parameterExpression,
                    PropertyName = "ProductID",
                    PropertyType = typeof(int).FullName,
                };
                var idExpression = new ConstantValueExpression
                {
                    Value = 680,
                };
                var filterExpression = new BinaryExpression
                {
                    LeftOperand = propertyExpression,
                    Operator = BinaryOperator.Equal,
                    RightOperand = idExpression,
                };
                var query = new Query
                {
                    FilterExpressionList = new[] { filterExpression },
                    IncludeData = true,
                    IncludeTotalCount = true,
                };

                // Retrieve data from service
                var result = service.GetProducts(query: query, clientInfo: null);
                var product = result.Data.SingleOrDefault();
                var productCount = result.TotalCount;

                // Modify data
                product.Color = "Silver";
                product.ChangeTracker.ModifiedProperties = new[] { "Color" };
                product.ChangeTracker.State = ObjectState.Modified;

                // Create change set
                var changeSet = new ProductManagerChangeSet { Products = new[] { product } };

                // Submit modification synchronously
                var submissionResult = service.SubmitChanges(changeSet: changeSet, clientInfo: null);
                var productRefreshData = submissionResult.Products.SingleOrDefault();

                // Submit modification asynchronously
                service.SubmitChangesCompleted += new EventHandler<SubmitChangesCompletedEventArgs>(Service_SubmitChangesCompleted);
                service.SubmitChangesAsync(changeSet: changeSet, clientInfo: null);
            }
        }

        private static void Service_SubmitChangesCompleted(object sender, SubmitChangesCompletedEventArgs e)
        {
            var result = e.Result;
            var productRefreshData = result.Products.SingleOrDefault();
        }
    }
}
