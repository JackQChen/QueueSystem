using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Chloe;
using Chloe.SqlServer;
using Db;
using System.Data;
using Dapper;

namespace ChloePerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MappingSpeedTest.SpeedTest();
            //MappingSpeedTest.GCMemoryTest();

            //LoopQueryTest.SpeedTest();
            //LoopQueryTest.GCMemoryTest();

            //LoopQueryTestWithNotCreateDbContext.SpeedTest();
            //LoopQueryTestWithNotCreateDbContext.GCMemoryTest();
        }
    }
}
