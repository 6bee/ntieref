using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerformaceTests.Framework;

namespace PerformaceTests
{
    class Start
    {
        static void Main(string[] args)
        {
            var testRunner = new TestRunner();
            testRunner.Run();

            Console.Write("done");
            Console.ReadLine();
        }
    }
}
