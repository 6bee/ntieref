using System;
using System.Transactions;
using IntegrationTest.Client.Domain;
using NUnit.Framework;
using UnitTests.Utils;

namespace UnitTests
{
    [TestFixture]
    public class ClientTransactionTests : AssertionHelper
    {
        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void Rollback()
        {
            // query, modify, rollback
            {
                var ctx = new NorthwindDataContext();

                var entity = ctx.Products.AsQueryable().First();

                Assert.True(string.Compare(entity.ProductName, "modified", StringComparison.InvariantCultureIgnoreCase) != 0);

                entity.ProductName = "modified";
                Expect(entity.HasChanges, Is.True);

                using (var scope = new TransactionScope())
                {
                    ctx.SaveChanges();
                    // not calling complete
                }

                Expect(entity.HasChanges, Is.True);
                Expect(entity.ProductName, Is.EqualTo("modified"));
            }
            // re-query and check
            {
                var ctx = new NorthwindDataContext();

                var entity = ctx.Products.AsQueryable().First();

                Expect(entity.HasChanges, Is.False);
                Expect(entity.ProductName, Is.Not.EqualTo("modified"));
            }
        }

        [Test]
        public void Commit()
        {
            string initialValue = null;

            // query, modify, commit
            {
                var ctx = new NorthwindDataContext();

                var entity = ctx.Products.AsQueryable().First();

                Assert.True(string.Compare(entity.ProductName, "modified", StringComparison.InvariantCultureIgnoreCase) != 0);
                initialValue = entity.ProductName;
                entity.ProductName = "modified";
                Expect(entity.HasChanges, Is.True);

                using (var scope = new TransactionScope())
                {
                    ctx.SaveChanges();
                    scope.Complete();
                }
                
                // run do-events as accept changes runs using dispatcher
                DispatcherUtil.DoEvents();
                
                Expect(entity.HasChanges, Is.False);
                Expect(entity.ProductName, Is.EqualTo("modified"));
            }

            // re-query and check
            {
                var ctx = new NorthwindDataContext();

                var entity = ctx.Products.AsQueryable().First();

                Expect(entity.HasChanges, Is.False);
                Expect(entity.ProductName, Is.EqualTo("modified"));

                // revert to original value
                entity.ProductName = initialValue;
                ctx.SaveChanges();
            }
        }
    }
}
