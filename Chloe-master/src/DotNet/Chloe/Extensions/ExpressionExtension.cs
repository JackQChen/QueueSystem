using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Extensions
{
    public static class ExpressionExtension
    {
        public static BinaryExpression Assign(MemberInfo propertyOrField, Expression instance, Expression value)
        {
            PropertyInfo propertyInfo = propertyOrField as PropertyInfo;
            if (propertyInfo != null)
            {
                var pro = Expression.Property(instance, propertyInfo);
                var setValue = Expression.Assign(pro, value);
                return setValue;
            }

            FieldInfo fieldInfo = propertyOrField as FieldInfo;
            if (fieldInfo != null)
            {
                var field = Expression.Field(instance, fieldInfo);
                var setValue = Expression.Assign(field, value);
                return setValue;
            }

            throw new ArgumentException();
        }

        internal static bool IsDerivedFromParameter(this MemberExpression exp)
        {
            ParameterExpression p;
            return IsDerivedFromParameter(exp, out p);
        }
        internal static bool IsDerivedFromParameter(this MemberExpression exp, out ParameterExpression p)
        {
            p = null;
            Expression prevExp = exp.Expression;
            MemberExpression memberExp = prevExp as MemberExpression;
            while (memberExp != null)
            {
                prevExp = memberExp.Expression;
                memberExp = prevExp as MemberExpression;
            }

            if (prevExp == null)/* 静态属性访问 */
                return false;

            if (prevExp.NodeType == ExpressionType.Parameter)
            {
                p = (ParameterExpression)prevExp;
                return true;
            }

            /* 当实体继承于某个接口或类时，会有这种情况 */
            if (prevExp.NodeType == ExpressionType.Convert)
            {
                prevExp = ((UnaryExpression)prevExp).Operand;
                if (prevExp.NodeType == ExpressionType.Parameter)
                {
                    p = (ParameterExpression)prevExp;
                    return true;
                }
            }

            return false;
        }

        public static Expression StripQuotes(this Expression exp)
        {
            while (exp.NodeType == ExpressionType.Quote)
            {
                exp = ((UnaryExpression)exp).Operand;
            }
            return exp;
        }

        public static Expression StripConvert(this Expression exp)
        {
            Expression operand = exp;
            while (operand.NodeType == ExpressionType.Convert || operand.NodeType == ExpressionType.ConvertChecked)
            {
                operand = ((UnaryExpression)operand).Operand;
            }
            return operand;
        }

        public static Stack<MemberExpression> Reverse(this MemberExpression exp)
        {
            var stack = new Stack<MemberExpression>();
            stack.Push(exp);
            while ((exp = exp.Expression as MemberExpression) != null)
            {
                stack.Push(exp);
            }
            return stack;
        }

        public static Expression MakeWrapperAccess(object value, Type targetType)
        {
            if (value == null)
            {
                if (targetType != null)
                    return Expression.Constant(value, targetType);
                else
                    return Expression.Constant(value, typeof(object));
            }

            object wrapper = WrapValue(value);
            ConstantExpression wrapperConstantExp = Expression.Constant(wrapper);
            Expression ret = Expression.MakeMemberAccess(wrapperConstantExp, wrapper.GetType().GetProperty("Value"));

            if (ret.Type != targetType)
            {
                ret = Expression.Convert(ret, targetType);
            }

            return ret;
        }

        static object WrapValue(object value)
        {
            Type valueType = value.GetType();

            if (valueType == UtilConstants.TypeOfString)
            {
                return new ConstantWrapper<string>((string)value);
            }
            else if (valueType == UtilConstants.TypeOfInt32)
            {
                return new ConstantWrapper<int>((int)value);
            }
            else if (valueType == UtilConstants.TypeOfInt64)
            {
                return new ConstantWrapper<long>((long)value);
            }
            else if (valueType == UtilConstants.TypeOfGuid)
            {
                return new ConstantWrapper<Guid>((Guid)value);
            }

            Type wrapperType = typeof(ConstantWrapper<>).MakeGenericType(valueType);
            ConstructorInfo constructor = wrapperType.GetConstructor(new Type[] { valueType });
            return constructor.Invoke(new object[] { value });
        }
    }
}
