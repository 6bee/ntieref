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
    public class DynamicPropertyValidationTests : AssertionHelper
    {
        private class TestValidationAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                // return true for negative integers
                if (value != null)
                {
                    if (value is long && (long)value < 0) return true;
                    if (value is int && (int)value < 0) return true;
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj is TestValidationAttribute;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        private const string PHYSICAL_PROPERTY = "Id";
        private const string DYNAMIC_PROPERTY = "DynamicProperty";
        private const int VALID_TEST_VALUE = -1;
        private const int INVALID_TEST_VALUE = 1;


        [Test]
        public void T1_SetValidValueBeforeRegistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = VALID_TEST_VALUE;
        }

        [Test]
        public void T1_SetInalidValueBeforeRegistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = INVALID_TEST_VALUE;
        }

        [Test]
        public void T1_SetInalidValueOnPhysicalPropertyBeforeRegistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            entity[PHYSICAL_PROPERTY] = INVALID_TEST_VALUE;
        }

        [Test]
        public void T2_RegisterValidatorForDynamicProperty()
        {
            DynamicContentEntity.RegisterValidator(typeof(DynamicContentEntity), DYNAMIC_PROPERTY, new TestValidationAttribute());
        }

        [Test]
        public void T2_RegisterValidatorForPhysicalProperty()
        {
            DynamicContentEntity.RegisterValidator(typeof(DynamicContentEntity), PHYSICAL_PROPERTY, new TestValidationAttribute());
        }

        [Test]
        public void T3_SetValidValueWithValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = VALID_TEST_VALUE;
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void T3_SetInalidValueWithValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = INVALID_TEST_VALUE;
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void T3_SetInalidValueOnPhysicaPropertyWithValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[PHYSICAL_PROPERTY] = INVALID_TEST_VALUE;
        }

        [Test]
        public void T4_UnregisterValidatorForDynamicProperty()
        {
            var result = DynamicContentEntity.UnregisterValidator(typeof(DynamicContentEntity), DYNAMIC_PROPERTY, new TestValidationAttribute());
            Expect(result, Is.True);
        }

        [Test]
        public void T4_UnregisterValidatorForPhysicalProperty()
        {
            var result = DynamicContentEntity.UnregisterValidator(typeof(DynamicContentEntity), PHYSICAL_PROPERTY, new TestValidationAttribute());
            Expect(result, Is.True);
        }

        [Test]
        public void T5_SetValidValueAfterUnregistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = VALID_TEST_VALUE;
        }

        [Test]
        public void T5_SetInalidValueAfterUnregistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[DYNAMIC_PROPERTY] = INVALID_TEST_VALUE;
        }

        [Test]
        public void T5_SetInalidValueOnPhysicalPropertyAfterUnregistrationOfValidator()
        {
            var entity = new DynamicContentEntity();
            entity.StartTracking();

            // set dynamic property
            entity[PHYSICAL_PROPERTY] = INVALID_TEST_VALUE;
        }
    }
}
