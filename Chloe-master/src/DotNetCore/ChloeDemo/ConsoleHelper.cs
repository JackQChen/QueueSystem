using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloeDemo
{
    class ConsoleHelper
    {
        public static void WriteLineAndReadKey(object val)
        {
            Console.WriteLine(val);
            Console.ReadKey();
        }

        public static void WriteLineAndReadKey(string val = "...")
        {
            Console.WriteLine(val);
            Console.ReadKey();
        }
    }
}
