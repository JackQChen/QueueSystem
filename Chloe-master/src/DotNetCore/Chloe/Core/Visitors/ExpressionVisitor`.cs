using System;
using System.Linq.Expressions;

namespace Chloe.Core.Visitors
{
    public abstract class ExpressionVisitor<T>
    {
        protected ExpressionVisitor()
        {
        }

        public virtual T Visit(Expression exp)
        {
            if (exp == null)
                return default(T);
            switch (exp.NodeType)
            {
                case ExpressionType.Not:
                    return this.VisitUnary_Not((UnaryExpression)exp);
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return this.VisitUnary_Convert((UnaryExpression)exp);
                case ExpressionType.Quote:
                    return this.VisitUnary_Quote((UnaryExpression)exp);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return this.VisitUnary_Negate((UnaryExpression)exp);
                case ExpressionType.ArrayLength:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return this.VisitBinary_Add((BinaryExpression)exp);
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.VisitBinary_Subtract((BinaryExpression)exp);
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return this.VisitBinary_Multiply((BinaryExpression)exp);
                case ExpressionType.Divide:
                    return this.VisitBinary_Divide((BinaryExpression)exp);
                case ExpressionType.Modulo:
                    return this.VisitBinary_Modulo((BinaryExpression)exp);
                case ExpressionType.And:
                    return this.VisitBinary_And((BinaryExpression)exp);
                case ExpressionType.AndAlso:
                    return this.VisitBinary_AndAlso((BinaryExpression)exp);
                case ExpressionType.Or:
                    return this.VisitBinary_Or((BinaryExpression)exp);
                case ExpressionType.OrElse:
                    return this.VisitBinary_OrElse((BinaryExpression)exp);
                case ExpressionType.LessThan:
                    return this.VisitBinary_LessThan((BinaryExpression)exp);
                case ExpressionType.LessThanOrEqual:
                    return this.VisitBinary_LessThanOrEqual((BinaryExpression)exp);
                case ExpressionType.GreaterThan:
                    return this.VisitBinary_GreaterThan((BinaryExpression)exp);
                case ExpressionType.GreaterThanOrEqual:
                    return this.VisitBinary_GreaterThanOrEqual((BinaryExpression)exp);
                case ExpressionType.Equal:
                    return this.VisitBinary_Equal((BinaryExpression)exp);
                case ExpressionType.NotEqual:
                    return this.VisitBinary_NotEqual((BinaryExpression)exp);
                case ExpressionType.Coalesce:
                    return this.VisitBinary_Coalesce((BinaryExpression)exp);
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                //case ExpressionType.TypeIs:
                //    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                    //case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                //case ExpressionType.Invoke:
                //    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual T VisitUnary(UnaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitUnary_Not(UnaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitUnary_Convert(UnaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitUnary_Quote(UnaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitUnary_Negate(UnaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Add(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Subtract(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Multiply(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Divide(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Modulo(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_And(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_AndAlso(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Or(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_OrElse(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitConstant(ConstantExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitParameter(ParameterExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_LessThan(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_LessThanOrEqual(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_GreaterThan(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_GreaterThanOrEqual(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Equal(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_NotEqual(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitBinary_Coalesce(BinaryExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitLambda(LambdaExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitMemberAccess(MemberExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitConditional(ConditionalExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitMethodCall(MethodCallExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitNew(NewExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitNewArray(NewArrayExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitMemberInit(MemberInitExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
        protected virtual T VisitListInit(ListInitExpression exp)
        {
            throw new NotImplementedException(exp.ToString());
        }
    }

}
