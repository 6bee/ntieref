using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;

namespace PerformaceTests.Framework
{
    public sealed class TestRunner
    {
        private readonly Assembly _assembly;

        private TextWriter TextWriter { get; set; }

        public TestRunner(Assembly assembly = null)
        {
            _assembly = assembly ?? Assembly.GetCallingAssembly();
            TextWriter = Console.Out;
        }

        public void Run()
        {
            var testFixtures = GetTestFixtures();

            foreach (var testFixture in testFixtures)
            {
                var instance = Activator.CreateInstance(testFixture.Key);

                foreach (var testMethod in testFixture.Value)
                {
                    var testAttribute = testMethod.GetCustomAttributes(typeof(TestAttribute), true)[0] as TestAttribute;

                    //if (testAttribute.ExecuteInParalell)
                    //{
                    //    var result = Parallel.For<TimeSpan>(0, testAttribute.NumberOfRepetes,//null,
                    //        () => TimeSpan.Zero,
                    //        (i, states, accumulatedDuration) =>
                    //        {
                    //            var duration = Run(instance, testMethod);
                    //            TextWriter.WriteLine("{0}.{1}()  Duration: {2}", testMethod.DeclaringType.Name, testMethod.Name, duration);
                    //            return accumulatedDuration + duration;
                    //        },
                    //        delegate(TimeSpan duration)
                    //        {
                    //            //TextWriter.WriteLine("{0}.{1}()  Average:  {2}", testMethod.DeclaringType.Name, testMethod.Name, TimeSpan.FromTicks(duration.Ticks / testAttribute.NumberOfRepetes));
                    //            TextWriter.WriteLine("{0}.{1}()  Average:  {2}", testMethod.DeclaringType.Name, testMethod.Name, duration);
                    //        });
                    //}
                    //else
                    //{
                    //}

                    var totalDuration = TimeSpan.Zero;

                    for (int i = 0; i < testAttribute.NumberOfRepetes; i++)
                    {
                        var duration = Run(instance, testMethod);
                        TextWriter.WriteLine("[{0:0000}] {1}.{2}()  Duration: {3}", i, testMethod.DeclaringType.Name, testMethod.Name, duration);
                        totalDuration += duration;
                    }

                    if (testAttribute.NumberOfRepetes > 1)
                    {
                        TextWriter.WriteLine("       {0}.{1}()  Average:  {2}", testMethod.DeclaringType.Name, testMethod.Name, TimeSpan.FromTicks(totalDuration.Ticks / testAttribute.NumberOfRepetes));
                    }

                    TextWriter.WriteLine();
                }
            }
        }

        private TimeSpan Run(object instance, MethodInfo testMethod)
        {
            var timestamp = DateTime.Now;
            testMethod.Invoke(instance, null);
            var duration = DateTime.Now - timestamp;
            return duration;
        }

        private IDictionary<Type, IEnumerable<MethodInfo>> GetTestFixtures()
        {
            return _assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToDictionary
                (
                    c => c,
                    c => c.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod)
                          .Where(m => m.GetCustomAttributes(typeof(TestAttribute), true).Length == 1)
                )
                .Where(e => e.Value.Any())
                .ToDictionary(e => e.Key, e => e.Value);
        }

        //private IEnumerable<MethodInfo> GetTestMethods()
        //{
        //    return _assembly.GetTypes()
        //        .Where(t => t.IsClass && !t.IsAbstract)
        //        .SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod))
        //        .Where(m => m.GetCustomAttributes(typeof(TestAttribute), true).Length == 1);
        //}
    }
}
