using Chloe.Extensions;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Core.Visitors
{
    public class ExpressionEvaluator : ExpressionVisitor<object>
    {
        static ExpressionEvaluator _evaluator = new ExpressionEvaluator();
        public static object Evaluate(Expression exp)
        {
            return _evaluator.Visit(exp);
        }
        protected override object VisitMemberAccess(MemberExpression exp)
        {
            object val = null;
            if (exp.Expression != null)
                val = this.Visit(exp.Expression);

            if (exp.Member.MemberType == MemberTypes.Property)
            {
                PropertyInfo pro = (PropertyInfo)exp.Member;
                return pro.GetValue(val, null);
            }
            else if (exp.Member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)exp.Member;
                return field.GetValue(val);
            }

            throw new NotSupportedException();
        }
        protected override object VisitUnary_Not(UnaryExpression exp)
        {
            var operandValue = this.Visit(exp.Operand);

            if ((bool)operandValue == true)
                return false;

            return true;
        }
        protected override object VisitUnary_Convert(UnaryExpression exp)
        {
            object operandValue = this.Visit(exp.Operand);

            //(int)null
            if (operandValue == null)
            {
                //(int)null
                if (exp.Type.IsValueType && !exp.Type.IsNullable())
                    throw new NullReferenceException();

                return null;
            }

            Type operandValueType = operandValue.GetType();

            if (exp.Type == operandValueType || exp.Type.IsAssignableFrom(operandValueType))
            {
                return operandValue;
            }

            Type underlyingType;

            if (exp.Type.IsNullable(out underlyingType))
            {
                //(int?)int
                if (underlyingType == operandValueType)
                {
                    var constructor = exp.Type.GetConstructor(new Type[] { operandValueType });
                    var val = constructor.Invoke(new object[] { operandValue });
                    return val;
                }
                else
                {
                    //如果不等，则诸如：(long?)int / (long?)int?  -->  (long?)((long)int) / (long?)((long)int?)
                    var c = Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(operandValue), underlyingType);
                    var cc = Expression.MakeUnary(ExpressionType.Convert, c, exp.Type);
                    return this.Visit(cc);
                }
            }

            //(int)int?
            if (operandValueType.IsNullable(out underlyingType))
            {
                if (underlyingType == exp.Type)
                {
                    var pro = operandValueType.GetProperty("Value");
                    var val = pro.GetValue(operandValue, null);
                    return val;
                }
                else
                {
                    //如果不等，则诸如：(long)int?  -->  (long)((long)int)
                    var c = Expression.MakeUnary(ExpressionType.Convert, Expression.Constant(operandValue), underlyingType);
                    var cc = Expression.MakeUnary(ExpressionType.Convert, c, exp.Type);
                    return this.Visit(cc);
                }
            }

            //(long)int
            if (operandValue is IConvertible)
            {
                return Convert.ChangeType(operandValue, exp.Type);
            }

            throw new NotSupportedException(string.Format("Does not support the type '{0}' converted to type '{1}'.", operandValueType.FullName, exp.Type.FullName));
        }
        protected override object VisitUnary_Quote(UnaryExpression exp)
        {
            var e = ExpressionExtension.StripQuotes(exp);
            return e;
        }
        protected override object VisitConstant(ConstantExpression exp)
        {
            return exp.Value;
        }
        protected override object VisitMethodCall(MethodCallExpression exp)
        {
            object instance = this.Visit(exp.Object);

            object[] arguments = exp.Arguments.Select(a => this.Visit(a)).ToArray();

            return exp.Method.Invoke(instance, arguments);
        }
        protected override object VisitNew(NewExpression exp)
        {
            object[] arguments = exp.Arguments.Select(a => this.Visit(a)).ToArray();

            return exp.Constructor.Invoke(arguments);
        }
        protected override object VisitNewArray(NewArrayExpression exp)
        {
            Array arr = Array.CreateInstance(exp.Type, exp.Expressions.Count);
            for (int i = 0; i < exp.Expressions.Count; i++)
            {
                var e = exp.Expressions[i];
                arr.SetValue(this.Visit(e), i);
            }

            return arr;
        }
    }
}
