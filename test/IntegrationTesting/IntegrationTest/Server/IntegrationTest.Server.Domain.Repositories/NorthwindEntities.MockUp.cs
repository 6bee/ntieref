using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Server.Domain.Repositories;
using NTier.Common.Domain.Model;
using IntegrationTest.Common.Domain.Model.Northwind;

namespace IntegrationTest.Server.Domain.Repositories
{
    public partial class NorthwindEntitiesMockUp : IRepository, INorthwindEntities
    {
        public NorthwindEntitiesMockUp()
        {
            Categories = new EntitySet<Category>
            (
                new Category { CategoryID = 1 },
                new Category { CategoryID = 2 }
            );

            var territory1 = new Territory { TerritoryID = "06897" };
            var territory2 = new Territory { TerritoryID = "19713" };
            var employee1 = new Employee { EmployeeID = 1 };
            employee1.Territories.Add(territory1);
            employee1.Territories.Add(territory2);
            Employees = new EntitySet<Employee>
            (
                employee1
            );

            Orders = new EntitySet<Order>
            (
            );
            OrderDetails = new EntitySet<OrderDetail>
            (
            );
            Products = new EntitySet<Product>
            (
            );

            Regions = new EntitySet<Region>
            (
            );

            Shippers = new EntitySet<Shipper>
            (
            );

            Suppliers = new EntitySet<Supplier>
            (
                new Supplier { SupplierID = 1 },
                new Supplier { SupplierID = 2 },
                new Supplier { SupplierID = 3 },
                new Supplier { SupplierID = 4 }
            );

            Territories = new EntitySet<Territory>
            (
                territory1,
                territory2
            );

            CustomerDemographics = new EntitySet<DemographicGroup>
            (
            );

            Customers = new EntitySet<Customer>
            (
            );

            DynamicContentEntities = new EntitySet<DynamicContentEntity>
            (
            );
        }


        #region IRepository

        public void Refresh(System.Data.Objects.RefreshMode refreshMode, NTier.Common.Domain.Model.Entity entity)
        {
        }

        public void Refresh(System.Data.Objects.RefreshMode refreshMode, IEnumerable<NTier.Common.Domain.Model.Entity> collection)
        {
        }

        public int SaveChanges()
        {
            return 1;
        }

        public void Dispose()
        {
        }

        #endregion IRepository

        #region INorthwindEntities

        public IEntitySet<Category> Categories { get; private set; }
        public IEntitySet<Employee> Employees{get;private set;}
        public IEntitySet<OrderDetail> OrderDetails { get; private set; }
        public IEntitySet<Order> Orders { get; private set; }
        public IEntitySet<Product> Products { get; private set; }
        public IEntitySet<Region> Regions { get; private set; }
        public IEntitySet<Shipper> Shippers { get; private set; }
        public IEntitySet<Supplier> Suppliers { get; private set; }
        public IEntitySet<Territory> Territories { get; private set; }
        public IEntitySet<DemographicGroup> CustomerDemographics { get; private set; }
        public IEntitySet<Customer> Customers { get; private set; }
        public IEntitySet<DynamicContentEntity> DynamicContentEntities { get; private set; }

        #endregion INorthwindEntities
    }
}
