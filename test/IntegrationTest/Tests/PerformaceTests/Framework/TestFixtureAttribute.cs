using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformaceTests.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TestFixtureAttribute : Attribute
    {
    }
}
