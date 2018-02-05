using Chloe.Descriptors;
using Chloe.Exceptions;
using Chloe.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chloe
{
    public static partial class DbContextExtension
    {
        /// <summary>
        /// int id = 1;
        /// dbContext.FormatSqlQuery&lt;User&gt;($"select Id,Name from Users where Id={id}");
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IEnumerable<T> FormatSqlQuery<T>(this IDbContext dbContext, FormattableString sql)
        {
            /*
             * Usage:
             * int id = 1;
             * dbContext.FormatSqlQuery<User>($"select Id,Name from Users where Id={id}").ToList();
             */

            (string Sql, DbParam[] Parameters) r = BuildSqlAndParameters(dbContext, sql);
            return dbContext.SqlQuery<T>(r.Sql, r.Parameters);
        }
        public static IEnumerable<T> FormatSqlQuery<T>(this IDbContext dbContext, FormattableString sql, CommandType cmdType)
        {
            /*
             * Usage:
             * int id = 1;
             * dbContext.FormatSqlQuery<User>($"select Id,Name from Users where Id={id}").ToList();
             */

            (string Sql, DbParam[] Parameters) r = BuildSqlAndParameters(dbContext, sql);
            return dbContext.SqlQuery<T>(r.Sql, cmdType, r.Parameters);
        }
        public static Task<List<T>> FormatSqlQueryAsync<T>(this IDbContext dbContext, FormattableString sql)
        {
            return Utils.MakeTask(() => DbContextExtension.FormatSqlQuery<T>(dbContext, sql).ToList());
        }
        public static Task<List<T>> FormatSqlQueryAsync<T>(this IDbContext dbContext, FormattableString sql, CommandType cmdType)
        {
            return Utils.MakeTask(() => DbContextExtension.FormatSqlQuery<T>(dbContext, sql, cmdType).ToList());
        }

        static (string Sql, DbParam[] Parameters) BuildSqlAndParameters(IDbContext dbContext, FormattableString sql)
        {
            List<string> formatArgs = new List<string>(sql.ArgumentCount);
            List<DbParam> parameters = new List<DbParam>(sql.ArgumentCount);

            string parameterPrefix = Utils.GetParameterPrefix(dbContext) + "P_";

            foreach (var arg in sql.GetArguments())
            {
                object paramValue = arg;
                if (paramValue == null || paramValue == DBNull.Value)
                {
                    formatArgs.Add("NULL");
                    continue;
                }

                Type paramType = arg.GetType();

                if (paramType.IsEnum)
                {
                    paramType = Enum.GetUnderlyingType(paramType);
                    if (paramValue != null)
                        paramValue = Convert.ChangeType(paramValue, paramType);
                }

                DbParam p;
                p = parameters.Where(a => Utils.AreEqual(a.Value, paramValue)).FirstOrDefault();

                if (p != null)
                {
                    formatArgs.Add(p.Name);
                    continue;
                }

                string paramName = parameterPrefix + parameters.Count.ToString();
                p = DbParam.Create(paramName, paramValue, paramType);
                parameters.Add(p);
                formatArgs.Add(p.Name);
            }

            string runSql = string.Format(sql.Format, formatArgs.ToArray());
            return (runSql, parameters.ToArray());
        }
    }
}
