using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloePerformanceTest
{
    public class SW
    {
        public static long Do(Action act)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            act();

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        public static T Do<T>(Func<T> fn)
        {
            T t;
            Stopwatch sw = new Stopwatch();

            sw.Start();

            t = fn();

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);

            return t;
        }
    }
}
