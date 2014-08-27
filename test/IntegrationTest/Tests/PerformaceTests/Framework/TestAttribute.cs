using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformaceTests.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestAttribute : Attribute
    {
        private int _numberOfRepetes = 1;
        public int NumberOfRepetes
        {
            get
            {
                return _numberOfRepetes;
            }
            set
            {
                if (value >= 0)
                {
                    _numberOfRepetes = value;
                }
            }
        }

        //public bool ExecuteInParalell { get; set; }
    }
}
