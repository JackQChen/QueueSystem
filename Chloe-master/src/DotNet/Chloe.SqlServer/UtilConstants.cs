using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.SqlServer
{
    static class UtilConstants
    {
        public const string DbParameterNamePrefix = "@";
        public static readonly string ParameterNamePrefix = DbParameterNamePrefix + "P_";

        public static readonly Type TypeOfVoid = typeof(void);
        public static readonly Type TypeOfInt16 = typeof(Int16);
        public static readonly Type TypeOfInt32 = typeof(Int32);
        public static readonly Type TypeOfInt64 = typeof(Int64);
        public static readonly Type TypeOfDecimal = typeof(Decimal);
        public static readonly Type TypeOfDouble = typeof(Double);
        public static readonly Type TypeOfSingle = typeof(Single);
        public static readonly Type TypeOfBoolean = typeof(Boolean);
        public static readonly Type TypeOfBoolean_Nullable = typeof(Boolean?);
        public static readonly Type TypeOfDateTime = typeof(DateTime);
        public static readonly Type TypeOfGuid = typeof(Guid);
        public static readonly Type TypeOfByte = typeof(Byte);
        public static readonly Type TypeOfChar = typeof(Char);
        public static readonly Type TypeOfString = typeof(String);
        public static readonly Type TypeOfObject = typeof(Object);
        public static readonly Type TypeOfTimeSpan = typeof(TimeSpan);

        public static readonly Type TypeOfMath = typeof(Math);
        public static readonly Type TypeOfSql = typeof(Sql);

        #region DbExpression constants

        public static readonly DbParameterExpression DbParameter_1 = DbExpression.Parameter(1);
        public static readonly DbConstantExpression DbConstant_Null_String = DbExpression.Constant(null, typeof(string));

        #endregion

        #region MemberInfo constants

        public static readonly PropertyInfo PropertyInfo_String_Length = typeof(string).GetProperty("Length");

        /* DateTime */
        public static readonly PropertyInfo PropertyInfo_DateTime_Now = typeof(DateTime).GetProperty("Now");
        public static readonly PropertyInfo PropertyInfo_DateTime_UtcNow = typeof(DateTime).GetProperty("UtcNow");
        public static readonly PropertyInfo PropertyInfo_DateTime_Today = typeof(DateTime).GetProperty("Today");
        public static readonly PropertyInfo PropertyInfo_DateTime_Date = typeof(DateTime).GetProperty("Date");
        public static readonly PropertyInfo PropertyInfo_DateTime_Year = typeof(DateTime).GetProperty("Year");
        public static readonly PropertyInfo PropertyInfo_DateTime_Month = typeof(DateTime).GetProperty("Month");
        public static readonly PropertyInfo PropertyInfo_DateTime_Day = typeof(DateTime).GetProperty("Day");
        public static readonly PropertyInfo PropertyInfo_DateTime_Hour = typeof(DateTime).GetProperty("Hour");
        public static readonly PropertyInfo PropertyInfo_DateTime_Minute = typeof(DateTime).GetProperty("Minute");
        public static readonly PropertyInfo PropertyInfo_DateTime_Second = typeof(DateTime).GetProperty("Second");
        public static readonly PropertyInfo PropertyInfo_DateTime_Millisecond = typeof(DateTime).GetProperty("Millisecond");
        public static readonly PropertyInfo PropertyInfo_DateTime_DayOfWeek = typeof(DateTime).GetProperty("DayOfWeek");


        /* String */
        public static readonly MethodInfo MethodInfo_String_Concat_String_String = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        public static readonly MethodInfo MethodInfo_String_Concat_Object_Object = typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) });
        public static readonly MethodInfo MethodInfo_String_Trim = typeof(string).GetMethod("Trim", Type.EmptyTypes);
        public static readonly MethodInfo MethodInfo_String_TrimStart = typeof(string).GetMethod("TrimStart", new Type[] { typeof(char[]) });
        public static readonly MethodInfo MethodInfo_String_TrimEnd = typeof(string).GetMethod("TrimEnd", new Type[] { typeof(char[]) });
        public static readonly MethodInfo MethodInfo_String_StartsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        public static readonly MethodInfo MethodInfo_String_EndsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        public static readonly MethodInfo MethodInfo_String_Contains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        public static readonly MethodInfo MethodInfo_String_IsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty", new Type[] { typeof(string) });
        public static readonly MethodInfo MethodInfo_String_ToUpper = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
        public static readonly MethodInfo MethodInfo_String_ToLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        public static readonly MethodInfo MethodInfo_String_Substring_Int32 = typeof(string).GetMethod("Substring", new Type[] { typeof(Int32) });
        public static readonly MethodInfo MethodInfo_String_Substring_Int32_Int32 = typeof(string).GetMethod("Substring", new Type[] { typeof(Int32), typeof(Int32) });
        public static readonly MethodInfo MethodInfo_String_Replace = typeof(string).GetMethod("Replace", new Type[] { typeof(string), typeof(string) });

        public static readonly MethodInfo MethodInfo_Guid_NewGuid = typeof(Guid).GetMethod("NewGuid");

        /* DbFunctions */
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffYears = typeof(DbFunctions).GetMethod("DiffYears");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffMonths = typeof(DbFunctions).GetMethod("DiffMonths");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffDays = typeof(DbFunctions).GetMethod("DiffDays");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffHours = typeof(DbFunctions).GetMethod("DiffHours");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffMinutes = typeof(DbFunctions).GetMethod("DiffMinutes");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffSeconds = typeof(DbFunctions).GetMethod("DiffSeconds");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffMilliseconds = typeof(DbFunctions).GetMethod("DiffMilliseconds");
        public static readonly MethodInfo MethodInfo_DbFunctions_DiffMicroseconds = typeof(DbFunctions).GetMethod("DiffMicroseconds");

        /* Sql */
        public static readonly MethodInfo MethodInfo_Sql_Equals = typeof(Sql).GetMethods().Where(a => a.Name == "Equals" && a.IsStatic && a.IsGenericMethod).First();
        public static readonly MethodInfo MethodInfo_Sql_NotEquals = typeof(Sql).GetMethod("NotEquals");
        #endregion

    }
}
