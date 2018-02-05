using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Oracle
{
    static class DbExpressionHelper
    {
        /// <summary>
        /// 尝试将 exp 转换成 DbParameterExpression。
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static DbExpression OptimizeDbExpression(this DbExpression exp)
        {
            DbExpression stripedExp = DbExpressionExtension.StripInvalidConvert(exp);

            DbExpression tempExp = stripedExp;

            List<DbConvertExpression> cList = null;
            while (tempExp.NodeType == DbExpressionType.Convert)
            {
                if (cList == null)
                    cList = new List<DbConvertExpression>();

                DbConvertExpression c = (DbConvertExpression)tempExp;
                cList.Add(c);
                tempExp = c.Operand;
            }

            if (tempExp.NodeType == DbExpressionType.Constant || tempExp.NodeType == DbExpressionType.Parameter)
                return stripedExp;

            if (tempExp.NodeType == DbExpressionType.MemberAccess)
            {
                DbMemberExpression dbMemberExp = (DbMemberExpression)tempExp;

                if (ExistDateTime_NowOrDateTime_UtcNow(dbMemberExp))
                    return stripedExp;

                DbParameterExpression val;
                if (DbExpressionExtension.TryConvertToParameterExpression(dbMemberExp, out val))
                {
                    if (cList != null)
                    {
                        if (val.Value == DBNull.Value)//如果是 null，则不需要 Convert 了，在数据库里没意义
                            return val;

                        DbConvertExpression c = null;
                        for (int i = cList.Count - 1; i > -1; i--)
                        {
                            DbConvertExpression item = cList[i];
                            c = new DbConvertExpression(item.Type, val);
                        }

                        return c;
                    }

                    return val;
                }
            }

            return stripedExp;
        }

        public static bool ExistDateTime_NowOrDateTime_UtcNow(this DbMemberExpression exp)
        {
            while (exp != null)
            {
                if (exp.Member == UtilConstants.PropertyInfo_DateTime_Now || exp.Member == UtilConstants.PropertyInfo_DateTime_UtcNow)
                {
                    return true;
                }

                exp = exp.Expression as DbMemberExpression;
            }

            return false;
        }

        /// <summary>
        /// 判定 exp 返回值肯定是 null 或 ''
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool AffirmExpressionRetValueIsNullOrEmpty(this DbExpression exp)
        {
            exp = DbExpressionExtension.StripConvert(exp);

            if (exp.NodeType == DbExpressionType.Constant)
            {
                var c = (DbConstantExpression)exp;
                return IsNullOrEmpty(c.Value);
            }

            if (exp.NodeType == DbExpressionType.Parameter)
            {
                var p = (DbParameterExpression)exp;
                return IsNullOrEmpty(p.Value);
            }

            return false;
        }

        static bool IsNullOrEmpty(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return true;

            string stringValue = obj as string;
            if (stringValue != null && stringValue == string.Empty)
                return true;

            return false;
        }
    }
}
