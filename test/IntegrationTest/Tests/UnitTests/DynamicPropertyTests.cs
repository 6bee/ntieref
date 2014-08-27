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
    public class DynamicPropertyTests : AssertionHelper
    {
        [Serializable]
        private class TextElement
        {
            public TextElement(Guid id)
            {
                _id = id;
            }

            public Guid Id { get { return _id; } }
            private readonly Guid _id;

            public string Text { get; set; }
        }

        [Serializable]
        private struct ValueElement
        {
            public ValueElement(Guid id)
            {
                _id = id;
                _value = 0;
            }

            public Guid Id { get { return _id; } }
            private readonly Guid _id;

            //public int? Value { get; set; }
            public int Value { get { return _value; } set { _value = value; } }
            private int _value;
        }

        private const string TEST_STRING = "The quick brown fox jumps over the lazy dog";
        private const int TEST_VALUE = 1234567890;
        private const long TEST_VALUE2 = 99L;

        private long Id { get; set; }

        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public void T0_Setup()
        {
            // create and strore an empty DynamicContentEntity
            var ctx = Default.DataContext();

            //var entity = new DynamicContentEntity();

            //ctx.Add(entity);
            var entity = ctx.DynamicContentEntities.CreateNew();

            ctx.SaveChanges();

            Id = entity.Id;
        }

        [Test]
        public void T1_SetAndGetDynamicValues()
        {
            var entity = new DynamicContentEntity();

            // set dynamic property
            entity["TestString"] = TEST_STRING;

            // read back
            Expect(entity["TestString"], Is.Not.Null);
            Expect(entity["TestString"], Is.InstanceOf<string>());
            Expect(entity["TestString"], Is.EqualTo(TEST_STRING));
        }

        [Test]
        public void T2_StoreDynamicValues()
        {
            var ctx = Default.DataContext();

            var entity = ctx.DynamicContentEntities.AsQueryable().Single(e => e.Id == Id);

            entity["TextElementProperty"] = new TextElement(Guid.NewGuid()) { Text = TEST_STRING };

            // read back
            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));

            entity["ValueElementProperty"] = new ValueElement(Guid.NewGuid()) { Value = TEST_VALUE };
            entity["ObjectProperty"] = new object();
            entity["BooleanProperty"] = true;
            entity["NullValueProperty"] = null;
            entity["LongProperty"] = (long)TEST_VALUE;

            ctx.SaveChanges();
        }

        [Test]
        public void T3_RetrieveDynamicValues()
        {
            var ctx = Default.DataContext();

            var entity = ctx.DynamicContentEntities.AsQueryable().Single(e => e.Id == Id);

            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));

            Expect(entity["ValueElementProperty"], Is.Not.Null);
            Expect(entity["ValueElementProperty"], Is.InstanceOf<ValueElement>());
            Expect(((ValueElement)entity["ValueElementProperty"]).Value, Is.EqualTo(TEST_VALUE));

            Expect(entity["ObjectProperty"], Is.Not.Null);
            Expect(entity["ObjectProperty"], Is.InstanceOf<object>());

            Expect(entity["BooleanProperty"], Is.Not.Null);
            Expect(entity["BooleanProperty"], Is.InstanceOf<bool>());
            Expect(entity["BooleanProperty"], Is.True);

            Expect(entity["NullValueProperty"], Is.Null);

            Expect(entity["NotExistingProperty"], Is.Null);

            Expect(entity["LongProperty"], Is.Not.Null);
            Expect(entity["LongProperty"], Is.InstanceOf<long>());
            Expect(entity["LongProperty"], Is.EqualTo(TEST_VALUE));
        }

        [Test]
        public void T4_ModifyDynamicValues()
        {
            var ctx = Default.DataContext();

            var entity = ctx.DynamicContentEntities.AsQueryable().Single(e => e.Id == Id);

            entity["ValueElementProperty"] = null;
            entity["LongProperty"] = TEST_VALUE2;

            ctx.SaveChanges();
        }

        [Test]
        public void T5_RetrieveDynamicValues()
        {
            var ctx = Default.DataContext();

            var entity = ctx.DynamicContentEntities.AsQueryable().Single(e => e.Id == Id);


            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));

            Expect(entity["ValueElementProperty"], Is.Null);

            Expect(entity["LongProperty"], Is.Not.Null);
            Expect(entity["LongProperty"], Is.InstanceOf<long>());
            Expect(entity["LongProperty"], Is.EqualTo(TEST_VALUE2));
        }

        [Test]
        public void T7_RevertDynamicValue()
        {
            var ctx = Default.DataContext();

            var entity = ctx.DynamicContentEntities.AsQueryable().Single(e => e.Id == Id);


            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));


            entity["TextElementProperty"] = "New Text 1";
            entity.RevertChanges("TextElementProperty");


            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));


            entity["TextElementProperty"] = "New Text 2";
            entity.RevertChanges();


            Expect(entity["TextElementProperty"], Is.Not.Null);
            Expect(entity["TextElementProperty"], Is.InstanceOf<TextElement>());
            Expect(((TextElement)entity["TextElementProperty"]).Text, Is.EqualTo(TEST_STRING));
        }


        // metadata for dynamic properties is currently not supported
        //[Test]
        //[ExpectedException(typeof(ValidationException))]
        //public void T7_DynamicPropertyValidation()
        //{
        //    var entity = new DynamicContentEntity();
        //    entity.StartTracking();
        //    entity["StringProperty"] = "123456";
        //}
    }
}
