using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Query
{
    static class DbExpressionEqualityComparer
    {
        public static bool EqualsCompare(DbExpression exp1, DbExpression exp2)
        {
            if (exp1.NodeType != exp2.NodeType)
                return false;

            if (exp1 == exp2)
                return true;

            switch (exp1.NodeType)
            {
                case DbExpressionType.ColumnAccess:
                    return AreEqual((DbColumnAccessExpression)exp1, (DbColumnAccessExpression)exp2);
                case DbExpressionType.Table:
                    return AreEqual((DbTableExpression)exp1, (DbTableExpression)exp2);
                case DbExpressionType.Constant:
                    return AreEqual((DbConstantExpression)exp1, (DbConstantExpression)exp2);
                case DbExpressionType.Convert:
                    return AreEqual((DbConvertExpression)exp1, (DbConvertExpression)exp2);
                case DbExpressionType.Parameter:
                    return AreEqual((DbParameterExpression)exp1, (DbParameterExpression)exp2);
                case DbExpressionType.MemberAccess:
                    return AreEqual((DbMemberExpression)exp1, (DbMemberExpression)exp2);
                case DbExpressionType.Call:
                    return AreEqual((DbMethodCallExpression)exp1, (DbMethodCallExpression)exp2);
                case DbExpressionType.Add:
                case DbExpressionType.Subtract:
                case DbExpressionType.Multiply:
                case DbExpressionType.Divide:
                case DbExpressionType.BitAnd:
                case DbExpressionType.BitOr:
                case DbExpressionType.Equal:
                case DbExpressionType.NotEqual:
                case DbExpressionType.LessThan:
                case DbExpressionType.LessThanOrEqual:
                case DbExpressionType.GreaterThan:
                case DbExpressionType.GreaterThanOrEqual:
                    return AreEqual((DbBinaryExpression)exp1, (DbBinaryExpression)exp2);
                default:
                    return exp1 == exp2;
            }
        }
        public static bool AreEqual(DbColumnAccessExpression exp1, DbColumnAccessExpression exp2)
        {
            if (exp1.Column.Name != exp2.Column.Name)
                return false;
            return exp1.Table.Name == exp2.Table.Name && exp1.Table.Schema == exp2.Table.Schema;
        }
        public static bool AreEqual(DbTableExpression exp1, DbTableExpression exp2)
        {
            return exp1.Table.Name == exp2.Table.Name && exp1.Table.Schema == exp2.Table.Schema;
        }
        public static bool AreEqual(DbConstantExpression exp1, DbConstantExpression exp2)
        {
            return Utils.AreEqual(exp1.Value, exp2.Value);
        }
        public static bool AreEqual(DbConvertExpression exp1, DbConvertExpression exp2)
        {
            if (exp1.Type != exp2.Type)
                return false;
            return EqualsCompare(exp1.Operand, exp2.Operand);
        }
        public static bool AreEqual(DbParameterExpression exp1, DbParameterExpression exp2)
        {
            return Utils.AreEqual(exp1.Value, exp2.Value);
        }
        public static bool AreEqual(DbMemberExpression exp1, DbMemberExpression exp2)
        {
            if (exp1.Member != exp2.Member)
                return false;
            return EqualsCompare(exp1.Expression, exp2.Expression);
        }
        public static bool AreEqual(DbMethodCallExpression exp1, DbMethodCallExpression exp2)
        {
            if (exp1.Method != exp2.Method)
                return false;
            if (exp1.Arguments.Count != exp2.Arguments.Count)
                return false;

            if (exp1.Object != null && exp2.Object != null)
            {
                /* instance method */
                if (!EqualsCompare(exp1.Object, exp2.Object))
                    return false;
            }

            for (int i = 0; i < exp1.Arguments.Count; i++)
            {
                if (!EqualsCompare(exp1.Arguments[i], exp2.Arguments[i]))
                    return false;
            }

            return true;
        }

        public static bool AreEqual(DbBinaryExpression exp1, DbBinaryExpression exp2)
        {
            if (exp1.Method != exp2.Method)
                return false;
            if (!EqualsCompare(exp1.Left, exp2.Left))
                return false;
            if (!EqualsCompare(exp1.Right, exp2.Right))
                return false;

            return true;
        }

    }
}
