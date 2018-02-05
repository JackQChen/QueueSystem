using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.InternalExtensions
{
    public static class DbExpressionExtension
    {
        public static DbExpression StripConvert(this DbExpression exp)
        {
            while (exp.NodeType == DbExpressionType.Convert)
            {
                exp = ((DbConvertExpression)exp).Operand;
            }

            return exp;
        }
        public static DbExpression StripInvalidConvert(this DbExpression exp)
        {
            if (exp.NodeType != DbExpressionType.Convert)
                return exp;

            DbConvertExpression convertExpression = (DbConvertExpression)exp;

            if (convertExpression.Type.IsEnum)
            {
                //(enumType)123
                if (typeof(int) == convertExpression.Operand.Type)
                    return StripInvalidConvert(convertExpression.Operand);

                DbConvertExpression newExp = new DbConvertExpression(typeof(int), convertExpression.Operand);
                return StripInvalidConvert(newExp);
            }

            Type underlyingType;

            //(int?)123
            if (ReflectionExtension.IsNullable(convertExpression.Type, out underlyingType))//可空类型转换
            {
                if (underlyingType == convertExpression.Operand.Type)
                    return StripInvalidConvert(convertExpression.Operand);

                DbConvertExpression newExp = new DbConvertExpression(underlyingType, convertExpression.Operand);
                return StripInvalidConvert(newExp);
            }

            //(int)enumTypeValue
            if (exp.Type == typeof(int))
            {
                //(int)enumTypeValue
                if (convertExpression.Operand.Type.IsEnum)
                    return StripInvalidConvert(convertExpression.Operand);

                //(int)NullableEnumTypeValue
                if (ReflectionExtension.IsNullable(convertExpression.Operand.Type, out underlyingType) && underlyingType.IsEnum)
                    return StripInvalidConvert(convertExpression.Operand);
            }

            //float long double and so on
            if (exp.Type.IsValueType)
            {
                //(long)NullableValue
                if (ReflectionExtension.IsNullable(convertExpression.Operand.Type, out underlyingType) && underlyingType == exp.Type)
                    return StripInvalidConvert(convertExpression.Operand);
            }

            if (convertExpression.Type == convertExpression.Operand.Type)
            {
                return StripInvalidConvert(convertExpression.Operand);
            }

            //如果是子类向父类转换
            if (exp.Type.IsAssignableFrom(convertExpression.Operand.Type))
                return StripInvalidConvert(convertExpression.Operand);

            return convertExpression;
        }


        /// <summary>
        /// 尝试将 exp 转换成 DbParameterExpression。
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryConvertToParameterExpression(this DbMemberExpression exp, out DbParameterExpression val)
        {
            val = null;
            if (!exp.IsEvaluable())
                return false;

            //求值
            val = exp.ConvertToParameterExpression();
            return true;
        }

        public static bool IsEvaluable(this DbMemberExpression memberExpression)
        {
            if (memberExpression == null)
                throw new ArgumentNullException("memberExpression");

            do
            {
                DbExpression prevExp = memberExpression.Expression;

                // prevExp == null 表示是静态成员
                if (prevExp == null || prevExp is DbConstantExpression)
                    return true;

                DbMemberExpression memberExp = prevExp as DbMemberExpression;
                if (memberExp == null)
                    return false;
                else
                    memberExpression = memberExp;

            } while (true);
        }

        /// <summary>
        /// 对 memberExpression 进行求值
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>返回 DbParameterExpression</returns>
        public static DbParameterExpression ConvertToParameterExpression(this DbMemberExpression memberExpression)
        {
            DbParameterExpression ret = null;
            //求值
            object val = Evaluate(memberExpression);

            ret = DbExpression.Parameter(val, memberExpression.Type);

            return ret;
        }

        /// <summary>
        /// 判定 exp 返回值肯定是 null
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNull(this DbExpression exp)
        {
            exp = DbExpressionExtension.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return c.Value == null || c.Value == DBNull.Value;
            }

            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return p.Value == null || p.Value == DBNull.Value;
            }

            return false;
        }
        /// <summary>
        /// 判定 exp 返回值肯定不是 null
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNotNull(this DbExpression exp)
        {
            exp = DbExpressionExtension.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return c.Value != null && c.Value != DBNull.Value;
            }

            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return p.Value != null && p.Value != DBNull.Value;
            }

            return false;
        }

        public static object Evaluate(this DbMemberExpression exp, object instance)
        {
            if (exp.Member.MemberType
              == MemberTypes.Field)
            {
                return ((FieldInfo)exp.Member).GetValue(instance);
            }
            else if (exp.Member.MemberType
                     == MemberTypes.Property)
            {
                return ((PropertyInfo)exp.Member).GetValue(instance, null);
            }

            throw new NotSupportedException();
        }
        public static object Evaluate(this DbExpression exp)
        {
            if (exp.NodeType == DbExpressionType.Constant)
                return ((DbConstantExpression)exp).Value;

            if (exp.NodeType == DbExpressionType.MemberAccess)
            {
                DbMemberExpression m = (DbMemberExpression)exp;
                object instance = null;
                if (m.Expression != null)
                {
                    instance = Evaluate(m.Expression);

                    /* 非静态成员，需要检查是否为空引用。Nullable<T>.HasValue 的情况比较特俗，暂不考虑 */
                    Type declaringType = m.Member.DeclaringType;
                    if (declaringType.IsClass || declaringType.IsInterface)
                    {
                        if (instance == null)
                            throw new NullReferenceException(string.Format("There is an object reference not set to an instance of an object in expression tree.The type of null object is '{0}'.", declaringType.FullName));
                    }
                }

                return Evaluate(m, instance);
            }

            throw new NotSupportedException();
        }
    }
}
