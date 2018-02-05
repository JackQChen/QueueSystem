using Chloe;
using Chloe.SqlServer;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloeDemo
{
    public class MsSqlTest
    {
        static MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString);

        public static void Test()
        {
            MsSqlTest.Query();
            //MsSqlTest.GroupQuery();
            //MsSqlTest.Insert();
            //MsSqlTest.Update();
            //MsSqlTest.Delete();
        }

        public static void Query()
        {
            object ret = null;

            var q = context.Query<TestEntity>();

            ret = q.Where(a => a.Id > 0).ToList();
            ret = q.Where(a => a.Id > 0).Skip(1).Take(999).ToList();
            ret = q.Where(a => a.Id > 0).OrderBy(a => a.F_Int32).ThenBy(a => a.F_Int64).Skip(1).Take(999).ToList();


            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void GroupQuery()
        {
            object ret = null;

            var q = context.Query<TestEntity>();

            var gq = q.GroupBy(a => a.F_Int32);

            gq = gq.Having(a => a.F_Int32 > 1 && AggregateFunctions.Count() > 1);

            ret = gq.Select(a => new { a.F_Int32, Count = AggregateFunctions.Count(), Sum = AggregateFunctions.Sum(a.F_Int32), Max = AggregateFunctions.Max(a.F_Int32), Min = AggregateFunctions.Min(a.F_Int32), Avg = AggregateFunctions.Average(a.F_Int32) }).ToList();


            ConsoleHelper.WriteLineAndReadKey();
        }



        public static void Insert()
        {
            var id = (int)context.Insert<TestEntity>(() => new TestEntity()
            {
                F_Byte = 1
                  ,
                F_Int16 = 16
                 ,
                F_Int32 = 32
                 ,
                F_Int64 = 64
                 ,
                F_Double = 1.123456
                 ,
                F_Float = 1.1234F
                 ,
                F_Decimal = 1.12345678M
                 ,
                F_Bool = true
                 ,
                F_DateTime = DateTime.Now
                 ,
                F_Guid = Guid.NewGuid()
                 ,
                F_String = "12345"
            });

            var entity = new TestEntity();
            entity.F_Byte = 1;
            entity.F_Int16 = 16;
            entity.F_Int32 = 32;
            entity.F_Int64 = 64;
            entity.F_Double = 1.123456;
            entity.F_Float = 1.1234F;
            entity.F_Decimal = 1.12345678M;
            entity.F_Bool = true;
            entity.F_DateTime = DateTime.Now;
            entity.F_Guid = Guid.NewGuid();
            entity.F_String = "12345";

            entity = context.Insert(entity);

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void Update()
        {
            object ret = null;

            ret = context.Update<TestEntity>(a => a.Id == 1, a => new TestEntity()
            {
                F_Byte = 1
                 ,
                F_Int16 = 16
                ,
                F_Int32 = 32
                ,
                F_Int64 = 64
                ,
                F_Double = 1.123456
                ,
                F_Float = 1.1234F
                ,
                F_Decimal = 1.12345678M
                ,
                F_Bool = true
                ,
                F_DateTime = DateTime.Now
                ,
                F_Guid = Guid.NewGuid()
                ,
                F_String = "12345"
            });

            Console.WriteLine(ret);

            var entity = new TestEntity();
            entity.Id = 1;
            entity.F_Byte = 1;
            entity.F_Int16 = 16;
            entity.F_Int32 = 32;
            entity.F_Int64 = 64;
            entity.F_Double = 1.123456;
            entity.F_Float = 1.1234F;
            entity.F_Decimal = 1.12345678M;
            entity.F_Bool = true;
            entity.F_DateTime = DateTime.Now;
            entity.F_Guid = Guid.NewGuid();
            entity.F_String = "12345";

            ret = context.Update(entity);

            Console.WriteLine(ret);

            context.TrackEntity(entity);


            entity.F_String = "lu";
            ret = context.Update(entity);

            Console.WriteLine(ret);

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void Delete()
        {
            object ret = null;

            ret = context.Delete<TestEntity>(a => a.Id == 0);

            TestEntity entity = new TestEntity();
            entity.Id = 0;

            ret = context.Delete(entity);

            ConsoleHelper.WriteLineAndReadKey();
        }
    }
}
