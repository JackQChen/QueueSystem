using Chloe.SqlServer;
using Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ChloePerformanceTest
{
    class MappingSpeedTest
    {
        static int takeCount = 50 * 10000;

        public static void GCMemoryTest()
        {
            /*
             * 内存分配测试通过 vs2013， 分析 --> 性能与诊断 --> 内存使用率 测试
             * 每次运行程序只能调用下面中的一个方法，不能同时调用
             */

            ChloeQueryTest(takeCount);
            //ChloeSqlQueryTest(takeCount);
            //DapperQueryTest(takeCount);
            //EFLinqQueryTest(takeCount);
            //EFSqlQueryTest(takeCount);
        }
        public static void SpeedTest()
        {
            long useTime = 0;

            //预热
            ChloeQueryTest(1);

            useTime = SW.Do(() =>
            {
                ChloeQueryTest(takeCount);
            });

            Console.WriteLine("ChloeQueryTest 一次查询{0}条数据总用时：{1}ms", takeCount, useTime);
            GC.Collect();

            useTime = SW.Do(() =>
            {
                ChloeSqlQueryTest(takeCount);
            });

            Console.WriteLine("ChloeSqlQueryTest 一次查询{0}条数据总用时：{1}ms", takeCount, useTime);
            GC.Collect();

            //预热
            DapperQueryTest(1);

            useTime = SW.Do(() =>
            {
                DapperQueryTest(takeCount);
            });

            Console.WriteLine("DapperQueryTest 一次查询{0}条数据总用时：{1}ms", takeCount, useTime);
            GC.Collect();

            //预热
            EFLinqQueryTest(1);

            useTime = SW.Do(() =>
            {
                EFLinqQueryTest(takeCount);
            });

            Console.WriteLine("EFLinqQueryTest 一次查询{0}条数据总用时：{1}ms", takeCount, useTime);
            GC.Collect();

            //预热
            EFSqlQueryTest(1);

            useTime = SW.Do(() =>
            {
                EFSqlQueryTest(takeCount);
            });

            Console.WriteLine("EFSqlQueryTest 一次查询{0}条数据总用时：{1}ms", takeCount, useTime);
            GC.Collect();

            Console.WriteLine("GAME OVER");
            Console.ReadKey();
        }

        static void ChloeQueryTest(int takeCount)
        {
            using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
            {
                var list = context.Query<TestEntity>().Take(takeCount).ToList();
            }
        }
        static void ChloeSqlQueryTest(int takeCount)
        {
            using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
            {
                var list = context.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", takeCount.ToString())).ToList();
            }
        }


        static void DapperQueryTest(int takeCount)
        {
            using (IDbConnection conn = DbHelper.CreateConnection())
            {
                var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity", takeCount.ToString())).ToList();
            }
        }

        static void EFLinqQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.TestEntity.AsNoTracking().Take(takeCount).ToList();
            }
        }
        static void EFSqlQueryTest(int takeCount)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", takeCount.ToString())).ToList();
            }
        }
    }
}
