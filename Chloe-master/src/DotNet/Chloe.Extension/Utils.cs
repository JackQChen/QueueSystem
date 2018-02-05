using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Extension
{
    static class Utils
    {
        public static void CheckNull(object obj, string paramName = null)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
        public static Task<T> MakeTask<T>(Func<T> func)
        {
            var task = new Task<T>(func);
            task.Start();
            return task;
        }

        public static string GetParameterPrefix(IDbContext dbContext)
        {
            Type dbContextType = dbContext.GetType();
            while (true)
            {
                if (dbContextType == null)
                    break;

                string dbContextTypeName = dbContextType.Name;
                switch (dbContextTypeName)
                {
                    case "MsSqlContext":
                    case "SQLiteContext":
                        return "@";
                    case "MySqlContext":
                        return "?";
                    case "OracleContext":
                        return ":";
                    default:
                        dbContextType = dbContextType.BaseType;
                        break;
                }
            }

            throw new NotSupportedException(dbContext.GetType().FullName);
        }
        public static DbParam[] BuildParams(IDbContext dbContext, object parameter)
        {
            List<DbParam> parameters = new List<DbParam>();

            if (parameter != null)
            {
                string parameterPrefix = GetParameterPrefix(dbContext);
                Type parameterType = parameter.GetType();
                var props = parameterType.GetProperties();
                foreach (var prop in props)
                {
                    if (prop.GetGetMethod() == null)
                    {
                        continue;
                    }

                    object value = ReflectionExtension.GetMemberValue(prop, parameter);

                    string paramName = parameterPrefix + prop.Name;

                    DbParam p = new DbParam(paramName, value, prop.PropertyType);
                    parameters.Add(p);
                }
            }

            return parameters.ToArray();
        }
    }
}
