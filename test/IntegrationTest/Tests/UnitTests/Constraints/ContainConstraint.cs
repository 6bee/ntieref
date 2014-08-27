using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;

namespace UnitTests.Constraints
{
    public sealed class ContainConstraint<T> : Constraint
    {
        private readonly T _item;
        private bool _objectIsCollectionType = false;
        private Type _wrongType = null;

        public ContainConstraint(T item)
        {
            _item = item;
        }

        public override bool Matches(object actual)
        {
            var collection = actual as System.Collections.IEnumerable;
            if (collection == null) return false;

            _objectIsCollectionType = true;
            foreach (object item in collection)
            {
                if (item is T)
                {
                    if (object.Equals((T)item, _item))
                    {
                        return true;
                    }
                }
                else if(item != null)
                {
                    _wrongType = item.GetType();
                }
            }

            return false;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            if (!_objectIsCollectionType)
            {
                writer.Write("Actual object must be a collection type");
            }
            else if (_wrongType != null)
            {
                writer.Write(string.Format("Elements are of type {0}", typeof(T).Name));
            }
            else
            {
                writer.Write(string.Format("Object '{0}' may be found in collection.", _item));
            }
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (!_objectIsCollectionType)
            {
                base.WriteActualValueTo(writer);
            }
            else if (_wrongType != null)
            {
                writer.Write(string.Format("{0}", _wrongType.Name));
            }
            else
            {
                writer.Write("Not found.");
            }
        }
    }
}
