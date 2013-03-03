using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NTier.Client.Domain;
using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NUnit.Framework;
using NTier.Common.Domain.Model;

namespace UnitTests
{
    [TestFixture]
    public class ServerValidationTests : AssertionHelper
    {
        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        [ExpectedException(typeof(ServerValidationException))]
        public void T0_ServerValidationException()
        {
            var ctx = new NorthwindDataContext();

            var entity = ctx.Regions.AsQueryable().First();

            entity.RegionDescription = "modified";

            ctx.SaveChanges();
        }

        [Test]
        public void T0_ServerValidationExceptionSuppressed()
        {
            var ctx = new NorthwindDataContext();
            ctx.IsServerValidationExceptionSuppressed = true;

            var entity = ctx.Regions.AsQueryable().First();

            entity.RegionDescription = "modified";

            ctx.SaveChanges();
        }

        [Test]
        public void T1_ClearErrorOnEntity()
        {
            var ctx = new NorthwindDataContext();

            var entity = ctx.Regions.AsQueryable().First();

            entity.RegionDescription = "modified";

            try
            {
                ctx.SaveChanges();
            }
            catch (ServerValidationException)
            {
            }

            Expect(entity.Errors.Count, Is.AtLeast(1));
            Expect(entity.IsValid, Is.False);

            entity.Errors.Clear();

            Expect(entity.Errors.Count, Is.EqualTo(0));
            Expect(entity.IsValid, Is.True);
        }

        [Test]
        public void T1_ClearErrorOnEntitySet()
        {
            var ctx = new NorthwindDataContext();

            var entity = ctx.Regions.AsQueryable().First();

            entity.RegionDescription = "modified";

            try
            {
                ctx.SaveChanges();
            }
            catch (ServerValidationException)
            {
            }

            Expect(entity.IsValid, Is.False);

            ctx.Regions.ClearErrors();

            Expect(entity.IsValid, Is.True);
        }

        [Test]
        public void T1_ClearErrorOnDataContext()
        {
            var ctx = new NorthwindDataContext();

            var entity = ctx.Regions.AsQueryable().First();

            entity.RegionDescription = "modified";

            try
            {
                ctx.SaveChanges();
            }
            catch (ServerValidationException)
            {
            }

            Expect(entity.IsValid, Is.False);

            ctx.ClearErrors();

            Expect(entity.IsValid, Is.True);
        }
    }
}
