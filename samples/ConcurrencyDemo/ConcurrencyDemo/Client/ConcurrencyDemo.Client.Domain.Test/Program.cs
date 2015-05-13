using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;
using ConcurrencyDemo.Client.Domain;
using ConcurrencyDemo.Common.Domain.Model.ConcurrencyTest;

namespace ConcurrencyDemo.Client.Domain.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new ConcurrencyTestDataContext();

            var entityA = new ARecord { Value = "[0] created payload" };
            ctx.ARecords.Add(entityA);
            ctx.SaveChanges();

            entityA.Value = string.Format("[1]  updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();

            entityA.Value = string.Format("[2] updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();


            var entityB = new BRecord { Value = "[0] created payload" };
            ctx.BRecords.Add(entityB);
            ctx.SaveChanges();

            entityB.Value = string.Format("[1]  updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();

            entityB.Value = string.Format("[2] updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();


            var entityC = new CRecord { Value = "[0] created payload" };
            ctx.CRecords.Add(entityC);
            ctx.SaveChanges();

            entityC.Value = string.Format("[1]  updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();

            entityC.Value = string.Format("[2] updated payload at {0}", DateTime.Now);
            ctx.SaveChanges();


            var newContext = new ConcurrencyTestDataContext();

            entityC.Value = string.Format("[3] updated payload at {0} via new data context", DateTime.Now);
            newContext.Attach(entityC);

            newContext.SaveChanges();
        }
    }
}
