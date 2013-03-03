using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NUnit.Framework;
using NTier.Client.Domain;
using NTier.Common.Domain.Model;

namespace UnitTests
{
    [TestFixture]
    public class ClientInfoTests : AssertionHelper
    {
        private const string CLIENT_INFO_PROPERTY_VALUE = "The quick brown fox jumps over the lazy dog";
        private const string CLIENT_INFO_PROPERTY_NAME = "TestString";

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T0_IncludeClientInfoInContext()
        {
            var ctx = new NorthwindDataContext();

            ctx.ClientInfo = new ClientInfo();
            ctx.ClientInfo[CLIENT_INFO_PROPERTY_NAME] = CLIENT_INFO_PROPERTY_VALUE;

            //var entity = ctx.Categories.AsQueryable().First();
            var entity = new Category();
            ctx.Add(entity);
            entity.AcceptChanges();

            ctx.Delete(entity);

            ServerValidationException serverValidationException = null;
            try
            {
                ctx.SaveChanges();
            }
            catch (ServerValidationException ex)
            {
                serverValidationException = ex;
            }

            Expect(serverValidationException, Is.Not.Null);

            Expect(entity.Errors.Count, Is.EqualTo(1));
            Expect(entity.IsValid, Is.False);

            var errorMessage = entity.Errors[0].Message;
            Expect(errorMessage, Is.EqualTo(CLIENT_INFO_PROPERTY_VALUE));
        }
    }
}
