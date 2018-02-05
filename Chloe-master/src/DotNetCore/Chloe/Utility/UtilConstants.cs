using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe
{
    static class UtilConstants
    {
        public const string DefaultTableAlias = "T";
        public const string DefaultColumnAlias = "C";

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
        public static readonly Type TypeOfByteArray = typeof(Byte[]);


        public static readonly ConstantExpression Constant_Null_String = Expression.Constant(null, typeof(string));
        public static readonly ConstantExpression Constant_Empty_String = Expression.Constant(string.Empty);
        public static readonly ConstantExpression Constant_Null_Boolean = Expression.Constant(null, typeof(Boolean?));
        public static readonly ConstantExpression Constant_True = Expression.Constant(true);
        public static readonly ConstantExpression Constant_False = Expression.Constant(false);
        public static readonly UnaryExpression Convert_TrueToNullable = Expression.Convert(Expression.Constant(true), typeof(Boolean?));
        public static readonly UnaryExpression Convert_FalseToNullable = Expression.Convert(Expression.Constant(false), typeof(Boolean?));

        public static readonly MethodInfo MethodInfo_String_IsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty", new Type[] { typeof(string) });
    }
}
