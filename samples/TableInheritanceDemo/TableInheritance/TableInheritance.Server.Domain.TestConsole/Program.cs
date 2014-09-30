using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TableInheritance.Server.Domain.Edmx;

namespace TableInheritance.Server.Domain.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new TableInheritanceDemoDBEntities();
            ctx.Configuration.LazyLoadingEnabled = false;
            ctx.Configuration.ProxyCreationEnabled = false;

            var entities = ctx.People
                //.AsQueryable()
                .Include("Manager")
                //.OfType<Person>()
                .OfType<Employee>()
                //.OfType<Customer>()
                .ToList();
        }
    }
}
