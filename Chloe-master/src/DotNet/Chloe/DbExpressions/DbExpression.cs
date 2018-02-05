using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Chloe.DbExpressions
{
    public abstract class DbExpression
    {
        DbExpressionType _nodeType;
        Type _type;

        protected DbExpression(DbExpressionType nodeType)
            : this(nodeType, UtilConstants.TypeOfVoid)
        {
        }
        protected DbExpression(DbExpressionType nodeType, Type type)
        {
            this._nodeType = nodeType;
            this._type = type;
        }

        public virtual DbExpressionType NodeType
        {
            get { return this._nodeType; }
        }
        public virtual Type Type
        {
            get { return this._type; }
        }

        public abstract T Accept<T>(DbExpressionVisitor<T> visitor);


        public static DbAddExpression Add(DbExpression left, DbExpression right, Type returnType, MethodInfo method)
        {
            return new DbAddExpression(returnType, left, right, method);
        }
        public static DbSubtractExpression Subtract(DbExpression left, DbExpression right, Type returnType)
        {
            return new DbSubtractExpression(returnType, left, right);
        }
        public static DbMultiplyExpression Multiply(DbExpression left, DbExpression right, Type returnType)
        {
            return new DbMultiplyExpression(returnType, left, right);
        }
        public static DbDivideExpression Divide(DbExpression left, DbExpression right, Type returnType)
        {
            return new DbDivideExpression(returnType, left, right);
        }
        public static DbModuloExpression Modulo(DbExpression left, DbExpression right, Type returnType)
        {
            return new DbModuloExpression(returnType, left, right);
        }

        public static DbBitAndExpression BitAnd(Type type, DbExpression left, DbExpression right)
        {
            return new DbBitAndExpression(type, left, right);
        }
        public static DbBitOrExpression BitOr(Type type, DbExpression left, DbExpression right)
        {
            return new DbBitOrExpression(type, left, right);
        }

        public static DbAndExpression And(DbExpression left, DbExpression right)
        {
            return new DbAndExpression(left, right);
        }
        public static DbOrExpression Or(DbExpression left, DbExpression right)
        {
            return new DbOrExpression(left, right);
        }
        public static DbEqualExpression Equal(DbExpression left, DbExpression right)
        {
            return new DbEqualExpression(left, right);
        }
        public static DbNotEqualExpression NotEqual(DbExpression left, DbExpression right)
        {
            return new DbNotEqualExpression(left, right);
        }
        public static DbNotExpression Not(DbExpression exp)
        {
            return new DbNotExpression(exp);
        }
        public static DbConvertExpression Convert(DbExpression operand, Type type)
        {
            return new DbConvertExpression(type, operand);
        }

        public static DbCaseWhenExpression CaseWhen(DbCaseWhenExpression.WhenThenExpressionPair whenThenExpPair, DbExpression elseExp, Type type)
        {
            List<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps = new List<DbCaseWhenExpression.WhenThenExpressionPair>(1);
            whenThenExps.Add(whenThenExpPair);
            return DbExpression.CaseWhen(whenThenExps, elseExp, type);
        }
        public static DbCaseWhenExpression CaseWhen(IList<DbCaseWhenExpression.WhenThenExpressionPair> whenThenExps, DbExpression elseExp, Type type)
        {
            return new DbCaseWhenExpression(type, whenThenExps, elseExp);
        }

        public static DbConstantExpression Constant(object value)
        {
            return new DbConstantExpression(value);
        }

        public static DbConstantExpression Constant(object value, Type type)
        {
            return new DbConstantExpression(value, type);
        }

        public static DbGreaterThanExpression GreaterThan(DbExpression left, DbExpression right)
        {
            return new DbGreaterThanExpression(left, right);
        }
        public static DbGreaterThanOrEqualExpression GreaterThanOrEqual(DbExpression left, DbExpression right)
        {
            return new DbGreaterThanOrEqualExpression(left, right);
        }

        public static DbLessThanExpression LessThan(DbExpression left, DbExpression right)
        {
            return new DbLessThanExpression(left, right);
        }
        public static DbLessThanOrEqualExpression LessThanOrEqual(DbExpression left, DbExpression right)
        {
            return new DbLessThanOrEqualExpression(left, right);
        }

        public static DbMemberExpression MemberAccess(MemberInfo member, DbExpression exp)
        {
            return new DbMemberExpression(member, exp);
        }

        public static DbMethodCallExpression MethodCall(DbExpression @object, MethodInfo method, IList<DbExpression> arguments)
        {
            return new DbMethodCallExpression(@object, method, arguments);
        }

        public static DbParameterExpression Parameter(object value)
        {
            return new DbParameterExpression(value);
        }

        public static DbParameterExpression Parameter(object value, Type defaultType)
        {
            if (value == null)
                return new DbParameterExpression(value, defaultType);
            else
                return new DbParameterExpression(value, value.GetType());
        }
    }
}
