using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnitTests.Constraints;

namespace UnitTests.Utils
{
    public static class To
    {
        public static ContainConstraint<T> Contain<T>(T item)
        {
            return new ContainConstraint<T>(item);
        }
    }
}
