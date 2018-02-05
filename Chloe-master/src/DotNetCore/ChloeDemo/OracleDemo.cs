using Chloe;
using Chloe.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloeDemo
{
    public class OracleDemo
    {
        /* WARNING: DbContext 是非线程安全的，正式使用不能设置为 static，并且用完务必要调用 Dispose 方法销毁对象 */
        //static OracleContext context = new OracleContext(new OracleConnectionFactory("Data Source=localhost/orcl;User ID=system;Password=sa;"));
        static OracleContext context = new OracleContext(new OracleConnectionFactory("Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST =localhost)(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME =chloe))); User ID =system; Password=sa"));

        public static void Run()
        {
            BasicQuery();
            JoinQuery();
            AggregateQuery();
            GroupQuery();
            ComplexQuery();  /* v2.18复杂查询 */
            Insert();
            Update();
            Delete();
            Method();
            ExecuteCommandText();
            DoWithTransaction();
            DoWithTransactionEx();

            ConsoleHelper.WriteLineAndReadKey();
        }


        public static void BasicQuery()
        {
            IQuery<User> q = context.Query<User>();

            q.Where(a => a.Id == 1).FirstOrDefault();
            /*
             * SELECT "USERS"."ID" AS "ID","USERS"."NAME" AS "NAME","USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME" FROM "USERS" "USERS" WHERE ("USERS"."ID" = 1 AND ROWNUM < 2)
             */


            //可以选取指定的字段
            q.Where(a => a.Id == 1).Select(a => new { a.Id, a.Name }).FirstOrDefault();
            /*
             * SELECT "USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" FROM "USERS" "USERS" WHERE ("USERS"."ID" = 1 AND ROWNUM < 2)
             */


            //分页
            q.Where(a => a.Id > 0).OrderBy(a => a.Age).Skip(20).Take(10).ToList();
            /*
             * SELECT "T"."ID" AS "ID","T"."NAME" AS "NAME","T"."GENDER" AS "GENDER","T"."AGE" AS "AGE","T"."CITYID" AS "CITYID","T"."OPTIME" AS "OPTIME" FROM (SELECT "TTAKE"."ID" AS "ID","TTAKE"."NAME" AS "NAME","TTAKE"."GENDER" AS "GENDER","TTAKE"."AGE" AS "AGE","TTAKE"."CITYID" AS "CITYID","TTAKE"."OPTIME" AS "OPTIME",ROWNUM AS "ROW_NUMBER_0" FROM (SELECT "USERS"."ID" AS "ID","USERS"."NAME" AS "NAME","USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME" FROM "USERS" "USERS" WHERE "USERS"."ID" > 0 ORDER BY "USERS"."AGE" ASC) "TTAKE" WHERE ROWNUM < 31) "T" WHERE "T"."ROW_NUMBER_0" > 20
             */


            /* like 查询 */
            q.Where(a => a.Name.Contains("so") || a.Name.StartsWith("s") || a.Name.EndsWith("o")).ToList();
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS" 
             * WHERE ("USERS"."NAME" LIKE '%' || N'so' || '%' OR "USERS"."NAME" LIKE N's' || '%' OR "USERS"."NAME" LIKE '%' || N'o')
             */


            /* in 一个数组 */
            List<User> users = null;
            List<int> userIds = new List<int>() { 1, 2, 3 };
            users = q.Where(a => userIds.Contains(a.Id)).ToList(); /* list.Contains() 方法组合就会生成 in一个数组 sql 语句 */
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS" 
             * WHERE "USERS"."ID" IN (1,2,3)
             */


            /* in 子查询 */
            users = q.Where(a => context.Query<City>().Select(c => c.Id).ToList().Contains((int)a.CityId)).ToList(); /* IQuery<T>.ToList().Contains() 方法组合就会生成 in 子查询 sql 语句 */
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS" 
             * WHERE "USERS"."CITYID" IN (SELECT "CITY"."ID" AS "C" FROM "CITY" "CITY")
             */


            /* distinct 查询 */
            q.Select(a => new { a.Name }).Distinct().ToList();
            /*
             * SELECT DISTINCT "USERS"."NAME" AS "NAME" FROM "USERS" "USERS"
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void JoinQuery()
        {
            var user_city_province = context.Query<User>()
                                     .InnerJoin<City>((user, city) => user.CityId == city.Id)
                                     .InnerJoin<Province>((user, city, province) => city.ProvinceId == province.Id);

            //查出一个用户及其隶属的城市和省份的所有信息
            var view = user_city_province.Select((user, city, province) => new { User = user, City = city, Province = province }).Where(a => a.User.Id > 1).ToList();
            /*
             * SELECT "USERS"."ID" AS "ID","USERS"."NAME" AS "NAME","USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","CITY"."ID" AS "ID0","CITY"."NAME" AS "NAME0","CITY"."PROVINCEID" AS "PROVINCEID","PROVINCE"."ID" AS "ID1","PROVINCE"."NAME" AS "NAME1" FROM "USERS" "USERS" INNER JOIN "CITY" "CITY" ON "USERS"."CITYID" = "CITY"."ID" INNER JOIN "PROVINCE" "PROVINCE" ON "CITY"."PROVINCEID" = "PROVINCE"."ID" WHERE "USERS"."ID" > 1
             */

            //也可以只获取指定的字段信息：UserId,UserName,CityName,ProvinceName
            user_city_province.Select((user, city, province) => new { UserId = user.Id, UserName = user.Name, CityName = city.Name, ProvinceName = province.Name }).Where(a => a.UserId > 1).ToList();
            /*
             * SELECT "USERS"."ID" AS "USERID","USERS"."NAME" AS "USERNAME","CITY"."NAME" AS "CITYNAME","PROVINCE"."NAME" AS "PROVINCENAME" FROM "USERS" "USERS" INNER JOIN "CITY" "CITY" ON "USERS"."CITYID" = "CITY"."ID" INNER JOIN "PROVINCE" "PROVINCE" ON "CITY"."PROVINCEID" = "PROVINCE"."ID" WHERE "USERS"."ID" > 1
             */


            /* quick join and paging. */
            context.JoinQuery<User, City>((user, city) => new object[]
            {
                JoinType.LeftJoin, user.CityId == city.Id
            })
            .Select((user, city) => new { User = user, City = city })
            .Where(a => a.User.Id > -1)
            .OrderByDesc(a => a.User.Age)
            .TakePage(1, 20)
            .ToList();

            context.JoinQuery<User, City, Province>((user, city, province) => new object[]
            {
                JoinType.LeftJoin, user.CityId == city.Id,          /* 表 User 和 City 进行Left连接 */
                JoinType.LeftJoin, city.ProvinceId == province.Id   /* 表 City 和 Province 进行Left连接 */
            })
            .Select((user, city, province) => new { User = user, City = city, Province = province })   /* 投影成匿名对象 */
            .Where(a => a.User.Id > -1)     /* 进行条件过滤 */
            .OrderByDesc(a => a.User.Age)   /* 排序 */
            .TakePage(1, 20)                /* 分页 */
            .ToList();

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void AggregateQuery()
        {
            IQuery<User> q = context.Query<User>();

            q.Select(a => AggregateFunctions.Count()).First();
            /*
             * SELECT COUNT(1) AS "C" FROM "USERS" "USERS" WHERE ROWNUM < 2
             */

            q.Select(a => new { Count = AggregateFunctions.Count(), LongCount = AggregateFunctions.LongCount(), Sum = AggregateFunctions.Sum(a.Age), Max = AggregateFunctions.Max(a.Age), Min = AggregateFunctions.Min(a.Age), Average = AggregateFunctions.Average(a.Age) }).First();
            /*
             * SELECT COUNT(1) AS "COUNT",COUNT(1) AS "LONGCOUNT",SUM("USERS"."AGE") AS "SUM",MAX("USERS"."AGE") AS "MAX",MIN("USERS"."AGE") AS "MIN",AVG("USERS"."AGE") AS "AVERAGE" FROM "USERS" "USERS" WHERE ROWNUM < 2
             */

            var count = q.Count();
            /*
             * SELECT COUNT(1) AS "C" FROM "USERS" "USERS"
             */

            var longCount = q.LongCount();
            /*
             * SELECT COUNT(1) AS "C" FROM "USERS" "USERS"
             */

            var sum = q.Sum(a => a.Age);
            /*
             * SELECT SUM("USERS"."AGE") AS "C" FROM "USERS" "USERS"
             */

            var max = q.Max(a => a.Age);
            /*
             * SELECT MAX("USERS"."AGE") AS "C" FROM "USERS" "USERS"
             */

            var min = q.Min(a => a.Age);
            /*
             * SELECT MIN("USERS"."AGE") AS "C" FROM "USERS" "USERS"
             */

            var avg = q.Average(a => a.Age);
            /*
             * SELECT AVG("USERS"."AGE") AS "C" FROM "USERS" "USERS"
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void GroupQuery()
        {
            IQuery<User> q = context.Query<User>();

            IGroupingQuery<User> g = q.Where(a => a.Id > 0).GroupBy(a => a.Age);

            g = g.Having(a => a.Age > 1 && AggregateFunctions.Count() > 0);

            g.Select(a => new { a.Age, Count = AggregateFunctions.Count(), Sum = AggregateFunctions.Sum(a.Age), Max = AggregateFunctions.Max(a.Age), Min = AggregateFunctions.Min(a.Age), Avg = AggregateFunctions.Average(a.Age) }).ToList();
            /*
             * SELECT "USERS"."AGE" AS "AGE",COUNT(1) AS "COUNT",SUM("USERS"."AGE") AS "SUM",MAX("USERS"."AGE") AS "MAX",MIN("USERS"."AGE") AS "MIN",AVG("USERS"."AGE") AS "AVG" FROM "USERS" "USERS" WHERE "USERS"."ID" > 0 GROUP BY "USERS"."AGE" HAVING ("USERS"."AGE" > 1 AND COUNT(1) > 0)
             */

            ConsoleHelper.WriteLineAndReadKey();
        }

        /*复杂查询*/
        public static void ComplexQuery()
        {
            /*
             * 支持 select * from Users where CityId in (1,2,3)    --in一个数组
             * 支持 select * from Users where CityId in (select Id from City)    --in子查询
             * 支持 select * from Users exists (select 1 from City where City.Id=Users.CityId)    --exists查询
             * 支持 select (select top 1 CityName from City where Users.CityId==City.Id) as CityName, Users.Id, Users.Name from Users    --select子查询
             * 支持 select 
             *            (select count(*) from Users where Users.CityId=City.Id) as UserCount,     --总数
             *            (select max(Users.Age) from Users where Users.CityId=City.Id) as MaxAge,  --最大年龄
             *            (select avg(Users.Age) from Users where Users.CityId=City.Id) as AvgAge   --平均年龄
             *      from City
             *      --统计查询
             */

            IQuery<User> userQuery = context.Query<User>();
            IQuery<City> cityQuery = context.Query<City>();

            List<User> users = null;

            /* in 一个数组 */
            List<int> userIds = new List<int>() { 1, 2, 3 };
            users = userQuery.Where(a => userIds.Contains(a.Id)).ToList();  /* list.Contains() 方法组合就会生成 in一个数组 sql 语句 */
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS" 
             * WHERE "USERS"."ID" IN (1,2,3)
             */


            /* in 子查询 */
            users = userQuery.Where(a => cityQuery.Select(c => c.Id).ToList().Contains((int)a.CityId)).ToList();  /* IQuery<T>.ToList().Contains() 方法组合就会生成 in 子查询 sql 语句 */
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS" 
             * WHERE "USERS"."CITYID" IN (SELECT "CITY"."ID" AS "C" FROM "CITY" "CITY")
             */


            /* IQuery<T>.Any() 方法组合就会生成 exists 子查询 sql 语句 */
            users = userQuery.Where(a => cityQuery.Where(c => c.Id == a.CityId).Any()).ToList();
            /*
             * SELECT 
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME"
             * FROM "USERS" "USERS" 
             * WHERE Exists (SELECT N'1' AS "C" FROM "CITY" "CITY" WHERE "CITY"."ID" = "USERS"."CITYID")
             */


            /* select 子查询 */
            var result = userQuery.Select(a => new
            {
                CityName = cityQuery.Where(c => c.Id == a.CityId).First().Name,
                User = a
            }).ToList();
            /*
             * SELECT 
             *      (SELECT "CITY"."NAME" AS "C" FROM "CITY" "CITY" WHERE ("CITY"."ID" = "USERS"."CITYID" AND ROWNUM < 2)) AS "CITYNAME",
             *      "USERS"."GENDER" AS "GENDER","USERS"."AGE" AS "AGE","USERS"."CITYID" AS "CITYID","USERS"."OPTIME" AS "OPTIME","USERS"."ID" AS "ID","USERS"."NAME" AS "NAME" 
             * FROM "USERS" "USERS"
             */


            /* 统计 */
            var statisticsResult = cityQuery.Select(a => new
            {
                UserCount = userQuery.Where(u => u.CityId == a.Id).Count(),
                MaxAge = userQuery.Where(u => u.CityId == a.Id).Max(c => c.Age),
                AvgAge = userQuery.Where(u => u.CityId == a.Id).Average(c => c.Age),
            }).ToList();
            /*
             * SELECT 
             *      (SELECT COUNT(1) AS "C" FROM "USERS" "USERS" WHERE "USERS"."CITYID" = "CITY"."ID") AS "USERCOUNT",
             *      (SELECT MAX("USERS"."AGE") AS "C" FROM "USERS" "USERS" WHERE "USERS"."CITYID" = "CITY"."ID") AS "MAXAGE",
             *      (SELECT AVG("USERS"."AGE") AS "C" FROM "USERS" "USERS" WHERE "USERS"."CITYID" = "CITY"."ID") AS "AVGAGE" 
             * FROM "CITY" "CITY"
             */

            ConsoleHelper.WriteLineAndReadKey();
        }

        public static void Insert()
        {
            /* User 实体打了序列标签，会自动获取序列值。返回主键 Id */
            int id = (int)context.Insert<User>(() => new User() { Name = "lu", Age = 18, Gender = Gender.Man, CityId = 1, OpTime = DateTime.Now });
            /*
             * SELECT "USERS_AUTOID"."NEXTVAL" FROM "DUAL"
             * Int32 :P_0 = 14;
               INSERT INTO "USERS"("NAME","AGE","GENDER","CITYID","OPTIME","ID") VALUES(N'lu',18,1,1,SYSTIMESTAMP,:P_0)
             */

            User user = new User();
            user.Name = "lu";
            user.Age = 18;
            user.Gender = Gender.Man;
            user.CityId = 1;
            user.OpTime = DateTime.Now;

            //会自动将自增 Id 设置到 user 的 Id 属性上
            user = context.Insert(user);
            /*
             * SELECT "USERS_AUTOID"."NEXTVAL" FROM "DUAL"
             * Int32 :P_0 = 15;
               String :P_1 = 'lu';
               Int32 :P_2 = 1;
               Int32 :P_3 = 18;
               DateTime :P_4 = '2016/9/5 9:16:59';
               INSERT INTO "USERS"("ID","NAME","GENDER","AGE","CITYID","OPTIME") VALUES(:P_0,:P_1,:P_2,:P_3,:P_2,:P_4)
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void Update()
        {
            context.Update<User>(a => a.Id == 1, a => new User() { Name = a.Name, Age = a.Age + 1, Gender = Gender.Man, OpTime = DateTime.Now });
            /*
             * UPDATE "USERS" SET "NAME"="USERS"."NAME","AGE"=("USERS"."AGE" + 1),"GENDER"=1,"OPTIME"=SYSTIMESTAMP WHERE "USERS"."ID" = 1
             */

            //批量更新
            //给所有女性年轻 1 岁
            context.Update<User>(a => a.Gender == Gender.Woman, a => new User() { Age = a.Age - 1, OpTime = DateTime.Now });
            /*
             * UPDATE "USERS" SET "AGE"=("USERS"."AGE" - 1),"OPTIME"=SYSTIMESTAMP WHERE "USERS"."GENDER" = 2
             */

            User user = new User();
            user.Id = 1;
            user.Name = "lu";
            user.Age = 28;
            user.Gender = Gender.Man;
            user.OpTime = DateTime.Now;

            context.Update(user); //会更新所有映射的字段
            /*
             * String :P_0 = 'lu';
               Int32 :P_1 = 1;
               Int32 :P_2 = 28;
               Nullable<Int32> :P_3 = NULL;
               DateTime :P_4 = '2016/9/5 9:20:07';
               UPDATE "USERS" SET "NAME"=:P_0,"GENDER"=:P_1,"AGE"=:P_2,"CITYID"=:P_3,"OPTIME"=:P_4 WHERE "USERS"."ID" = :P_1
             */


            /*
             * 支持只更新属性值已变的属性
             */

            context.TrackEntity(user);//在上下文中跟踪实体
            user.Name = user.Name + "1";
            context.Update(user);//这时只会更新被修改的字段
            /*
             * String :P_0 = 'lu1';
               Int32 :P_1 = 1;
               UPDATE "USERS" SET "NAME"=:P_0 WHERE "USERS"."ID" = :P_1
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void Delete()
        {
            context.Delete<User>(a => a.Id == 1);
            /*
             * DELETE FROM "USERS" WHERE "USERS"."ID" = 1
             */

            //批量删除
            //删除所有不男不女的用户
            context.Delete<User>(a => a.Gender == null);
            /*
             * DELETE FROM "USERS" WHERE "USERS"."GENDER" IS NULL
             */

            User user = new User();
            user.Id = 1;
            context.Delete(user);
            /*
             * Int32 :P_0 = 1;
               DELETE FROM "USERS" WHERE "USERS"."ID" = :P_0
             */

            ConsoleHelper.WriteLineAndReadKey(1);
        }

        public static void Method()
        {
            IQuery<User> q = context.Query<User>();

            var space = new char[] { ' ' };

            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddDays(1);
            var ret = q.Select(a => new
            {
                Id = a.Id,

                String_Length = (int?)a.Name.Length,//LENGTH("USERS"."NAME")
                Substring = a.Name.Substring(0),//SUBSTR("USERS"."NAME",0 + 1,LENGTH("USERS"."NAME"))
                Substring1 = a.Name.Substring(1),//SUBSTR("USERS"."NAME",1 + 1,LENGTH("USERS"."NAME"))
                Substring1_2 = a.Name.Substring(1, 2),//SUBSTR("USERS"."NAME",1 + 1,2)
                ToLower = a.Name.ToLower(),//LOWER("USERS"."NAME")
                ToUpper = a.Name.ToUpper(),//UPPER("USERS"."NAME")
                IsNullOrEmpty = string.IsNullOrEmpty(a.Name),//too long
                Contains = (bool?)a.Name.Contains("s"),//
                Trim = a.Name.Trim(),//TRIM("USERS"."NAME")
                TrimStart = a.Name.TrimStart(space),//LTRIM("USERS"."NAME")
                TrimEnd = a.Name.TrimEnd(space),//RTRIM("USERS"."NAME")
                StartsWith = (bool?)a.Name.StartsWith("s"),//
                EndsWith = (bool?)a.Name.EndsWith("s"),//
                Replace = a.Name.Replace("l", "L"),

                /* oracle is not supported DbFunctions.Diffxx. */
                //DiffYears = DbFunctions.DiffYears(startTime, endTime),//
                //DiffMonths = DbFunctions.DiffMonths(startTime, endTime),//
                //DiffDays = DbFunctions.DiffDays(startTime, endTime),//
                //DiffHours = DbFunctions.DiffHours(startTime, endTime),//
                //DiffMinutes = DbFunctions.DiffMinutes(startTime, endTime),//
                //DiffSeconds = DbFunctions.DiffSeconds(startTime, endTime),//
                //DiffMilliseconds = DbFunctions.DiffMilliseconds(startTime, endTime),//
                //DiffMicroseconds = DbFunctions.DiffMicroseconds(startTime, endTime),//

                /* ((CAST(:P_0 AS DATE)-CAST(:P_1 AS DATE)) * 86400000 + CAST(TO_CHAR(CAST(:P_0 AS TIMESTAMP),'ff3') AS NUMBER) - CAST(TO_CHAR(CAST(:P_1 AS TIMESTAMP),'ff3') AS NUMBER)) / 86400000 */
                SubtractTotalDays = endTime.Subtract(startTime).TotalDays,//
                SubtractTotalHours = endTime.Subtract(startTime).TotalHours,//...
                SubtractTotalMinutes = endTime.Subtract(startTime).TotalMinutes,//...
                SubtractTotalSeconds = endTime.Subtract(startTime).TotalSeconds,//...
                SubtractTotalMilliseconds = endTime.Subtract(startTime).TotalMilliseconds,//...

                AddYears = startTime.AddYears(1),//ADD_MONTHS(:P_0,12 * 1)
                AddMonths = startTime.AddMonths(1),//ADD_MONTHS(:P_0,1)
                AddDays = startTime.AddDays(1),//(:P_0 + 1)
                AddHours = startTime.AddHours(1),//(:P_0 + NUMTODSINTERVAL(1,'HOUR'))
                AddMinutes = startTime.AddMinutes(2),//(:P_0 + NUMTODSINTERVAL(2,'MINUTE'))
                AddSeconds = startTime.AddSeconds(120),//(:P_0 + NUMTODSINTERVAL(120,'SECOND'))
                                                       //AddMilliseconds = startTime.AddMilliseconds(20000),//不支持

                Now = DateTime.Now,//SYSTIMESTAMP
                UtcNow = DateTime.UtcNow,//SYS_EXTRACT_UTC(SYSTIMESTAMP)
                Today = DateTime.Today,//TRUNC(SYSDATE,'DD')
                Date = DateTime.Now.Date,//TRUNC(SYSTIMESTAMP,'DD')
                Year = DateTime.Now.Year,//CAST(TO_CHAR(SYSTIMESTAMP,'yyyy') AS NUMBER)
                Month = DateTime.Now.Month,//CAST(TO_CHAR(SYSTIMESTAMP,'mm') AS NUMBER)
                Day = DateTime.Now.Day,//CAST(TO_CHAR(SYSTIMESTAMP,'dd') AS NUMBER)
                Hour = DateTime.Now.Hour,//CAST(TO_CHAR(SYSTIMESTAMP,'hh24') AS NUMBER)
                Minute = DateTime.Now.Minute,//CAST(TO_CHAR(SYSTIMESTAMP,'mi') AS NUMBER)
                Second = DateTime.Now.Second,//CAST(TO_CHAR(SYSTIMESTAMP,'ss') AS NUMBER)
                Millisecond = DateTime.Now.Millisecond,//CAST(TO_CHAR(SYSTIMESTAMP,'ff3') AS NUMBER)
                DayOfWeek = DateTime.Now.DayOfWeek,//(CAST(TO_CHAR(SYSTIMESTAMP,'D') AS NUMBER) - 1)

                Int_Parse = int.Parse("1"),//CAST(N'1' AS NUMBER)
                Int16_Parse = Int16.Parse("11"),//CAST(N'11' AS NUMBER)
                Long_Parse = long.Parse("2"),//CAST(N'2' AS NUMBER)
                Double_Parse = double.Parse("3"),//CAST(N'3' AS BINARY_DOUBLE)
                Float_Parse = float.Parse("4"),//CAST(N'4' AS BINARY_FLOAT)
                Decimal_Parse = decimal.Parse("5"),//CAST(N'5' AS NUMBER)
                                                   //Guid_Parse = Guid.Parse("D544BC4C-739E-4CD3-A3D3-7BF803FCE179"),//不支持

                Bool_Parse = bool.Parse("1"),//
                DateTime_Parse = DateTime.Parse("1992-1-16"),//TO_TIMESTAMP(N'1992-1-16','yyyy-mm-dd hh24:mi:ssxff')

                B = a.Age == null ? false : a.Age > 1,
            }).ToList();

            ConsoleHelper.WriteLineAndReadKey();
        }

        public static void ExecuteCommandText()
        {
            List<User> users = context.SqlQuery<User>("select * from Users where Age > :age", DbParam.Create(":age", 12)).ToList();

            int rowsAffected = context.Session.ExecuteNonQuery("update Users set name=:name where Id = 1", DbParam.Create(":name", "Chloe"));

            /* 
             * 执行存储过程:
             * User user = context.SqlQuery<User>("Proc_GetUser", CommandType.StoredProcedure, DbParam.Create(":id", 1)).FirstOrDefault();
             * rowsAffected = context.Session.ExecuteNonQuery("Proc_UpdateUserName", CommandType.StoredProcedure, DbParam.Create(":name", "Chloe"));
             */

            ConsoleHelper.WriteLineAndReadKey();
        }

        public static void DoWithTransactionEx()
        {
            context.DoWithTransaction(() =>
            {
                context.Update<User>(a => a.Id == 1, a => new User() { Name = a.Name, Age = a.Age + 1, Gender = Gender.Man, OpTime = DateTime.Now });
                context.Delete<User>(a => a.Id == 1024);
            });

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void DoWithTransaction()
        {
            try
            {
                context.Session.BeginTransaction();

                /* do some things here */
                context.Update<User>(a => a.Id == 1, a => new User() { Name = a.Name, Age = a.Age + 1, Gender = Gender.Man, OpTime = DateTime.Now });
                context.Delete<User>(a => a.Id == 1024);

                context.Session.CommitTransaction();
            }
            catch
            {
                if (context.Session.IsInTransaction)
                    context.Session.RollbackTransaction();
                throw;
            }

            ConsoleHelper.WriteLineAndReadKey();
        }
    }
}