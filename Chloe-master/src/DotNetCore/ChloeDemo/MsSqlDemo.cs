using Chloe;
using Chloe.Core;
using Chloe.SqlServer;
using Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChloeDemo
{
    //[TestClass]
    public class MsSqlDemo
    {
        /* WARNING: DbContext 是非线程安全的，正式使用不能设置为 static，并且用完务必要调用 Dispose 方法销毁对象 */
        static MsSqlContext context = new MsSqlContext(DbHelper.ConnectionString);

        public static void Run()
        {
            BasicQuery();
            JoinQuery();
            AggregateQuery();
            GroupQuery();
            ComplexQuery();  /* v2.18复杂查询 */
            Insert();
            BulkInsert();
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
             * SELECT TOP (1) [Users].[Id] AS [Id],[Users].[Name] AS [Name],[Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime] FROM [Users] AS [Users] WHERE [Users].[Id] = 1
             */


            //可以选取指定的字段
            q.Where(a => a.Id == 1).Select(a => new { a.Id, a.Name }).FirstOrDefault();
            /*
             * SELECT TOP (1) [Users].[Id] AS [Id],[Users].[Name] AS [Name] FROM [Users] AS [Users] WHERE [Users].[Id] = 1
             */


            //分页
            q.Where(a => a.Id > 0).OrderBy(a => a.Age).Skip(20).Take(10).ToList();
            /*
             * SELECT TOP (10) [T].[Id] AS [Id],[T].[Name] AS [Name],[T].[Gender] AS [Gender],[T].[Age] AS [Age],[T].[CityId] AS [CityId],[T].[OpTime] AS [OpTime] FROM (SELECT [Users].[Id] AS [Id],[Users].[Name] AS [Name],[Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],ROW_NUMBER() OVER(ORDER BY [Users].[Age] ASC) AS [ROW_NUMBER_0] FROM [Users] AS [Users] WHERE [Users].[Id] > 0) AS [T] WHERE [T].[ROW_NUMBER_0] > 20
             */


            /* like 查询 */
            q.Where(a => a.Name.Contains("so") || a.Name.StartsWith("s") || a.Name.EndsWith("o")).ToList();
            /*
             * SELECT 
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name] 
             * FROM [Users] AS [Users] 
             * WHERE ([Users].[Name] LIKE '%' + N'so' + '%' OR [Users].[Name] LIKE N's' + '%' OR [Users].[Name] LIKE '%' + N'o')
             */


            /* in 一个数组 */
            List<User> users = null;
            List<int> userIds = new List<int>() { 1, 2, 3 };
            users = q.Where(a => userIds.Contains(a.Id)).ToList(); /* list.Contains() 方法组合就会生成 in一个数组 sql 语句 */
            /*
             * SELECT 
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name]
             * FROM [Users] AS [Users] 
             * WHERE [Users].[Id] IN (1,2,3)
             */


            /* in 子查询 */
            users = q.Where(a => context.Query<City>().Select(c => c.Id).ToList().Contains((int)a.CityId)).ToList(); /* IQuery<T>.ToList().Contains() 方法组合就会生成 in 子查询 sql 语句 */
            /*
             * SELECT 
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name]
             * FROM [Users] AS [Users] 
             * WHERE [Users].[CityId] IN (SELECT [City].[Id] AS [C] FROM [City] AS [City])
             */


            /* distinct 查询 */
            q.Select(a => new { a.Name }).Distinct().ToList();
            /*
             * SELECT DISTINCT [Users].[Name] AS [Name] FROM [Users] AS [Users]
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void JoinQuery()
        {
            //建立连接
            var user_city_province = context.Query<User>()
                                    .InnerJoin<City>((user, city) => user.CityId == city.Id)
                                    .InnerJoin<Province>((user, city, province) => city.ProvinceId == province.Id);

            //查出用户及其隶属的城市和省份的所有信息
            var view = user_city_province.Select((user, city, province) => new { User = user, City = city, Province = province }).Where(a => a.User.Id > 1).ToList();
            /*
             * SELECT [Users].[Id] AS [Id],[Users].[Name] AS [Name],[Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[City].[Id] AS [Id0],[City].[Name] AS [Name0],[City].[ProvinceId] AS [ProvinceId],[Province].[Id] AS [Id1],[Province].[Name] AS [Name1] FROM [Users] AS [Users] INNER JOIN [City] AS [City] ON [Users].[CityId] = [City].[Id] INNER JOIN [Province] AS [Province] ON [City].[ProvinceId] = [Province].[Id] WHERE [Users].[Id] > 1
             */

            //也可以只获取指定的字段信息：UserId,UserName,CityName,ProvinceName
            user_city_province.Select((user, city, province) => new { UserId = user.Id, UserName = user.Name, CityName = city.Name, ProvinceName = province.Name }).Where(a => a.UserId > 1).ToList();
            /*
             * SELECT [Users].[Id] AS [UserId],[Users].[Name] AS [UserName],[City].[Name] AS [CityName],[Province].[Name] AS [ProvinceName] FROM [Users] AS [Users] INNER JOIN [City] AS [City] ON [Users].[CityId] = [City].[Id] INNER JOIN [Province] AS [Province] ON [City].[ProvinceId] = [Province].[Id] WHERE [Users].[Id] > 1
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
             * SELECT TOP (1) COUNT(1) AS [C] FROM [Users] AS [Users]
             */

            q.Select(a => new { Count = AggregateFunctions.Count(), LongCount = AggregateFunctions.LongCount(), Sum = AggregateFunctions.Sum(a.Age), Max = AggregateFunctions.Max(a.Age), Min = AggregateFunctions.Min(a.Age), Average = AggregateFunctions.Average(a.Age) }).First();
            /*
             * SELECT TOP (1) COUNT(1) AS [Count],COUNT_BIG(1) AS [LongCount],CAST(SUM([Users].[Age]) AS INT) AS [Sum],MAX([Users].[Age]) AS [Max],MIN([Users].[Age]) AS [Min],CAST(AVG([Users].[Age]) AS FLOAT) AS [Average] FROM [Users] AS [Users]
             */

            var count = q.Count();
            /*
             * SELECT COUNT(1) AS [C] FROM [Users] AS [Users]
             */

            var longCount = q.LongCount();
            /*
             * SELECT COUNT_BIG(1) AS [C] FROM [Users] AS [Users]
             */

            var sum = q.Sum(a => a.Age);
            /*
             * SELECT CAST(SUM([Users].[Age]) AS INT) AS [C] FROM [Users] AS [Users]
             */

            var max = q.Max(a => a.Age);
            /*
             * SELECT MAX([Users].[Age]) AS [C] FROM [Users] AS [Users]
             */

            var min = q.Min(a => a.Age);
            /*
             * SELECT MIN([Users].[Age]) AS [C] FROM [Users] AS [Users]
             */

            var avg = q.Average(a => a.Age);
            /*
             * SELECT CAST(AVG([Users].[Age]) AS FLOAT) AS [C] FROM [Users] AS [Users]
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
             * SELECT [Users].[Age] AS [Age],COUNT(1) AS [Count],CAST(SUM([Users].[Age]) AS INT) AS [Sum],MAX([Users].[Age]) AS [Max],MIN([Users].[Age]) AS [Min],CAST(AVG([Users].[Age]) AS FLOAT) AS [Avg] FROM [Users] AS [Users] WHERE [Users].[Id] > 0 GROUP BY [Users].[Age] HAVING ([Users].[Age] > 1 AND COUNT(1) > 0)
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
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name] 
             * FROM [Users] AS [Users] 
             * WHERE [Users].[Id] IN (1,2,3)
             */


            /* in 子查询 */
            users = userQuery.Where(a => cityQuery.Select(c => c.Id).ToList().Contains((int)a.CityId)).ToList();  /* IQuery<T>.ToList().Contains() 方法组合就会生成 in 子查询 sql 语句 */
            /*
             * SELECT 
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name] 
             * FROM [Users] AS [Users] 
             * WHERE [Users].[CityId] IN (SELECT [City].[Id] AS [C] FROM [City] AS [City])
             */


            /* IQuery<T>.Any() 方法组合就会生成 exists 子查询 sql 语句 */
            users = userQuery.Where(a => cityQuery.Where(c => c.Id == a.CityId).Any()).ToList();
            /*
             * SELECT 
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name] 
             * FROM [Users] AS [Users] 
             * WHERE Exists (SELECT N'1' AS [C] FROM [City] AS [City] WHERE [City].[Id] = [Users].[CityId])
             */


            /* select 子查询 */
            var result = userQuery.Select(a => new
            {
                CityName = cityQuery.Where(c => c.Id == a.CityId).First().Name,
                User = a
            }).ToList();
            /*
             * SELECT 
             *      (SELECT TOP (1) [City].[Name] AS [C] FROM [City] AS [City] WHERE [City].[Id] = [Users].[CityId]) AS [CityName],
             *      [Users].[Gender] AS [Gender],[Users].[Age] AS [Age],[Users].[CityId] AS [CityId],[Users].[OpTime] AS [OpTime],[Users].[Id] AS [Id],[Users].[Name] AS [Name] 
             * FROM [Users] AS [Users]
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
             *      (SELECT COUNT(1) AS [C] FROM [Users] AS [Users] WHERE [Users].[CityId] = [City].[Id]) AS [UserCount],
             *      (SELECT MAX([Users].[Age]) AS [C] FROM [Users] AS [Users] WHERE [Users].[CityId] = [City].[Id]) AS [MaxAge],
             *      (SELECT CAST(AVG([Users].[Age]) AS FLOAT) AS [C] FROM [Users] AS [Users] WHERE [Users].[CityId] = [City].[Id]) AS [AvgAge]
             * FROM [City] AS [City]
             */

            ConsoleHelper.WriteLineAndReadKey();
        }

        public static void Insert()
        {
            //返回主键 Id
            int id = (int)context.Insert<User>(() => new User() { Name = "lu", Age = 18, Gender = Gender.Man, CityId = 1, OpTime = DateTime.Now });
            /*
             * INSERT INTO [Users]([Name],[Age],[Gender],[CityId],[OpTime]) VALUES(N'lu',18,1,1,GETDATE());SELECT @@IDENTITY
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
             * String @P_0 = 'lu';
               Gender @P_1 = Man;
               Int32 @P_2 = 18;
               Int32 @P_3 = 1;
               DateTime @P_4 = '2016/8/26 18:41:08';
               INSERT INTO [Users]([Name],[Gender],[Age],[CityId],[OpTime]) VALUES(@P_0,@P_1,@P_2,@P_3,@P_4);SELECT @@IDENTITY
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void BulkInsert()
        {
            List<User> models = new List<User>();
            models.Add(new User() { Name = "lu", Age = 18, Gender = Gender.Woman, CityId = 1, OpTime = DateTime.Now });
            models.Add(new User() { Name = "shuxin", Age = 18, Gender = Gender.Man, CityId = 1, OpTime = DateTime.Now });

            /* 利用 SqlBulkCopy 批量插入数据 */
            context.BulkInsert(models, batchSize: null, bulkCopyTimeout: null, keepIdentity: false);

            ConsoleHelper.WriteLineAndReadKey(1);
        }
        public static void Update()
        {
            context.Update<User>(a => a.Id == 1, a => new User() { Name = a.Name, Age = a.Age + 1, Gender = Gender.Man, OpTime = DateTime.Now });
            /*
             * UPDATE [Users] SET [Name]=[Users].[Name],[Age]=([Users].[Age] + 1),[Gender]=1,[OpTime]=GETDATE() WHERE [Users].[Id] = 1
             */

            //批量更新
            //给所有女性年轻 1 岁
            context.Update<User>(a => a.Gender == Gender.Woman, a => new User() { Age = a.Age - 1, OpTime = DateTime.Now });
            /*
             * UPDATE [Users] SET [Age]=([Users].[Age] - 1),[OpTime]=GETDATE() WHERE [Users].[Gender] = 2
             */

            User user = new User();
            user.Id = 1;
            user.Name = "lu";
            user.Age = 28;
            user.Gender = Gender.Man;
            user.OpTime = DateTime.Now;

            context.Update(user); //会更新所有映射的字段
            /*
             * String @P_0 = 'lu';
               Gender @P_1 = Man;
               Int32 @P_2 = 28;
               Nullable<Int32> @P_3 = NULL;
               DateTime @P_4 = '2016/8/26 18:18:36';
               Int32 @P_5 = 1;
               UPDATE [Users] SET [Name]=@P_0,[Gender]=@P_1,[Age]=@P_2,[CityId]=@P_3,[OpTime]=@P_4 WHERE [Users].[Id] = @P_5
             */


            /*
             * 支持只更新属性值已变的属性
             */

            context.TrackEntity(user);//在上下文中跟踪实体
            user.Name = user.Name + "1";
            context.Update(user);//这时只会更新被修改的字段
            /*
             * String @P_0 = 'lu1';
               Int32 @P_1 = 1;
               UPDATE [Users] SET [Name]=@P_0 WHERE [Users].[Id] = @P_1
             */

            ConsoleHelper.WriteLineAndReadKey();
        }
        public static void Delete()
        {
            context.Delete<User>(a => a.Id == 1);
            /*
             * DELETE [Users] WHERE [Users].[Id] = 1
             */

            //批量删除
            //删除所有不男不女的用户
            context.Delete<User>(a => a.Gender == null);
            /*
             * DELETE [Users] FROM [Users] WHERE [Users].[Gender] IS NULL
             */

            User user = new User();
            user.Id = 1;
            context.Delete(user);
            /*
             * Int32 @P_0 = 1;
               DELETE [Users] WHERE [Users].[Id] = @P_0
             */

            ConsoleHelper.WriteLineAndReadKey(1);
        }

        public static void Method()
        {
            IQuery<User> q = context.Query<User>();

            var space = new char[] { ' ' };

            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddDays(1);
            q.Select(a => new
            {
                Id = a.Id,

                String_Length = (int?)a.Name.Length,//LEN([Users].[Name])
                Substring = a.Name.Substring(0),//SUBSTRING([Users].[Name],0 + 1,LEN([Users].[Name]))
                Substring1 = a.Name.Substring(1),//SUBSTRING([Users].[Name],1 + 1,LEN([Users].[Name]))
                Substring1_2 = a.Name.Substring(1, 2),//SUBSTRING([Users].[Name],1 + 1,2)
                ToLower = a.Name.ToLower(),//LOWER([Users].[Name])
                ToUpper = a.Name.ToUpper(),//UPPER([Users].[Name])
                IsNullOrEmpty = string.IsNullOrEmpty(a.Name),//too long
                Contains = (bool?)a.Name.Contains("s"),//
                Trim = a.Name.Trim(),//RTRIM(LTRIM([Users].[Name]))
                TrimStart = a.Name.TrimStart(space),//LTRIM([Users].[Name])
                TrimEnd = a.Name.TrimEnd(space),//RTRIM([Users].[Name])
                StartsWith = (bool?)a.Name.StartsWith("s"),//
                EndsWith = (bool?)a.Name.EndsWith("s"),//
                Replace = a.Name.Replace("l", "L"),

                DiffYears = DbFunctions.DiffYears(startTime, endTime),//DATEDIFF(YEAR,@P_0,@P_1)
                DiffMonths = DbFunctions.DiffMonths(startTime, endTime),//DATEDIFF(MONTH,@P_0,@P_1)
                DiffDays = DbFunctions.DiffDays(startTime, endTime),//DATEDIFF(DAY,@P_0,@P_1)
                DiffHours = DbFunctions.DiffHours(startTime, endTime),//DATEDIFF(HOUR,@P_0,@P_1)
                DiffMinutes = DbFunctions.DiffMinutes(startTime, endTime),//DATEDIFF(MINUTE,@P_0,@P_1)
                DiffSeconds = DbFunctions.DiffSeconds(startTime, endTime),//DATEDIFF(SECOND,@P_0,@P_1)
                DiffMilliseconds = DbFunctions.DiffMilliseconds(startTime, endTime),//DATEDIFF(MILLISECOND,@P_0,@P_1)
                //DiffMicroseconds = DbFunctions.DiffMicroseconds(startTime, endTime),//DATEDIFF(MICROSECOND,@P_0,@P_1)  Exception

                /* No longer support method 'DateTime.Subtract(DateTime d)', instead of using 'DbFunctions.DiffXX' */
                //SubtractTotalDays = endTime.Subtract(startTime).TotalDays,//CAST(DATEDIFF(DAY,@P_0,@P_1)
                //SubtractTotalHours = endTime.Subtract(startTime).TotalHours,//CAST(DATEDIFF(HOUR,@P_0,@P_1)
                //SubtractTotalMinutes = endTime.Subtract(startTime).TotalMinutes,//CAST(DATEDIFF(MINUTE,@P_0,@P_1)
                //SubtractTotalSeconds = endTime.Subtract(startTime).TotalSeconds,//CAST(DATEDIFF(SECOND,@P_0,@P_1)
                //SubtractTotalMilliseconds = endTime.Subtract(startTime).TotalMilliseconds,//CAST(DATEDIFF(MILLISECOND,@P_0,@P_1)

                AddYears = startTime.AddYears(1),//DATEADD(YEAR,1,@P_0)
                AddMonths = startTime.AddMonths(1),//DATEADD(MONTH,1,@P_0)
                AddDays = startTime.AddDays(1),//DATEADD(DAY,1,@P_0)
                AddHours = startTime.AddHours(1),//DATEADD(HOUR,1,@P_0)
                AddMinutes = startTime.AddMinutes(2),//DATEADD(MINUTE,2,@P_0)
                AddSeconds = startTime.AddSeconds(120),//DATEADD(SECOND,120,@P_0)
                AddMilliseconds = startTime.AddMilliseconds(20000),//DATEADD(MILLISECOND,20000,@P_0)

                Now = DateTime.Now,//GETDATE()
                UtcNow = DateTime.UtcNow,//GETUTCDATE()
                Today = DateTime.Today,//CAST(GETDATE() AS DATE)
                Date = DateTime.Now.Date,//CAST(GETDATE() AS DATE)
                Year = DateTime.Now.Year,//DATEPART(YEAR,GETDATE())
                Month = DateTime.Now.Month,//DATEPART(MONTH,GETDATE())
                Day = DateTime.Now.Day,//DATEPART(DAY,GETDATE())
                Hour = DateTime.Now.Hour,//DATEPART(HOUR,GETDATE())
                Minute = DateTime.Now.Minute,//DATEPART(MINUTE,GETDATE())
                Second = DateTime.Now.Second,//DATEPART(SECOND,GETDATE())
                Millisecond = DateTime.Now.Millisecond,//DATEPART(MILLISECOND,GETDATE())
                DayOfWeek = DateTime.Now.DayOfWeek,//(DATEPART(WEEKDAY,GETDATE()) - 1)

                Int_Parse = int.Parse("1"),//CAST(N'1' AS INT)
                Int16_Parse = Int16.Parse("11"),//CAST(N'11' AS SMALLINT)
                Long_Parse = long.Parse("2"),//CAST(N'2' AS BIGINT)
                Double_Parse = double.Parse("3"),//CAST(N'3' AS FLOAT)
                Float_Parse = float.Parse("4"),//CAST(N'4' AS REAL)
                //Decimal_Parse = decimal.Parse("5"),//CAST(N'5' AS DECIMAL)  ps: 'Decimal.Parse(string s)' is not supported now,because we don't know the precision and scale information.
                Guid_Parse = Guid.Parse("D544BC4C-739E-4CD3-A3D3-7BF803FCE179"),//CAST(N'D544BC4C-739E-4CD3-A3D3-7BF803FCE179' AS UNIQUEIDENTIFIER) AS [Guid_Parse]

                Bool_Parse = bool.Parse("1"),//CASE WHEN CAST(N'1' AS BIT) = CAST(1 AS BIT) THEN CAST(1 AS BIT) WHEN NOT (CAST(N'1' AS BIT) = CAST(1 AS BIT)) THEN CAST(0 AS BIT) ELSE NULL END AS [Bool_Parse]
                DateTime_Parse = DateTime.Parse("1992-1-16"),//CAST(N'1992-1-16' AS DATETIME) AS [DateTime_Parse]

                B = a.Age == null ? false : a.Age > 1,
            }).ToList();

            ConsoleHelper.WriteLineAndReadKey();
        }

        public static void ExecuteCommandText()
        {
            List<User> users = context.SqlQuery<User>("select * from Users where Age > @age", DbParam.Create("@age", 12)).ToList();

            int rowsAffected = context.Session.ExecuteNonQuery("update Users set name=@name where Id = 1", DbParam.Create("@name", "Chloe"));

            /* 
             * 执行存储过程:
             * User user = context.SqlQuery<User>("Proc_GetUser", CommandType.StoredProcedure, DbParam.Create("@id", 1)).FirstOrDefault();
             * rowsAffected = context.Session.ExecuteNonQuery("Proc_UpdateUserName", CommandType.StoredProcedure, DbParam.Create("@name", "Chloe"));
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
