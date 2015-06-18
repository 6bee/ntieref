using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using TableInheritance.Client.Domain;
using TableInheritance.Common.Domain.Model.TableInheritanceDemoDB;

namespace TableInheritance.Client.Domain.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new TableInheritanceDemoDBDataContext();

            var customer = new Customer
            {
                FirstName = "El",
                LastName = "Customer",
                CustomerStatus = 1,
            };
            ctx.Add(customer);

            var employee = new Employee
            {
                FirstName = "Don",
                LastName = "Employee",
                EntryDate = new DateTime(2010, 01, 01),
            };
            ctx.Add(employee);

            ctx.SaveChanges();


            var customers = ctx.People
                .AsQueryable()
                .Where(x => x.FirstName == "El")
                .OfType<Customer>()
                .Where(x => x.CustomerStatus == 1)
                .ToList();

            var employees = ctx.People
                .AsQueryable()
                .Include("Manager")
                .Where(x => x.FirstName == "Don")
                .OfType<Employee>()
                .Where(x => x.EntryDate > new DateTime(2000, 01, 01))
                .ToList();
            
            var people = ctx.People
                .AsQueryable()
                .ToList();
        }
    }
}
