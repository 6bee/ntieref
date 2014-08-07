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
                FirstName = "Peter",
                LastName = "Pan",
                CustomerStatus = 1,
            };
            ctx.Add(customer);
            ctx.SaveChanges();
        }
    }
}
