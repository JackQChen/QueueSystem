using Chloe.SqlServer;
using Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Chloe;
using System.Data.SqlClient;


namespace ChloePerformanceTest
{
    /// <summary>
    /// 测试 ORM 的查询能力。测试方法：一次查询一条数据，循环执行多次查询，计算所用时间和内存分配以及GC次数
    /// 该类测试循环体内包括创建 DbContext 上下文
    /// </summary>
    class LoopQueryTest
    {
        static int takeCount = 1;
        static int queryCount = 20000;

        public static void GCMemoryTest()
        {
            /*
             * 内存分配测试通过 vs 自带诊断与分析工具测，vs --> 分析 --> 性能与诊断 --> 内存使用率。
             * 每次运行程序只能调用下面中的一个方法，不能同时调用
             */

            ChloeQueryTest(takeCount, queryCount);
            //ChloeSqlQueryTest(takeCount, queryCount);
            //DapperQueryTest(takeCount, queryCount);
            //EFLinqQueryTest(takeCount, queryCount);
            //EFSqlQueryTest(takeCount, queryCount);
        }
        public static void SpeedTest()
        {
            long useTime = 0;

            //预热
            ChloeQueryTest(1, 1);
            useTime = SW.Do(() =>
            {
                ChloeQueryTest(takeCount, queryCount);
            });
            Console.WriteLine("ChloeQueryTest 执行{0}次查询总用时：{1}ms", queryCount, useTime);
            GC.Collect();


            useTime = SW.Do(() =>
            {
                ChloeSqlQueryTest(takeCount, queryCount);
            });
            Console.WriteLine("ChloeSqlQueryTest 执行{0}次查询总用时：{1}ms", queryCount, useTime);
            GC.Collect();


            //预热
            DapperQueryTest(1, 1);
            useTime = SW.Do(() =>
            {
                DapperQueryTest(takeCount, queryCount);
            });
            Console.WriteLine("DapperQueryTest 执行{0}次查询总用时：{1}ms", queryCount, useTime);
            GC.Collect();

            //预热
            EFLinqQueryTest(1, 1);
            useTime = SW.Do(() =>
            {
                EFLinqQueryTest(takeCount, queryCount);
            });
            Console.WriteLine("EFLinqQueryTest 执行{0}次查询总用时：{1}ms", queryCount, useTime);
            GC.Collect();


            //预热
            EFSqlQueryTest(1, 1);
            useTime = SW.Do(() =>
            {
                EFSqlQueryTest(takeCount, queryCount);
            });
            Console.WriteLine("EFSqlQueryTest 执行{0}次查询总用时：{1}ms", queryCount, useTime);
            GC.Collect();


            Console.WriteLine("GAME OVER");
            Console.ReadKey();
        }


        static void ChloeQueryTest(int takeCount, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
                {
                    int id = 0;
                    var list = context.Query<TestEntity>().Where(a => a.Id > id).Take(takeCount).ToList();
                }
            }
        }
        static void ChloeSqlQueryTest(int takeCount, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                using (MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString))
                {
                    int id = 0;
                    var list = context.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", takeCount.ToString()), DbParam.Create("@Id", id)).ToList();
                }
            }
        }

        static void DapperQueryTest(int takeCount, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                using (IDbConnection conn = DbHelper.CreateConnection())
                {
                    int id = 0;
                    var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", takeCount.ToString()), new { Id = id }).ToList();
                }
            }
        }
        static void EFLinqQueryTest(int takeCount, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                using (EFContext efContext = new EFContext())
                {
                    int id = 0;
                    var list = efContext.TestEntity.AsNoTracking().Where(a => a.Id > id).Take(takeCount).ToList();
                }
            }
        }
        static void EFSqlQueryTest(int takeCount, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                using (EFContext efContext = new EFContext())
                {
                    int id = 0;
                    var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", takeCount.ToString()), new SqlParameter("@Id", id)).ToList();
                }
            }
        }
    }
}
